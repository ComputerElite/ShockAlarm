using System.Net.Http.Headers;
using System.Text.Json;
using ComputerUtils.Logging;
using ComputerUtils.Webserver;
using OpenShock.SDK.CSharp;
using OpenShock.SDK.CSharp.Models;
using ShockAlarm.Alarm;
using ShockAlarm.Database;
using ShockAlarm.Users;

namespace ShockAlarm.Server;

public class AlarmServer
{
    public static void AddRoutes(HttpServer server)
    {
        server.AddRoute("GET", "/api/v1/tokens", request =>
        {
            request.allowAllOrigins = true;
            User? user = UserManagementServer.GetUserBySession(request);
            if (user == null)
            {
                ApiError.SendUnauthorized(request);
                return true;
            }

            using (AppDbContext d = new())
            {
                request.SendString(JsonSerializer.Serialize(d.OpenshockApiTokens.Where(x=> x.User == user).ToList()), "application/json");
            }
            return true;
        });
        server.AddRoute("POST", "/api/v1/tokens", request =>
        {
            request.allowAllOrigins = true;
            User? user = UserManagementServer.GetUserBySession(request);
            if (user == null)
            {
                ApiError.SendUnauthorized(request);
                return true;
            }

            OpenshockApiToken? token;
            try
            {
                token = JsonSerializer.Deserialize<OpenshockApiToken>(request.bodyString);
            } catch
            {
                ApiError.MalformedRequest(request);
                return true;
            }

            if (token == null)
            {
                ApiError.MalformedRequest(request);
                return true;
            }
            
            
            // Get username of token
            OpenShockApiClient client = new(new ApiClientOptions {Token = token.Token});
            var self = client.GetSelf().Result;
            if (self.IsT1)
            {
                request.SendString(JsonSerializer.Serialize(new ApiError {Message = "Invalid token"}), "application/json", 400);
                return true;
            }
            
            using (AppDbContext d = new())
            {
                d.Attach(user);
                token.User = user;
                token.ForOpenShockUser = self.AsT0.Value.Name;
                d.OpenshockApiTokens.Add(token);
                d.SaveChanges();
            }
            request.SendString(JsonSerializer.Serialize(new ApiResponse
            {
                CreatedId = token.Id,
                Success = true
            }), "application/json");
            return true;
        });
        server.AddRoute("DELETE", "/api/v1/tokens", request =>
        {
            request.allowAllOrigins = true;
            User? user = UserManagementServer.GetUserBySession(request);
            if (user == null)
            {
                ApiError.SendUnauthorized(request);
                return true;
            }

            OpenshockApiToken? token;
            try
            {
                token = JsonSerializer.Deserialize<OpenshockApiToken>(request.bodyString);
            } catch
            {
                ApiError.MalformedRequest(request);
                return true;
            }

            if (token == null)
            {
                ApiError.MalformedRequest(request);
                return true;
            }
            
            using (AppDbContext d = new())
            {
                d.Attach(user);
                OpenshockApiToken? existingToken = d.OpenshockApiTokens.FirstOrDefault(x => x.Id == token.Id);
                if(existingToken == null)
                {
                    ApiError.SendNotFound(request);
                    return true;
                }
                if(existingToken.User.Id != user.Id)
                {
                    ApiError.SendUnauthorized(request);
                    return true;
                }
                d.Remove(existingToken);
                d.SaveChanges();
            }
            request.SendString(JsonSerializer.Serialize(new ApiResponse
            {
                Success = true
            }));
            return true;
        });
        server.AddRoute("GET", "/api/v1/shockers/", request =>
        {
            request.allowAllOrigins = true;
            User? user = UserManagementServer.GetUserBySession(request);
            if (user == null)
            {
                ApiError.SendUnauthorized(request);
                return true;
            }
            List<Shocker> shockers = new();
            using (AppDbContext d = new())
            {
                List<OpenshockApiToken> tokens = d.OpenshockApiTokens.Where(x => x.User.Id == user.Id).ToList();
                foreach (OpenshockApiToken token in tokens)
                {
                    OpenShockApiClient apiClient = GetApiClient(token);
                    var devices = apiClient.GetOwnShockers();
                    devices.Wait();
                    if (devices.Result.IsT1)
                    {
                        request.SendString(JsonSerializer.Serialize(new ApiError {Message = "Token invalid"}), "application/json", 401);
                        return true;
                    }
                    List<ResponseDeviceWithShockers> devicesAll = devices.Result.AsT0.Value.ToList();
                    foreach (ResponseDeviceWithShockers device in devicesAll)
                    {
                        foreach (ShockerResponse sr in device.Shockers)
                        {
                            shockers.Add(new Shocker
                            {
                                ShockerId = sr.Id.ToString(),
                                Name = device.Name + "." + sr.Name,
                                ApiTokenId = token.Id,
                                Paused = sr.IsPaused,
                                Limits = new OpenShockShockerLimits(),
                                Permissions = new OpenShockShockerPermissions()
                            });
                        }
                    }
                    List<OpenShockDevicesContainer> sharedShockers = GetSharedShockers(token) ?? new List<OpenShockDevicesContainer>();
                    foreach (OpenShockDevicesContainer container in sharedShockers)
                    {
                        foreach (OpenShockDevice device in container.devices)
                        {
                            foreach (OpenShockShocker sr in device.shockers)
                            {
                                shockers.Add(new Shocker
                                {
                                    ShockerId = sr.id,
                                    Name = device.name + "." + sr.name,
                                    ApiTokenId = token.Id,
                                    Paused = sr.isPaused,
                                    Limits = sr.limits,
                                    Permissions = sr.permissions
                                });
                            }
                        }
                    }
                }
            }
            
            request.SendString(JsonSerializer.Serialize(shockers), "application/json");
            return true;
        },true);
        server.AddRoute("POST", "/api/v1/alarms", request =>
        {
            request.allowAllOrigins = true;
            User? user = UserManagementServer.GetUserBySession(request);
            if (user == null)
            {
                ApiError.SendUnauthorized(request);
                return true;
            }
            Alarm.Alarm? alarm;
            try
            {
                alarm = JsonSerializer.Deserialize<Alarm.Alarm>(request.bodyString);
            } catch
            {
                ApiError.MalformedRequest(request);
                return true;
            }

            if (alarm == null)
            {
                ApiError.MalformedRequest(request);
                return true;
            }
            alarm.User = user;
            using (AppDbContext d = new())
            {
                Alarm.Alarm? existingAlarm = d.Alarms.FirstOrDefault(x => x.Id == alarm.Id);
                if (existingAlarm != null)
                {
                    if (existingAlarm.User.Id != user.Id)
                    {
                        ApiError.SendUnauthorized(request);
                        return true;
                    }

                    d.Alarms.Remove(existingAlarm);
                    d.SaveChanges();
                }
            }
            using(AppDbContext d = new())
            {
                d.Attach(user);
                foreach (Shocker s in alarm.Shockers)
                {
                    s.ApiToken = d.OpenshockApiTokens.FirstOrDefault(x => x.Id == s.ApiTokenId);
                    s.Permissions.Id = null;
                    s.Limits.Id = null;
                }

                alarm.UpdateNextTrigger();
                d.Alarms.Add(alarm);
                d.SaveChanges();
            }
            request.SendString(JsonSerializer.Serialize(new ApiResponse
            {
                CreatedId = alarm.Id,
                Success = true
            }));
            return true;
        });
        server.AddRoute("GET", "/api/v1/alarms", request =>
        {
            request.allowAllOrigins = true;
            User? user = UserManagementServer.GetUserBySession(request);
            if (user == null)
            {
                ApiError.SendUnauthorized(request);
                return true;
            }
            List<Alarm.Alarm> alarms;
            using (AppDbContext d = new())
            {
                alarms = d.Alarms.Where(x => x.User.Id == user.Id).ToList();
            }
            request.SendString(JsonSerializer.Serialize(alarms), "application/json");
            return true;
        });
        server.AddRoute("DELETE", "/api/v1/alarms" , request =>
        {
            request.allowAllOrigins = true;
            User? user = UserManagementServer.GetUserBySession(request);
            if (user == null)
            {
                ApiError.SendUnauthorized(request);
                return true;
            }

            Alarm.Alarm? toDelete;
            try
            {
                toDelete = JsonSerializer.Deserialize<Alarm.Alarm>(request.bodyString);
            } catch
            {
                ApiError.MalformedRequest(request);
                return true;
            }
            
            if (toDelete == null)
            {
                ApiError.MalformedRequest(request);
                return true;
            }
            
            using (AppDbContext d = new())
            {
                d.Attach(user);
                Alarm.Alarm? existingAlarm = d.Alarms.FirstOrDefault(x => x.Id == toDelete.Id);
                if(existingAlarm == null)
                {
                    ApiError.SendNotFound(request);
                    return true;
                }
                if(existingAlarm.User.Id != user.Id)
                {
                    ApiError.SendUnauthorized(request);
                    return true;
                }
                d.Remove(existingAlarm);
                d.SaveChanges();
            }
            request.SendString(JsonSerializer.Serialize(new ApiResponse
            {
                Success = true
            }));
            return true;
        });
    }

    private static List<OpenShockDevicesContainer>? GetSharedShockers(OpenshockApiToken token)
    {
        HttpClient client = new();
        client.DefaultRequestHeaders.Add("OpenShockToken", token.Token);
        client.DefaultRequestHeaders.Add("accept", "application/json");
        client.DefaultRequestHeaders.Add("User-Agent", "ShockAlarm");
        
        HttpResponseMessage response = client.GetAsync(token.Server + "/1/shockers/shared").Result;
        if (!response.IsSuccessStatusCode)
        {
            Logger.Log("Failed to get shared shockers");
            return new List<OpenShockDevicesContainer>();
        }

        try
        {
            string txt = response.Content.ReadAsStringAsync().Result;
            Logger.Log(txt);
            var sharedShockers = JsonSerializer.Deserialize<OpenShockApiData<List<OpenShockDevicesContainer>>>(txt);
            Logger.Log(JsonSerializer.Serialize(sharedShockers));
            return sharedShockers?.data;
        } catch(Exception e)
        {
            Logger.Log("Failed to parse shared shockers: " + e);
            return new List<OpenShockDevicesContainer>();
        }
    }

    private static OpenShockApiClient? GetApiClientBasedOnTokenId(User user, string token)
    {
        using(AppDbContext d = new())
        {
            OpenshockApiToken? t = d.OpenshockApiTokens.FirstOrDefault(x => x.Id == token);
            if (t == null || t.User.Id != user.Id)
            {
                Console.WriteLine("Invalid token");
                return null;
            }

            return GetApiClient(t);
        }
    }

    public static OpenShockApiClient GetApiClient(OpenshockApiToken t)
    {
        
        return new OpenShockApiClient(new ApiClientOptions { Token = t.Token, Server = new Uri(t.Server)});
    }

    private static List<Alarm.Alarm> AlarmCache { get; set; } = new List<Alarm.Alarm>();
    private static DateTime LastCacheUpdate { get; set; } = DateTime.MinValue;
    public static void UpdateAlarmCache()
    {
        AlarmCache.Clear();
        using (AppDbContext d = new())
        {
            AlarmCache = d.Alarms.ToList();
        }
    }
    public static List<Alarm.Alarm> GetAllAlarms()
    {
        if(LastCacheUpdate.AddMinutes(1) < DateTime.UtcNow)
        {
            UpdateAlarmCache();
        }

        return AlarmCache;
    }
}
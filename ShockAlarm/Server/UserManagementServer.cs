using System.Net;
using System.Text.Json;
using ComputerUtils.Logging;
using ComputerUtils.Webserver;
using ShockAlarm.Users;

namespace ShockAlarm.Server;

public class UserManagementServer
{
    public static User? GetUserBySession(ServerRequest request)
    {
        string? session = null;
        string? authorizationHeader = request.context.Request.Headers.Get("Authorization");
        if(authorizationHeader != null)
        {
            if (!authorizationHeader.StartsWith("Bearer ")) return null;
            session = authorizationHeader.Substring("Bearer ".Length);
        } else
        {
            Cookie? sessionCookie = request.cookies["session"];
            if (sessionCookie == null)
            {
                session = request.queryString.Get("session");
            }
            else
            {
                session = sessionCookie.Value;
            }
        }

        if (session == null) return null;
        //Logger.Log("Requested session is "+session);
        User? u = UserManager.GetUserBySession(session);
        return u;
    }
    
    public static void AddUsermanagementEndpoints(HttpServer server)
    {
        server.AddRoute("POST", "/api/v1/user/login", request =>
        {
            request.allowAllOrigins = true;
            LoginRequest? r = null;
            try
            {
                r = JsonSerializer.Deserialize<LoginRequest>(request.bodyString);
            } catch(Exception e)
            {
                ApiError.MalformedRequest(request);
            }
            if(r == null)
            {
                ApiError.MalformedRequest(request);
                return true;
            }
            LoginResponse response = UserManager.Login(r);
            request.SendString(JsonSerializer.Serialize(response), "application/json", response.Success ? 200 : 400);
            return true;
        });
        server.AddRoute("POST", "/api/v1/user/register", request =>
        {
            request.allowAllOrigins = true;
            LoginRequest? r = null;
            try
            {
                r = JsonSerializer.Deserialize<LoginRequest>(request.bodyString);
            } catch(Exception e)
            {
                ApiError.MalformedRequest(request);
            }
            if(r == null)
            {
                ApiError.MalformedRequest(request);
                return true;
            }
            LoginResponse response = UserManager.Register(r);
            request.SendString(JsonSerializer.Serialize(response), "application/json", response.Success ? 200 : 400);
            return true;
        });
        server.AddRoute("GET", "/api/v1/user/my_sessions", request =>
        {
            request.allowAllOrigins = true;
            User? u = GetUserBySession(request);
            if (u == null)
            {
                ApiError.SendUnauthorized(request);
                return true;
            }
            List<UserSession> sessions = UserManager.GetSessionsForUser(u);
            request.SendString(JsonSerializer.Serialize(sessions), "application/json", 200);
            return true;
        });
        server.AddRoute("GET", "/api/v1/user/me", request =>
        {
            request.allowAllOrigins = true;
            User? u = GetUserBySession(request);
            if (u == null)
            {
                ApiError.SendUnauthorized(request);
                return true;
            }
            request.SendString(JsonSerializer.Serialize(u), "application/json", 200);
            return true;
        });
    }
}
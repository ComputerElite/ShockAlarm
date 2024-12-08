using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using ComputerUtils.Logging;
using OpenShock.SDK.CSharp;
using OpenShock.SDK.CSharp.Models;
using Quartz;
using ShockAlarm.Server;
using ShockAlarm.Users;

namespace ShockAlarm.Alarm;

public class Alarm
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }
    public string Name { get; set; }
    public string Cron { get; set; }
    public string TimeZone { get; set; } = "UTC";
    public DateTime? NextTrigger { get; set; }
    public bool DisableAfterFirstTrigger { get; set; }
    public bool Enabled { get; set; }
    public List<Shocker> Shockers { get; set; }
    public User User { get; set; }

    public void UpdateNextTrigger()
    {
        var cron = new CronExpression(Cron);
        cron.TimeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZone);
        NextTrigger = cron.GetNextValidTimeAfter(DateTime.UtcNow)?.UtcDateTime ?? DateTime.MaxValue;
        Logger.Log(Name + " at " + NextTrigger + " UTC");
    }

    public bool TriggerIfApplicable(DateTime now)
    {
        bool changed = false;
        if (!Enabled) return changed;
        if (NextTrigger == null)
        {
            changed = true;
            UpdateNextTrigger();
        }
        if (NextTrigger <= now)
        {
            changed = true;
            Trigger();
        }

        return changed;
    }
    
    public void Trigger()
    {
        Logger.Log("Triggering alarm " + Name);
        // Group shockers by token
        List<Shocker> shockersForThread = new List<Shocker>();
        var shockersByToken = Shockers.GroupBy(s => s.ApiTokenId);
        foreach (var shockerGroup in shockersByToken)
        {
            var token = shockerGroup.First().ApiToken;
            Logger.Log("Token is null: " + (token == null));
            if (token == null) continue;
            OpenShockApiClient client = AlarmServer.GetApiClient(token);
            List<Control> controls = new();
            foreach (var shocker in shockerGroup)
            {
                if (!shocker.Enabled) continue;
                if(shocker.Tone == null) controls.Add(shocker.CompileControl());
                else
                {
                    shockersForThread.Add(shocker);
                }
            }
            Logger.Log(JsonSerializer.Serialize(controls));
            client.ControlShocker(new ControlRequest {CustomName = Name, Shocks = controls});
        }
        if(shockersForThread.Count > 0)
        {
            AlarmServer.EnableAlarm(Id);
            // Prepare shockers
            List<AlarmToneComponent> components = new();
            DateTime nowUtc = DateTime.UtcNow;
            foreach (Shocker shocker in shockersForThread)
            {
                foreach (AlarmToneComponent component in shocker.Tone?.Components ?? new List<AlarmToneComponent>())
                {
                    Logger.Log("Preparing shocker " + shocker.Name + " for component " + component.Id);
                    component.Shocker = shocker;
                    component.TriggerTime = nowUtc.AddSeconds(component.TriggerSeconds);
                    components.Add(component);
                }
            }

            components = components.OrderBy(x => x.TriggerSeconds).ToList();
            
            new Thread(() =>
            {
                while (components.Count > 0)
                {
                    if(!AlarmServer.IsAlarmActive(Id)) break;
                    if (components[0].TriggerTime <= DateTime.UtcNow)
                    {
                        AlarmToneComponent component = components[0];
                        components.RemoveAt(0);
                        Logger.Log(
                            "Triggering component " + component.Id + " for shocker " + component.Shocker.Name);
                        OpenShockApiClient client = AlarmServer.GetApiClient(component.Shocker.ApiToken);
                        client.ControlShocker(new ControlRequest
                        {
                            CustomName = Name,
                            Shocks = new List<Control> { component.CompileControl() }
                        });
                        continue;
                    }

                    Thread.Sleep(50);
                }
                AlarmServer.DisableAlarm(Id);
            }).Start();
        }
        

        if (DisableAfterFirstTrigger)
        {
            Enabled = false;
        }

        UpdateNextTrigger();
    }
}
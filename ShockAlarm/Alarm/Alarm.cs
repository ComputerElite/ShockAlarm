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
    public DateTime? NextTrigger { get; set; }
    public bool DisableAfterFirstTrigger { get; set; }
    public bool Enabled { get; set; }
    public List<Shocker> Shockers { get; set; }
    public User User { get; set; }

    public void UpdateNextTrigger()
    {
        Logger.Log("Updating next trigger for alarm " + Name + "  " + Cron);
        var cron = new CronExpression(Cron);
        NextTrigger = cron.GetNextValidTimeAfter(DateTime.UtcNow)?.UtcDateTime ?? DateTime.MaxValue;
        Logger.Log(NextTrigger.ToString());
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
            Logger.Log("Triggering alarm " + Name);
            // Group shockers by token
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
                    if(shocker.Enabled) controls.Add(shocker.CompileControl());
                }
                Logger.Log(JsonSerializer.Serialize(controls));
                client.ControlShocker(new ControlRequest {CustomName = Name, Shocks = controls});
            }

            if (DisableAfterFirstTrigger)
            {
                Enabled = false;
            }

            UpdateNextTrigger();
            changed = true;
        }

        return changed;
    }
}
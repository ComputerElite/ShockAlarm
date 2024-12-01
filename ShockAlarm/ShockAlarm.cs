using System.Text.Json;
using OpenShock.SDK.CSharp.Models;

namespace ShockAlarm;

public class ShockAlarm
{
    public DateTime AlarmTime { get; set; }
    public Control Shock { get; set; }
}

public class ShockAlarmConfig
{
    public List<ShockAlarm> alarms { get; set; } = new();
    public string ApiToken { get; set; }
    public static ShockAlarmConfig Instance { get; set; } = new();
    
    public static void LoadConfig()
    {
        if (!File.Exists("config.json"))
        {
            SaveConfig();
        }
        else
        {
            Instance = JsonSerializer.Deserialize<ShockAlarmConfig>(File.ReadAllText("config.json"));
        }
    }
    public static void SaveConfig()
    {
        File.WriteAllText("config.json", JsonSerializer.Serialize(Instance));
    }
}
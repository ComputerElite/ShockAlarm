using ComputerUtils.Logging;
using OneOf;
using OneOf.Types;
using OpenShock.SDK.CSharp;
using OpenShock.SDK.CSharp.Models;
using ShockAlarm;

public class ProgramPP
{
    static OpenShockApiClient apiClient;

    static void AlarmThread()
    {
        while (true)
        {
            for (int i = 0; i < ShockAlarmConfig.Instance.alarms.Count; i++)
            {
                ShockAlarm.ShockAlarm alarm = ShockAlarmConfig.Instance.alarms[i];
                if (DateTime.Now > alarm.AlarmTime)
                {
                    apiClient.ControlShocker(new ControlRequest
                    {
                        CustomName = null,
                        Shocks = new List<Control>
                        {
                            alarm.Shock
                        }
                    });
                    ShockAlarmConfig.Instance.alarms.Remove(alarm);
                    ShockAlarmConfig.SaveConfig();
                }
            }
                
            Thread.Sleep(1000);
        }
    }
    
    static void Main(string[] args)
    {
        ShockAlarmConfig.LoadConfig();
        apiClient = new OpenShockApiClient(new ApiClientOptions
        {
            Token = ShockAlarmConfig.Instance.ApiToken,
        });
        Thread alarmThread = new(AlarmThread);
        alarmThread.Start();
        while (true)
        {
            Console.WriteLine("1. Set api token");
            Console.WriteLine("2. Shock something");
            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    SetApiToken();
                    break;
                case "2":
                    Shock();
                    break;
            }
        }
    }

    static void Shock()
    {
        Control control = InputControl();
        if (control == null) return;
        Console.WriteLine("Enter alarm time (yyyy-MM-dd HH:mm:ss):");
        string timeInput = Console.ReadLine();
        DateTime alarmTime = DateTime.Parse(timeInput);
        ShockAlarmConfig.Instance.alarms.Add(new ShockAlarm.ShockAlarm
        {
            AlarmTime = alarmTime,
            Shock = control
        });
        ShockAlarmConfig.SaveConfig();
    }

    static Control? InputControl()
    {
        Task<OneOf<Success<IReadOnlyCollection<ResponseDeviceWithShockers>>, UnauthenticatedError>> devices =apiClient.GetOwnShockers();
        devices.Wait();
        if (devices.Result.IsT1)
        {
            Console.WriteLine("Unauthenticated");
            return null;
        }
        var shockers = devices.Result.AsT0.Value;
        if (shockers.Count == 0)
        {
            Console.WriteLine("No shockers found");
            return null;
        }
        Console.WriteLine("Select a shocker to shock:");
        List<ShockerResponse> shockerResponses = new();
        foreach (ResponseDeviceWithShockers device in shockers)
        {
            shockerResponses.AddRange(device.Shockers);
        }
        for (int i = 0; i < shockerResponses.Count; i++)
        {
            Console.WriteLine($"{i}. {shockerResponses[i].Name}");
        }
        string shockerInput = Console.ReadLine();
        if (int.TryParse(shockerInput, out int shockerIndex)&&
            shockerIndex < shockerResponses.Count)
        {
            var shocker = shockerResponses[shockerIndex];
            Console.WriteLine("Enter duration in ms:");
            string durationInput = Console.ReadLine();
            Console.WriteLine("One of Shock, Vibrate, or Beep:");
            ushort duration = ushort.Parse(durationInput);
            string typeInput = Console.ReadLine();
            ControlType t = ControlType.Shock;
            switch (typeInput.ToLower())
            {
                case "vibrate":
                    t = ControlType.Vibrate;
                    break;
                case "beep":
                    t = ControlType.Sound;
                    break;
                case "shock":
                    t = ControlType.Shock;
                    break;
            }
            Console.WriteLine("Intensity (0-100):");
            string intensityInput = Console.ReadLine();
            byte intensity = byte.Parse(intensityInput);



            return new Control
            {
                Duration = duration,
                Exclusive = true,
                Id = shocker.Id,
                Type = t,
                Intensity = intensity
            };
        }

        return null;
    }

    static void SetApiToken()
    {
        Console.WriteLine("Enter api token: ");
        var token = Console.ReadLine();
        ShockAlarmConfig.Instance.ApiToken = token;
        ShockAlarmConfig.SaveConfig();
    
    }
}


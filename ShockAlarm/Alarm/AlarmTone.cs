using System.ComponentModel.DataAnnotations.Schema;
using OpenShock.SDK.CSharp.Models;
using ShockAlarm.Users;

namespace ShockAlarm.Alarm;

public class AlarmTone
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }
    public string Name { get; set; }
    public bool IsPublic { get; set; }
    public User User { get; set; }
    public List<AlarmToneComponent> Components { get; set; }
}

public class AlarmToneComponent : ShockerControlData
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }

    public double TriggerSeconds { get; set; } = 0;
    [NotMapped]
    public DateTime TriggerTime { get; set; }
    [NotMapped]
    public Shocker Shocker { get; set; }

    public Control CompileControl()
    {
        return new Control
        {
            Duration = Duration,
            Exclusive = true,
            Id = new Guid(Shocker.ShockerId),
            Type = ControlType,
            Intensity = Intensity
        };
    }
}
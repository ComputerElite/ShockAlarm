using System.ComponentModel.DataAnnotations.Schema;
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

    public int TriggerSeconds { get; set; } = 0;
}
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using OpenShock.SDK.CSharp.Models;

namespace ShockAlarm.Alarm;

public class Shocker
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }
    public string? ShockerId { get; set; }
    public string Name { get; set; } = "unnamed shocker";
    public bool Enabled { get; set; } = false;
    public ControlType ControlType { get; set; } = ControlType.Vibrate;
    public byte Intensity { get; set; } = 50;
    public ushort Duration { get; set; } = 500;
    public string ApiTokenId { get; set; }
    [JsonIgnore]
    public OpenshockApiToken? ApiToken { get; set; }

    [JsonIgnore]
    public Alarm? Alarm { get; set; }

    public Shocker() {}

    public Shocker(ShockerResponse r, OpenshockApiToken token)
    {
        this.ShockerId = r.Id.ToString();
        this.Name = r.Name;
    }

    public Control CompileControl()
    {
        return new Control
        {
            Duration = Duration,
            Exclusive = true,
            Id = new Guid(ShockerId),
            Type = ControlType,
            Intensity = Intensity
        };
    }
}
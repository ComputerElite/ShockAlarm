using System.ComponentModel.DataAnnotations.Schema;
using ShockAlarm.Users;

namespace ShockAlarm.Alarm;

public class OpenshockApiToken
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }
    public string Token { get; set; }
    public string ForOpenShockUser { get; set; }
    public User User { get; set; }
    public string Server { get; set; } = "https://api.openshock.app";
}
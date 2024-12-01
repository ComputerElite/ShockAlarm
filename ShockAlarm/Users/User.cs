using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ShockAlarm.Users;

public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }

    public bool IsAdmin { get; set; } = false;
    public string Username { get; set; }
    [JsonIgnore]
    public string PasswordHash { get; set; }
    [JsonIgnore]
    public string Salt { get; set; }
    [JsonIgnore]
    public bool TwoFactorEnabled { get; set; }

    public static User DefaultAdminUser = new User()
    {
        Id = "root",
        Username = "root",
        PasswordHash = "4813494d137e1631bba301d5acab6e7bb7aa74ce1185d456565ef51d737677b2",
        Salt = "",
        IsAdmin = true
    };
}
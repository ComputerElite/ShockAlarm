using System.ComponentModel.DataAnnotations.Schema;

namespace ShockAlarm.Alarm;

public class OpenShockApiData<T>
{
    public T data { get; set; }
}

public class OpenShockDevicesContainer
{
    public List<OpenShockDevice> devices { get; set; }
}

public class OpenShockDevice
{
    public string name { get; set; }
    public string id { get; set; }
    public List<OpenShockShocker> shockers { get; set; }
}

public class OpenShockShocker
{
    public string name { get; set; }
    public string id { get; set; }
    public bool isPaused { get; set; }
    public bool isDisabled { get; set; }
    public OpenShockShockerLimits limits { get; set; }
    public OpenShockShockerPermissions permissions { get; set; }
}

public class OpenShockShockerLimits
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }

    public byte intensity { get; set; } = 100;
    public ushort duration { get; set; } = 30000;
}

public class OpenShockShockerPermissions
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }
    public bool shock { get; set; } = true;
    public bool vibrate { get; set; } = true;
    public bool sound { get; set; } = true;
}
using System;

namespace TaskManagerApi.Models.Settings;

public class ServerSettings
{
    public ClientConfig Client { get; set; }
    public ApiServices ApiServices { get; set; }
    public ConnectionStrings ConnectionStrings { get; set; }
    public string OwnIp { get; set; }
}

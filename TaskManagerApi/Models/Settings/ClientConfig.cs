using System;

namespace TaskManagerApi.Models.Settings;

public class ClientConfig
{
    public string ClientId { get; set; }
    public string Authority { get; set; }
    public string ResponseType { get; set; }
    public bool SaveTokens { get; set; }
    public bool UsePkce { get; set; }
    public string CallbackPath { get; set; }
    public string[] Scopes { get; set; }
    public string GetClaimsFromUserInfoEndpoint { get; set; }
}

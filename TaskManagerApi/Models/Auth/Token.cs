using System;

namespace TaskManagerApi.Models.Auth;

public class Token
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}

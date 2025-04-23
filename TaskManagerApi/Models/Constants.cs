using System;

namespace TaskManagerApi.Models;

public class Constants
{
    public static class IdentityCustomOpenId
    {
        public static class DetailsFromToken
        {
            public const string ACCOUNT_ID = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
        }
    }
}

using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace HermeApp.Web.AdditionalClasses
{
    public class EmailBasedUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.Name)?.Value!;
        }
    }
}

using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Emprise.MudServer.Hubs
{
    public class UserIdProvider : IMyUserIdProvider
    {


        public string GetUserId(HubConnectionContext context)
        {
            var userId = context.User?.FindFirst($"_PlayerId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            return userId;
        }
    }

}

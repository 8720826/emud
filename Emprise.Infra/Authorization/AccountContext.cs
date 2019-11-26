using Emprise.Domain.Core.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Emprise.Infra.Authorization
{
    public class AccountContext : IAccountContext
    {
        private readonly IHttpContextAccessor _accessor;

        public AccountContext(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }


        public int UserId
        {
            get
            {
                int.TryParse(_accessor.HttpContext.User?.FindFirst($"_UserId")?.Value, out int id);
                return id;
            }
        }

        /// <summary>
        /// 聊天ID
        /// </summary>
        public string ConnectionId
        {
            get
            {
                return _accessor.HttpContext.User?.FindFirst($"_ConnectionId")?.Value;
            }
        }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email
        {
            get
            {
                return _accessor.HttpContext.User?.FindFirst($"_Email")?.Value;
            }
        }

        public int PlayerId
        {
            get
            {
                int.TryParse(_accessor.HttpContext.User?.FindFirst($"_PlayerId")?.Value, out int id);
                return id;
            }
        }

        public string PlayerName
        {
            get
            {
                return _accessor.HttpContext.User?.FindFirst($"_PlayerName")?.Value;
            }
        }

        public int RoomId
        {
            get
            {
                int.TryParse(_accessor.HttpContext.User?.FindFirst($"_RoomId")?.Value, out int id);
                return id;
            }
        }

        public int FactionId
        {
            get
            {
                int.TryParse(_accessor.HttpContext.User?.FindFirst($"_FactionId")?.Value, out int id);
                return id;
            }
        }

        public bool IsAuthenticated()
        {
            return _accessor.HttpContext.User.Identity.IsAuthenticated;
        }

        public IEnumerable<Claim> GetClaimsIdentity()
        {
            return _accessor.HttpContext.User.Claims;
        }
    }
}

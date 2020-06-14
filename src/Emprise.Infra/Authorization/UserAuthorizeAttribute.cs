using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Infra.Authorization
{

    public class UserAuthorizeAttribute : AuthorizeAttribute
    {
        public const string CustomerAuthenticationScheme = "user";
        public UserAuthorizeAttribute()
        {
            this.AuthenticationSchemes = CustomerAuthenticationScheme;
        }
    }
}

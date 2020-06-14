using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Infra.Authorization
{

    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        public const string CustomerAuthenticationScheme = "admin";
        public AdminAuthorizeAttribute()
        {
            this.AuthenticationSchemes = CustomerAuthenticationScheme;
        }
    }
}

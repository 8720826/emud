using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Infra.Authorization
{
    public class ApiAuthenticationAttribute : AuthorizeAttribute
    {
        public ApiAuthenticationAttribute() : base()
        {
            AuthenticationSchemes = JwtAuthenticationScheme.Api;
        }
    }

    public static class JwtAuthenticationScheme
    {
        public const string Api = "Api";
    }
}

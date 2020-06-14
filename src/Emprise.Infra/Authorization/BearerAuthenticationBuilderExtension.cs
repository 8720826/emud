using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;


namespace Emprise.Infra.Authorization
{
    public static class BearerAuthenticationBuilderExtension
    {


        public static AuthenticationBuilder AddHubAuth(this AuthenticationBuilder builder)
        {
            return builder.AddCookie("user", x => {
                x.Cookie.Name = "user";
                x.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents
                {
                    OnValidatePrincipal = context => {
                        context.HttpContext.User = context.Principal;
                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}

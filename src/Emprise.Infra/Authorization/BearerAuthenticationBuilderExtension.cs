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
        public static AuthenticationBuilder AddJwtBearerAuth(this AuthenticationBuilder builder,string securityKey)
        {
            //app验证
            return builder.AddJwtBearer(JwtAuthenticationScheme.Api,
                option => {
                    option.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(securityKey.PadRight(16, '0'))),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters parameters) =>
                        {
                            return expires > DateTime.UtcNow;
                        }
                    };
                    option.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            context.HttpContext.User = context.Principal;
                            return Task.CompletedTask;
                        }
                    };

 
                }
            );
        }

        public static AuthenticationBuilder AddJwtBearerAuth(this AuthenticationBuilder builder)
        {
            //app验证
            return builder.AddJwtBearer(JwtAuthenticationScheme.Api,
                option => {
                    option.TokenValidationParameters = new TokenValidationParameters
                    {
                        LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters parameters) =>
                        {
                            return false;
                        }
                    };
                }
            );
        }
    }
}

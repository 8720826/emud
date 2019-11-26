using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Infra.Authorization
{
    public static class Identity
    {
        public static async Task SignIn(this HttpContext content, string authenticationScheme, JwtAccount account)
        {

            account.ConnectionId = Guid.NewGuid().ToString().Replace("-", "");

            var claims = new List<Claim>();
            var props = account.GetType().GetProperties();
            foreach (var prop in props)
            {
                var value = prop.GetValue(account);
                if (value != null)
                {
                    claims.Add(new Claim($"_{prop.Name}", value.ToString()));
                }
               
            }

            var claimsIdentity = new ClaimsIdentity(authenticationScheme);
            claimsIdentity.AddClaims(claims);

            await content.SignInAsync(authenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties()
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24 * 30),//有效时间
                    AllowRefresh = true
                }
            );
        }

        public static async Task SignOut(this HttpContext content, string authenticationScheme)
        {
            await content.SignOutAsync(authenticationScheme);
        }

        public static async Task<bool> HasLogin(this HttpContext content)
        {
            return await Task.FromResult(content.User.Identity.IsAuthenticated);
        }
    }
}

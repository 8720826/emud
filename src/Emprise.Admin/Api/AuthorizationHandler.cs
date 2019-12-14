using Emprise.Admin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emprise.Admin.Api
{
    public class AuthorizationHandler : DelegatingHandler
    {
        private readonly IServiceProvider _services;
        readonly ILogger<AuthorizationHandler> _logger;
        public AuthorizationHandler(
            ILogger<AuthorizationHandler> logger
            , IServiceProvider services
           )
        {
            _services = services;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {

            using (var scope = _services.CreateScope())
            {
                var appConfig = scope.ServiceProvider.GetRequiredService<IOptions<AppConfig>>().Value;
                var httpAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();


                if (!appConfig.Site.IsApiEnable)
                {
                    return new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK };
                }

                if (request.Headers.Authorization == null)
                {
                    var claim = httpAccessor.HttpContext.User.Claims;

                    //签名证书(秘钥，加密算法)
                    var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appConfig.Site.ApiKey.PadRight(16, '0'))), SecurityAlgorithms.HmacSha256);

                    //生成token  [注意]需要nuget添加Microsoft.AspNetCore.Authentication.JwtBearer包，并引用System.IdentityModel.Tokens.Jwt命名空间
                    var jwtSecurityToken = new JwtSecurityToken("Issuer", "Audience", claim, DateTime.Now, DateTime.Now.AddMinutes(3), creds);

                    var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }


                return await base.SendAsync(request, cancellationToken);
            }
        }
    }
}

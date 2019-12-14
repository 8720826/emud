using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Admin.Api
{
    public static class MudClientBuilderExtension
    {
        public static IHttpClientBuilder AddMudRefit(this IServiceCollection services, string baseAddress)
        {
            return services.AddRefitClient<IMudClient>(new RefitSettings
            {
                ContentSerializer = new JsonContentSerializer(
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    })
            })
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(baseAddress);
            });
        }
    }
}

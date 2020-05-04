using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Models.Config;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Emprise.Admin.Pages.Config
{
    public class IndexModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly AppConfig _appConfig;
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;
        private IDatabase _db;
        public IndexModel(IMapper mapper, IOptionsMonitor<AppConfig> appConfig, ILogger<IndexModel> logger, IConfiguration configuration)
        {
            _mapper = mapper;
            _appConfig = appConfig.CurrentValue;
            _logger = logger;
            _configuration = configuration;

            var redis = ConnectionMultiplexer.Connect(_configuration.GetConnectionString("Redis"));
            _db = redis.GetDatabase();
        }



        public string Tips { get; set; }
        public string SueccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public List<ConfigDto> ConfigDtos { get; set; }

        [BindProperty]
        public string UrlReferer { get; set; }

        public Dictionary<string, string> Configs = new Dictionary<string, string>();


        public async Task OnGetAsync()
        {
            UrlReferer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/Config/Index");
            }

            ConfigDtos = new List<ConfigDto>();

            var configurations = _db.HashGetAll("configurations");
            Configs = configurations.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());


            GetInfoPropertys(typeof(AppConfig));

            /*
            var props = typeof(AppConfig).GetProperties();
            foreach (var prop in props)
            {
                var name = prop.Name;
                var value = prop.GetValue(_appConfig)?.ToString();
                if (prop.PropertyType.IsClass)
                {
                    var props2 = prop.PropertyType.GetProperties();
                    foreach (var prop2 in props2)
                    {
                        var name2 = prop2.Name;

                        var value2 = "";// prop2.GetValue(value)?.ToString();
                         var attribute2 = prop2.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;

                         ConfigDtos.Add(new ConfigDto
                         {
                             Key = $"{name}:{name2}",
                             Name = attribute2?.Name ?? name2,
                             Value = value2?.ToString()
                         });
                    }

                }
                else
                {
                    
                    var attribute = prop.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;

                    ConfigDtos.Add(new ConfigDto
                    {
                        Key = name,
                        Name = attribute?.Name ?? name,
                        Value = value
                    });
                }

            }
            */
        }

        public void GetInfoPropertys(Type type , string parentName="")
        {


            PropertyInfo[] props = type.GetProperties();
            foreach (PropertyInfo prop in props)
            {
             
                string name = prop.Name;
                string key = string.IsNullOrEmpty(parentName) ? $"{name}" : $"{parentName}:{name}";
               
                if (prop.PropertyType.IsValueType || prop.PropertyType.Name.StartsWith("String"))
                {
                    Configs.TryGetValue(key, out string value);
                    var attribute = prop.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute;
                    ConfigDtos.Add(new ConfigDto
                    {
                        Key = key,
                        Name = attribute?.DisplayName ?? name,
                        Value = value, 
                        Type = prop.PropertyType
                    });
                }
                else
                {
                    GetInfoPropertys(prop.PropertyType, key);
                }

            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            SueccessMessage = "";
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                ErrorMessage = ModelState.Where(e => e.Value.Errors.Count > 0).Select(e => e.Value.Errors.First().ErrorMessage).First();
                return Page();
            }
            try
            {
                SetValueFrom(typeof(AppConfig));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
            }


            foreach (var config in Configs)
            {
                _db.HashSet("configurations", config.Key, config.Value);
            }

        

            SueccessMessage = $"添加成功！";


            return Redirect(UrlReferer);
        }


        public void SetValueFrom(Type type, string parentName = "")
        {
            PropertyInfo[] props = type.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                string name = prop.Name;
                string key = string.IsNullOrEmpty(parentName) ? $"{name}" : $"{parentName}:{name}";
                if (prop.PropertyType.IsValueType || prop.PropertyType.Name.StartsWith("String"))
                {
                    var value = Request.Form[key];
                    Configs.TryAdd(key, value);
                }
                else
                {
                    SetValueFrom(prop.PropertyType,key);

                }

            }

        }
    }
}
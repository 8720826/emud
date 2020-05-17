using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Models.Config;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using Emprise.Admin.Data;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Emprise.Domain.Core.Enums;
using Emprise.Admin.Api;

namespace Emprise.Admin.Pages.Config
{
    public class EditModel : BasePageModel
    {
        private readonly IConfiguration _configuration;
        private IDatabase _redisDb;
        public EditModel(IMudClient mudClient,
            IMapper mapper,
            ILogger<EditModel> logger,
            EmpriseDbContext db,
            IOptionsMonitor<AppConfig> appConfig,
            IHttpContextAccessor httpAccessor,
            IConfiguration configuration)
            : base(db, appConfig, httpAccessor, mapper, logger, mudClient)
        {
            _configuration = configuration;
            var redis = ConnectionMultiplexer.Connect(_configuration.GetConnectionString("Redis"));
            _redisDb = redis.GetDatabase();
        }

        public string ErrorMessage { get; set; }

        public List<ConfigDto> ConfigDtos { get; set; }

        [BindProperty]
        public string UrlReferer { get; set; }

        public Dictionary<string, string> Configs = new Dictionary<string, string>();

        public Dictionary<string, string> NewConfigs = new Dictionary<string, string>();
        public async Task OnGetAsync()
        {
            UrlReferer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/Config/Index");
            }

            ConfigDtos = new List<ConfigDto>();

            var configurations = _redisDb.HashGetAll("configurations");
            Configs = configurations.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());


            GetValue(typeof(AppConfig));

        }

        public void GetValue(Type type, string parentName = "")
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
                    GetValue(prop.PropertyType, key);
                }

            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                ErrorMessage = ModelState.Where(e => e.Value.Errors.Count > 0).Select(e => e.Value.Errors.First().ErrorMessage).First();
                return Page();
            }
            try
            {
                SetValue(typeof(AppConfig));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
            }

            var configurations = _redisDb.HashGetAll("configurations");
            Configs = configurations.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());

            string content = "";
            foreach (var config in NewConfigs)
            {
                if (!Configs.ContainsKey(config.Key))
                {
                    content += $"[{config.Key}]:''=>'{config.Value}';\r\n";
                    _redisDb.HashSet("configurations", config.Key, config.Value);
                }
                else if (Configs[config.Key]!= config.Value)
                {
                    content += $"[{config.Key}]:'{Configs[config.Key]}'=>'{config.Value}';\r\n";
                    _redisDb.HashSet("configurations", config.Key, config.Value);
                }
            }

            

            await AddSuccess(new OperatorLog
            {
                Type = OperatorLogType.修改配置,
                Content = content
            });

            return Redirect(UrlReferer);
        }


        public void SetValue(Type type, string parentName = "")
        {
            PropertyInfo[] props = type.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                string name = prop.Name;
                string key = string.IsNullOrEmpty(parentName) ? $"{name}" : $"{parentName}:{name}";
                if (prop.PropertyType.IsValueType || prop.PropertyType.Name.StartsWith("String"))
                {
                    var value = Request.Form[key];
                    NewConfigs.TryAdd(key, value);
                }
                else
                {
                    SetValue(prop.PropertyType, key);

                }

            }

        }
    }
}
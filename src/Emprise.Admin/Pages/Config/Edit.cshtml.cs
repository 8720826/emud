using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Emprise.Domain.Core.Enums;
using Emprise.Admin.Api;
using Emprise.Application.Config.Services;
using Emprise.Application.Config.Dtos;
using Emprise.Domain.Config.Models;

namespace Emprise.Admin.Pages.Config
{
    public class EditModel : BasePageModel
    {

        private readonly IConfigAppService _configAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public EditModel(
            ILogger<EditModel> logger,
            IConfigAppService configAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _configAppService = configAppService;
            _appConfig = appConfig.CurrentValue;

        }

        public string ErrorMessage { get; set; }

        public List<ConfigModel> Configs { get; set; }



        //public Dictionary<string, string> Configs = new Dictionary<string, string>();

        public Dictionary<string, string> NewConfigs = new Dictionary<string, string>();
        public async Task OnGetAsync()
        {
            Configs = await _configAppService.GetConfigs();

        }

        /*
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
        */
        public async Task<IActionResult> OnPostAsync()
        {
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                ErrorMessage = ModelState.Where(e => e.Value.Errors.Count > 0).Select(e => e.Value.Errors.First().ErrorMessage).First();
                return Page();
            }

            var dic = Request.Form.Distinct().ToDictionary(x => x.Key, x => x.Value.ToString());
            var result = await _configAppService.UpdateConfigs(dic); ;
            if (!result.IsSuccess)
            {
                ErrorMessage = result.Message;
                return Page();
            }
            else
            {
                return RedirectToPage("./Index");
            }

            /*
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
            */
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
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

namespace Emprise.Admin.Pages.Config
{
    public class EditModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;
        private IDatabase _db;
        public EditModel(IMapper mapper, ILogger<IndexModel> logger, IConfiguration configuration)
        {
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;

            var redis = ConnectionMultiplexer.Connect(_configuration.GetConnectionString("Redis"));
            _db = redis.GetDatabase();
        }

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


            foreach (var config in Configs)
            {
                _db.HashSet("configurations", config.Key, config.Value);
            }



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
                    Configs.TryAdd(key, value);
                }
                else
                {
                    SetValue(prop.PropertyType, key);

                }

            }

        }
    }
}
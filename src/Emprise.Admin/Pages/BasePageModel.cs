using AutoMapper;
using Emprise.Admin.Api;
using Emprise.Admin.Data;
using Emprise.Admin.Entity;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Models;
using Emprise.Infra.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Emprise.Admin.Pages
{
    public class BasePageModel : PageModel
    {
        protected readonly EmpriseDbContext _db;
        protected readonly AppConfig _appConfig;
        protected readonly IMapper _mapper;
        protected readonly ILogger<BasePageModel> _logger;
        protected readonly IMudClient _mudClient;
        protected readonly IHttpContextAccessor _httpAccessor;

        public BasePageModel(EmpriseDbContext db, IOptionsMonitor<AppConfig> appConfig, IHttpContextAccessor httpAccessor, IMapper mapper, ILogger<BasePageModel> logger, IMudClient mudClient)
        {
            _appConfig = appConfig.CurrentValue;
            _db = db;
            _httpAccessor = httpAccessor;
            _mapper = mapper;
            _logger = logger;
            _mudClient = mudClient;
        }

        protected async Task AddSuccess(OperatorLog operatorLog)
        {
            _db.OperatorLogs.Add(new OperatorLogEntity
            {
                AdminName = HttpContext.User.Identity.Name ?? operatorLog.Name,
                CreateDate = DateTime.Now,
                Content = operatorLog.Content,
                IsSuccess = true,
                Type = operatorLog.Type,
                Ip = _httpAccessor.HttpContext.GetUserIp()
            });
            await _db.SaveChangesAsync();
        }

        protected async Task AddError(OperatorLog operatorLog)
        {
            _db.OperatorLogs.Add(new OperatorLogEntity
            {
                AdminName = HttpContext.User.Identity.Name ?? operatorLog.Name,
                CreateDate = DateTime.Now,
                Content = operatorLog.Content,
                IsSuccess = false,
                Type = operatorLog.Type,
                Ip = _httpAccessor.HttpContext.GetUserIp()
            });
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// 比较实体差异
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="source">修改前</param>
        /// <param name="current">修改后</param>
        /// <returns></returns>
        protected string DifferenceComparison<T1, T2>(T1 source, T2 current)
        {
            string diff = "";
            Type t1 = source.GetType();
            Type t2 = current.GetType();
            PropertyInfo[] property2 = t2.GetProperties();
            //排除主键和基础字段
            List<string> exclude = new List<string>() { "Id" };
            foreach (PropertyInfo p in property2)
            {
                string name = p.Name;
                if (exclude.Contains(name)) { continue; }
                var value1 = t1.GetProperty(name)?.GetValue(source, null)?.ToString();
                var value2 = p.GetValue(current, null)?.ToString();
                if (value1 != value2)
                {
                    diff += $"[{name}]:'{value1}'=>'{value2}';\r\n";
                }
            }
            return diff;
        }
    }

    public class OperatorLog
    {
        public OperatorLogType Type { set; get; }

        public string Content { set; get; }

        public string Name { set; get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Emprise.Admin.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Enums;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Emprise.Admin.Pages
{
    public class LogoutModel : BasePageModel
    {
        public LogoutModel(IMapper mapper, ILogger<LogoutModel> logger,EmpriseDbContext db,  IOptionsMonitor<AppConfig> appConfig, IHttpContextAccessor httpAccessor)
            : base(db, appConfig, httpAccessor, mapper, logger)
        {

        }
        public async Task<IActionResult> OnGetAsync()
        {
            await HttpContext.SignOutAsync("admin");

            await AddSuccess(new OperatorLog
            {
                Type = OperatorLogType.退出登录,
                Content = ""
            });
            return RedirectToPage("Login");
        }
    }
}
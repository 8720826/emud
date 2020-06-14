using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Emprise.Domain.Core.Models;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Emprise.Application.Admin.Services;

namespace Emprise.Web.Areas.Admin.Pages
{
    public class LogoutModel : BaseAdminPageModel
    {
        private readonly IAdminAppService _adminAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public LogoutModel(
            ILogger<LogoutModel> logger,
            IAdminAppService adminAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _adminAppService = adminAppService;
            _appConfig = appConfig.CurrentValue;

        }
        public async Task<IActionResult> OnGetAsync()
        {
            var result = await _adminAppService.Logout();
            if (!result.IsSuccess)
            {
                return Page();
            }
            else
            {
                return RedirectToPage("Login");
            }
        }
    }
}
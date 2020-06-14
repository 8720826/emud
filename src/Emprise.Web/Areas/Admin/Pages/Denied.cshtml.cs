using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emprise.Web.Areas.Admin.Pages
{
    public class DeniedModel : BaseAdminPageModel
    {
        private readonly AppConfig _appConfig;
        public DeniedModel(
            ILogger<DeniedModel> logger,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _appConfig = appConfig.CurrentValue;

        }

        public void OnGet()
        {

        }
    }
}
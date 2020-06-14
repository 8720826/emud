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
    public class IndexModel : BaseAdminPageModel
    {

        private readonly AppConfig _appConfig;
        public IndexModel(
            ILogger<IndexModel> logger,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _appConfig = appConfig.CurrentValue;

        }

        public string SiteName = "";


        public void OnGet()
        {
            SiteName = _appConfig?.Site?.Name;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emprise.Admin.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AppConfig _appConfig;

        public IndexModel(ILogger<IndexModel> logger, IOptionsMonitor<AppConfig> appConfig)
        {
            _logger = logger;
            _appConfig = appConfig.CurrentValue;
        }

        public string SiteName="";


        public void OnGet()
        {
            SiteName = _appConfig?.Site?.Name;
        }
    }
}

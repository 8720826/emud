using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Emprise.Web.Pages.User
{
    public class VerifyEmailModel : PageModel
    {
        public SiteConfig SiteConfig { get; set; }

        private readonly AppConfig _appConfig;
        public VerifyEmailModel(IOptions<AppConfig> appConfig)
        {
            _appConfig = appConfig.Value;


        }
        public void OnGet()
        {
            SiteConfig = _appConfig.Site;
        }
    }
}
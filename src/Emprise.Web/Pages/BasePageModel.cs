using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Notifications;
using Emprise.Infra;
using Emprise.Infra.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Web.Pages
{
    [UserAuthorize]
    public class BasePageModel : PageModel
    {
        public SiteConfig SiteConfig { get; set; }

        public BasePageModel(IOptionsMonitor<AppConfig> appConfig)
        {
            SiteConfig = appConfig.CurrentValue.Site?? new SiteConfig
            {
                Name = "Emprise Mud"
            };
        }
    }
}

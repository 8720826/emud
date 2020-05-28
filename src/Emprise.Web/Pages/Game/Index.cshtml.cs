using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Domain.Core.Authorization;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Emprise.Web.Pages.Game
{
    public class IndexModel : BasePageModel
    {
        private readonly IAccountContext _account;

        public IndexModel(IAccountContext account, IOptionsMonitor<AppConfig> appConfig) : base(appConfig)
        {
            _account = account;
        }

        public string Title { get; set; }

        public IActionResult OnGet()
        {
            if (_account.PlayerId == 0)
            {
                return Content("请重新进入游戏");
            }

            Title = _appConfig.Site.Name;


            return Page();
        }
    }
}
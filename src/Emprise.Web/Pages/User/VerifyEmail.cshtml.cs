using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Domain.Core.Authorization;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Emprise.Web.Pages.User
{
    [AllowAnonymous]
    public class VerifyEmailModel : BasePageModel
    {
        private readonly IAccountContext _accountContext;
        private readonly AppConfig _appConfig;
        public VerifyEmailModel(IAccountContext accountContext, IOptions<AppConfig> appConfig) : base(appConfig)
        {
            _appConfig = appConfig.Value;
            _accountContext = accountContext;


        }

        public bool HasLogin { get; set; } = false; 

        public string Email { get; set; }

        public void OnGet()
        {
            SiteConfig = _appConfig.Site;

            if (_accountContext != null)
            {
                HasLogin = true;
                Email = _accountContext.Email;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Models;
using Emprise.Infra.Authorization;
using Emprise.Infra.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Emprise.Web.Pages.User
{
    [AllowAnonymous]
    public class RegModel : BasePageModel
    {
        public RegModel(IOptionsMonitor<AppConfig> appConfig) : base(appConfig)
        {

        }

        public bool IsNeedEmail { get; set; }

        public bool IsNeedVerifyEmail { get; set; }

        public string Email { get; set; }

        public string Code { get; set; }

        public async Task<IActionResult> OnGetAsync(string email, string code)
        {

            IsNeedEmail = _appConfig.Site.IsNeedEmail;

            IsNeedVerifyEmail = _appConfig.Site.IsNeedVerifyEmail;

            Email = email;
            Code = code;
            if (await HttpContext.HasLogin())
            {
                return RedirectToPage("/User/Index");
            }

            if (IsNeedVerifyEmail && string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
            {
                return RedirectToPage("/User/VerifyEmail");
            }
            if (!email.IsEmail())
            {
                return RedirectToPage("/User/VerifyEmail");
            }
            
            return Page();
        }
    }
}
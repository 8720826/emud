using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public RegModel(IOptions<AppConfig> appConfig) : base(appConfig)
        {
            IsNeedEmail = appConfig.Value.Site.IsNeedEmail;

            IsNeedVerifyEmail = appConfig.Value.Site.IsNeedVerifyEmail;
        }

        public bool IsNeedEmail { get; set; }

        public bool IsNeedVerifyEmail { get; set; }

        public string Email { get; set; }

        public string Code { get; set; }

        public async Task<IActionResult> OnGetAsync(string email, string code)
        {
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
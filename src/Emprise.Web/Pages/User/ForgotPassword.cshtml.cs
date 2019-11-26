using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Domain.Core.Models;
using Emprise.Infra.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Emprise.Web.Pages.User
{
    [AllowAnonymous]
    public class ForgotPasswordModel : BasePageModel
    {
        public ForgotPasswordModel(IOptions<AppConfig> appConfig) : base(appConfig)
        {

        }
        public async Task<IActionResult> OnGetAsync()
        {
            if (await HttpContext.HasLogin())
            {
                return RedirectToPage("/User/Index");
            }
            return Page();
        }
    }
}
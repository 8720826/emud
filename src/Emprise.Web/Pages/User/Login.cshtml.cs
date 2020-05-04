using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Application.User.Services;
using Emprise.Domain.Core.Authorization;
using Emprise.Domain.Core.Models;
using Emprise.Infra.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Emprise.Web.Pages.User
{
    [AllowAnonymous]
    public class LoginModel : BasePageModel
    {
        public LoginModel(IOptionsMonitor<AppConfig> appConfig) : base(appConfig)
        {

        }
        public string Email { get; set; }



        public async Task<IActionResult> OnGetAsync(string email)
        {
            Email = email;
            if (await HttpContext.HasLogin())
            {
                return RedirectToPage("/User/Index");
            }



            return Page();
        }
    }
}
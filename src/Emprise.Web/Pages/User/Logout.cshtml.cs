using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Application.User.Services;
using Emprise.Domain.Core.Authorization;
using Emprise.Infra.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Emprise.Web.Pages.User
{
    public class LogoutModel : PageModel
    {
        private readonly IAccountContext _accountContext;
        private readonly IHttpContextAccessor _httpAccessor;


        public LogoutModel(IAccountContext accountContext, IHttpContextAccessor httpAccessor)
        {
            _accountContext = accountContext;
            _httpAccessor = httpAccessor;
        }


        public async Task<IActionResult> OnGetAsync()
        {
            await _httpAccessor.HttpContext.SignOut("user");

            return RedirectToPage("/User/Login");
        }
    }
}

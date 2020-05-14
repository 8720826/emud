using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Admin.Data;
using Emprise.Admin.Models.User;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Emprise.Admin.Pages.User
{
    public class PasswordModel : PageModel
    {
        protected readonly EmpriseDbContext _db;
        private readonly AppConfig _appConfig;

        public PasswordModel(EmpriseDbContext db, IOptionsMonitor<AppConfig> appConfig)
        {
            _db = db;
            _appConfig = appConfig.CurrentValue;
        }

        public string ErrorMessage { get; set; }


        [BindProperty]
        public ModifyPasswordInput ModifyPasswordInput { get; set; }


        [BindProperty]
        public string UrlReferer { get; set; }

        public void OnGet(int id)
        {
            UrlReferer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/User/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }



            try
            {
                var user = await _db.Users.FindAsync(id);
                if (user == null)
                {
                    ErrorMessage = "用户不存在";
                    return Page();
                }
                user.Password = ModifyPasswordInput.NewPassword.ToMd5();
                await _db.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }




            return Redirect(UrlReferer);
        }
    }
}
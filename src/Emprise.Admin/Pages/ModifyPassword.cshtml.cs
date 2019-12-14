using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Admin.Data;
using Emprise.Admin.Extensions;
using Emprise.Admin.Models;
using Emprise.Admin.Models.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Emprise.Admin.Pages
{
    public class ModifyPasswordModel : PageModel
    {
        protected readonly EmpriseDbContext _db;
        private readonly AppConfig _appConfig;

        public ModifyPasswordModel(EmpriseDbContext db, IOptions<AppConfig> appConfig)
        {
            _db = db;
            _appConfig = appConfig.Value;
        }

        public string SueccessMessage { get; set; }
        public string ErrorMessage { get; set; }


        [BindProperty]
        public ModifyPasswordInput ModifyPasswordInput { get; set; }


        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            SueccessMessage = "";
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                ErrorMessage = ModelState.Where(e => e.Value.Errors.Count > 0).Select(e => e.Value.Errors.First().ErrorMessage).First();
                return Page();
            }

            var admin = await _db.Admins.FirstOrDefaultAsync(x => x.Name == "admin");
            if (admin == null)
            {
                ErrorMessage = "账号或密码错误";
                return Page();
            }

            if (admin.Password != ModifyPasswordInput.Password.ToMd5())
            {
                ErrorMessage = "账号或密码错误";
                return Page();
            }

            if (ModifyPasswordInput.NewPassword != ModifyPasswordInput.ConfirmPassword)
            {
                ErrorMessage = "两次新密码必须一致";
                return Page();
            }

            _db.Attach(admin).State = EntityState.Modified;
            admin.Password = ModifyPasswordInput.NewPassword.ToMd5();

            await _db.SaveChangesAsync();

            SueccessMessage = "修改成功！";

            return Page();
        }
    }
}
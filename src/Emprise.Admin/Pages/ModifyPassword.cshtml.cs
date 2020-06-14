using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.Admin.Dtos;
using Emprise.Application.Admin.Services;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emprise.Admin.Pages
{
    public class ModifyPasswordModel : BasePageModel
    {
        private readonly IAdminAppService _adminAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public ModifyPasswordModel(
            ILogger<ModifyPasswordModel> logger,
            IAdminAppService adminAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _adminAppService = adminAppService;
            _appConfig = appConfig.CurrentValue;

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

            var result = await _adminAppService.ModifyPassword(this.User.Identity.Name, ModifyPasswordInput);
            if (!result.IsSuccess)
            {
                ErrorMessage = result.Message;

                return Page();
            }
            else
            {
                return RedirectToPage("Index");
            }

            /*
            var admin = await _db.Admins.FirstOrDefaultAsync(x => x.Name == this.User.Identity.Name);
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

            return Page();*/
        }
    }
}
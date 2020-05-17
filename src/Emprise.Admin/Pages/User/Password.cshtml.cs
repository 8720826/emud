using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Api;
using Emprise.Admin.Data;
using Emprise.Admin.Models.User;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Admin.Pages.User
{
    public class PasswordModel : BasePageModel
    {

        public PasswordModel(IMudClient mudClient,
            IMapper mapper,
            ILogger<PasswordModel> logger,
            EmpriseDbContext db,
            IOptionsMonitor<AppConfig> appConfig,
            IHttpContextAccessor httpAccessor)
            : base(db, appConfig, httpAccessor, mapper, logger, mudClient)
        {

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

                await AddSuccess(new OperatorLog
                {
                    Type = OperatorLogType.修改玩家,
                    Content = $"id={id}，{JsonConvert.SerializeObject(ModifyPasswordInput)}"
                });

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await AddError(new OperatorLog
                {
                    Type = OperatorLogType.修改玩家,
                    Content = $"id={id}，Data={JsonConvert.SerializeObject(ModifyPasswordInput)},ErrorMessage={ErrorMessage}"
                });
                return Page();
            }




            return Redirect(UrlReferer);
        }
    }
}
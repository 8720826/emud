using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Api;
using Emprise.Admin.Data;
using Emprise.Admin.Models.Email;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Admin.Pages.Email
{
    public class EditModel : BasePageModel
    {

        public EditModel(IMudClient mudClient,
            IMapper mapper,
            ILogger<EditModel> logger,
            EmpriseDbContext db,
            IOptionsMonitor<AppConfig> appConfig,
            IHttpContextAccessor httpAccessor)
            : base(db, appConfig, httpAccessor, mapper, logger, mudClient)
        {

        }

        [BindProperty]
        public EmailInput Email { get; set; }

        public string ErrorMessage { get; set; }


        [BindProperty]
        public string UrlReferer { get; set; }



        public async Task<IActionResult> OnGetAsync(int id)
        {
            UrlReferer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/Email/Index");
            }

            if (id > 0)
            {
                var email = await _db.Emails.FindAsync(id);

                Email = _mapper.Map<EmailInput>(email);

                return Page();
            }
            else
            {
                return RedirectToPage("/Email/Index");
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
                var email = await _db.Emails.FindAsync(id);
                if (email == null)
                {
                    ErrorMessage = $"邮件 {id} 不存在！";
                    return Page();
                }
                var content = DifferenceComparison(email, Email);
                _mapper.Map(Email, email);

                await _db.SaveChangesAsync();



                await AddSuccess(new OperatorLog
                {
                    Type = OperatorLogType.修改邮件,
                    Content = $"Id = {id},Data = {content}"
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await AddError(new OperatorLog
                {
                    Type = OperatorLogType.修改邮件,
                    Content = $"Id = {id},Data={JsonConvert.SerializeObject(Email)},ErrorMessage={ErrorMessage}"
                });
                return Page();
            }

            return Redirect(UrlReferer);
        }
    }
}
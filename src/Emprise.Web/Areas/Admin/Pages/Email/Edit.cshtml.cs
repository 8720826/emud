using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.Email.Dtos;
using Emprise.Application.Email.Services;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Web.Areas.Admin.Pages.Email
{
    public class EditModel : BaseAdminPageModel
    {

        private readonly IEmailAppService _emailAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public EditModel(
            ILogger<EditModel> logger,
            IEmailAppService emailAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _emailAppService = emailAppService;
            _appConfig = appConfig.CurrentValue;

        }

        [BindProperty]
        public EmailInput Email { get; set; }

        public string ErrorMessage { get; set; }




        public async Task<IActionResult> OnGetAsync(int id)
        {

            if (id > 0)
            {
                var email = await _emailAppService.Get(id);

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

            var result = await _emailAppService.Update(id, Email);
            if (!result.IsSuccess)
            {
                ErrorMessage = result.Message;
                return Page();
            }
            else
            {
                return RedirectToPage("/Email/Index");
            }

            /*
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
            */
        }
    }
}
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
    public class AddModel : BaseAdminPageModel
    {

        private readonly IEmailAppService _emailAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public AddModel(
            ILogger<AddModel> logger,
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

        [BindProperty]
        public string UrlReferer { get; set; }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _emailAppService.Add(Email);
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
                var email = _mapper.Map<EmailEntity>(Email);
                await _db.Emails.AddAsync(email);
                await _db.SaveChangesAsync();

                await AddSuccess(new OperatorLog
                {
                    Type = OperatorLogType.发送邮件,
                    Content = JsonConvert.SerializeObject(Email)
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;

                await AddError(new OperatorLog
                {
                    Type = OperatorLogType.发送邮件,
                    Content = $"Data={JsonConvert.SerializeObject(Email)},ErrorMessage={ErrorMessage}"
                });
                return Page();
            }



            return Redirect(UrlReferer);
            */
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.User.Dtos;
using Emprise.Application.User.Services;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Web.Areas.Admin.Pages.User
{
    public class PasswordModel : BaseAdminPageModel
    {

        private readonly IUserAppService _userAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public PasswordModel(
            ILogger<PasswordModel> logger,
            IUserAppService userAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _userAppService = userAppService;
            _appConfig = appConfig.CurrentValue;
        }

        public string ErrorMessage { get; set; }


        [BindProperty]
        public ModifyPasswordInput ModifyPasswordInput { get; set; }



        public void OnGet(int id)
        {

        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _userAppService.ModifyPassword(id, ModifyPasswordInput);
            if (!result.IsSuccess)
            {
                ErrorMessage = result.Message;
                return Page();
            }
            else
            {
                return RedirectToPage("/Ware/Index");
            }
        }
    }
}
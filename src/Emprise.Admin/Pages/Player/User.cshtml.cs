using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Api;
using Emprise.Application.Player.Services;
using Emprise.Application.User.Services;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Models;
using Emprise.Domain.User.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emprise.Admin.Pages.Player
{
    public class UserModel : BasePageModel
    {

        private readonly IPlayerAppService _playerAppService;
        private readonly IUserAppService _userAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public UserModel(
            ILogger<UserModel> logger,
            IPlayerAppService playerAppService,
            IUserAppService userAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _playerAppService = playerAppService;
            _userAppService = userAppService;
            _appConfig = appConfig.CurrentValue;

        }


        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }

        public UserEntity User { get; set; }

        public async Task OnGetAsync(int id)
        {
            User = await _playerAppService.GetUser(id);

        }


        public async Task<IActionResult> OnPostAsync([FromBody]EnableData enableData)
        {
            await _userAppService.SetEnabled(enableData.SId, enableData.IsEnable);

            return await Task.FromResult(new JsonResult(enableData));

        }

        public class EnableData
        {
            public int SId { get; set; }
            public bool IsEnable { get; set; }
        }
    }
}
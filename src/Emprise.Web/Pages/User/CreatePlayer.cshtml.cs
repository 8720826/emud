using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Application.Player.Services;
using Emprise.Application.User.Models;
using Emprise.Application.User.Services;
using Emprise.Domain.Core.Authorization;
using Emprise.Domain.Core.Entity;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Player.Entity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Emprise.Web.Pages.User
{
    public class CreatePlayerModel : BasePageModel
    {
        private readonly IAccountContext _accountContext;
        private readonly IUserAppService _userAppService;
        private readonly IPlayerAppService _playerAppService;
        public CreatePlayerModel(IAccountContext accountContext, IUserAppService userAppService, IPlayerAppService playerAppService, IOptionsMonitor<AppConfig> appConfig) : base(appConfig)
        {
            _accountContext = accountContext;
            _userAppService = userAppService;
            _playerAppService = playerAppService;
        }

        public UserModel UserModel { get; set; }

        public PlayerEntity Player { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            UserModel = await _userAppService.GetUser(_accountContext.UserId);
            if (UserModel == null)
            {
                await HttpContext.SignOutAsync();
                return RedirectToPage("/User/Index");
            }

            Player = await _playerAppService.GetUserPlayer(_accountContext.UserId);

            if (Player != null)
            {
                return RedirectToPage("/User/Index");
            }


            return Page();
        }
    }
}
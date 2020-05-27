using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Application.Player.Dtos;
using Emprise.Application.Player.Services;
using Emprise.Application.User.Models;
using Emprise.Application.User.Services;
using Emprise.Domain.Core.Authorization;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Entity;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Notifications;
using Emprise.Domain.Player.Entity;
using Emprise.MudServer.Commands;
using MediatR;
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
        private readonly IMediatorHandler _bus;
        private readonly DomainNotificationHandler _notifications;
        public CreatePlayerModel(IAccountContext accountContext, IUserAppService userAppService, IPlayerAppService playerAppService, 
            IOptionsMonitor<AppConfig> appConfig,
            INotificationHandler<DomainNotification> notifications,
            IMediatorHandler bus) : base(appConfig)
        {
            _accountContext = accountContext;
            _userAppService = userAppService;
            _playerAppService = playerAppService;
            _notifications = (DomainNotificationHandler)notifications;
            _bus = bus;
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

        public async Task<IActionResult> OnPostAsync([FromBody]PlayerCreateDto dto)
        {
            var userId = _accountContext.UserId;

            var command = new CreateCommand(dto.Name, dto.Gender, userId, dto.Str, dto.Con, dto.Dex, dto.Int);
            await _bus.SendCommand(command);

            if (_notifications.HasNotifications())
            {
                var errorMessage = string.Join("；", _notifications.GetNotifications().Select(x => x.Content));
                return await Task.FromResult(new JsonResult(new
                {
                    status = false,
                    errorMessage
                }));
            }

            return await Task.FromResult(new JsonResult(new
            {
                status = true
            }));

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Application.Player.Services;
using Emprise.Domain.Core.Authorization;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Notifications;
using Emprise.MudServer.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Emprise.Web.Pages.Game
{
    public class JoinModel : BasePageModel
    {
        private readonly IAccountContext _account;
        private readonly IPlayerAppService _playerAppService;
        private readonly DomainNotificationHandler _notifications;
        private readonly IMediatorHandler _bus;

        public JoinModel(IPlayerAppService playerAppService, IAccountContext account, 
            INotificationHandler<DomainNotification> notifications, IOptionsMonitor<AppConfig> appConfig, IMediatorHandler bus) : base(appConfig)
        {
            _account = account;
            _playerAppService = playerAppService;
            _notifications = (DomainNotificationHandler)notifications;
            _bus = bus;
        }

        public async Task<IActionResult> OnGetAsync(int playerId = 0)
        {
            if (playerId == 0)
            {
                return Content("请重新进入游戏");
            }

            var command = new JoinGameCommand(_account.UserId, playerId);
            await _bus.SendCommand(command);

            if (_notifications.HasNotifications())
            {
                var errorMessage = string.Join("；", _notifications.GetNotifications().Select(x => x.Content));
                return Content(errorMessage);
            }

            return RedirectToPage("/Game/Index");

        }
    }
}
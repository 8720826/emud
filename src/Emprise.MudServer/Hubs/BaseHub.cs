using Emprise.Application.Player.Services;
using Emprise.Domain.Core.Authorization;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Notifications;
using Emprise.MudServer.Hubs.Models;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.MudServer.Hubs
{
    public class BaseHub : Hub<IHubClient>
    {
        protected readonly IAccountContext _account;
        protected readonly DomainNotificationHandler _notifications;
        protected readonly IMudOnlineProvider _mudOnlineProvider;
        protected readonly AppConfig _appConfig;
        protected readonly IPlayerAppService _playerAppService;
        protected readonly ILogger<BaseHub> _logger;

        public BaseHub(IAccountContext account, INotificationHandler<DomainNotification> notifications, IMudOnlineProvider mudOnlineProvider, IOptions<AppConfig> appConfig, IPlayerAppService playerAppService, ILogger<MudHub> logger)
        {
            _account = account;
            _notifications = (DomainNotificationHandler)notifications;
            _mudOnlineProvider = mudOnlineProvider;
            _appConfig = appConfig.Value;
            _playerAppService = playerAppService;
            _logger = logger;
        }

        #region 连接

        /// <summary>
        /// 客户端重连接时
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            await JoinAsync();
            await base.OnConnectedAsync();
        }


        /// <summary>
        /// 断线
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }


        #endregion

        private async Task JoinAsync()
        {
            var connectionId = await _mudOnlineProvider.GetConnectionId(_account.PlayerId);
            if (!string.IsNullOrEmpty(connectionId) && connectionId != Context.ConnectionId)
            {
                await KickOut(connectionId);
            }

            await _mudOnlineProvider.SetConnectionId(_account.PlayerId, Context.ConnectionId);

            await Groups.AddToGroupAsync(Context.ConnectionId, "room_" + _account.RoomId);
            await Groups.AddToGroupAsync(Context.ConnectionId, "faction_" + _account.FactionId);

        

            await _playerAppService.InitGame(_account.PlayerId);
        }

        private bool IsValidOperation()
        {
            return (!_notifications.HasNotifications());
        }

        protected async Task<bool> DoCommand(Func<Task> func)
        {
            var connectionId = await _mudOnlineProvider.GetConnectionId(_account.PlayerId);
            if (string.IsNullOrEmpty(connectionId))
            {
                await ShowSystemMessage(Context.ConnectionId, "你已经断线，请刷新或重新登录");
                return await Task.FromResult(false);
            }

            if (connectionId != Context.ConnectionId)
            {
                //await KickOut(connectionId);
                await ShowSystemMessage(Context.ConnectionId, "你已经断线，请刷新或重新登录");
                return await Task.FromResult(false);
            }

            await func?.Invoke();


            if (!IsValidOperation())
            {
                foreach (var notification in _notifications.GetNotifications())
                {
                    await ShowSystemMessage(Context.ConnectionId, notification.Content);
                }
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
        }

        protected async Task ShowSystemMessage(string connectionId, string content)
        {
            var systemMessage = new SystemMessage()
            {
                Content = content
            };

            await Clients.Client(connectionId).ShowMessage(systemMessage);
        }

        protected async Task KickOut(string connectionId)
        {
            await ShowSystemMessage(connectionId, "您的帐号在其他地方登录，您已被迫下线！");
            await Clients.Client(connectionId).Offline();
 
        }

    }
}

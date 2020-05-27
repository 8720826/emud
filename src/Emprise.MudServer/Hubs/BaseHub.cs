using Emprise.Domain.Core.Authorization;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Notifications;
using Emprise.MudServer.Commands;
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
        public readonly ILogger<BaseHub> _logger;
        private readonly IMediatorHandler _bus;

        public BaseHub(IAccountContext account, INotificationHandler<DomainNotification> notifications, IMudOnlineProvider mudOnlineProvider, IOptionsMonitor<AppConfig> appConfig,  ILogger<MudHub> logger, IMediatorHandler bus)
        {
            _account = account;
            _notifications = (DomainNotificationHandler)notifications;
            _mudOnlineProvider = mudOnlineProvider;
            _appConfig = appConfig.CurrentValue;
            _logger = logger;
            _bus = bus;
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

        
            _logger.LogDebug($"InitGame:{_account.PlayerId}");
            var command = new InitGameCommand(_account.PlayerId);
            await _bus.SendCommand(command).ConfigureAwait(false); ;
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
                Content = content, 
                Type= MessageTypeEnum.提示
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

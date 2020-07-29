using DotNetCore.CAP;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Interfaces.Ioc;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Email.Entity;
using Emprise.Domain.Email.Services;
using Emprise.MudServer.Events.EmailEvents;
using Emprise.MudServer.Handles;
using Emprise.MudServer.JobHandlers;
using Emprise.MudServer.Queues;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.MudServer.Consumers
{
    public interface IPlayerLevelUpConsumer : ITransient
    {

        Task<bool> PlayerLevelUpQueueConsumer(PlayerLevelUpQueue message);
        

    }

    public class PlayerLevelUpConsumer : BaseConsumer, IPlayerLevelUpConsumer, ICapSubscribe
    {
        private readonly ILogger<ChatConsumer> _logger;
        private readonly IPlayerLevelUpHandler _playerLevelUpHandler;
        private readonly IMudProvider _mudProvider;
      
        public PlayerLevelUpConsumer(
            IPlayerLevelUpHandler playerLevelUpHandler,
            ILogger<ChatConsumer> logger, 
            IUnitOfWork uow,
            IMudProvider mudProvider,
            IRedisDb redisDb) : base(uow, redisDb)
        {
            _logger = logger;
            _playerLevelUpHandler = playerLevelUpHandler;
            _mudProvider = mudProvider;
        }

        [CapSubscribe("PlayerLevelUpQueue")]
        public async Task<bool> PlayerLevelUpQueueConsumer(PlayerLevelUpQueue message)
        {
            _logger.LogDebug($"Consumer Get Queue {JsonConvert.SerializeObject(message)} ready");

            await _playerLevelUpHandler.Execute(message.PlayerId);

            if (await Commit())
            {
                //await _bus.RaiseEvent(new ReceivedEmailEvent(playerId)).ConfigureAwait(false);
            }

            return true;
        }
        
    }
}

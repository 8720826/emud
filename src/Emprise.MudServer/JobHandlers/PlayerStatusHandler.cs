using Emprise.Application.Player.Services;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Interfaces.Ioc;
using Emprise.Domain.Core.Queue.Models;
using Emprise.Domain.ItemDrop.Models;
using Emprise.Domain.ItemDrop.Services;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.Player.Services;
using Emprise.Domain.Room.Services;
using Emprise.MudServer.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.MudServer.Handles
{
    public interface IPlayerStatusHandler : IScoped
    {
        Task Execute(PlayerStatusModel model);
    }
    public class PlayerStatusHandler : IPlayerStatusHandler
    {
        private readonly IMudProvider _mudProvider;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly ILogger<PlayerStatusHandler> _logger;
        private readonly IRecurringQueue _recurringQueue;
        private readonly IMediatorHandler _bus;
        private readonly IMudOnlineProvider _mudOnlineProvider;
        private readonly IRoomItemDropDomainService  _roomItemDropDomainService;
        private readonly IItemDropDomainService  _itemDropDomainService;
        private readonly IItemDropRateDomainService  _itemDropRateDomainService;
        public PlayerStatusHandler(
            IMudProvider mudProvider, 
            IPlayerDomainService playerDomainService, 
            ILogger<PlayerStatusHandler> logger, 
            IRecurringQueue recurringQueue,
            IMudOnlineProvider mudOnlineProvider,
           IRoomItemDropDomainService roomItemDropDomainService,
           IItemDropDomainService itemDropDomainService,
           IItemDropRateDomainService itemDropRateDomainService,
            IMediatorHandler bus)
        {
            _mudProvider = mudProvider;
            _playerDomainService = playerDomainService;
            _logger = logger;
            _recurringQueue = recurringQueue;
            _bus = bus;
            _mudOnlineProvider = mudOnlineProvider;
            _roomItemDropDomainService = roomItemDropDomainService;
            _itemDropDomainService = itemDropDomainService;
            _itemDropRateDomainService = itemDropRateDomainService;
        }
        public async Task Execute(PlayerStatusModel model)
        {
            int playerId = model.PlayerId;
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                _logger.LogDebug($"player == null");
                return;
            }

            var online = await _mudOnlineProvider.GetPlayerOnline(playerId);
            if (online == null)
            {
                //玩家离线后，从队列删除，并且修改状态为空闲
                await _recurringQueue.Remove<PlayerStatusModel>(playerId);
                if (player.Status!= PlayerStatusEnum.空闲)
                {
                    player.Status = PlayerStatusEnum.空闲;
                    await _playerDomainService.Update(player);

                    await _bus.RaiseEvent(new PlayerStatusChangedEvent(player)).ConfigureAwait(false);
                }
                return;
            }

            if (player.Status!= model.Status)
            {
                await _recurringQueue.Remove<PlayerStatusModel>(playerId);
                return;
            }

          
            //TODO
            switch (model.Status)
            {
                case PlayerStatusEnum.伐木:
                case PlayerStatusEnum.挖矿:
                case PlayerStatusEnum.打猎:
                case PlayerStatusEnum.采药:
                case PlayerStatusEnum.钓鱼:
                case PlayerStatusEnum.打工:

                    await DoWork(player);
                    break;

                case PlayerStatusEnum.疗伤:
                    await Heal(player);
                    break;

                case PlayerStatusEnum.打坐:
                    await Muse(player);
                    break;

                case PlayerStatusEnum.战斗:
                    await Fighting(player);
                    break;
            }

          
        }


        private async Task DoWork(PlayerEntity player)
        {
            await _mudProvider.ShowMessage(player.Id, $"你正在{player.Status}。。。");
            WorkTypeEnum workType = WorkTypeEnum.伐木;
            switch (player.Status)
            {
                case PlayerStatusEnum.伐木:
                    workType = WorkTypeEnum.伐木;
                    break;
                case PlayerStatusEnum.挖矿:
                    workType = WorkTypeEnum.挖矿;
                    break;
                case PlayerStatusEnum.打猎:
                    workType = WorkTypeEnum.打猎;
                    break;
                case PlayerStatusEnum.采药:
                    workType = WorkTypeEnum.采药;
                    break;
                case PlayerStatusEnum.钓鱼:
                    workType = WorkTypeEnum.钓鱼;
                    break;
                case PlayerStatusEnum.打工:
                    workType = WorkTypeEnum.打工;
                    break;

                default:
                    return;
            }

            var ids = (await _roomItemDropDomainService.GetAll()).Where(x => x.RoomId == player.RoomId).Select(x=>x.ItemDropId).ToList();

            var itemDrop = (await _itemDropDomainService.GetAll()).Where(x => ids.Contains(x.Id)).FirstOrDefault(x => x.WorkType == workType);
            if (itemDrop == null)
            {
                return;
            }
           

            var itemDropRates = (await _itemDropRateDomainService.GetAll()).Where(x => x.ItemDropId == itemDrop.Id).ToList();
            if (itemDropRates?.Count == 0)
            {
                return;
            }

            var random = new Random();
            int maxWeight = 100;//掉落总权重
            var itemDropModels = new List<ItemDropRateModel>();
            foreach (var itemDropRate in itemDropRates.OrderBy(x=>x.Order))
            {
                if (itemDropRate.Percent < random.Next(0, 100))
                {
                    continue;
                }

                int number = random.Next(Math.Min(itemDropRate.MinNumber, itemDropRate.MaxNumber), itemDropRate.MaxNumber + 1);
                if (number <= 0)
                {
                    continue;
                }

                //掉落
                maxWeight -= itemDropRate.Weight;

                var itemDropModel = new ItemDropRateModel
                {
                    DropType = itemDropRate.DropType,
                    Number = number,
                    WareId = itemDropRate.WareId
                };
                itemDropModels.Add(itemDropModel);



                if (maxWeight <= 0)
                {
                    break;
                }
            }

            if (itemDropModels.Count == 0)
            {
                //没有掉落
                return;
            }

            foreach (var itemDropModel in itemDropModels)
            {
                await _mudProvider.ShowMessage(player.Id, $"掉落，{itemDropModel.DropType.ToString()},{itemDropModel.WareId},{itemDropModel.Number}。。。");
            }
        }



        private async Task Heal(PlayerEntity player)
        {
            await _mudProvider.ShowMessage(player.Id, $"你正在{player.Status}。。。");
        }

        private async Task Muse(PlayerEntity player)
        {
            await _mudProvider.ShowMessage(player.Id, $"你正在{player.Status}。。。");
        }

        private async Task Fighting(PlayerEntity player)
        {
            await _mudProvider.ShowMessage(player.Id, $"你正在{player.Status}。。。");
        }
    }
}

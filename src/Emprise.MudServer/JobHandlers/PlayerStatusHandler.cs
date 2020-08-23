using Emprise.Application.Player.Services;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Interfaces.Ioc;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Queue.Models;
using Emprise.Domain.ItemDrop.Models;
using Emprise.Domain.ItemDrop.Services;
using Emprise.Domain.Npc.Entity;
using Emprise.Domain.Npc.Services;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.Player.Services;
using Emprise.Domain.Room.Services;
using Emprise.Domain.Skill.Services;
using Emprise.Domain.Ware.Entity;
using Emprise.Domain.Ware.Services;
using Emprise.MudServer.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        private readonly IWareDomainService _wareDomainService;
        private readonly IPlayerWareDomainService _playerWareDomainService;
        private readonly ISkillDomainService _skillDomainService;
        private readonly IPlayerSkillDomainService _playerSkillDomainService;
        private readonly INpcDomainService _npcDomainService;
        private readonly IRedisDb _redisDb;
        public PlayerStatusHandler(
            IMudProvider mudProvider, 
            IPlayerDomainService playerDomainService, 
            ILogger<PlayerStatusHandler> logger, 
            IRecurringQueue recurringQueue,
            IMudOnlineProvider mudOnlineProvider,
           IRoomItemDropDomainService roomItemDropDomainService,
           IItemDropDomainService itemDropDomainService,
           IItemDropRateDomainService itemDropRateDomainService,
           IWareDomainService wareDomainService,
           IPlayerWareDomainService playerWareDomainService,
           ISkillDomainService skillDomainService,
           IPlayerSkillDomainService playerSkillDomainService,
           INpcDomainService npcDomainService,
           IRedisDb redisDb,
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
            _wareDomainService = wareDomainService;
            _playerWareDomainService = playerWareDomainService;
            _skillDomainService = skillDomainService;
            _playerSkillDomainService = playerSkillDomainService;
            _npcDomainService = npcDomainService;
            _redisDb = redisDb;
        }
        public async Task Execute(PlayerStatusModel model)
        {
            int playerId = model.PlayerId;
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                return;
            }

            var online = await _mudOnlineProvider.GetPlayerOnline(playerId);
            if (online == null)
            {
                //玩家离线后，从队列删除，并且修改状态为空闲
                await _recurringQueue.Remove<PlayerStatusModel>($"player_{playerId}");
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
                await _recurringQueue.Remove<PlayerStatusModel>($"player_{playerId}");
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

                case PlayerStatusEnum.切磋:
                    await Fighting(player, model.TargetType, model.TargetId);
                    break;

                case PlayerStatusEnum.战斗:
                    await Killing(player, model.TargetId);
                    break;

                case PlayerStatusEnum.修练:

                    await LearnSkill(player, model.TargetId);
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

            var playerAttributeChanged = false;
            List<string> dropContents = new List<string>() ;

            foreach (var itemDropModel in itemDropModels)
            {
                switch (itemDropModel.DropType)
                {
                    case ItemDropTypeEnum.潜能:
                        playerAttributeChanged = true;
                        player.Pot += itemDropModel.Number;
                        dropContents.Add($"潜能 +{itemDropModel.Number}");
                        break;
                    case ItemDropTypeEnum.经验:
                        playerAttributeChanged = true;
                        player.Exp += itemDropModel.Number;
                        dropContents.Add($"经验 +{itemDropModel.Number}");
                        break;
                    case ItemDropTypeEnum.金钱:
                        playerAttributeChanged = true;
                        player.Money += itemDropModel.Number;
                        dropContents.Add($" +{itemDropModel.Number.ToMoney()}");
                        break;
                    case ItemDropTypeEnum.物品:
                        #region MyRegion
                        int wareId = itemDropModel.WareId;
                        int number = itemDropModel.Number;

                        var ware = await _wareDomainService.Get(wareId);
                        if (ware == null)
                        {
                            continue;
                        }

                        dropContents.Add($"{ware.Name} X{number}");

                        var canStack = ware.Category != WareCategoryEnum.武器;

                        if (canStack)
                        {
                            var playerWare = await _playerWareDomainService.Get(x => x.WareId == ware.Id && x.PlayerId == player.Id);
                            if (playerWare == null)
                            {
                                playerWare = new PlayerWareEntity
                                {
                                    IsBind = false,
                                    IsTemp = false,
                                    Level = 1,
                                    Number = number,
                                    Damage = 0,
                                    PlayerId = player.Id,
                                    Status = WareStatusEnum.卸下,
                                    WareId = wareId,
                                    WareName = ware.Name
                                };
                                await _playerWareDomainService.Add(playerWare);
                            }
                            else
                            {
                                playerWare.Number += number;
                                await _playerWareDomainService.Update(playerWare);
                            }
                        }
                        else
                        {
                            var playerWare = new PlayerWareEntity
                            {
                                IsBind = false,
                                IsTemp = false,
                                Level = 1,
                                Number = number,
                                Damage = 0,
                                PlayerId = player.Id,
                                Status = WareStatusEnum.卸下,
                                WareId = wareId,
                                WareName = ware.Name
                            };
                            await _playerWareDomainService.Add(playerWare);
                        }
                        #endregion


                        break;
                }
               
            }

            if (playerAttributeChanged)
            {
                await _bus.RaiseEvent(new PlayerAttributeChangedEvent(player)).ConfigureAwait(false);
            }


            if (dropContents.Count > 0)
            {
                await _mudProvider.ShowMessage(player.Id, $"获得{ string.Join("，", dropContents)   }。");   
            }


        }


        /// <summary>
        /// 疗伤，恢复体力
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private async Task Heal(PlayerEntity player)
        {
            if (player.Hp >= player.MaxHp)
            {
                await _mudProvider.ShowMessage(player.Id, $"{player.Status}结束，你站了起来。。。");

                await StopAction(player);
            }
            else
            {
                Random random = new Random();
                int addHp = random.Next((player.MaxHp / 40) + 1, (player.MaxHp / 20) + 1);
                if (player.Hp + addHp > player.MaxHp)
                {
                    addHp = player.MaxHp - player.Hp;
                }
                player.Hp += addHp;

                await _mudProvider.ShowMessage(player.Id, $"你正在{player.Status}。。。");
                await _mudProvider.ShowMessage(player.Id, $"你感觉身体好多了，气血恢复 +{addHp}。。。");
                await _bus.RaiseEvent(new PlayerAttributeChangedEvent(player)).ConfigureAwait(false);
            }
         
        }

        /// <summary>
        /// 打坐，恢复内力
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private async Task Muse(PlayerEntity player)
        {
            if (player.Mp >= player.MaxMp)
            {
                await StopAction(player);
                await _mudProvider.ShowMessage(player.Id, $"{player.Status}结束，你站了起来。。。");
            }
            else
            {
                Random random = new Random();
                int addMp = random.Next((player.MaxMp / 40) + 1, (player.MaxMp / 20) + 1);
                if (player.Mp + addMp > player.MaxMp)
                {
                    addMp = player.MaxMp - player.Mp;
                }
                player.Mp += addMp;
                await _playerDomainService.Update(player);

                await _mudProvider.ShowMessage(player.Id, $"你正在{player.Status}。。。");
                await _mudProvider.ShowMessage(player.Id, $"你感觉精神好多了，内力恢复 +{addMp}。。。");
                await _bus.RaiseEvent(new PlayerAttributeChangedEvent(player)).ConfigureAwait(false);
            }
          
        }

        private async Task Fighting(PlayerEntity player, TargetTypeEnum? targetType, int targetId)
        {
            if (targetType == TargetTypeEnum.Npc)
            {
                var npc = await _npcDomainService.Get(targetId);
                if (npc == null)
                {
                    await StopAction(player);
                    return;
                }

                var npcFightingPlayerId = await _redisDb.StringGet<int>(string.Format(RedisKey.NpcFighting, npc.Id));
                if (npcFightingPlayerId != player.Id)
                {
                    await StopAction(player);
                    return;
                }

                await _redisDb.StringSet(string.Format(RedisKey.NpcFighting, npc.Id), player.Id, DateTime.Now.AddSeconds(60));

                await FightingNpc(player, npc);
            }
            else if (targetType == TargetTypeEnum.玩家)
            {
                var target = await _playerDomainService.Get(targetId);
                if (target == null)
                {
                    await StopAction(player);
                    return;
                }

                await FightingPlayer(player, target);
            }
            else
            {
                await StopAction(player);
            }


          

        }

        private async Task FightingNpc(PlayerEntity player, NpcEntity npc)
        {


            await _mudProvider.ShowMessage(player.Id, $"【切磋】你正在攻击[{npc.Name}]。。。");
        }

        private async Task FightingPlayer(PlayerEntity player, PlayerEntity target)
        {
            await _mudProvider.ShowMessage(player.Id, $"【切磋】你正在攻击[{target.Name}]。。。");
        }


        private async Task Killing(PlayerEntity player, int targetId)
        {


            await _mudProvider.ShowMessage(player.Id, $"你正在{player.Status}，{targetId}。。。");

        }

        private async Task LearnSkill(PlayerEntity player, int playerSkillId)
        {
            var playerSkill = await _playerSkillDomainService.Get(playerSkillId);
            if (playerSkill == null)
            {
                await _mudProvider.ShowMessage(player.Id, $"你结束了修练！");

                await StopAction(player);
                return;
            }

            var skill = await _skillDomainService.Get(playerSkill.SkillId);
            if (skill == null)
            {
                await _mudProvider.ShowMessage(player.Id, $"你结束了修练！！");

                await StopAction(player);
                return;
            }

            if (player.Pot <= 0)
            {
                await _mudProvider.ShowMessage(player.Id, $"潜能耗尽，你结束了修练！");

                await StopAction(player);
                return;
            }

            if (player.Level * 10 < playerSkill.Level)
            {
                await _mudProvider.ShowMessage(player.Id, $"武功最高等级不能超过自身等级的10倍！");

                await StopAction(player);
                return;
            }

            var @int = player.Int * 3 + player.IntAdd;
            var nextLevelExp = (playerSkill.Level + 1) * (playerSkill.Level + 1) * 50;
            var levelUpExp = nextLevelExp - playerSkill.Level * playerSkill.Level * 50;

            Random random = new Random();
            int exp = random.Next(1, levelUpExp / 10);

            var needPot = exp * 100 / @int;

            if (player.Pot < needPot)
            {
                await _mudProvider.ShowMessage(player.Id, $"潜能即将耗尽，你结束了修练！");

                await StopAction(player);
                return ;
            }

            var effect = exp * 1000 / levelUpExp;

            string effectWords;
            if (effect > 80)
            {
                effectWords = "恍然大悟";
            }
            else if (effect > 50)
            {
                effectWords = "有所顿悟";
            }
            else if (effect > 20)
            {
                effectWords = "略有所获";
            }
            else if (effect > 10)
            {
                effectWords = "略有所获";
            }
            else
            {
                effectWords = "似乎没有什么进展";
            }

            player.Pot -= needPot;
            await _playerDomainService.Update(player);
            playerSkill.SkillName = skill.Name;
            playerSkill.Exp += exp;
            await _mudProvider.ShowMessage(player.Id, $"你消耗潜能{needPot}，{effectWords}，[{skill.Name}]经验增加{exp}！");

            if (playerSkill.Exp > nextLevelExp)
            {
                playerSkill.Level++;
                await _mudProvider.ShowMessage(player.Id, $"[{skill.Name}]等级上升为 Lv.{playerSkill.Level}！");
            }

           

            await _playerSkillDomainService.Update(playerSkill);

          

            await _bus.RaiseEvent(new PlayerAttributeChangedEvent(player)).ConfigureAwait(false);
        }




        private async Task StopAction(PlayerEntity player)
        {
            await _recurringQueue.Remove<PlayerStatusModel>($"player_{player.Id}");
            player.Status = PlayerStatusEnum.空闲;
            await _playerDomainService.Update(player);
            await _bus.RaiseEvent(new PlayerStatusChangedEvent(player)).ConfigureAwait(false);

        }
    }
}

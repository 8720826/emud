using AutoMapper;
using Emprise.Domain.Core.Authorization;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.CommandHandlers;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Notifications;
using Emprise.Domain.Core.Queue.Models;
using Emprise.Domain.Npc.Services;
using Emprise.Domain.Player.Services;
using Emprise.Domain.PlayerRelation.Services;
using Emprise.Domain.Room.Services;
using Emprise.MudServer.Commands;
using Emprise.MudServer.Commands.NpcActionCommands;
using Emprise.MudServer.Commands.SkillCommands;
using Emprise.MudServer.Events;
using Emprise.MudServer.Queues;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emprise.MudServer.CommandHandlers
{
    public class PlayerStatusHandler : CommandHandler,
        IRequestHandler<MeditateCommand, Unit>,
        IRequestHandler<StopActionCommand, Unit>,
        IRequestHandler<ExertCommand, Unit>,
        IRequestHandler<FishCommand, Unit>,
        IRequestHandler<DigCommand, Unit>,
        IRequestHandler<CollectCommand, Unit>,
        IRequestHandler<CutCommand, Unit>,
        IRequestHandler<HuntCommand, Unit>,
        IRequestHandler<WorkCommand, Unit>,
        IRequestHandler<LearnSkillCommand, Unit>,
        IRequestHandler<FightWithNpcCommand, Unit>


    {
        private readonly IMediatorHandler _bus;
        private readonly ILogger<PlayerStatusHandler> _logger;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly IMapper _mapper;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly IRoomDomainService _roomDomainService;
        private readonly IAccountContext _account;
        private readonly IDelayedQueue _delayedQueue;
        private readonly IRecurringQueue _recurringQueue;
        private readonly IMudProvider _mudProvider;
        private readonly AppConfig _appConfig;
        private readonly IRedisDb _redisDb;
        private readonly IMemoryCache _cache;
        private readonly IMudOnlineProvider _mudOnlineProvider;
        private readonly IQueueHandler _queueHandler;
        private readonly INpcDomainService _npcDomainService;


        public PlayerStatusHandler(
            IMediatorHandler bus,
            ILogger<PlayerStatusHandler> logger,
            IHttpContextAccessor httpAccessor,
            IMapper mapper,
            IPlayerDomainService playerDomainService,
            INpcDomainService npcDomainService,
            IRoomDomainService roomDomainService,
            IAccountContext account,
            IDelayedQueue delayedQueue,
            IRecurringQueue recurringQueue,
            IMudProvider mudProvider,
            IOptionsMonitor<AppConfig> appConfig,
            IRedisDb redisDb,
            IMemoryCache cache,
            INotificationHandler<DomainNotification> notifications,
            IMudOnlineProvider mudOnlineProvider,
            IQueueHandler queueHandler,
            IUnitOfWork uow) : base(uow, bus, notifications)
        {

            _cache = cache;
            _bus = bus;
            _logger = logger;
            _httpAccessor = httpAccessor;
            _mapper = mapper;
            _playerDomainService = playerDomainService;
            _roomDomainService = roomDomainService;
            _account = account;
            _delayedQueue = delayedQueue;
            _recurringQueue = recurringQueue;
            _mudProvider = mudProvider;
            _appConfig = appConfig.CurrentValue;
            _redisDb = redisDb;
            _mudOnlineProvider = mudOnlineProvider;
            _queueHandler = queueHandler;
            _npcDomainService = npcDomainService;
        }

        public async Task<Unit> Handle(WorkCommand command, CancellationToken cancellationToken)
        {
            await BeginChangeStatus(new PlayerStatusModel
            {
                PlayerId = command.PlayerId,
                Status = PlayerStatusEnum.打工
            });
            return Unit.Value;
        }

        public async Task<Unit> Handle(MeditateCommand command, CancellationToken cancellationToken)
        {
            await BeginChangeStatus(new PlayerStatusModel
            {
                PlayerId = command.PlayerId,
                Status = PlayerStatusEnum.打坐
            });
            return Unit.Value;
        }

        public async Task<Unit> Handle(ExertCommand command, CancellationToken cancellationToken)
        {
            await BeginChangeStatus(new PlayerStatusModel
            {
                PlayerId = command.PlayerId,
                Status = PlayerStatusEnum.疗伤
            });
            return Unit.Value;
        }

        public async Task<Unit> Handle(FishCommand command, CancellationToken cancellationToken)
        {
            await BeginChangeStatus(new PlayerStatusModel
            {
                PlayerId = command.PlayerId,
                Status = PlayerStatusEnum.钓鱼
            });
            return Unit.Value;
        }

        public async Task<Unit> Handle(DigCommand command, CancellationToken cancellationToken)
        {
            await BeginChangeStatus(new PlayerStatusModel
            {
                PlayerId = command.PlayerId,
                Status = PlayerStatusEnum.挖矿
            });
            return Unit.Value;
        }

        public async Task<Unit> Handle(CollectCommand command, CancellationToken cancellationToken)
        {
            await BeginChangeStatus(new PlayerStatusModel
            {
                PlayerId = command.PlayerId,
                Status = PlayerStatusEnum.采药
            });
            return Unit.Value;
        }

        public async Task<Unit> Handle(CutCommand command, CancellationToken cancellationToken)
        {
            await BeginChangeStatus(new PlayerStatusModel
            {
                PlayerId = command.PlayerId,
                Status = PlayerStatusEnum.伐木
            });
            return Unit.Value;
        }

        public async Task<Unit> Handle(HuntCommand command, CancellationToken cancellationToken)
        {
            await BeginChangeStatus(new PlayerStatusModel
            {
                PlayerId = command.PlayerId,
                Status = PlayerStatusEnum.打猎
            });
            return Unit.Value;
        }

        public async Task<Unit> Handle(LearnSkillCommand command, CancellationToken cancellationToken)
        {
            await BeginChangeStatus(new PlayerStatusModel
            {
                PlayerId = command.PlayerId,
                Status = PlayerStatusEnum.修练,
                TargetId = command.MySkillId
            });
            return Unit.Value;
        }

        public async Task<Unit> Handle(StopActionCommand command, CancellationToken cancellationToken)
        {
            //await _bus.RaiseEvent(new DomainNotification("功能暂时未实现"));

            var playerId = command.PlayerId;
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }
            var status = player.Status;
            if (status == PlayerStatusEnum.空闲)
            {
                return Unit.Value;
            }


            player.Status = PlayerStatusEnum.空闲;
            await _playerDomainService.Update(player);

            switch (status)
            {
                case PlayerStatusEnum.打坐:
                    await _mudProvider.ShowMessage(playerId, "你停止了打坐。。。");

                    break;

                case PlayerStatusEnum.打工:
                    await _mudProvider.ShowMessage(playerId, "你停止了打工。。。");
                    break;

                case PlayerStatusEnum.伐木:
                    await _mudProvider.ShowMessage(playerId, "你停止了伐木。。。");
                    break;

                case PlayerStatusEnum.打猎:
                    await _mudProvider.ShowMessage(playerId, "你停止了打猎。。。");
                    break;

                case PlayerStatusEnum.挖矿:
                    await _mudProvider.ShowMessage(playerId, "你停止了挖矿。。。");
                    break;

                case PlayerStatusEnum.疗伤:
                    await _mudProvider.ShowMessage(playerId, "你停止了疗伤。。。");
                    break;

                case PlayerStatusEnum.采药:
                    await _mudProvider.ShowMessage(playerId, "你停止了采药。。。");
                    break;

                case PlayerStatusEnum.钓鱼:
                    await _mudProvider.ShowMessage(playerId, "你停止了钓鱼。。。");
                    break;

                default:
                    await _mudProvider.ShowMessage(playerId, $"你停止了{status}。。。");
                    break;
            }

            await _recurringQueue.Remove<PlayerStatusModel>($"player_{playerId}");

            if (await Commit())
            {
                await _bus.RaiseEvent(new PlayerStatusChangedEvent(player)).ConfigureAwait(false);
            }
            return Unit.Value;
        }

        public async Task<Unit> Handle(FightWithNpcCommand command, CancellationToken cancellationToken)
        {
            var playerId = command.PlayerId;
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                return Unit.Value;
            }

            var npcId = command.NpcId;
            var npc = await _npcDomainService.Get(npcId);
            if (npc == null)
            {
                return Unit.Value;
            }


            if (!npc.CanFight)
            {
                await _bus.RaiseEvent(new DomainNotification($"你不能与[{npc.Name}]切磋！"));
                return Unit.Value;
            }


            if (player.RoomId != npc.RoomId)
            {
                await _bus.RaiseEvent(new DomainNotification($"[{npc.Name}]已经离开此地，无法发起切磋！"));
                return Unit.Value;
            }

            if (npc.IsDead)
            {
                await _bus.RaiseEvent(new DomainNotification($"[{npc.Name}]已经死了，无法发起切磋！"));
                return Unit.Value;
            }

            var npcFightingPlayerId = await _redisDb.StringGet<int>(string.Format(RedisKey.NpcFighting, npc.Id));
            if (npcFightingPlayerId > 0 && npcFightingPlayerId != playerId)
            {
                await _bus.RaiseEvent(new DomainNotification($"[{npc.Name}]拒绝了你的切磋请求！"));
                return Unit.Value;
            }



            var hasChangedStatus= await BeginChangeStatus(new PlayerStatusModel
            {
                PlayerId = playerId,
                Status = PlayerStatusEnum.切磋,
                TargetType = TargetTypeEnum.Npc,
                TargetId = npcId
            });

            if (hasChangedStatus)
            {
                await _mudProvider.ShowMessage(playerId, $"【切磋】你对着[{npc.Name}]说道：在下[{player.Name}]，领教壮士的高招！");

                await _mudProvider.ShowMessage(playerId, $"【切磋】[{npc.Name}]说道：「既然小兄弟赐教，在下只好奉陪，我们点到为止。」");

                await _redisDb.StringSet(string.Format(RedisKey.NpcFighting, npc.Id), playerId, DateTime.Now.AddSeconds(20));

                int minDelay = npc.Speed;
                int maxDelay = minDelay + 1000;

                var actionPoint = await _redisDb.StringGet<int>(string.Format(RedisKey.ActionPoint, playerId));
                await _mudProvider.ShowActionPoint(playerId, actionPoint);

                await _recurringQueue.Publish($"npc_{npc.Id}", new NpcStatusModel
                {
                    NpcId = npc.Id,
                    Status = NpcStatusEnum.切磋,
                    TargetId = playerId,
                    TargetType = TargetTypeEnum.玩家
                }, minDelay, maxDelay);

                await _mudProvider.ShowBox(playerId, new { boxName = "fighting" });
            }


            return Unit.Value;
        }

        private async Task<bool> BeginChangeStatus(PlayerStatusModel playerStatusModel)
        {
            var playerId = playerStatusModel.PlayerId;
            var newStatus = playerStatusModel.Status;
            var targetId = playerStatusModel.TargetId;
            var targetType = playerStatusModel.TargetType;

            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return false;
            }
            var status = player.Status;
            if (status == newStatus)
            {
                return false;
            }


            if (player.Status != PlayerStatusEnum.空闲)
            {
                await _bus.RaiseEvent(new DomainNotification($"你正在{player.Status}中，请先停下！"));
                return false;
            }

            var workTimes = await _redisDb.StringGet<int>(string.Format(RedisKey.WorkTimes, playerId, newStatus));

            int minDelay = 5000;
            int maxDelay = 15000;

            switch (newStatus)
            {
                case PlayerStatusEnum.打猎:
                    await _mudProvider.ShowMessage(playerId, "你开始在丛林中寻找猎物的踪影。");
                    if (workTimes <= 0)
                    {
                        await _queueHandler.SendQueueMessage(new CompleteQuestNewbieQuestQueue(playerId, NewbieQuestEnum.第一次打猎));
                    }
                    break;

                case PlayerStatusEnum.伐木:
                    await _mudProvider.ShowMessage(playerId, "你拿起斧头，对着一棵大树嘿呦嘿呦得砍了起来。");

                    //新手任务
                    if (workTimes <= 0)
                    {
                        await _queueHandler.SendQueueMessage(new CompleteQuestNewbieQuestQueue(playerId, NewbieQuestEnum.第一次伐木));
                    }
                    break;

                case PlayerStatusEnum.采药:
                    await _mudProvider.ShowMessage(playerId, "你开始在草丛中搜寻草药的踪影。");
                    if (workTimes <= 0)
                    {
                        await _queueHandler.SendQueueMessage(new CompleteQuestNewbieQuestQueue(playerId, NewbieQuestEnum.第一次采药));
                    }
                    break;

                case PlayerStatusEnum.挖矿:
                    await _mudProvider.ShowMessage(playerId, "你挥动铁锹，开始挖矿。");
                    if (workTimes <= 0)
                    {
                        await _queueHandler.SendQueueMessage(new CompleteQuestNewbieQuestQueue(playerId, NewbieQuestEnum.第一次挖矿));
                    }
                    break;

                case PlayerStatusEnum.钓鱼:
                    await _mudProvider.ShowMessage(playerId, "你把鱼竿一甩，开始等待鱼儿上钩。");
                    if (workTimes <= 0)
                    {
                        await _queueHandler.SendQueueMessage(new CompleteQuestNewbieQuestQueue(playerId, NewbieQuestEnum.第一次钓鱼));
                    }
                    break;

                case PlayerStatusEnum.疗伤:
                    if (player.Hp >= player.MaxHp)
                    {
                        await _mudProvider.ShowMessage(playerId, "你没有受伤，无需治疗。");
                        return false;
                    }
                    await _mudProvider.ShowMessage(playerId, "你盘膝坐下，开始运功疗伤。");
                    if (workTimes <= 0)
                    {
                        await _queueHandler.SendQueueMessage(new CompleteQuestNewbieQuestQueue(playerId, NewbieQuestEnum.第一次疗伤));
                    }
                    break;

                case PlayerStatusEnum.打坐:
                    if (player.Mp >= player.MaxMp)
                    {
                        await _mudProvider.ShowMessage(playerId, "你内力充沛，无需打坐。");
                        return false;
                    }
                    await _mudProvider.ShowMessage(playerId, "你盘膝坐下，开始打坐。");
                    if (workTimes <= 0)
                    {
                        await _queueHandler.SendQueueMessage(new CompleteQuestNewbieQuestQueue(playerId, NewbieQuestEnum.第一次打坐));
                    }
                    break;

                case PlayerStatusEnum.修练:
                    if (player.Pot <= 0)
                    {
                        await _mudProvider.ShowMessage(playerId, "你的潜能不够，无法修练。");
                        return false;
                    }
                    if (targetId <= 0)
                    {
                        return false;
                    }

                    await _mudProvider.ShowMessage(playerId, "你开始修练。。。");
                    if (workTimes <= 0)
                    {
                        await _queueHandler.SendQueueMessage(new CompleteQuestNewbieQuestQueue(playerId, NewbieQuestEnum.第一次修练));
                    }

                    break;


                case PlayerStatusEnum.切磋:

                    minDelay = player.Speed;
                    maxDelay = minDelay + 1000;

                    break;

                default:
                    await _mudProvider.ShowMessage(playerId, $"你开始{newStatus}。。。");
                    break;
            }


            player.Status = newStatus;
            await _playerDomainService.Update(player);

            //新手任务
            workTimes++;
            await _redisDb.StringSet(string.Format(RedisKey.ChatTimes, playerId), workTimes);


            await _recurringQueue.Publish($"player_{playerId}", new PlayerStatusModel
            {
                PlayerId = player.Id,
                Status = newStatus,
                TargetType = targetType,
                TargetId = targetId
            }, minDelay, maxDelay);

            if (await Commit())
            {
                await _bus.RaiseEvent(new PlayerStatusChangedEvent(player)).ConfigureAwait(false);
            }

            return true;
        }
    }
}

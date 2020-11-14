using AutoMapper;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.CommandHandlers;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Notifications;
using Emprise.MudServer.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emprise.Domain.Npc.Services;
using Emprise.Domain.Player.Services;
using Emprise.Domain.Ware.Services;
using Microsoft.Extensions.Caching.Memory;
using Emprise.Domain.Ware.Models;
using Emprise.Domain.Player.Models;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Ware.Entity;
using Newtonsoft.Json;
using Emprise.MudServer.Commands.WareCommands;
using Emprise.Domain.Skill.Services;

namespace Emprise.MudServer.CommandHandlers
{
    
    public class WareCommandHandler : CommandHandler,
        IRequestHandler<ShowMyPackCommand, Unit>,
        IRequestHandler<ShowMyWeaponCommand, Unit>,
        IRequestHandler<LoadWareCommand, Unit>,
        IRequestHandler<UnLoadWareCommand, Unit>,
        IRequestHandler<ShowWareCommand, Unit>,
        IRequestHandler<DropWareCommand, Unit>,
        IRequestHandler<ShowShopCommand, Unit>,
        IRequestHandler<ShowStoreWareCommand, Unit>,
        IRequestHandler<BuyStoreWareCommand, Unit>

        


    {
        private readonly IMediatorHandler _bus;
        private readonly ILogger<WareCommandHandler> _logger;
        private readonly INpcDomainService _npcDomainService;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly IWareDomainService _wareDomainService;
        private readonly IPlayerWareDomainService _playerWareDomainService;
        private readonly IStoreWareDomainService _storeWareDomainService;
        private readonly ISkillDomainService _skillDomainService;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly IRedisDb _redisDb;
        private readonly IMudProvider _mudProvider;


        public WareCommandHandler(
            IMediatorHandler bus,
            ILogger<WareCommandHandler> logger,
            INpcDomainService npcDomainService,
            IPlayerDomainService playerDomainService,
            IWareDomainService wareDomainService,
            IPlayerWareDomainService playerWareDomainService,
            IStoreWareDomainService storeWareDomainService,
            ISkillDomainService skillDomainService,
            IMapper mapper,
            IMemoryCache cache, 
            IRedisDb redisDb,
            IMudProvider mudProvider,
            INotificationHandler<DomainNotification> notifications,
            IUnitOfWork uow) : base(uow, bus, notifications)
        {

            _bus = bus;
            _logger = logger;
            _npcDomainService = npcDomainService;
            _mapper = mapper;
            _cache = cache;
            _playerDomainService = playerDomainService;
            _wareDomainService = wareDomainService;
            _playerWareDomainService = playerWareDomainService;
            _storeWareDomainService = storeWareDomainService;
            _skillDomainService = skillDomainService;
            _redisDb = redisDb;
            _mudProvider = mudProvider;
        }

        public async Task<Unit> Handle(ShowMyPackCommand command, CancellationToken cancellationToken)
        {
            var playerId = command.PlayerId;
            var myPack = new MyPack()
            {
                Money = "",
                Wares = new List<WareModel>()

            };
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                return Unit.Value;
            }

            myPack.Money = player.Money.ToMoney();

            var playerWares = await _playerWareDomainService.GetAll(player.Id);
            if (playerWares == null)
            {
                return Unit.Value;
            }

            var ids = playerWares.Select(x => x.WareId);

            var wares = (await _wareDomainService.GetAll()).Where(x => ids.Contains(x.Id));
            foreach (var playerWare in playerWares)
            {
                var ware = wares.FirstOrDefault(x => x.Id == playerWare.WareId);
                if (ware != null)
                {
                    var wareModel = _mapper.Map<WareModel>(ware);
                    wareModel.PlayerWareId = playerWare.Id;
                    wareModel.Number = playerWare.Number;
                    wareModel.Status = playerWare.Status;
                    myPack.Wares.Add(wareModel);
                }

            }

            await _mudProvider.ShowMyPack(playerId, myPack);

            return Unit.Value;
        }

        public async Task<Unit> Handle(ShowMyWeaponCommand command, CancellationToken cancellationToken)
        {
            var playerId = command.PlayerId;
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                return Unit.Value;
            }

            var parts = new List<WarePartEnum>() {
                WarePartEnum.头部,
                WarePartEnum.武器,
                WarePartEnum.衣服,
                WarePartEnum.裤子,
                WarePartEnum.腰带,
                WarePartEnum.鞋子
            };

            List<Weapon> myWeapons = new List<Weapon>();

            var playerWares = await _playerWareDomainService.GetAll(player.Id);
            var ids = playerWares?.Where(x => x.Status == WareStatusEnum.装备).Select(x => x.WareId);

            var wares = (await _wareDomainService.GetAll()).Where(x => ids.Contains(x.Id));

            foreach (var part in parts)
            {
                WareEntity ware = null;
                switch (part)
                {
                    case WarePartEnum.头部:
                        ware = wares.FirstOrDefault(x => x.Type == WareTypeEnum.帽);
                        break;

                    case WarePartEnum.武器:
                         ware = wares.FirstOrDefault(x => x.Type == WareTypeEnum.刀 || x.Type == WareTypeEnum.剑 || x.Type == WareTypeEnum.枪);

                        break;

                    case WarePartEnum.腰带:
                         //ware = wares.FirstOrDefault(x => x.Type == WareTypeEnum.帽);
                        break;

                    case WarePartEnum.衣服:
                         ware = wares.FirstOrDefault(x => x.Type == WareTypeEnum.衣服);
                        break;

                    case WarePartEnum.裤子:
                        // ware = wares.FirstOrDefault(x => x.Type == WareTypeEnum.);
                        break;
                    case WarePartEnum.鞋子:
                         ware = wares.FirstOrDefault(x => x.Type == WareTypeEnum.鞋);
                        break;

                }

                var weapon = new Weapon { Part = part.ToString() };
                if (ware != null)
                {
                    weapon.Ware = _mapper.Map<WareModel>(ware);
                }
                myWeapons.Add(weapon);
            }

            await _mudProvider.ShowMyWeapon(playerId, myWeapons);

            return Unit.Value;
        }


        public async Task<Unit> Handle(LoadWareCommand command, CancellationToken cancellationToken)
        {
            var myWareId = command.MyWareId;
            var playerId = command.PlayerId;

            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            var playerWare = await _playerWareDomainService.Get(myWareId);
            if (playerWare == null || playerWare.PlayerId != playerId)
            {
                await _bus.RaiseEvent(new DomainNotification($"武器不存在！"));
                return Unit.Value;
            }

            if (playerWare.Status == WareStatusEnum.装备)
            {
                await _bus.RaiseEvent(new DomainNotification($"武器已装备！"));
                return Unit.Value;
            }

            var ware = await _wareDomainService.Get(playerWare.WareId);
            if (ware == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"武器状态异常！"));
                return Unit.Value;
            }


            var playerWares = await _playerWareDomainService.GetAll(playerId);

            var ids = playerWares.Where(x => x.Status == WareStatusEnum.装备).Select(x => x.WareId).ToList();
            var wareQuery = await _wareDomainService.GetAll();
            var wares = wareQuery.Where(x => ids.Contains(x.Id));

            WareTypeEnum[] wareTypes = null;
            WareEntity loadWare = null;
            switch (ware.Type) 
            {
                case WareTypeEnum.刀:
                case WareTypeEnum.剑:
                case WareTypeEnum.枪:
                    wareTypes = new[] { WareTypeEnum.刀, WareTypeEnum.剑, WareTypeEnum.枪 };
                    break;

                case WareTypeEnum.衣服:
                    wareTypes = new[] { WareTypeEnum.衣服 };
                    break;

                case WareTypeEnum.鞋:
                    wareTypes = new[] { WareTypeEnum.鞋 };
                    break;

                case WareTypeEnum.帽:
                    wareTypes = new[] { WareTypeEnum.帽 };
                    break;
            }
            if (wareTypes != null)
            {
                loadWare = wares.FirstOrDefault(x => wareTypes.Contains(x.Type));
            }

            if (loadWare!=null)
            {
                await _bus.RaiseEvent(new DomainNotification($"你已经装备了 [{loadWare.Name}]！"));
                return Unit.Value;
            }


            playerWare.Status = WareStatusEnum.装备;
            await _playerWareDomainService.Update(playerWare);

            var wareEffectAttr = await Computed(playerId);

            player.Atk = wareEffectAttr.Atk;
            player.Def = wareEffectAttr.Def;

            await _playerDomainService.Update(player);

            /*
            var wareModel = _mapper.Map<WareModel>(ware);
            wareModel.PlayerWareId = playerWare.Id;
            wareModel.Number = playerWare.Number;
            wareModel.Status = playerWare.Status;
            await _mudProvider.LoadWare(playerId, wareModel);
            */

            var wareModel = await Computed(ware, playerWare);
            await _mudProvider.ShowWare(playerId, wareModel);

            await _mudProvider.ShowMessage(playerId, $"你装备了 [{wareModel.Name}]！");

            return Unit.Value;
        }

        public async Task<Unit> Handle(UnLoadWareCommand command, CancellationToken cancellationToken)
        {
            var myWareId = command.MyWareId;
            var playerId = command.PlayerId;

            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            var playerWare = await _playerWareDomainService.Get(myWareId);
            if (playerWare == null || playerWare.PlayerId != playerId)
            {
                await _bus.RaiseEvent(new DomainNotification($"武器不存在！"));
                return Unit.Value;
            }

            if (playerWare.Status == WareStatusEnum.卸下)
            {
                await _bus.RaiseEvent(new DomainNotification($"武器已卸下！"));
                return Unit.Value;
            }


            var ware = await _wareDomainService.Get(playerWare.WareId);
            if (ware == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"武器状态异常！"));
                return Unit.Value;
            }



            playerWare.Status = WareStatusEnum.卸下;
            await _playerWareDomainService.Update(playerWare);

            var wareEffectAttr = await Computed(playerId);

            player.Atk = wareEffectAttr.Atk;
            player.Def = wareEffectAttr.Def;

            await _playerDomainService.Update(player);

            /*
            var wareModel = _mapper.Map<WareModel>(ware);
            wareModel.PlayerWareId = playerWare.Id;
            wareModel.Number = playerWare.Number;
            wareModel.Status = playerWare.Status;
            await _mudProvider.UnLoadWare(playerId, wareModel);
            */
            var wareModel = await Computed(ware, playerWare);
            await _mudProvider.ShowWare(playerId, wareModel);

            await _mudProvider.ShowMessage(playerId, $"你卸下了 [{wareModel.Name}]！");

            return Unit.Value;
        }


        public async Task<Unit> Handle(ShowWareCommand command, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Handle ShowWareCommand:{JsonConvert.SerializeObject(command)}");

            var myWareId = command.MyWareId;
            var playerId = command.PlayerId;

            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            var playerWare = await _playerWareDomainService.Get(myWareId);
            if (playerWare == null || playerWare.PlayerId != playerId)
            {
                await _bus.RaiseEvent(new DomainNotification($"物品不存在！"));
                return Unit.Value;
            }

            var ware = await _wareDomainService.Get(playerWare.WareId);
            if (ware == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"物品状态异常！"));
                return Unit.Value;
            }


            var wareModel = await Computed(ware, playerWare);
            await _mudProvider.ShowWare(playerId, wareModel);

            return Unit.Value;
        }

        public async Task<Unit> Handle(DropWareCommand command, CancellationToken cancellationToken)
        {
            var myWareId = command.MyWareId;
            var playerId = command.PlayerId;

            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            var playerWare = await _playerWareDomainService.Get(myWareId);
            if (playerWare == null || playerWare.PlayerId != playerId)
            {
                await _bus.RaiseEvent(new DomainNotification($"物品不存在！"));
                return Unit.Value;
            }

            if (playerWare.Status != WareStatusEnum.卸下)
            {
                await _bus.RaiseEvent(new DomainNotification($"该物品{ playerWare.Status }中，不能丢弃！"));
                return Unit.Value;
            }


            var ware = await _wareDomainService.Get(playerWare.WareId);
            if (ware == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"物品状态异常！"));
                return Unit.Value;
            }

            await _playerWareDomainService.Delete(playerWare);


            var wareModel = _mapper.Map<WareModel>(ware);
            wareModel.PlayerWareId = playerWare.Id;

            await _mudProvider.DropWare(playerId, wareModel);

            await _mudProvider.ShowMessage(playerId, $"你丢掉了 [{wareModel.Name}]！");

            return Unit.Value;
        }



        public async Task<Unit> Handle(ShowShopCommand command, CancellationToken cancellationToken)
        {
            var playerId = command.PlayerId;

            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            List<StoreWareModel> storeWareModels = new List<StoreWareModel>();

            var storeWares = (await _storeWareDomainService.GetAll()).ToList();
            if (storeWares == null)
            {
                return Unit.Value;
            }

            var ids = storeWares.Select(x => x.WareId).ToList();

            var wares = (await _wareDomainService.GetAll()).Where(x => ids.Contains(x.Id));
            foreach (var storeWare in storeWares)
            {
                var ware = wares.FirstOrDefault(x => x.Id == storeWare.WareId);
                if (ware != null)
                {
                    var wareModel = _mapper.Map<StoreWareModel>(ware);
                    wareModel.StoreWareId = storeWare.Id;
                    wareModel.Number = storeWare.Number;
                    wareModel.Price = storeWare.Price;
                    wareModel.OriginalPrice = storeWare.OriginalPrice;
                    wareModel.PriceType = storeWare.PriceType;
                    wareModel.IsBind = storeWare.IsBind;
                    wareModel.PriceDesc = storeWare.Price.ToMoney();
                    storeWareModels.Add(wareModel);
                }

            }


            await _mudProvider.ShowShop(playerId, storeWareModels);
            return Unit.Value;
        }


        
        public async Task<Unit> Handle(ShowStoreWareCommand command, CancellationToken cancellationToken)
        {
            var playerId = command.PlayerId;
            var storeWareId = command.StoreWareId;


            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            var storeWare = await _storeWareDomainService.Get(storeWareId);
            if (storeWare == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"物品不存在！"));
                return Unit.Value;
            }


            var ware = await _wareDomainService.Get(storeWare.WareId);
            if (ware == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"物品不存在！"));
                return Unit.Value;
            }

            var wareModel = _mapper.Map<StoreWareModel>(ware);
            wareModel.StoreWareId = storeWare.Id;
            wareModel.Number = storeWare.Number;
            wareModel.Price = storeWare.Price;
            wareModel.OriginalPrice = storeWare.OriginalPrice;
            wareModel.PriceType = storeWare.PriceType;
            wareModel.IsBind = storeWare.IsBind;
            wareModel.PriceDesc = storeWare.Price.ToMoney();
 
            await _mudProvider.ShowStoreWare(playerId, wareModel);
            return Unit.Value;
        }

        
        public async Task<Unit> Handle(BuyStoreWareCommand command, CancellationToken cancellationToken)
        {
            var playerId = command.PlayerId;
            var storeWareId = command.StoreWareId;
            var number = command.Number;

            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"角色不存在！"));
                return Unit.Value;
            }

            var storeWare = await _storeWareDomainService.Get(storeWareId);
            if (storeWare == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"物品不存在！"));
                return Unit.Value;
            }


            var ware = await _wareDomainService.Get(storeWare.WareId);
            if (ware == null)
            {
                await _bus.RaiseEvent(new DomainNotification($"物品不存在！"));
                return Unit.Value;
            }

         

            int money = storeWare.Price * number;

            switch (storeWare.PriceType)
            {
                case PriceTypeEnum.点券:
                    await _bus.RaiseEvent(new DomainNotification($"该物品无法购买！"));
                    break;

                case PriceTypeEnum.元宝:
                    await _bus.RaiseEvent(new DomainNotification($"该物品无法购买！"));
                    break;

                case PriceTypeEnum.银两:
                    if (player.Money < money)
                    {
                        await _bus.RaiseEvent(new DomainNotification($"先去赚点钱吧！"));
                        return Unit.Value;
                    }

                    player.Money -= money;

                    break;
            }

            await _playerDomainService.Update(player);

            var isStack = true;
            if (ware.Category == WareCategoryEnum.服装|| ware.Category == WareCategoryEnum.武器)
            {
                isStack = false;
            }

            var newPlayerWare = new PlayerWareEntity
            {
                PlayerId = playerId,
                Damage = 0,
                IsBind = false,
                IsTemp = false,
                Level = 0,
                Number = number,
                Status = WareStatusEnum.卸下,
                WareId = ware.Id,
                WareName = ware.Name
            };

            if (isStack)
            {
                var playerWare = await _playerWareDomainService.Get(x => x.PlayerId == playerId && x.WareId == ware.Id);
                if (playerWare == null)
                {
                    await _playerWareDomainService.Add(newPlayerWare);
                }
                else
                {
                    playerWare.Number += number;
                    playerWare.WareName += ware.Name;
                    await _playerWareDomainService.Update(playerWare);
                }
            }
            else
            {
                await _playerWareDomainService.Add(newPlayerWare);
            }


            await _mudProvider.ShowMessage(playerId, $"你购买了 {number}个 [{ware.Name}]！");

            return Unit.Value;
        }

        private async Task<WareEffectAttr> Computed(int playerId)
        {
            var wareEffectAttr = new WareEffectAttr();
            var playerWares = await _playerWareDomainService.GetAll(playerId);

            var ids = playerWares.Where(x => x.Status == WareStatusEnum.装备).Select(x => x.WareId).ToList();

            var wareQuery = await _wareDomainService.GetAll();
            var wares = wareQuery.Where(x => ids.Contains(x.Id));
            foreach (var item in wares)
            {
                var effects = JsonConvert.DeserializeObject<List<WareEffect>>(item.Effect);
                foreach (var effect in effects)
                {
                    foreach(var attr in effect.Attrs)
                    {
                        int.TryParse(attr.Val,out int val);

                        switch (attr.Attr)
                        {
                            case "Atk":
                                wareEffectAttr.Atk += val;
                                break;

                            case "Def":
                                wareEffectAttr.Def += val;
                                break;
                        }
                    }
                }
            }

            return wareEffectAttr;
        }


        private async Task<WareModel> Computed(WareEntity ware, PlayerWareEntity playerWare)
        {
            var wareModel = _mapper.Map<WareModel>(ware);
            wareModel.PlayerWareId = playerWare.Id;
            wareModel.Number = playerWare.Number;
            wareModel.Status = playerWare.Status;

            var wareEffectAttr = new WareEffectAttr();
            var effects = JsonConvert.DeserializeObject<List<WareEffect>>(ware.Effect);
            foreach (var effect in effects)
            {
                foreach (var attr in effect.Attrs)
                {
                    int.TryParse(attr.Val, out int val);

                    switch (attr.Attr)
                    {
                        case "Atk":
                            wareEffectAttr.Atk += val;
                            break;

                        case "Def":
                            wareEffectAttr.Def += val;
                            break;

                        case "Hp":
                            wareEffectAttr.Hp += val;
                            break;

                        case "Mp":
                            wareEffectAttr.Mp += val;
                            break;
                    }
                }
            }

            wareModel.WareEffect = wareEffectAttr;

            return await Task.FromResult(wareModel);
        }
    }
}

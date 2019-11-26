using AutoMapper;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Models.Chat;
using Emprise.Domain.Log.Entity;
using Emprise.Domain.Log.Services;
using Emprise.Domain.Npc.Services;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.Player.Events;
using Emprise.Domain.Player.Models;
using Emprise.Domain.Player.Services;
using Emprise.Domain.Room.Models;
using Emprise.Domain.Room.Services;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emprise.Domain.User.EventHandlers
{
    public class PlayerEventHandler :
        INotificationHandler<EntityUpdatedEvent<PlayerEntity>>,
        INotificationHandler<EntityInsertedEvent<PlayerEntity>>,
        INotificationHandler<EntityDeletedEvent<PlayerEntity>>,
        INotificationHandler<MovedEvent>,
        INotificationHandler<PlayerInRoomEvent>,
        INotificationHandler<InitGameEvent>,
        INotificationHandler<PlayerStatusChangedEvent>,
        INotificationHandler<SendMessageEvent>

        


    {
        public const string Player = "Player_{0}";

        private readonly IRoomDomainService _roomDomainService;
        private readonly INpcDomainService _npcDomainService;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly IMudProvider _mudProvider;
        private readonly IMudOnlineProvider _chatOnlineProvider;
        private readonly IMapper  _mapper;
        private readonly AppConfig _appConfig;
        private readonly IChatLogDomainService _chatLogDomainService;

        public PlayerEventHandler(IRoomDomainService roomDomainService, INpcDomainService npcDomainService, IPlayerDomainService playerDomainService, IMudProvider chatProvider, IMudOnlineProvider chatOnlineProvider, IMapper mapper, IOptions<AppConfig> appConfig, IChatLogDomainService chatLogDomainService)
        {
            _roomDomainService = roomDomainService;
            _npcDomainService = npcDomainService;
            _playerDomainService = playerDomainService;
            _mudProvider = chatProvider;
            _chatOnlineProvider = chatOnlineProvider;
            _mapper = mapper;
            _appConfig = appConfig.Value;
            _chatLogDomainService = chatLogDomainService;
        }

        public async Task Handle(EntityUpdatedEvent<PlayerEntity> message, CancellationToken cancellationToken)
        {

        }

        public async Task Handle(EntityInsertedEvent<PlayerEntity> message, CancellationToken cancellationToken)
        {
            await Task.Run(() => {
               
            });
        }

        public async Task Handle(EntityDeletedEvent<PlayerEntity> message, CancellationToken cancellationToken)
        {

        }
        
        public async Task Handle(MovedEvent message, CancellationToken cancellationToken)
        {
            var player = message.Player;
            var roomIn = message.RoomIn;
            var roomOut = message.RoomOut;

            //更新玩家在线数据
            await _chatOnlineProvider.SetPlayerOnline(new PlayerOnlineModel
            {
                IsOnline = true,
                LastDate = DateTime.Now,
                Level = player.Level,
                PlayerName = player.Name,
                PlayerId = player.Id,
                RoomId = player.RoomId,
                Gender = player.Gender,
                Title = player.Title
            });

            //更新玩家聊天房间
            await _mudProvider.RemoveFromRoom(player.Id, roomOut.Id);
            await _mudProvider.AddToRoom(player.Id, roomIn.Id);

            //更新新房间玩家列表
            var playersIn = await _chatOnlineProvider.GetPlayerList(roomIn.Id);
            await _mudProvider.UpdateRoomPlayerList(roomIn.Id, playersIn);

            //更新旧房间玩家列表
            var playersOut = await _chatOnlineProvider.GetPlayerList(roomOut.Id);
            await _mudProvider.UpdateRoomPlayerList(roomOut.Id, playersOut);

            //更新当前房间及出口
            var ids = new List<int>() { roomIn.West, roomIn.East, roomIn.South, roomIn.North }.Where(x => x > 0);
            var rooms = await _roomDomainService.GetAll(x => ids.Contains(x.Id));
            rooms.Add(roomIn);
            var roomInfos = _mapper.Map<List<RoomModel>>(rooms);
            var obj = new
            {
                CurrentRoom = roomInfos.FirstOrDefault(x => x.Id == roomIn.Id),
                WestRoom = roomInfos.FirstOrDefault(x => x.Id == roomIn.West) ?? new RoomModel(),
                EastRoom = roomInfos.FirstOrDefault(x => x.Id == roomIn.East) ?? new RoomModel(),
                SouthRoom = roomInfos.FirstOrDefault(x => x.Id == roomIn.South) ?? new RoomModel(),
                NorthRoom = roomInfos.FirstOrDefault(x => x.Id == roomIn.North) ?? new RoomModel()
            };
            await _mudProvider.Move(player.Id, obj);

            //更新当前玩家显示的npc列表
            var nps = await _npcDomainService.Query(x => x.RoomId == roomIn.Id);
            await _mudProvider.UpdatePlayerRoomNpcList(player.Id, nps);


            //输出移动信息
            await _mudProvider.ShowMessage(player.Id, $"你来到了{roomIn.Name}。");
            await _mudProvider.ShowRoomOthersMessage(roomIn.Id, player.Id, $"[{player.Name}] 从{roomOut.Name}走了过来。");
            await _mudProvider.ShowRoomOthersMessage(roomOut.Id, player.Id, $"[{player.Name}] 往{roomIn.Name}离开。");



        }
        

        public async Task Handle(PlayerInRoomEvent message, CancellationToken cancellationToken)
        {
            var player = message.Player;
            var room = message.Room;

            //var model = await _chatOnlineProvider.GetPlayerOnline(player.Id);
            //var isOnline = model != null;


            await _chatOnlineProvider.SetPlayerOnline(new PlayerOnlineModel
            {
                IsOnline = true,
                LastDate = DateTime.Now,
                Level = player.Level,
                PlayerName = player.Name,
                PlayerId = player.Id,
                RoomId = player.RoomId,
                Gender = player.Gender,
                Title = player.Title
            });

            var ids = new List<int>() { room.West, room.East, room.South, room.North }.Where(x => x > 0);

            var rooms = await _roomDomainService.GetAll(x => ids.Contains(x.Id));

            rooms.Add(room);

            var roomInfos = _mapper.Map<List<RoomModel>>(rooms);

            var obj = new
            {
                CurrentRoom = roomInfos.FirstOrDefault(x => x.Id == room.Id),
                WestRoom = roomInfos.FirstOrDefault(x => x.Id == room.West),
                EastRoom = roomInfos.FirstOrDefault(x => x.Id == room.East),
                SouthRoom = roomInfos.FirstOrDefault(x => x.Id == room.South),
                NorthRoom = roomInfos.FirstOrDefault(x => x.Id == room.North)
            };

            await _mudProvider.Move(player.Id, obj);


            var nps = await _npcDomainService.Query(x => x.RoomId == room.Id);
            await _mudProvider.UpdatePlayerRoomNpcList(player.Id, nps);


            await _mudProvider.AddToRoom(player.Id, room.Id);

            await _mudProvider.ShowMessage(player.Id, $"你来到了{room.Name}。");


            var players = await _chatOnlineProvider.GetPlayerList(room.Id);
            await _mudProvider.UpdateRoomPlayerList(room.Id, players);

            var myInfo = _mapper.Map<MyInfo>(player);
            await _mudProvider.UpdatePlayerInfo(player.Id, myInfo);
        }

        public async Task Handle(InitGameEvent message, CancellationToken cancellationToken)
        {
            var player = message.Player;

            var model = await _chatOnlineProvider.GetPlayerOnline(player.Id);
            var isOnline = model != null;

            if (isOnline)
            {
                await _mudProvider.ShowMessage(player.Id, $"<font color=#FF5555>你重新连接到【{_appConfig.Site.Name}】。</font>");
            }
            else
            {
                await _mudProvider.ShowMessage(player.Id, _appConfig.Site.WelcomeWords.Replace("{name}", player.Name));
            }

        }

        public async Task Handle(PlayerStatusChangedEvent message, CancellationToken cancellationToken)
        {
            var player = message.Player;
            var myInfo = _mapper.Map<MyInfo>(player);
            await _mudProvider.UpdatePlayerInfo(player.Id, myInfo);

        }

        public async Task Handle(SendMessageEvent message, CancellationToken cancellationToken)
        {
            var playerId = message.PlayerId;
            var content = message.Content;

            //await _mudProvider.ShowMessage(playerId, $"你说：{content}");

            await _chatLogDomainService.Add(new ChatLogEntity
            {
                PlayerId = playerId,
                Content = content,
                PostDate = DateTime.Now
            });
            // await _delayedQueue.Publish(new MessageModel { Content = receivedMessage.Content, PlayerId = _account.PlayerId }, 2, 10);

        }
        
    }
}

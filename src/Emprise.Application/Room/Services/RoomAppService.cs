using AutoMapper;
using Emprise.Application.Room.Models;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Services;
using Emprise.Domain.Log.Entity;
using Emprise.Domain.Log.Services;
using Emprise.Domain.Map.Services;
using Emprise.Domain.Player.Services;
using Emprise.Domain.Room.Entity;
using Emprise.Domain.Room.Models;
using Emprise.Domain.Room.Services;
using Emprise.Infra.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Emprise.Application.User.Services
{
    public class RoomAppService : BaseAppService, IRoomAppService
    {
        private readonly IMediatorHandler _bus;
        private readonly IMapper _mapper;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly IRoomDomainService _roomDomainService;
        private readonly IMapDomainService _mapDomainService;
        private readonly IOperatorLogDomainService _operatorLogDomainService;
        public RoomAppService(IMediatorHandler bus,
            IMapper mapper, 
            IPlayerDomainService playerDomainService, 
            IRoomDomainService roomDomainService, 
            IMapDomainService mapDomainService,
            IOperatorLogDomainService operatorLogDomainService,
            IUnitOfWork uow) 
            : base(uow)
        {
            _bus = bus;
            _mapper = mapper;
            _playerDomainService = playerDomainService;
            _roomDomainService = roomDomainService;
            _mapDomainService = mapDomainService;
            _operatorLogDomainService = operatorLogDomainService;
        }

        public async Task<RoomEntity> Get(int id)
        {
            var room = await _roomDomainService.Get(id);
            return room;
        }

        public async Task<RoomModel> GetCurrent(int playerId)
        {
            var player = await _playerDomainService.Get(playerId);

            var room = await _roomDomainService.Get(player.RoomId);

            var roomModel = _mapper.Map<RoomModel>(room);


            return roomModel;
        }



        public async Task<ResultDto> Add(RoomInput item,int mapId,int roomId, string position)
        {
            var result = new ResultDto { Message = "" };

            try
            {
                var map = await _mapDomainService.Get(mapId);
                if (map == null)
                {
                    result.Message = $"参数错误（mapId={mapId}）";

                    return result;
                }

                var room = _mapper.Map<RoomEntity>(item);
                room.MapId = mapId;
                await _roomDomainService.Add(room);

                if (roomId > 0)
                {
                    var oldRoom = await _roomDomainService.Get(roomId);
                    if (oldRoom != null && oldRoom.MapId == mapId)
                    {
                        switch (position)
                        {
                            case "west":
                                if (oldRoom.East == 0)
                                {
                                    oldRoom.East = room.Id;
                                    oldRoom.EastName = room.Name;

                                    room.West = oldRoom.Id;
                                    room.WestName = oldRoom.Name;
                                }
                                break;

                            case "east":
                                if (oldRoom.West == 0)
                                {
                                    oldRoom.West = room.Id;
                                    oldRoom.WestName = room.Name;


                                    room.East = oldRoom.Id;
                                    room.EastName = oldRoom.Name;
                                }
                                break;

                            case "south":
                                if (oldRoom.South == 0)
                                {
                                    oldRoom.South = room.Id;
                                    oldRoom.SouthName = room.Name;

                                    room.North = oldRoom.Id;
                                    room.NorthName = oldRoom.Name;
                                }
                                break;

                            case "north":
                                if (oldRoom.North == 0)
                                {
                                    oldRoom.North = room.Id;
                                    oldRoom.NorthName = room.Name;

                                    room.South = oldRoom.Id;
                                    room.SouthName = oldRoom.Name;
                                }
                                break;

                        }
                    }
                }

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.添加地图,
                    Content = JsonConvert.SerializeObject(item)
                });

                await Commit();
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                await _operatorLogDomainService.AddError(new OperatorLogEntity
                {
                    Type = OperatorLogType.添加地图,
                    Content = $"Data={JsonConvert.SerializeObject(item)},ErrorMessage={result.Message}"
                });
                await Commit();
            }
            return result;
        }

        public async Task<ResultDto> Update(int id, RoomInput item)
        {
            var result = new ResultDto { Message = "" };
            try
            {
                var room = await _roomDomainService.Get(id);
                if (room == null)
                {
                    result.Message = $"房间 {id} 不存在！";
                    return result;
                }


                var content = room.ComparisonTo(item);
                int west = room.West;
                int east = room.East;
                int south = room.South;
                int north = room.North;

                _mapper.Map(item, room);

                List<int> changedIds = new List<int>();
                changedIds.Add(room.Id);

                //已修改
                if (west != room.West)
                {
                    //原来已设置
                    if (west > 0)
                    {
                        var oldRoomWest = await _roomDomainService.Get(west);
                        if (oldRoomWest != null)
                        {
                            oldRoomWest.East = 0;
                            oldRoomWest.EastName = "";
                            changedIds.Add(oldRoomWest.Id);
                        }
                    }
                }

                //已设置
                if (room.West > 0 && room.West != room.Id)
                {
                    var roomWest = await _roomDomainService.Get(room.West);
                    if (roomWest != null && roomWest.MapId == room.MapId)
                    {
                        roomWest.East = room.Id;
                        roomWest.EastName = room.Name;
                        room.WestName = roomWest.Name;
                        changedIds.Add(roomWest.Id);
                    }
                    else
                    {
                        room.West = 0;
                        room.WestName = "";
                    }
                }
                else
                {
                    room.West = 0;
                    room.WestName = "";
                }


                if (east != room.East)
                {
                    if (east > 0)
                    {
                        var oldRoomEast = await _roomDomainService.Get(east);
                        if (oldRoomEast != null)
                        {
                            oldRoomEast.West = 0;
                            oldRoomEast.WestName = "";
                            changedIds.Add(oldRoomEast.Id);
                        }
                    }
                }

                if (room.East > 0 && room.East != room.Id)
                {
                    var roomEast = await _roomDomainService.Get(room.East);
                    if (roomEast != null && roomEast.MapId == room.MapId)
                    {
                        roomEast.West = room.Id;
                        roomEast.WestName = room.Name;
                        room.EastName = roomEast.Name;
                        changedIds.Add(roomEast.Id);
                    }
                    else
                    {
                        room.East = 0;
                        room.EastName = "";
                    }
                }
                else
                {
                    room.East = 0;
                    room.EastName = "";
                }


                if (south != room.South)
                {
                    if (south > 0)
                    {
                        var oldRoomSouth = await _roomDomainService.Get(south);
                        if (oldRoomSouth != null)
                        {
                            oldRoomSouth.North = 0;
                            oldRoomSouth.NorthName = "";
                            changedIds.Add(oldRoomSouth.Id);
                        }
                    }
                }

                if (room.South > 0 && room.South != room.Id)
                {
                    var roomSouth = await _roomDomainService.Get(room.South);
                    if (roomSouth != null && roomSouth.MapId == room.MapId)
                    {
                        roomSouth.North = room.Id;
                        roomSouth.NorthName = room.Name;
                        room.SouthName = roomSouth.Name;
                        changedIds.Add(roomSouth.Id);
                    }
                    else
                    {
                        room.South = 0;
                        room.SouthName = "";
                    }
                }
                else
                {
                    room.South = 0;
                    room.SouthName = "";
                }



                if (north != room.North)
                {
                    if (north > 0)
                    {
                        var oldRoomNorth = await _roomDomainService.Get(north);
                        if (oldRoomNorth != null)
                        {
                            oldRoomNorth.South = 0;
                            oldRoomNorth.SouthName = "";
                            changedIds.Add(oldRoomNorth.Id);
                        }
                    }
                }

                if (room.North > 0 && room.North != room.Id)
                {
                    var roomNorth = await _roomDomainService.Get(room.North);
                    if (roomNorth != null && roomNorth.MapId == room.MapId)
                    {
                        roomNorth.South = room.Id;
                        roomNorth.SouthName = room.Name;
                        room.NorthName = roomNorth.Name;
                        changedIds.Add(roomNorth.Id);
                    }
                    else
                    {
                        room.North = 0;
                        room.NorthName = "";
                    }
                }
                else
                {
                    room.North = 0;
                    room.NorthName = "";
                }

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.修改房间,
                    Content = $"Id = {id},Data = {content}"
                });

                await Commit();

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                await _operatorLogDomainService.AddError(new OperatorLogEntity
                {
                    Type = OperatorLogType.修改房间,
                    Content = $"Data={JsonConvert.SerializeObject(item)},ErrorMessage={result.Message}"
                });
                await Commit();
            }
            return result;
        }

        public async Task<ResultDto> Delete(int id)
        {
            var result = new ResultDto { Message = "" };
            try
            {
                var room = await _roomDomainService.Get(id);
                if (room == null)
                {
                    result.Message = $"房间 {id} 不存在！";
                    return result;
                }


                await _roomDomainService.Delete(room);

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.删除房间,
                    Content = JsonConvert.SerializeObject(room)
                });

                await Commit();

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                await _operatorLogDomainService.AddError(new OperatorLogEntity
                {
                    Type = OperatorLogType.删除房间,
                    Content = $"id={id}，ErrorMessage={result.Message}"
                });
                await Commit();
            }
            return result;
        }

        public async Task<Paging<RoomEntity>> GetPaging(int mapId,string keyword, int pageIndex)
        {

            var query = await _roomDomainService.GetAll();
            query = query.Where(x => x.MapId == mapId);
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Name.Contains(keyword));
            }
            query = query.OrderBy(x => x.Id);

            return await query.Paged(pageIndex);
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Api;
using Emprise.Admin.Models.Room;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Admin.Pages.Room
{
    public class EditModel : BasePageModel
    {
        public EditModel(IMudClient mudClient,
            IMapper mapper, 
            ILogger<AddModel> logger, 
            EmpriseDbContext db, 
            IOptionsMonitor<AppConfig> appConfig, 
            IHttpContextAccessor httpAccessor)
            : base(db, appConfig, httpAccessor, mapper, logger, mudClient)
        {

        }

        public int Id { get; set; }



        [BindProperty]
        public RoomInput Room { get; set; }

        public int MapId { get; set; }

        public string ErrorMessage { get; set; }


        [BindProperty]
        public string UrlReferer { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            UrlReferer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/Room/Index");
            }

            if (id > 0)
            {
                Id = id;
                var room = await _db.Rooms.FindAsync(id);

                Room = _mapper.Map<RoomInput>(room);

                MapId = room.MapId;

                return Page();
            }
            else
            {
                return RedirectToPage("/Room/Index");
            }
        }


        public async Task<IActionResult> OnPostAsync(int id)
        {
            Id = id;
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {


                var room = await _db.Rooms.FindAsync(id);
                if (room == null)
                {
                    ErrorMessage = $"房间 {id} 不存在！";
                    return Page();
                }

                var content = DifferenceComparison(room, Room);

                int west = room.West;
                int east = room.East;
                int south = room.South;
                int north = room.North;

                _mapper.Map(Room, room);

                List<int> changedIds = new List<int>();
                changedIds.Add(room.Id);

                //已修改
                if (west != room.West)
                {
                    //原来已设置
                    if (west > 0)
                    {
                        var oldRoomWest = await _db.Rooms.FindAsync(west);
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
                    var roomWest = await _db.Rooms.FindAsync(room.West);
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
                        var oldRoomEast = await _db.Rooms.FindAsync(east);
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
                    var roomEast = await _db.Rooms.FindAsync(room.East);
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
                        var oldRoomSouth = await _db.Rooms.FindAsync(south);
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
                    var roomSouth = await _db.Rooms.FindAsync(room.South);
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
                        var oldRoomNorth = await _db.Rooms.FindAsync(north);
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
                    var roomNorth = await _db.Rooms.FindAsync(room.North);
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


                await _db.SaveChangesAsync();

                await AddSuccess(new OperatorLog
                {
                    Type = OperatorLogType.修改房间,
                    Content = $"Id = {id},Data = {content}"
                });

                foreach (var roomId in changedIds)
                {
                    var result = await _mudClient.EditRoom(roomId);
                    ErrorMessage += result.StatusCode;
                }
            }
            catch(Exception ex)
            {
                ErrorMessage = ex.Message;
                await AddError(new OperatorLog
                {
                    Type = OperatorLogType.修改房间,
                    Content = $"Id = {id},Data={JsonConvert.SerializeObject(Room)},ErrorMessage={ErrorMessage}"
                });
                return Page();
            }

            return Redirect(UrlReferer);
        }
    }
}
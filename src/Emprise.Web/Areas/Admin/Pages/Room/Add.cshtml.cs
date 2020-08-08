using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.Map.Services;
using Emprise.Application.Room.Models;
using Emprise.Application.User.Services;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Models;
using Emprise.Web.Areas.Admin.Pages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Admin.Pages.Room
{
    public class AddModel : BaseAdminPageModel
    {
        private readonly IRoomAppService _romAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public AddModel(
            ILogger<AddModel> logger,
            IRoomAppService romAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _romAppService = romAppService;
            _appConfig = appConfig.CurrentValue;

        }

        [BindProperty]
        public RoomInput Room { get; set; }

        public int MapId { get; set; }

        public string Tips { get; set; }
        public string ErrorMessage { get; set; }


        public async Task OnGetAsync(int id, string position, int mapId)
        {

            MapId = mapId;
            if (id > 0)
            {
                var room = await _romAppService.Get(id);
                if (room != null)
                {
                    Tips = $"在房间 {room.Name}(id={id}) 的{position}添加新房间";

                }
            }

        }

        public async Task<IActionResult> OnPostAsync(int id, string position, int mapId)
        {
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }

            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _romAppService.Add(Room, mapId, id, position);
            if (!result.IsSuccess)
            {
                ErrorMessage = result.Message;
                return Page();
            }
            else
            {
                return RedirectToPage("/Room/Index",new { mapId = mapId });
            }

            /*
            try
            {
                var map = await _db.Maps.FindAsync(mapId);
                if (map == null)
                {
                    return RedirectToPage("/Map/Index");
                }


                var room = _mapper.Map<RoomEntity>(Room);
                room.MapId = mapId;
                await _db.Rooms.AddAsync(room);


                if (id > 0)
                {
                    var oldRoom = await _db.Rooms.FindAsync(id);
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

                await _db.SaveChangesAsync();


                await AddSuccess(new OperatorLog
                {
                    Type = OperatorLogType.添加房间,
                    Content = JsonConvert.SerializeObject(Room)
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await AddError(new OperatorLog
                {
                    Type = OperatorLogType.添加房间,
                    Content = $"Data={JsonConvert.SerializeObject(Room)},ErrorMessage={ErrorMessage}"
                });
                return Page();
            }

            return Redirect(UrlReferer);
            */
        }
    }
}
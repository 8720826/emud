using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Api;
using Emprise.Admin.Data;
using Emprise.Admin.Entity;
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
    public class AddModel : BasePageModel
    {


        public AddModel(IMudClient mudClient,
            IMapper mapper,
            ILogger<AddModel> logger,
            EmpriseDbContext db,
            IOptionsMonitor<AppConfig> appConfig,
            IHttpContextAccessor httpAccessor)
            : base(db, appConfig, httpAccessor, mapper, logger, mudClient)
        {

        }

        [BindProperty]
        public RoomInput Room { get; set; }

        public string Tips { get; set; }
        public string ErrorMessage { get; set; }

        [BindProperty]
        public string UrlReferer { get; set; }

        public async Task OnGetAsync(int id, string position, int mapId)
        {
            UrlReferer = Request.Headers["Referer"].ToString();

            if (id > 0)
            {
                var room = await _db.Rooms.FindAsync(id);
                if (room != null)
                {
                    Tips = $"在房间 {room.Name}(id={id}) 的{position}添加新房间";

                    if (string.IsNullOrEmpty(UrlReferer))
                    {
                        UrlReferer = Url.Page("/Room/Edit/"+ id);
                    }
                }
            }

            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/Room/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(int id, string position, int mapId)
        {
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }




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

        }
    }
}
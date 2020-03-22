using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Data;
using Emprise.Admin.Models.Room;
using Emprise.Domain.Room.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Emprise.Admin.Pages.Room
{
    public class AddModel : PageModel
    {
        protected readonly EmpriseDbContext _db;
        private readonly IMapper _mapper;


        public AddModel(EmpriseDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
           
        }

        [BindProperty]
        public RoomInput Room { get; set; }

        public string Tips { get; set; }
        public string SueccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        [BindProperty]
        public string UrlReferer { get; set; }

        public async Task OnGetAsync(int id, string position)
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

        public async Task<IActionResult> OnPostAsync(int id, string position)
        {
            SueccessMessage = "";
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                ErrorMessage = ModelState.Where(e => e.Value.Errors.Count > 0).Select(e => e.Value.Errors.First().ErrorMessage).First();
                return Page();
            }

            var room = _mapper.Map<RoomEntity>(Room);
            await _db.Rooms.AddAsync(room);

            await _db.SaveChangesAsync();

            if (id > 0)
            {
                var oldRoom = await _db.Rooms.FindAsync(id);
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

            await _db.SaveChangesAsync();


            SueccessMessage = $"添加成功！";

            //return RedirectToPage("Edit",new { id= room.Id });

            return Redirect(UrlReferer);

        }
    }
}
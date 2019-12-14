using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Api;
using Emprise.Admin.Data;
using Emprise.Admin.Models.Room;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Emprise.Admin.Pages.Room
{
    public class EditModel : PageModel
    {
        protected readonly EmpriseDbContext _db;
        private readonly IMapper _mapper;
        private readonly IMudClient _mudClient;
        public EditModel(EmpriseDbContext db, IMapper mapper, IMudClient mudClient)
        {
            _db = db;
            _mapper = mapper;
            _mudClient = mudClient;
        }
        public int Id { get; set; }



        [BindProperty]
        public RoomInput Room { get; set; }

        public string SueccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public async Task OnGetAsync(int id)
        {
            Id = id;
            var room = await _db.Rooms.FindAsync(id);

            Room = _mapper.Map<RoomInput>(room);

        }


        public async Task<IActionResult> OnPostAsync(int id)
        {
            Id = id;
            SueccessMessage = "";
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                ErrorMessage = ModelState.Where(e => e.Value.Errors.Count > 0).Select(e => e.Value.Errors.First().ErrorMessage).First();
                return Page();
            }

            var room = await _db.Rooms.FindAsync(id);
            if (room == null)
            {
                ErrorMessage = $"房间 {id} 不存在！";
                return Page();
            }

            int west = room.West;
            int east = room.East;
            int south = room.South;
            int north = room.North;

            _mapper.Map(Room, room);

            List<int> changedIds = new List<int>();
            changedIds.Add(room.Id);

            if (west != room.West)
            {
                if (west > 0)
                {
                    var oldRoomWest = await _db.Rooms.FindAsync(west);
                    oldRoomWest.East = 0;
                    oldRoomWest.EastName = "";

                    changedIds.Add(oldRoomWest.Id);
                }
            }

            if (room.West > 0)
            {
                var roomWest = await _db.Rooms.FindAsync(room.West);
                roomWest.East = room.Id;
                roomWest.EastName = room.Name;

                room.WestName = roomWest.Name;

                changedIds.Add(roomWest.Id);
            }
            else
            {
                room.WestName = "";
            }


            if (east != room.East)
            {
                if (east > 0)
                {
                    var oldRoomEast = await _db.Rooms.FindAsync(east);
                    oldRoomEast.West = 0;
                    oldRoomEast.WestName = "";

                    changedIds.Add(oldRoomEast.Id);
                }
            }

            if (room.East > 0)
            {
                var roomEast = await _db.Rooms.FindAsync(room.East);
                roomEast.West = room.Id;
                roomEast.WestName = room.Name;

                room.EastName = roomEast.Name;

                changedIds.Add(roomEast.Id);
            }
            else
            {
                room.EastName = "";
            }

            if (south != room.South)
            {
                if (south > 0)
                {
                    var oldRoomSouth = await _db.Rooms.FindAsync(south);
                    oldRoomSouth.North = 0;
                    oldRoomSouth.NorthName = "";

                    changedIds.Add(oldRoomSouth.Id);
                }
            }

            if (room.South > 0)
            {
                var roomSouth = await _db.Rooms.FindAsync(room.South);
                roomSouth.North = room.Id;
                roomSouth.NorthName = room.Name;

                room.SouthName = roomSouth.Name;

                changedIds.Add(roomSouth.Id);
            }
            else
            {
                room.SouthName = "";
            }

            if (north != room.North)
            {
                if (north > 0)
                {
                    var oldRoomNorth = await _db.Rooms.FindAsync(north);
                    oldRoomNorth.South = 0;
                    oldRoomNorth.SouthName = "";

                    changedIds.Add(oldRoomNorth.Id);
                }
            }

            if (room.North > 0)
            {
                var roomNorth = await _db.Rooms.FindAsync(room.North);
                roomNorth.South = room.Id;
                roomNorth.SouthName = room.Name;

                room.NorthName = roomNorth.Name;

                changedIds.Add(roomNorth.Id);
            }
            else
            {
                room.NorthName = "";
            }



            await _db.SaveChangesAsync();

            foreach (var roomId in changedIds)
            {
               var result =   await _mudClient.EditRoom(roomId);

                ErrorMessage += result.StatusCode;
            }




            SueccessMessage = $"修改成功！";

            Room = _mapper.Map<RoomInput>(room);

            return Page();
        

        }
    }
}
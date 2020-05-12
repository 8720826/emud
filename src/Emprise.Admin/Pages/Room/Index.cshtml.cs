using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Admin.Data;
using Emprise.Admin.Entity;
using Emprise.Admin.Extensions;
using Emprise.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Emprise.Admin.Pages.Room
{
    public class IndexModel : PageModel
    {
        protected readonly EmpriseDbContext _db;

        public IndexModel(EmpriseDbContext db)
        {
            _db = db;
        }


        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }

        public MapEntity Map { get; set; }

        public Paging<RoomEntity> Paging { get; set; }

        public async Task<IActionResult> OnGetAsync(int mapId, int pageIndex)
        {
            Map = await _db.Maps.FindAsync(mapId);
            if (Map == null)
            {
                return RedirectToPage("/Map/Index");
            }


            var query = _db.Rooms.Where(x => x.MapId == mapId);
            if (!string.IsNullOrEmpty(Keyword))
            {
                query = query.Where(x => x.Name.Contains(Keyword));
            }

            query = query.OrderBy(x => x.Id);

            Paging = query.Paged(pageIndex, 10, query.Count());

            return Page();
        }


    }
}
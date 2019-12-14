using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Admin.Data;
using Emprise.Admin.Extensions;
using Emprise.Admin.Models;
using Emprise.Domain.Room.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Emprise.Admin.Pages.Room
{
    public class ListModel : PageModel
    {

        protected readonly EmpriseDbContext _db;

        public ListModel(EmpriseDbContext db)
        {
            _db = db;
        }
        public Paging<RoomEntity> Paging { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }

        public void OnGet(int pageIndex)
        {
            var query = _db.Rooms.OrderBy(x => x.Id);
            if (!string.IsNullOrEmpty(Keyword))
            {
                query = _db.Rooms.Where(x => x.Name.Contains(Keyword)).OrderBy(x => x.Id);
            }

            Paging = query.Paged(pageIndex, 10, query.Count());
        }
    }
}
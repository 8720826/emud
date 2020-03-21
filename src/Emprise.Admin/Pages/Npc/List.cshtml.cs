using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Admin.Data;
using Emprise.Admin.Extensions;
using Emprise.Admin.Models;
using Emprise.Domain.Npc.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Emprise.Admin.Pages.Npc
{
    public class ListModel : PageModel
    {
        protected readonly EmpriseDbContext _db;

        public ListModel(EmpriseDbContext db)
        {
            _db = db;
        }


        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }


        public Paging<NpcEntity> Paging { get; set; }

        public void OnGet(int pageIndex)
        {
            var query = _db.Npcs.Include(x => x.NpcScripts).ThenInclude(y => y.Script).OrderBy(x => x.Id);
            if (!string.IsNullOrEmpty(Keyword))
            {
                query = _db.Npcs.Include(x => x.NpcScripts).ThenInclude(y => y.Script).Where(x => x.Name.Contains(Keyword)).OrderBy(x => x.Id);
            }

            Paging = query.Paged(pageIndex, 10, query.Count());
        }

    }
}
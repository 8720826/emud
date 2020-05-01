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
using Microsoft.EntityFrameworkCore;

namespace Emprise.Admin.Pages.Script
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


        public Paging<ScriptEntity> Paging { get; set; }

        public void OnGet(int pageIndex)
        {
            var query = _db.Scripts.Include(x => x.NpcScripts).ThenInclude(y => y.Npc).AsQueryable();
            if (!string.IsNullOrEmpty(Keyword))
            {
                query = query.Where(x => x.Name.Contains(Keyword));
            }



            query = query.OrderBy(x => x.Id);

            Paging = query.Paged(pageIndex, 10, query.Count());
        }

    }
}
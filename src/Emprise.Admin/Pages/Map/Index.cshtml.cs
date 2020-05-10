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

namespace Emprise.Admin.Pages.Map
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

        public Paging<MapEntity> Paging { get; set; }

        public void OnGet(int pageIndex)
        {
            var query = _db.Maps.OrderBy(x => x.Id);
            if (!string.IsNullOrEmpty(Keyword))
            {
                query = _db.Maps.Where(x => x.Name.Contains(Keyword)).OrderBy(x => x.Id);
            }

            Paging = query.Paged(pageIndex, 10, query.Count());
        }
    }
}
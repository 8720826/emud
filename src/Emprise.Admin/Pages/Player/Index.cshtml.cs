using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Data;
using Emprise.Admin.Entity;
using Emprise.Admin.Extensions;
using Emprise.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Emprise.Admin.Pages.Player
{
    public class IndexModel : PageModel
    {
        private readonly IMapper _mapper;

        protected readonly EmpriseDbContext _db;

        public IndexModel(EmpriseDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }


        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }

        public Paging<PlayerEntity> Paging { get; set; }

        public void OnGet(int pageIndex)
        {
            var query = _db.Players.OrderBy(x => x.Id);
            if (!string.IsNullOrEmpty(Keyword))
            {
                query = _db.Players.Where(x => x.Name.Contains(Keyword)).OrderBy(x => x.Id);
            }

            Paging = query.Paged(pageIndex, 10, query.Count());


        }
    }
}
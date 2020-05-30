using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Api;
using Emprise.Admin.Data;
using Emprise.Admin.Entity;
using Emprise.Admin.Extensions;
using Emprise.Admin.Models;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emprise.Admin.Pages.Npc
{
    public class ListModel : BasePageModel
    {

        public ListModel(IMudClient mudClient,
            IMapper mapper,
            ILogger<ListModel> logger,
            EmpriseDbContext db,
            IOptionsMonitor<AppConfig> appConfig,
            IHttpContextAccessor httpAccessor)
            : base(db, appConfig, httpAccessor, mapper, logger, mudClient)
        {

        }


        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }


        public Paging<NpcEntity> Paging { get; set; }

        public async Task OnGetAsync(int pageIndex)
        {
            var query = _db.Npcs.Include(x => x.NpcScripts).ThenInclude(y => y.Script).OrderBy(x => x.Id);
            if (!string.IsNullOrEmpty(Keyword))
            {
                query = _db.Npcs.Include(x => x.NpcScripts).ThenInclude(y => y.Script).Where(x => x.Name.Contains(Keyword)).OrderBy(x => x.Id);
            }

            Paging = await query.Paged(pageIndex);
        }

    }
}
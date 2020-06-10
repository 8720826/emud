using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Api;
using Emprise.Admin.Data;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Models;
using Emprise.Domain.ItemDrop.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emprise.Admin.Pages.ItemDrop
{
    public class IndexModel : BasePageModel
    {
        public IndexModel(IMudClient mudClient,
            IMapper mapper,
            ILogger<IndexModel> logger,
            EmpriseDbContext db,
            IOptionsMonitor<AppConfig> appConfig,
            IHttpContextAccessor httpAccessor)
            : base(db, appConfig, httpAccessor, mapper, logger, mudClient)
        {

        }


        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }

        public Paging<ItemDropEntity> Paging { get; set; }

        public async Task OnGetAsync(int pageIndex)
        {
            var query = _db.ItemDrops.OrderBy(x => x.Id);
            if (!string.IsNullOrEmpty(Keyword))
            {
                query = _db.ItemDrops.Where(x => x.Name.Contains(Keyword)).OrderBy(x => x.Id);
            }

            Paging = await query.Paged(pageIndex);
        }
    }
}
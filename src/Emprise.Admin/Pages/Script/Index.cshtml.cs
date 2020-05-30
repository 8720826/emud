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

namespace Emprise.Admin.Pages.NpcScript
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

        [BindProperty(SupportsGet = true)]
        public int NpcId { get; set; }

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


        public async Task<IActionResult> OnPostAsync([FromBody]EnableData enableData)
        {
            try
            {
                var npcScript = await _db.Scripts.FindAsync(enableData.SId);
                if (npcScript == null)
                {
                    return await Task.FromResult(new JsonResult(enableData));
                }

                npcScript.IsEnable = enableData.IsEnable;
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
            }


            return  await Task.FromResult(new JsonResult(enableData));

        }

        public class EnableData
        {
            public int SId { get; set; }
            public bool IsEnable { get; set; }
        }
    }
}
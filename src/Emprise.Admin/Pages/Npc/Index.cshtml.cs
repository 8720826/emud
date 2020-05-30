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
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Admin.Pages.Npc
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

        public Paging<NpcEntity> Paging { get; set; }

        public void OnGet(int pageIndex)
        {
            var query = _db.Npcs.Include(x=>x.NpcScripts).ThenInclude(y => y.Script).OrderBy(x => x.Id);
            if (!string.IsNullOrEmpty(Keyword))
            {
                query = _db.Npcs.Include(x => x.NpcScripts).ThenInclude(y => y.Script).Where(x => x.Name.Contains(Keyword)).OrderBy(x => x.Id);
            }

            Paging = query.Paged(pageIndex, 10, query.Count());


        }

        public async Task<IActionResult> OnPostAsync([FromBody]EnableData enableData)
        {
            try
            {
                var npc = await _db.Npcs.FindAsync(enableData.SId);
                if (npc == null)
                {
                    return await Task.FromResult(new JsonResult(enableData));
                }

                npc.IsEnable = enableData.IsEnable;
                await _db.SaveChangesAsync();

                await AddSuccess(new OperatorLog
                {
                    Type = OperatorLogType.修改Npc,
                    Content = JsonConvert.SerializeObject(enableData)
                });
            }
            catch(Exception ex)
            {
                await AddError(new OperatorLog
                {
                    Type = OperatorLogType.修改Npc,
                    Content = $"Data={JsonConvert.SerializeObject(enableData)},ErrorMessage={ex.Message}"
                });
            }

            return await Task.FromResult(new JsonResult(enableData));

        }

        public class EnableData
        {
            public int SId { get; set; }
            public bool IsEnable { get; set; }
        }
    }
}
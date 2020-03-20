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

namespace Emprise.Admin.Pages.Npc
{
    public class ScriptModel : PageModel
    {
        protected readonly EmpriseDbContext _db;

        public ScriptModel(EmpriseDbContext db)
        {
            _db = db;
        }

        [BindProperty(SupportsGet = true)]
        public int NpcId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }

        public Paging<ScriptEntity> Paging { get; set; }

        public void OnGet(int pageIndex)
        {
            var scriptIds = _db.NpcScripts.Where(x => x.NpcId == NpcId).Select(x => x.ScriptId).ToList();

            var query = _db.Script.Where(x => scriptIds.Contains(x.Id)).OrderBy(x => x.Id);
            if (!string.IsNullOrEmpty(Keyword))
            {
                query = _db.Script.Where(x => scriptIds.Contains(x.Id) && x.Name.Contains(Keyword)).OrderBy(x => x.Id);
            }

            Paging = query.Paged(pageIndex, 10, query.Count());
        }


        public async Task<IActionResult> OnPostAsync([FromBody]EnableData enableData)
        {
            var npcScript = _db.Script.Find(enableData.SId);
            if (npcScript == null)
            {
                return await Task.FromResult(new JsonResult(enableData));
            }

            npcScript.IsEnable = enableData.IsEnable;
            await _db.SaveChangesAsync();

            return await Task.FromResult(new JsonResult(enableData));

        }

        public class EnableData
        {
            public int SId { get; set; }
            public bool IsEnable { get; set; }
        }
    }
}
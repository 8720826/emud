using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Admin.Data;
using Emprise.Domain.Npc.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Emprise.Admin.Pages.NpcScript
{
    public class IndexModel : PageModel
    {
        protected readonly EmpriseDbContext _db;

        public IndexModel(EmpriseDbContext db)
        {
            _db = db;
        }




        public List<NpcScriptEntity> NpcScripts { get; set; }

        public void OnGet(int sId)
        {
            NpcScripts = _db.NpcScripts.Where(x => x.ScriptId==sId).ToList();
        }
    }
}
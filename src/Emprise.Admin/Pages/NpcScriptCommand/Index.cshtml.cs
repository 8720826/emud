using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Admin.Data;
using Emprise.Domain.Npc.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Emprise.Admin.Pages.NpcScriptCommand
{
    public class IndexModel : PageModel
    {
        protected readonly EmpriseDbContext _db;

        public IndexModel(EmpriseDbContext db)
        {
            _db = db;
        }




        public List<NpcScriptCommandEntity> Commands { get; set; }

        public void OnGet(int sId)
        {
            Commands = _db.NpcScriptCommands.Where(x => x.ScriptId==sId).ToList();
        }
    }
}
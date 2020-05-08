using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Data;
using Emprise.Admin.Entity;
using Emprise.Admin.Models.Npc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Emprise.Admin.Pages.Npc
{
    public class EditModel : PageModel
    {
        protected readonly EmpriseDbContext _db;
        private readonly IMapper _mapper;

        public EditModel(EmpriseDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [BindProperty]
        public NpcInput Npc { get; set; }

        public string Tips { get; set; }
        public string SueccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        [BindProperty]
        public List<int> ScriptIds { get; set; } = new List<int>();

        public List<ScriptEntity> Scripts { get; set; } = new List<ScriptEntity>();

        [BindProperty]
        public string UrlReferer { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            UrlReferer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/Npc/Index");
            }

            if (id > 0)
            {
                var npc = await _db.Npcs.FindAsync(id);

                Npc = _mapper.Map<NpcInput>(npc);

                var npcScripts = _db.NpcScripts.Where(x => x.NpcId == id);

                var ids = npcScripts.Select(x => x.ScriptId).ToList();


                Scripts = _db.Scripts.Where(x => ids.Contains(x.Id)).ToList();

                return Page();
            }
            else
            {
                return RedirectToPage("/Npc/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            SueccessMessage = "";
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                ErrorMessage = ModelState.Where(e => e.Value.Errors.Count > 0).Select(e => e.Value.Errors.First().ErrorMessage).First();
                return Page();
            }


            var npc = await _db.Npcs.FindAsync(id);
            _mapper.Map(Npc, npc);
            if (npc.InitWords == null)
            {
                npc.InitWords = "";
            }



            var npcScripts = _db.NpcScripts.Where(x => x.NpcId == id);
            foreach(var npcScript in npcScripts)
            {
                if (!ScriptIds.Contains(npcScript.ScriptId))
                {
                    _db.NpcScripts.Remove(npcScript);
                }
                else
                {
                    ScriptIds.Remove(npcScript.ScriptId);
                }
            }


            foreach (var scriptId in ScriptIds)
            {
                _db.NpcScripts.Add(new NpcScriptEntity { NpcId = id, ScriptId = scriptId });
            }
            await _db.SaveChangesAsync();



            SueccessMessage = $"修改成功！";

            //return RedirectToPage("Edit", new { id = npc.Id });

            return Redirect(UrlReferer);
        }
    }
}
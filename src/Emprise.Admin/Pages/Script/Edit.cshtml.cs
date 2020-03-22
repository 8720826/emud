using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Data;
using Emprise.Admin.Models.Script;
using Emprise.Domain.Npc.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Emprise.Admin.Pages.NpcScript
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
        public ScriptInput Script { get; set; }

        public List<NpcEntity> Npcs { get; set; } = new List<NpcEntity>();


        [BindProperty]
        public List<int> NpcIds { get; set; }

        public string Tips { get; set; }
        public string SueccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        [BindProperty]
        public string UrlReferer { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            UrlReferer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/Script/Index");
            }

            if (id > 0)
            {
                var script = await _db.Scripts.FindAsync(id);

                Script = _mapper.Map<ScriptInput>(script);


                var npcScripts = _db.NpcScripts.Where(x => x.ScriptId == id);

                var ids = npcScripts.Select(x => x.NpcId).ToList();


                Npcs = _db.Npcs.Where(x => ids.Contains(x.Id)).ToList();

                return Page();

            }
            else
            {
                return RedirectToPage("/Script/Index");
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

            var script = await _db.Scripts.FindAsync(id);
            _mapper.Map(Script, script);
            await _db.SaveChangesAsync();


            var npcScripts = _db.NpcScripts.Where(x => x.ScriptId == id);
            foreach (var npcScript in npcScripts)
            {
                if (!NpcIds.Contains(npcScript.NpcId))
                {
                    _db.NpcScripts.Remove(npcScript);
                }
                else
                {
                    NpcIds.Remove(npcScript.NpcId);
                }
            }
            await _db.SaveChangesAsync();

            foreach (var npcId in NpcIds)
            {
                _db.NpcScripts.Add(new NpcScriptEntity { NpcId = npcId, ScriptId = id });
            }
            await _db.SaveChangesAsync();

            SueccessMessage = $"修改成功！";

            //return RedirectToPage("Edit", new { id = script.Id });

            return Redirect(UrlReferer);
        }



    }
}
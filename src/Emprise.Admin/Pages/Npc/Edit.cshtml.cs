using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Data;
using Emprise.Admin.Models.Npc;
using Emprise.Domain.Npc.Entity;
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

        public async Task OnGetAsync(int id)
        {
            if (id > 0)
            {
                var npc = await _db.Npcs.FindAsync(id);

                Npc = _mapper.Map<NpcInput>(npc);
            }
        }

        public async Task<IActionResult> OnPostAsync(int id, string position)
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

            npc.ScriptName = "";
            if (npc.ScriptId > 0)
            {
                var script = await _db.Scripts.FindAsync(npc.ScriptId);
                if (script != null)
                {
                    npc.ScriptName = script.Name;
                }
            }

            await _db.SaveChangesAsync();



            SueccessMessage = $"修改成功！";

            return RedirectToPage("Edit", new { id = npc.Id });


        }
    }
}
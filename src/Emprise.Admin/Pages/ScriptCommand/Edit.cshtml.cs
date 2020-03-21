using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Data;
using Emprise.Admin.Models.NpcScript;
using Emprise.Domain.Npc.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Emprise.Admin.Pages.ScriptCommand
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
        public ScriptCommandInput ScriptCommand { get; set; }

        public string Tips { get; set; }
        public string SueccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public async Task OnGetAsync(int id)
        {
            if (id > 0)
            {
                var scriptCommand = await _db.ScriptCommands.FindAsync(id);

                ScriptCommand = _mapper.Map<ScriptCommandInput>(scriptCommand);
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


            var scriptCommand = await _db.ScriptCommands.FindAsync(id);
            _mapper.Map(ScriptCommand, scriptCommand);
    
            await _db.SaveChangesAsync();



            SueccessMessage = $"修改成功！";

            return RedirectToPage("Edit", new { id = scriptCommand.Id });


        }
    }
}
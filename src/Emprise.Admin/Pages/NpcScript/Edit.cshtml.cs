using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Data;
using Emprise.Admin.Models.Script;
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
        public NpcScriptInput NpcScript { get; set; }

        public string Tips { get; set; }
        public string SueccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public async Task OnGetAsync(int id)
        {
            if (id > 0)
            {
                var task = await _db.NpcScripts.FindAsync(id);

                NpcScript = _mapper.Map<NpcScriptInput>(task);
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

            var script = await _db.NpcScripts.FindAsync(id);

            _mapper.Map(NpcScript, script);


            await _db.SaveChangesAsync();



            SueccessMessage = $"修改成功！";

            return RedirectToPage("Edit", new { id = script.Id });


        }
    }
}
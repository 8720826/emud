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
    public class DeleteModel : PageModel
    {
        protected readonly EmpriseDbContext _db;

        public DeleteModel(EmpriseDbContext db)
        {
            _db = db;
        }

        public ScriptEntity NpcScript { get; set; }

        public string SueccessMessage { get; set; }
        public string ErrorMessage { get; set; }
        public void OnGet(int id = 0)
        {
            NpcScript = _db.Scripts.Find(id);
        }

        public async Task<IActionResult> OnPostAsync(int id = 0)
        {
            SueccessMessage = "";
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                ErrorMessage = ModelState.Where(e => e.Value.Errors.Count > 0).Select(e => e.Value.Errors.First().ErrorMessage).First();
                return Page();
            }

             _db.Scripts.Remove(NpcScript);
            await _db.SaveChangesAsync();

            SueccessMessage = $"删除成功！";

            return RedirectToPage("Index");


        }
    }
}
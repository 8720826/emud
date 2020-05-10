using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Admin.Data;
using Emprise.Admin.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Emprise.Admin.Pages.ScriptCommand
{
    public class DeleteModel : PageModel
    {
        protected readonly EmpriseDbContext _db;

        public DeleteModel(EmpriseDbContext db)
        {
            _db = db;
        }

        public ScriptCommandEntity ScriptCommand { get; set; }

        public string SueccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        [BindProperty]
        public string UrlReferer { get; set; }


        public async Task<IActionResult> OnGetAsync(int id = 0)
        {
            UrlReferer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/Script/Index");
            }

            if (id > 0)
            {
                ScriptCommand = await _db.ScriptCommands.FindAsync(id);
                return Page();
            }
            else
            {
                return RedirectToPage("/Script/Index");
            }
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
            var scriptCommand = await _db.ScriptCommands.FindAsync(id);
            _db.ScriptCommands.Remove(scriptCommand);
            await _db.SaveChangesAsync();

            SueccessMessage = $"删除成功！";

            //return RedirectToPage("Index");

            return Redirect(UrlReferer);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Admin.Data;
using Emprise.Admin.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Emprise.Admin.Pages.Quest
{
    public class DeleteModel : PageModel
    {
        protected readonly EmpriseDbContext _db;

        public DeleteModel(EmpriseDbContext db)
        {
            _db = db;
        }

        public QuestEntity Quest { get; set; }

        public string ErrorMessage { get; set; }

        [BindProperty]
        public string UrlReferer { get; set; }


        public async Task<IActionResult> OnGetAsync(int id = 0)
        {
            UrlReferer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/Quest/Index");
            }

            if (id > 0)
            {
                Quest = await _db.Quests.FindAsync(id);
                return Page();
            }
            else
            {
                return RedirectToPage("/Quest/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(int id = 0)
        {
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var quest = await _db.Quests.FindAsync(id);
                _db.Quests.Remove(quest);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }


            return Redirect(UrlReferer);
        }
    }
}
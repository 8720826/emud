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
    public class IndexModel : PageModel
    {
        protected readonly EmpriseDbContext _db;

        public IndexModel(EmpriseDbContext db)
        {
            _db = db;
        }


        public int SId { get; set; }

        public List<ScriptCommandEntity> Commands { get; set; }

        [BindProperty]
        public string UrlReferer { get; set; }

        public void OnGet(int sId, string @ref = "")
        {
            SId = sId;
            UrlReferer = @ref;
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Request.Headers["Referer"].ToString();
            }
          
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/Script/Index");
            }


            Commands = _db.ScriptCommands.Where(x => x.ScriptId==sId).ToList();
        }
    }
}
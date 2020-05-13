using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Data;
using Emprise.Admin.Entity;
using Emprise.Admin.Models.Script;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Emprise.Admin.Pages.NpcScript
{
    public class AddModel : PageModel
    {
        protected readonly EmpriseDbContext _db;
        private readonly IMapper _mapper;

        public AddModel(EmpriseDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [BindProperty]
        public ScriptInput Script { get; set; }

        public string ErrorMessage { get; set; }

        [BindProperty]
        public string UrlReferer { get; set; }

        public void OnGet()
        {
            UrlReferer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/Script/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }



            try
            {
                var script = _mapper.Map<ScriptEntity>(Script);
                await _db.Scripts.AddAsync(script);
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
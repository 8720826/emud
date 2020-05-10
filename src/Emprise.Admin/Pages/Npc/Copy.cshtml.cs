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
    public class CopyModel : PageModel
    {
        protected readonly EmpriseDbContext _db;
        private readonly IMapper _mapper;

        public CopyModel(EmpriseDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [BindProperty]
        public NpcInput Npc { get; set; }

        public string ErrorMessage { get; set; }

        [BindProperty]
        public string UrlReferer { get; set; }

        public async Task OnGetAsync(int id)
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
            }


        }

        public async Task<IActionResult> OnPostAsync(int id, string position)
        {
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var npc = _mapper.Map<NpcEntity>(Npc);

                await _db.Npcs.AddAsync(npc);

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
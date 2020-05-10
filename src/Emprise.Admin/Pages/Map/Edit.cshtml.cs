using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Data;
using Emprise.Admin.Models.Map;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Emprise.Admin.Pages.Map
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
        public MapInput Map { get; set; }

        public string ErrorMessage { get; set; }


        [BindProperty]
        public string UrlReferer { get; set; }



        public async Task<IActionResult> OnGetAsync(int id)
        {
            UrlReferer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/Map/Index");
            }

            if (id > 0)
            {
                var map = await _db.Maps.FindAsync(id);

                Map = _mapper.Map<MapInput>(map);

                return Page();
            }
            else
            {
                return RedirectToPage("/Map/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var map = await _db.Maps.FindAsync(id);
                _mapper.Map(Map, map);

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Data;
using Emprise.Admin.Entity;
using Emprise.Admin.Models.Quest;
using Emprise.Domain.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Emprise.Admin.Pages.Quest
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
        public QuestInput Quest { get; set; }

        public string ErrorMessage { get; set; }

        public Array Conditions { get; set; }

        [BindProperty]
        public string UrlReferer { get; set; }

        public void OnGet()
        {
            Conditions = Enum.GetNames(typeof(QuestTriggerConditionEnum));


            UrlReferer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/Quest/Index");
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
                var task = _mapper.Map<QuestEntity>(Quest);
                await _db.Quests.AddAsync(task);
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Data;
using Emprise.Admin.Models.Quest;
using Emprise.Domain.Core.Enum;
using Emprise.Domain.Quest.Entity;
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

        public string Tips { get; set; }
        public string SueccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public Array Conditions { get; set; }

        [BindProperty]
        public string UrlReferer { get; set; }

        public async Task OnGetAsync(int id)
        {
            Conditions = Enum.GetNames(typeof(QuestTriggerConditionEnum));

            if (id > 0)
            {
                var task = await _db.Quests.FindAsync(id);

            }

            UrlReferer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/Task/Index");
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

            var task = _mapper.Map<QuestEntity>(Quest);
            await _db.Quests.AddAsync(task);

            await _db.SaveChangesAsync();



            SueccessMessage = $"添加成功！";

            return Redirect(UrlReferer);


        }
    }
}
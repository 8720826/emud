using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Data;
using Emprise.Admin.Models.Quest;
using Emprise.Domain.Core.Enum;
using Emprise.Domain.Quest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Emprise.Admin.Pages.Quest
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
        public QuestInput Quest { get; set; }

        public string Tips { get; set; }
        public string SueccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public List<QuestTrigger> TakeConditions { get; set; } = new List<QuestTrigger>();
        

        public List<QuestTarget> TaskTargets { get; set; } = new List<QuestTarget>();

        public List<QuestConsume> TaskConsumes { get; set; } = new List<QuestConsume>();

        public List<QuestReward> TaskRewards { get; set; } = new List<QuestReward>();


        [BindProperty]
        public string UrlReferer { get; set; }

        public async Task OnGetAsync(int id)
        {
            if (id > 0)
            {
                var quest = await _db.Quests.FindAsync(id);

                Quest = _mapper.Map<QuestInput>(quest);



                if (!string.IsNullOrEmpty(quest.TakeCondition))
                {
                    TakeConditions = JsonConvert.DeserializeObject<List<QuestTrigger>>(quest.TakeCondition);
                }

                

                if (!string.IsNullOrEmpty(quest.Target))
                {
                    TaskTargets = JsonConvert.DeserializeObject<List<QuestTarget>>(quest.Target);
                }

                if (!string.IsNullOrEmpty(quest.Consume))
                {
                    TaskConsumes = JsonConvert.DeserializeObject<List<QuestConsume>>(quest.Consume);
                }

                if (!string.IsNullOrEmpty(quest.Reward))
                {
                    TaskRewards = JsonConvert.DeserializeObject<List<QuestReward>>(quest.Reward);
                }

            }

            UrlReferer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/Quest/Index");
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

            var quest = await _db.Quests.FindAsync(id);

            _mapper.Map(Quest, quest);


            await _db.SaveChangesAsync();



            SueccessMessage = $"修改成功！";

            return Redirect(UrlReferer);


        }
    }
}
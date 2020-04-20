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

        public Array Conditions { get; set; }


        public List<TaskTrigger> TaskTriggers { get; set; } = new List<TaskTrigger>();

        public List<TaskTarget> TaskTargets { get; set; } = new List<TaskTarget>();

        public List<TaskConsume> TaskConsumes { get; set; } = new List<TaskConsume>();

        public List<TaskReward> TaskRewards { get; set; } = new List<TaskReward>();


        [BindProperty]
        public string UrlReferer { get; set; }

        public async Task OnGetAsync(int id)
        {
            if (id > 0)
            {
                var quest = await _db.Quests.FindAsync(id);

                Quest = _mapper.Map<QuestInput>(quest);

                Conditions = Enum.GetNames(typeof(QuestTriggerConditionEnum));

                if (!string.IsNullOrEmpty(quest.TriggerCondition))
                {
                    TaskTriggers = JsonConvert.DeserializeObject<List<TaskTrigger>>(quest.TriggerCondition);
                }


                if (!string.IsNullOrEmpty(quest.Target))
                {
                    TaskTargets = JsonConvert.DeserializeObject<List<TaskTarget>>(quest.Target);
                }

                if (!string.IsNullOrEmpty(quest.Consume))
                {
                    TaskConsumes = JsonConvert.DeserializeObject<List<TaskConsume>>(quest.Consume);
                }

                if (!string.IsNullOrEmpty(quest.Reward))
                {
                    TaskRewards = JsonConvert.DeserializeObject<List<TaskReward>>(quest.Reward);
                }

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

            var quest = await _db.Quests.FindAsync(id);

            _mapper.Map(Quest, quest);


            await _db.SaveChangesAsync();



            SueccessMessage = $"修改成功！";

            return Redirect(UrlReferer);


        }
    }
}
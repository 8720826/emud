using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Data;
using Emprise.Admin.Models.Tasks;
using Emprise.Domain.Core.Enum;
using Emprise.Domain.Tasks.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Emprise.Admin.Pages.Tasks
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
        public TaskInput Task { get; set; }

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
                var task = await _db.Tasks.FindAsync(id);

                Task = _mapper.Map<TaskInput>(task);

                Conditions = Enum.GetNames(typeof(TaskTriggerConditionEnum));

                if (!string.IsNullOrEmpty(task.TriggerCondition))
                {
                    TaskTriggers = JsonConvert.DeserializeObject<List<TaskTrigger>>(task.TriggerCondition);
                }


                if (!string.IsNullOrEmpty(task.Target))
                {
                    TaskTargets = JsonConvert.DeserializeObject<List<TaskTarget>>(task.Target);
                }

                if (!string.IsNullOrEmpty(task.Consume))
                {
                    TaskConsumes = JsonConvert.DeserializeObject<List<TaskConsume>>(task.Consume);
                }

                if (!string.IsNullOrEmpty(task.Reward))
                {
                    TaskRewards = JsonConvert.DeserializeObject<List<TaskReward>>(task.Reward);
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

            var task = await _db.Tasks.FindAsync(id);

            _mapper.Map(Task, task);


            await _db.SaveChangesAsync();



            SueccessMessage = $"修改成功！";

            return Redirect(UrlReferer);


        }
    }
}
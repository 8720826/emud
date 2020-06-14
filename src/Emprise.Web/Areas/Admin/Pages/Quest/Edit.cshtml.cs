using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.Quest.Dtos;
using Emprise.Application.Quest.Services;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Quest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Web.Areas.Admin.Pages.Quest
{
    public class EditModel : BaseAdminPageModel
    {
        private readonly IQuestAppService _questAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public EditModel(
            ILogger<EditModel> logger,
            IQuestAppService questAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _questAppService = questAppService;
            _appConfig = appConfig.CurrentValue;
        }


        [BindProperty]
        public QuestInput Quest { get; set; }

        public string ErrorMessage { get; set; }

        public List<QuestTrigger> TakeConditions { get; set; } = new List<QuestTrigger>();
        

        public List<QuestTarget> TaskTargets { get; set; } = new List<QuestTarget>();

        public List<QuestConsume> TaskConsumes { get; set; } = new List<QuestConsume>();

        public List<QuestReward> TaskRewards { get; set; } = new List<QuestReward>();



        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id > 0)
            {
                var quest = await _questAppService.Get(id);
                if (quest == null)
                {
                    ErrorMessage = $"任务 {id} 不存在！";
                    return Page();
                }
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

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _questAppService.Update(id, Quest);
            if (!result.IsSuccess)
            {
                ErrorMessage = result.Message;
                return Page();
            }
            else
            {
                return RedirectToPage("/Quest/Index");
            }
            /*
            try
            {
                var quest = await _db.Quests.FindAsync(id);
                if (quest == null)
                {
                    ErrorMessage = $"任务 {id} 不存在！";
                    return Page();
                }
                var content = DifferenceComparison(quest, Quest);
                _mapper.Map(Quest, quest);
                await _db.SaveChangesAsync();

                await AddSuccess(new OperatorLog
                {
                    Type = OperatorLogType.修改任务,
                    Content = $"Id = {id},Data = {content}"
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await AddError(new OperatorLog
                {
                    Type = OperatorLogType.修改任务,
                    Content = $"Id = {id},Data={JsonConvert.SerializeObject(Quest)},ErrorMessage={ErrorMessage}"
                });
                return Page();
            }

            return Redirect(UrlReferer);
            */

        }
    }
}
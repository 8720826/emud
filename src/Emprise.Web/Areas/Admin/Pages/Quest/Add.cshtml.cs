using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.Quest.Dtos;
using Emprise.Application.Quest.Services;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Web.Areas.Admin.Pages.Quest
{
    public class AddModel : BaseAdminPageModel
    {
        private readonly IQuestAppService _questAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public AddModel(
            ILogger<AddModel> logger,
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

        public Array Conditions { get; set; }


        public void OnGet()
        {
            Conditions = Enum.GetNames(typeof(QuestTriggerConditionEnum));

        }

        public async Task<IActionResult> OnPostAsync()
        {
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                Conditions = Enum.GetNames(typeof(QuestTriggerConditionEnum));
                return Page();
            }

            var result = await _questAppService.Add( Quest);
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
                var task = _mapper.Map<QuestEntity>(Quest);
                await _db.Quests.AddAsync(task);
                await _db.SaveChangesAsync();

                await AddSuccess(new OperatorLog
                {
                    Type = OperatorLogType.添加任务,
                    Content = JsonConvert.SerializeObject(Quest)
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await AddError(new OperatorLog
                {
                    Type = OperatorLogType.添加任务,
                    Content = $"Data={JsonConvert.SerializeObject(Quest)},ErrorMessage={ErrorMessage}"
                });
                return Page();
            }



            return Redirect(UrlReferer);
            */

        }
    }
}
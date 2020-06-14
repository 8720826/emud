using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.Quest.Services;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Quest.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Web.Areas.Admin.Pages.Quest
{
    public class DeleteModel : BaseAdminPageModel
    {
        private readonly IQuestAppService _questAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public DeleteModel(
            ILogger<DeleteModel> logger,
            IQuestAppService questAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _questAppService = questAppService;
            _appConfig = appConfig.CurrentValue;
        }


        public QuestEntity Quest { get; set; }

        public string ErrorMessage { get; set; }



        public async Task<IActionResult> OnGetAsync(int id = 0)
        {
            if (id > 0)
            {
                Quest = await _questAppService.Get(id);
                if (Quest == null)
                {
                    ErrorMessage = $"任务 {id} 不存在！";
                    return Page();
                }
                return Page();
            }
            else
            {
                return RedirectToPage("/Quest/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(int id = 0)
        {
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _questAppService.Delete(id);
            if (!result.IsSuccess)
            {
                ErrorMessage = result.Message;
                return Page();
            }
            else
            {
                return RedirectToPage("/Ware/Index");
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
                _db.Quests.Remove(quest);
                await _db.SaveChangesAsync();

                await AddSuccess(new OperatorLog
                {
                    Type = OperatorLogType.删除任务,
                    Content = JsonConvert.SerializeObject(quest)
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await AddError(new OperatorLog
                {
                    Type = OperatorLogType.删除任务,
                    Content = $"id={id}，ErrorMessage={ErrorMessage}"
                });
                return Page();
            }


            return Redirect(UrlReferer);
            */
        }
    }
}
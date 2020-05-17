using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Api;
using Emprise.Admin.Data;
using Emprise.Admin.Entity;
using Emprise.Admin.Extensions;
using Emprise.Admin.Models;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Admin.Pages.Quest
{
    public class DeleteModel : BasePageModel
    {
        public DeleteModel(IMudClient mudClient,
            IMapper mapper,
            ILogger<DeleteModel> logger,
            EmpriseDbContext db,
            IOptionsMonitor<AppConfig> appConfig,
            IHttpContextAccessor httpAccessor)
            : base(db, appConfig, httpAccessor, mapper, logger, mudClient)
        {

        }

        public QuestEntity Quest { get; set; }

        public string ErrorMessage { get; set; }

        [BindProperty]
        public string UrlReferer { get; set; }


        public async Task<IActionResult> OnGetAsync(int id = 0)
        {
            UrlReferer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/Quest/Index");
            }

            if (id > 0)
            {
                Quest = await _db.Quests.FindAsync(id);
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
        }
    }
}
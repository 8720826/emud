using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Api;
using Emprise.Admin.Data;
using Emprise.Admin.Models.NpcScript;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Npc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Admin.Pages.ScriptCommand
{
    public class EditModel : BasePageModel
    {
        public EditModel(IMudClient mudClient,
            IMapper mapper,
            ILogger<EditModel> logger,
            EmpriseDbContext db,
            IOptionsMonitor<AppConfig> appConfig,
            IHttpContextAccessor httpAccessor)
            : base(db, appConfig, httpAccessor, mapper, logger, mudClient)
        {

        }

        [BindProperty]
        public ScriptCommandInput ScriptCommand { get; set; }

        public string ErrorMessage { get; set; }

        public Array Conditions { get; set; }

        public Array Fields { get; set; }

        public Array Relations { get; set; }

        public Array Events { get; set; }

        public Array Commands { get; set; }

        public Array Activities { get; set; }

        [BindProperty]
        public string UrlReferer { get; set; }


        public List<CaseIf> CaseIfs { get; set; } = new List<CaseIf>();

        public List<CaseThen> CaseThens { get; set; } = new List<CaseThen>();

        public List<CaseElse> CaseElses { get; set; } = new List<CaseElse>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            ErrorMessage = "";
            UrlReferer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/ScriptCommand/Index",new { sId = id });
            }

            Conditions = Enum.GetNames(typeof(ConditionTypeEnum));

            Fields = Enum.GetNames(typeof(PlayerConditionFieldEnum));

            Relations = Enum.GetNames(typeof(LogicalRelationTypeEnum));

            Events = Enum.GetNames(typeof(PlayerEventTypeEnum));

            Commands = Enum.GetNames(typeof(CommandTypeEnum));

            Activities = Enum.GetNames(typeof(ActivityTypeEnum));

            if (id > 0)
            {
                var scriptCommand = await _db.ScriptCommands.FindAsync(id);

                ScriptCommand = _mapper.Map<ScriptCommandInput>(scriptCommand);

                if (!string.IsNullOrEmpty(scriptCommand.CaseIf))
                {
                    CaseIfs = JsonConvert.DeserializeObject<List<CaseIf>>(scriptCommand.CaseIf);
                }


                if (!string.IsNullOrEmpty(scriptCommand.CaseThen))
                {
                    CaseThens = JsonConvert.DeserializeObject<List<CaseThen>>(scriptCommand.CaseThen);
                }


                if (!string.IsNullOrEmpty(scriptCommand.CaseElse))
                {
                    CaseElses = JsonConvert.DeserializeObject<List<CaseElse>>(scriptCommand.CaseElse);
                }


                return Page();

            }
            else
            {
                return RedirectToPage("/Script/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {

                Conditions = Enum.GetNames(typeof(ConditionTypeEnum));

                Fields = Enum.GetNames(typeof(PlayerConditionFieldEnum));

                Relations = Enum.GetNames(typeof(LogicalRelationTypeEnum));

                Events = Enum.GetNames(typeof(PlayerEventTypeEnum));

                Commands = Enum.GetNames(typeof(CommandTypeEnum));
                return Page();
            }




            try
            {
                var scriptCommand = await _db.ScriptCommands.FindAsync(id);
                var content = DifferenceComparison(scriptCommand, ScriptCommand);
                _mapper.Map(ScriptCommand, scriptCommand);

                if (scriptCommand.IsEntry)
                {
                    var scriptCommands = _db.ScriptCommands.Where(x => x.ScriptId == scriptCommand.ScriptId).ToList();

                    foreach (var command in scriptCommands.Where(x => x.Id != scriptCommand.Id))
                    {
                        command.IsEntry = false;
                    }
                }

                await _db.SaveChangesAsync();

                await AddSuccess(new OperatorLog
                {
                    Type = OperatorLogType.修改脚本分支,
                    Content = $"Id = {id},Data = {content}"
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await AddError(new OperatorLog
                {
                    Type = OperatorLogType.修改脚本分支,
                    Content = $"Id = {id},Data={JsonConvert.SerializeObject(ScriptCommand)},ErrorMessage={ErrorMessage}"
                });
                return Page();
            }

            return Redirect(UrlReferer);
        }
    }
}
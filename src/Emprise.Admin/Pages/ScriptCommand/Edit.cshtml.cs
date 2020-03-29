using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Data;
using Emprise.Admin.Models.Npc;
using Emprise.Admin.Models.NpcScript;
using Emprise.Domain.Core.Enum;
using Emprise.Domain.Npc.Entity;
using Emprise.Domain.Npc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Emprise.Admin.Pages.ScriptCommand
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
        public ScriptCommandInput ScriptCommand { get; set; }

        public string Tips { get; set; }
        public string SueccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public Array Conditions { get; set; }

        public Array Fields { get; set; }

        public Array Relations { get; set; }

        public Array Events { get; set; }

        public Array Commands { get; set; }

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
            SueccessMessage = "";
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                ErrorMessage = ModelState.Where(e => e.Value.Errors.Count > 0).Select(e => e.Value.Errors.First().ErrorMessage).First();

                Conditions = Enum.GetNames(typeof(ConditionTypeEnum));

                Fields = Enum.GetNames(typeof(PlayerConditionFieldEnum));

                Relations = Enum.GetNames(typeof(LogicalRelationTypeEnum));

                Events = Enum.GetNames(typeof(PlayerEventTypeEnum));

                Commands = Enum.GetNames(typeof(CommandTypeEnum));
                return Page();
            }


            var scriptCommand = await _db.ScriptCommands.FindAsync(id);
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



            SueccessMessage = $"修改成功！";

            //return RedirectToPage("/ScriptCommand/Index", new { sId = scriptCommand.ScriptId });

            return Redirect(UrlReferer);
        }
    }
}
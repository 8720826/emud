using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.Npc.Dtos;
using Emprise.Application.Npc.Services;
using Emprise.Application.User.Services;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Npc.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Web.Areas.Admin.Pages.Npc
{
    public class EditModel : BaseAdminPageModel
    {
        private readonly INpcAppService _npcAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public EditModel(
            ILogger<EditModel> logger,
            INpcAppService npcAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _npcAppService = npcAppService;
            _appConfig = appConfig.CurrentValue;

        }

        [BindProperty]
        public NpcInput Npc { get; set; }

        public string ErrorMessage { get; set; }

        [BindProperty]
        public List<int> ScriptIds { get; set; } = new List<int>();

        public List<ScriptEntity> Scripts { get; set; } = new List<ScriptEntity>();



        public async Task<IActionResult> OnGetAsync(int id)
        {


            if (id > 0)
            {
                var npc = await _npcAppService.Get(id);
                Npc = _mapper.Map<NpcInput>(npc);

                Scripts = await _npcAppService.GetScripts(id);

                return Page();
            }
            else
            {
                return RedirectToPage("/Npc/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _npcAppService.Update(id, Npc, ScriptIds);
            if (!result.IsSuccess)
            {
                ErrorMessage = result.Message;
                return Page();
            }
            else
            {
                return RedirectToPage("/Npc/Index");
            }
            /*
            try
            {
                var npc = await _npcAppService.Get(id);
                if (npc == null)
                {
                    ErrorMessage = $"Npc {id} 不存在！";
                    return Page();
                }
                var content = DifferenceComparison(npc, Npc);
                _mapper.Map(Npc, npc);

                var npcScripts = _db.NpcScripts.Where(x => x.NpcId == id);
                foreach (var npcScript in npcScripts)
                {
                    if (!ScriptIds.Contains(npcScript.ScriptId))
                    {
                        _db.NpcScripts.Remove(npcScript);
                    }
                    else
                    {
                        ScriptIds.Remove(npcScript.ScriptId);
                    }
                }


                foreach (var scriptId in ScriptIds)
                {
                    _db.NpcScripts.Add(new NpcScriptEntity { NpcId = id, ScriptId = scriptId });
                }
                await _db.SaveChangesAsync();

                var result = await _mudClient.EditNpc(id);

                await AddSuccess(new OperatorLog
                {
                    Type = OperatorLogType.修改Npc,
                    Content = $"Id = {id},Data = {content}"
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await AddError(new OperatorLog
                {
                    Type = OperatorLogType.修改Npc,
                    Content = $"Id = {id},Data={JsonConvert.SerializeObject(Npc)},ErrorMessage={ErrorMessage}"
                });
                return Page();
            }

            return Redirect(UrlReferer);
            */
        }
    }
}
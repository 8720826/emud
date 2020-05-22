using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Api;
using Emprise.Admin.Data;
using Emprise.Admin.Entity;
using Emprise.Admin.Models.Npc;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Admin.Pages.Npc
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
        public NpcInput Npc { get; set; }

        public string ErrorMessage { get; set; }

        [BindProperty]
        public List<int> ScriptIds { get; set; } = new List<int>();

        public List<ScriptEntity> Scripts { get; set; } = new List<ScriptEntity>();

        [BindProperty]
        public string UrlReferer { get; set; }



        public async Task<IActionResult> OnGetAsync(int id)
        {
            UrlReferer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/Npc/Index");
            }

            if (id > 0)
            {
                var npc = await _db.Npcs.FindAsync(id);

                Npc = _mapper.Map<NpcInput>(npc);

                var npcScripts = _db.NpcScripts.Where(x => x.NpcId == id);

                var ids = npcScripts.Select(x => x.ScriptId).ToList();


                Scripts = _db.Scripts.Where(x => ids.Contains(x.Id)).ToList();

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

            try
            {
                var npc = await _db.Npcs.FindAsync(id);
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
        }
    }
}
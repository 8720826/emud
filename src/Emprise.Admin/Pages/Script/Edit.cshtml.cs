using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Api;
using Emprise.Admin.Data;
using Emprise.Admin.Entity;
using Emprise.Admin.Models.Script;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Admin.Pages.NpcScript
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
        public ScriptInput Script { get; set; }

        public List<NpcEntity> Npcs { get; set; } = new List<NpcEntity>();


        [BindProperty]
        public List<int> NpcIds { get; set; }

        public string ErrorMessage { get; set; }

        [BindProperty]
        public string UrlReferer { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            UrlReferer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/Script/Index");
            }

            if (id > 0)
            {
                var script = await _db.Scripts.FindAsync(id);

                Script = _mapper.Map<ScriptInput>(script);


                var npcScripts = _db.NpcScripts.Where(x => x.ScriptId == id);

                var ids = npcScripts.Select(x => x.NpcId).ToList();


                Npcs = _db.Npcs.Where(x => ids.Contains(x.Id)).ToList();

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
                return Page();
            }


            try
            {
                var script = await _db.Scripts.FindAsync(id);
                var content = DifferenceComparison(script, Script);

                _mapper.Map(Script, script);

                var npcScripts = _db.NpcScripts.Where(x => x.ScriptId == id);
                foreach (var npcScript in npcScripts)
                {
                    if (!NpcIds.Contains(npcScript.NpcId))
                    {
                        _db.NpcScripts.Remove(npcScript);
                    }
                    else
                    {
                        NpcIds.Remove(npcScript.NpcId);
                    }
                }


                foreach (var npcId in NpcIds)
                {
                    _db.NpcScripts.Add(new NpcScriptEntity { NpcId = npcId, ScriptId = id });
                }
                await _db.SaveChangesAsync();

                await AddSuccess(new OperatorLog
                {
                    Type = OperatorLogType.修改脚本,
                    Content = $"Id = {id},Data = {content}"
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await AddError(new OperatorLog
                {
                    Type = OperatorLogType.修改脚本,
                    Content = $"Id = {id},Data={JsonConvert.SerializeObject(Script)},ErrorMessage={ErrorMessage}"
                });
                return Page();
            }

            return Redirect(UrlReferer);
        }



    }
}
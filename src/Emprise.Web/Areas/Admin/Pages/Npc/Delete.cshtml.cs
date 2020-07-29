using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
    public class DeleteModel : BaseAdminPageModel
    {
        private readonly INpcAppService _npcAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public DeleteModel(
            ILogger<DeleteModel> logger,
            INpcAppService npcAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _npcAppService = npcAppService;
            _appConfig = appConfig.CurrentValue;

        }

        public NpcEntity Npc { get; set; }

        public string ErrorMessage { get; set; }



        public async Task<IActionResult> OnGetAsync(int id)
        {

            if (id > 0)
            {
                Npc = await _npcAppService.Get(id);
                if (Npc == null)
                {
                    ErrorMessage = $"Npc {id} 不存在！";
                    return Page();
                }
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

            var result = await _npcAppService.Delete(id);
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
                var npc = await _db.Npcs.FindAsync(id);
                if (npc == null)
                {
                    ErrorMessage = $"Npc {id} 不存在！";
                    return Page();
                }
                _db.Npcs.Remove(npc);
                await _db.SaveChangesAsync();
                await AddSuccess(new OperatorLog
                {
                    Type = OperatorLogType.删除Npc,
                    Content = JsonConvert.SerializeObject(npc)
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await AddError(new OperatorLog
                {
                    Type = OperatorLogType.删除Npc,
                    Content = $"id={id}，ErrorMessage={ErrorMessage}"
                });
                return Page();
            }

            return Redirect(UrlReferer);
            */
        }
    }
}
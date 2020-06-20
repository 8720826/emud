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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Web.Areas.Admin.Pages.Npc
{
    public class CopyModel : BaseAdminPageModel
    {
        private readonly INpcAppService _npcAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public CopyModel(
            ILogger<CopyModel> logger,
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



        public async Task<IActionResult> OnGetAsync(int id)
        {

            if (id > 0)
            {
                var npc = await _npcAppService.Get(id);
                if (npc == null)
                {
                    ErrorMessage = $"Npc {id} 不存在！";
                    return Page();
                }
                Npc = _mapper.Map<NpcInput>(npc);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }


            var result = await _npcAppService.Add(Npc);
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
                var npc = _mapper.Map<NpcEntity>(Npc);
                await _db.Npcs.AddAsync(npc);
                await _db.SaveChangesAsync();

                await AddSuccess(new OperatorLog
                {
                    Type = OperatorLogType.复制Npc,
                    Content = JsonConvert.SerializeObject(Npc)
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await AddError(new OperatorLog
                {
                    Type = OperatorLogType.复制Npc,
                    Content = $"Data={JsonConvert.SerializeObject(Npc)},ErrorMessage={ErrorMessage}"
                });
                return Page();
            }



            return Redirect(UrlReferer);
            */
        }
    }
}
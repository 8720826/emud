using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.Npc.Services;
using Emprise.Application.User.Services;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Npc.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emprise.Web.Areas.Admin.Pages.Npc
{
    public class ListModel : BaseAdminPageModel
    {

        private readonly INpcAppService _npcAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public ListModel(
            ILogger<ListModel> logger,
            INpcAppService npcAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _npcAppService = npcAppService;
            _appConfig = appConfig.CurrentValue;

        }


        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }


        public Paging<NpcEntity> Paging { get; set; }

        public Dictionary<int, List<ScriptEntity>> Scripts { get; set; }

        public async Task OnGetAsync(int pageIndex)
        {
            Paging = await _npcAppService.GetPaging(Keyword, pageIndex);

            var ids = Paging.Data.Select(x => x.Id).ToList();

            Scripts = await _npcAppService.GetScripts(ids);
        }

    }
}
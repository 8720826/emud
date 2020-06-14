using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.Quest.Services;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Quest.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emprise.Admin.Pages.Quest
{
    public class IndexModel : BasePageModel
    {
        private readonly IQuestAppService _questAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public IndexModel(
            ILogger<IndexModel> logger,
            IQuestAppService questAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _questAppService = questAppService;
            _appConfig = appConfig.CurrentValue;
        }


        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }

        public Paging<QuestEntity> Paging { get; set; }

        public async Task OnGetAsync(int pageIndex)
        {
            Paging = await _questAppService.GetPaging(Keyword, pageIndex);
        }
    }
}
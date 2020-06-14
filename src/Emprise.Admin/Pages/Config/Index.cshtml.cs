using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.Config.Services;
using Emprise.Domain.Config.Models;
using Emprise.Domain.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emprise.Admin.Pages.Config
{
    public class IndexModel : BasePageModel
    {
        private readonly IConfigAppService _configAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public IndexModel(
            ILogger<IndexModel> logger,
            IConfigAppService configAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _configAppService = configAppService;
            _appConfig = appConfig.CurrentValue;

        }

        public string ErrorMessage { get; set; }

        public List<ConfigModel> Configs { get; set; }


        public async Task OnGetAsync()
        {
            Configs = await _configAppService.GetConfigs();
        }

    }
}
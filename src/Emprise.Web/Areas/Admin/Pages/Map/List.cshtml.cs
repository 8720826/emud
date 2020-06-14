using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.Map.Services;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Map.Entity;
using Emprise.Web.Areas.Admin.Pages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emprise.Web.Areas.Admin.Pages.Map
{
    public class ListModel : BaseAdminPageModel
    {

        private readonly IMapAppService _mapAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public ListModel(
            ILogger<ListModel> logger,
            IMapAppService mapAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _mapAppService = mapAppService;
            _appConfig = appConfig.CurrentValue;

        }


        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }

        public Paging<MapEntity> Paging { get; set; }

        public async Task OnGetAsync(int pageIndex)
        {
            Paging = await _mapAppService.GetPaging(Keyword, pageIndex);
        }
    }
}
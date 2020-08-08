using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.Map.Services;
using Emprise.Application.User.Services;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Map.Entity;
using Emprise.Domain.Room.Entity;
using Emprise.Web.Areas.Admin.Pages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emprise.Admin.Pages.Room
{
    public class IndexModel : BaseAdminPageModel
    {
        private readonly IMapAppService _mapAppService;
        private readonly IRoomAppService _roomAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public IndexModel(
            ILogger<IndexModel> logger,
            IMapAppService mapAppService,
            IRoomAppService roomAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _mapAppService = mapAppService;
            _roomAppService = roomAppService;
            _appConfig = appConfig.CurrentValue;

        }


        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }

        public MapEntity Map { get; set; }


        public Paging<RoomEntity> Paging { get; set; }


        public async Task<IActionResult> OnGetAsync(int mapId, int pageIndex=1)
        {
            Map = await _mapAppService.Get(mapId);
            if (Map == null)
            {
                return RedirectToPage("/Map/Index");
            }

            Paging = await _roomAppService.GetPaging(mapId, Keyword, pageIndex);

            return Page();
        }


    }
}
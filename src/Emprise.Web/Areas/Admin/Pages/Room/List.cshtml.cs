using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.Map.Services;
using Emprise.Application.User.Services;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Room.Entity;
using Emprise.Web.Areas.Admin.Pages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emprise.Admin.Pages.Room
{
    public class ListModel : BaseAdminPageModel
    {
        private readonly IMapAppService _mapAppService;
        private readonly IRoomAppService _roomAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public ListModel(
            ILogger<ListModel> logger,
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


        public Paging<RoomEntity> Paging { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }

        public int MapId { get; set; }

        public int RoomId { get; set; }

        public async Task OnGetAsync(int pageIndex,int mapId, int roomId)
        {
            MapId = mapId;
            RoomId = roomId;


            Paging = await _roomAppService.GetPaging(mapId, Keyword, pageIndex);
        }
    }
}
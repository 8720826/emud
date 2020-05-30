using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Api;
using Emprise.Admin.Data;
using Emprise.Admin.Entity;
using Emprise.Admin.Extensions;
using Emprise.Admin.Models;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emprise.Admin.Pages.Room
{
    public class ListModel : BasePageModel
    {
        public ListModel(IMudClient mudClient,
            IMapper mapper,
            ILogger<ListModel> logger,
            EmpriseDbContext db,
            IOptionsMonitor<AppConfig> appConfig,
            IHttpContextAccessor httpAccessor)
            : base(db, appConfig, httpAccessor, mapper, logger, mudClient)
        {

        }


        public Paging<RoomEntity> Paging { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }

        public int MapId { get; set; }

        public int RoomId { get; set; }

        public void OnGet(int pageIndex,int mapId, int roomId)
        {
            MapId = mapId;
            RoomId = roomId;
            var query = _db.Rooms.AsQueryable();
            if (mapId > 0)
            {
                query = query.Where(x => x.MapId == mapId);
            }
            if (roomId > 0)
            {
                query = query.Where(x => x.Id == roomId);
            }

            if (!string.IsNullOrEmpty(Keyword))
            {
                query = query.Where(x => x.Name.Contains(Keyword));
            }

            query = query.OrderBy(x => x.Id);

            Paging = query.Paged(pageIndex, 10, query.Count());
        }
    }
}
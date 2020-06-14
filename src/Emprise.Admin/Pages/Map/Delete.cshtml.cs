using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.Map.Services;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Map.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Admin.Pages.Map
{
    public class DeleteModel : BasePageModel
    {
        private readonly IMapAppService _mapAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public DeleteModel(
            ILogger<DeleteModel> logger,
            IMapAppService mapAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _mapAppService = mapAppService;
            _appConfig = appConfig.CurrentValue;

        }

        public MapEntity Map { get; set; }

        public string ErrorMessage { get; set; }


        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id > 0)
            {
                Map = await _mapAppService.Get(id);
                return Page();
            }
            else
            {
                return RedirectToPage("/Map/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _mapAppService.Delete(id);
            if (!result.IsSuccess)
            {
                ErrorMessage = result.Message;
                return Page();
            }
            else
            {
                return RedirectToPage("/Map/Index");
            }
            /*
            try
            {
                var roomCount = await _db.Rooms.CountAsync(x => x.MapId == id);
                if (roomCount > 0)
                {
                    ErrorMessage = $"该地图下还有{roomCount}个房间，删除失败";

                    await AddError(new OperatorLog
                    {
                        Type = OperatorLogType.删除地图,
                        Content = $"id={id}，ErrorMessage={ErrorMessage}"
                    });
                    return Page();
                }


                var map = await _db.Maps.FindAsync(id); 
                if (map == null)
                {
                    ErrorMessage = $"地图 {id} 不存在！";
                    return Page();
                }
                _db.Maps.Remove(map);
                await _db.SaveChangesAsync();

                await AddSuccess(new OperatorLog
                {
                    Type = OperatorLogType.删除地图,
                    Content = JsonConvert.SerializeObject(map)
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;

                await AddError(new OperatorLog
                {
                    Type = OperatorLogType.删除地图,
                    Content = $"id={id}，ErrorMessage={ErrorMessage}"
                });
                return Page();
            }

            return Redirect(UrlReferer);
            */
        }
    }
}
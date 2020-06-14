using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.Map.Dtos;
using Emprise.Application.Map.Services;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Admin.Pages.Map
{
    public class EditModel : BasePageModel
    {
        private readonly IMapAppService _mapAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public EditModel(
            ILogger<EditModel> logger,
            IMapAppService mapAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _mapAppService = mapAppService;
            _appConfig = appConfig.CurrentValue;

        }

        [BindProperty]
        public MapInput Map { get; set; }

        public string ErrorMessage { get; set; }





        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id > 0)
            {
                var map = await _mapAppService.Get(id);

                Map = _mapper.Map<MapInput>(map);

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

            var result = await _mapAppService.Update(id, Map);
            if (!result.IsSuccess)
            {
                ErrorMessage = result.Message;
                return Page();
            }
            else
            {
                return RedirectToPage("/Ware/Index");
            }

            /*
            try
            {
                var map = await _db.Maps.FindAsync(id);
                if (map == null)
                {
                    ErrorMessage = $"地图 {id} 不存在！";
                    return Page();
                }
                var content = DifferenceComparison(map, Map);
                _mapper.Map(Map, map);
                await _db.SaveChangesAsync();



                await AddSuccess(new OperatorLog
                {
                    Type = OperatorLogType.修改地图,
                    Content = $"Id = {id},Data = {content}"
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await AddError(new OperatorLog
                {
                    Type = OperatorLogType.修改地图,
                    Content = $"Id = {id},Data={JsonConvert.SerializeObject(Map)},ErrorMessage={ErrorMessage}"
                });
                return Page();
            }

            return Redirect(UrlReferer);
            */
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Data;
using Emprise.Admin.Entity;
using Emprise.Admin.Models.Map;
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
    public class AddModel : BasePageModel
    {

        public AddModel(IMapper mapper, ILogger<AddModel> logger, EmpriseDbContext db, IOptionsMonitor<AppConfig> appConfig, IHttpContextAccessor httpAccessor) 
            : base(db, appConfig, httpAccessor, mapper, logger)
        {

        }

        [BindProperty]
        public MapInput Map { get; set; }

        public string ErrorMessage { get; set; }

        [BindProperty]
        public string UrlReferer { get; set; }

        public void OnGet()
        {
            UrlReferer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/Map/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var map = _mapper.Map<MapEntity>(Map);

                await _db.Maps.AddAsync(map);

                await _db.SaveChangesAsync();

                await AddSuccess(new OperatorLog
                {
                    Type = OperatorLogType.添加地图,
                    Content = JsonConvert.SerializeObject(Map)
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;

                await AddError(new OperatorLog
                {
                    Type = OperatorLogType.添加地图,
                    Content = JsonConvert.SerializeObject(Map)
                });
                return Page();
            }



            return Redirect(UrlReferer);
        }
    }
}
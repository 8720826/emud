using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Data;
using Emprise.Admin.Models.Map;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emprise.Admin.Pages.Map
{
    public class EditModel : BasePageModel
    {
        public EditModel(IMapper mapper, ILogger<EditModel> logger, EmpriseDbContext db, IOptionsMonitor<AppConfig> appConfig, IHttpContextAccessor httpAccessor) 
            : base(db, appConfig, httpAccessor, mapper, logger)
        {

        }

        [BindProperty]
        public MapInput Map { get; set; }

        public string ErrorMessage { get; set; }


        [BindProperty]
        public string UrlReferer { get; set; }



        public async Task<IActionResult> OnGetAsync(int id)
        {
            UrlReferer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/Map/Index");
            }

            if (id > 0)
            {
                var map = await _db.Maps.FindAsync(id);

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

            try
            {
                var map = await _db.Maps.FindAsync(id);
                var content = DifferenceComparison(map, Map);
                _mapper.Map(Map, map);
                await _db.SaveChangesAsync();



                await AddSuccess(new OperatorLog
                {
                    Type = OperatorLogType.修改地图,
                    Content = content
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await AddError(new OperatorLog
                {
                    Type = OperatorLogType.修改地图,
                    Content = $"id={id}，ErrorMessage={ErrorMessage}"
                });
                return Page();
            }

            return Redirect(UrlReferer);
        }
    }
}
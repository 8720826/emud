using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Data;
using Emprise.Admin.Entity;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Models;
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
        public DeleteModel(IMapper mapper, ILogger<DeleteModel> logger, EmpriseDbContext db, IOptionsMonitor<AppConfig> appConfig, IHttpContextAccessor httpAccessor) 
            : base(db, appConfig, httpAccessor, mapper, logger)
        {
 
        }

        public MapEntity Map { get; set; }

        public string ErrorMessage { get; set; }

        [BindProperty]
        public string UrlReferer { get; set; }


        public async Task<IActionResult> OnGetAsync(int id)
        {
            UrlReferer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/Npc/Index");
            }

            if (id > 0)
            {
                Map = await _db.Maps.FindAsync(id);
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
        }
    }
}
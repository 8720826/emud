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
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emprise.Admin.Pages.User
{
    public class IndexModel : BasePageModel
    {
        public IndexModel(IMudClient mudClient,
            IMapper mapper,
            ILogger<IndexModel> logger,
            EmpriseDbContext db,
            IOptionsMonitor<AppConfig> appConfig,
            IHttpContextAccessor httpAccessor)
            : base(db, appConfig, httpAccessor, mapper, logger, mudClient)
        {

        }


        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }

        public Paging<UserEntity> Paging { get; set; }

        public void OnGet(int pageIndex)
        {
            var query = _db.Users.OrderBy(x => x.Id);
            if (!string.IsNullOrEmpty(Keyword))
            {
                query = _db.Users.Where(x => x.Email.Contains(Keyword)).OrderBy(x => x.Id);
            }

            Paging = query.Paged(pageIndex, 10, query.Count());


        }


        public async Task<IActionResult> OnPostAsync([FromBody]EnableData enableData)
        {
            try
            {
                var user = await _db.Users.FindAsync(enableData.SId);
                if (user == null)
                {
                    return await Task.FromResult(new JsonResult(enableData));
                }

                user.Status = enableData.IsEnable ? UserStatusEnum.正常 : UserStatusEnum.封禁;
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "");
            }


            return await Task.FromResult(new JsonResult(enableData));

        }

        public class EnableData
        {
            public int SId { get; set; }
            public bool IsEnable { get; set; }
        }
    }
}
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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emprise.Admin.Pages.Log
{
    public class OperatorLogModel : BasePageModel
    {

        public OperatorLogModel(IMudClient mudClient,
            IMapper mapper,
            ILogger<OperatorLogModel> logger,
            EmpriseDbContext db,
            IOptionsMonitor<AppConfig> appConfig,
            IHttpContextAccessor httpAccessor)
            : base(db, appConfig, httpAccessor, mapper, logger, mudClient)
        {

        }


        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }

        public Paging<OperatorLogEntity> Paging { get; set; }

        public void OnGet(int pageIndex)
        {
            var query = _db.OperatorLogs.AsQueryable();
            if (!string.IsNullOrEmpty(Keyword))
            {
                query = query.Where(x => x.Content.Contains(Keyword));
            }

            query = query.OrderByDescending(x => x.Id);

            Paging = query.Paged(pageIndex, 10, query.Count());
        }

        public async Task<IActionResult> OnPostClearLogAsync(int type)
        {
            switch (type)
            {
                case 1:
                    var logs1 = await _db.OperatorLogs.Where(x => x.CreateDate < DateTime.Now.AddYears(-1)).ToArrayAsync();
                    _db.OperatorLogs.RemoveRange(logs1);
                    await _db.SaveChangesAsync();

                    await AddSuccess(new OperatorLog
                    {
                        Type = OperatorLogType.清除1年前记录,
                        Content = $"清除{logs1.Length}条记录"
                    });
                    break;

                case 2:
                    var logs2 = await _db.OperatorLogs.Where(x => x.CreateDate < DateTime.Now.AddMonths(-6)).ToArrayAsync();
                    _db.OperatorLogs.RemoveRange(logs2);
                    await _db.SaveChangesAsync();

                    await AddSuccess(new OperatorLog
                    {
                        Type = OperatorLogType.清除半年前记录,
                        Content = $"清除{logs2.Length}条记录"
                    });
                    break;

                case 3:
                    var logs3 = await _db.OperatorLogs.Where(x => x.CreateDate < DateTime.Now.AddMonths(-3)).ToArrayAsync();
                    _db.OperatorLogs.RemoveRange(logs3);
                    await _db.SaveChangesAsync();

                    await AddSuccess(new OperatorLog
                    {
                        Type = OperatorLogType.清除3个月前记录,
                        Content = $"清除{logs3.Length}条记录"
                    });
                    break;

                case 4:
                    var logs4 = await _db.OperatorLogs.Where(x => x.CreateDate < DateTime.Now.AddMonths(-1)).ToArrayAsync();
                    _db.OperatorLogs.RemoveRange(logs4);
                    await _db.SaveChangesAsync();

                    await AddSuccess(new OperatorLog
                    {
                        Type = OperatorLogType.清除1个月前记录,
                        Content = $"清除{logs4.Length}条记录"
                    });
                    break;
            }




            return RedirectToPage("OperatorLog");
        }
    }
}
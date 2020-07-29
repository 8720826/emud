using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.Log.Services;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Log.Entity;
using Emprise.Web.Areas.Admin.Pages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emprise.Web.Areas.Admin.Pages.Log
{
    public class IndexModel : BaseAdminPageModel
    {
        private readonly IOperatorLogAppService _logAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public IndexModel(
            ILogger<IndexModel> logger,
            IOperatorLogAppService logAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _logAppService = logAppService;
            _appConfig = appConfig.CurrentValue;

        }


        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }

        public Paging<OperatorLogEntity> Paging { get; set; }

        public async Task OnGetAsync(int pageIndex)
        {
            Paging = await _logAppService.GetPaging(Keyword, pageIndex);
        }

        public async Task<IActionResult> OnPostClearLogAsync(int type)
        {
            switch (type)
            {
                case 1:

                    await _logAppService.ClearLog(DateTime.Now.AddYears(-1));
                    break;

                case 2:
                    await _logAppService.ClearLog(DateTime.Now.AddMonths(-6));
                    break;

                case 3:
                    await _logAppService.ClearLog(DateTime.Now.AddMonths(-3));
                    break;

                case 4:
                    await _logAppService.ClearLog(DateTime.Now.AddMonths(-1));
                    break;
            }




            return RedirectToPage("OperatorLog");
        }
    }
}
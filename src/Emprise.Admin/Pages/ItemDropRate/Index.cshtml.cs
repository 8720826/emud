using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Api;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emprise.Admin.Pages.ItemDropRate
{
    public class IndexModel : BasePageModel
    {
        public IndexModel(IMudClient mudClient,
            IMapper mapper,
            ILogger<IndexModel> logger,
            IOperatorLogAppService operatorLogAppService,
            IOptionsMonitor<AppConfig> appConfig,
            IHttpContextAccessor httpAccessor)
            : base(appConfig, httpAccessor, mapper, logger, operatorLogAppService, mudClient)
        {

        }


        public int SId { get; set; }

        public List<ItemDropRateEntity> ItemDropRates { get; set; }

        [BindProperty]
        public string UrlReferer { get; set; }

        public void OnGet(int sId, string @ref = "")
        {
            SId = sId;
            UrlReferer = @ref;
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Request.Headers["Referer"].ToString();
            }

            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/Script/Index");
            }


            ItemDropRates = _db.ItemDropRates.Where(x => x.ItemDropId == sId).ToList();
        }
    }
}
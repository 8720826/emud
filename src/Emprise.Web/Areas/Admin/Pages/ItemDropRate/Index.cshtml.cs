using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Domain.Core.Models;
using Emprise.Domain.ItemDrop.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emprise.Web.Areas.Admin.Pages.ItemDropRate
{
    public class IndexModel : BaseAdminPageModel
    {
       // private readonly IItemDropAppService _itemDropAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public IndexModel(
            ILogger<IndexModel> logger,
            //IItemDropAppService itemDropAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
           // _itemDropAppService = itemDropAppService;
            _appConfig = appConfig.CurrentValue;

        }


        public int SId { get; set; }

        public List<ItemDropRateEntity> ItemDropRates { get; set; }


        public void OnGet(int sId)
        {
            SId = sId;

            //ItemDropRates = _db.ItemDropRates.Where(x => x.ItemDropId == sId).ToList();
        }
    }
}
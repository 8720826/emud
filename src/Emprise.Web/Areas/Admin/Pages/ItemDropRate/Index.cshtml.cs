using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.ItemDrop.Services;
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
        private readonly IItemDropRateAppService _itemDropRateAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public IndexModel(
            ILogger<IndexModel> logger,
            IItemDropRateAppService itemDropRateAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _itemDropRateAppService = itemDropRateAppService;
            _appConfig = appConfig.CurrentValue;

        }


        public int Id { get; set; }

        public List<ItemDropRateEntity> ItemDropRates { get; set; }


        public async Task OnGetAsync(int id)
        {
            Id = id;

            ItemDropRates = await _itemDropRateAppService.GetAll(id);
        }
    }
}
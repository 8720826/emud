using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.ItemDrop.Dtos;
using Emprise.Application.ItemDrop.Services;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emprise.Web.Areas.Admin.Pages.ItemDropRate
{
    public class AddModel : BaseAdminPageModel
    {
        private readonly IItemDropAppService _itemDropAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public AddModel(
            ILogger<AddModel> logger,
            IItemDropAppService itemDropAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _itemDropAppService = itemDropAppService;
            _appConfig = appConfig.CurrentValue;

        }

        [BindProperty]
        public ItemDropRateInput ItemDropRate { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }
    }
}

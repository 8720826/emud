using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.ItemDrop.Services;
using Emprise.Domain.Core.Models;
using Emprise.Domain.ItemDrop.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emprise.Web.Areas.Admin.Pages.ItemDropRate
{
    public class DeleteModel : BaseAdminPageModel
    {
        private readonly IItemDropAppService _itemDropAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public DeleteModel(
            ILogger<DeleteModel> logger,
            IItemDropAppService itemDropAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _itemDropAppService = itemDropAppService;
            _appConfig = appConfig.CurrentValue;

        }

        public ItemDropRateEntity ItemDropRate { get; set; }

        public string ErrorMessage { get; set; }


        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id > 0)
            {
                ItemDropRate = await _itemDropAppService.GetRate(id);
                return Page();
            }
            else
            {
                return RedirectToPage("/ItemDrop/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _itemDropAppService.DeleteRate(id);
            if (!result.IsSuccess)
            {
                ErrorMessage = result.Message;
                return Page();
            }
            else
            {
                return RedirectToPage("/ItemDrop/Index");
            }
        }
    }
}

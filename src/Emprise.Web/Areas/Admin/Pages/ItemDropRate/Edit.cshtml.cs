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
    public class EditModel : BaseAdminPageModel
    {
        private readonly IItemDropRateAppService _itemDropRateAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public EditModel(
            ILogger<AddModel> logger,
            IItemDropRateAppService itemDropRateAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _itemDropRateAppService = itemDropRateAppService;
            _appConfig = appConfig.CurrentValue;

        }

        [BindProperty]
        public ItemDropRateInput ItemDropRate { get; set; }

        public string ErrorMessage { get; set; }

        public int Id { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Id = id;
            if (id > 0)
            {
                var rate = await _itemDropRateAppService.Get(id);

                ItemDropRate = _mapper.Map<ItemDropRateInput>(rate);

                return Page();
            }
            else
            {
                return RedirectToPage("/ItemDrop/Index");
            }
        }



        public async Task<IActionResult> OnPostAsync(int id)
        {
            Id = id;
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _itemDropRateAppService.Update(id, ItemDropRate);
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

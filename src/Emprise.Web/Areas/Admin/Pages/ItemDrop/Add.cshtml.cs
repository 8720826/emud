using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.ItemDrop.Dtos;
using Emprise.Application.ItemDrop.Services;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Web.Areas.Admin.Pages.ItemDrop
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
        public ItemDropInput ItemDrop { get; set; }

        public string ErrorMessage { get; set; }



        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _itemDropAppService.Add(ItemDrop);
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
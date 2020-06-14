using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.Player.Services;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Player.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Admin.Pages.Player
{
    public class DeleteModel : BasePageModel
    {

        private readonly IPlayerAppService _playerAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public DeleteModel(
            ILogger<IndexModel> logger,
            IPlayerAppService playerAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _playerAppService = playerAppService;
            _appConfig = appConfig.CurrentValue;

        }

        public PlayerEntity Player { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {

            if (id > 0)
            {
                Player = await _playerAppService.Get(id);
                if (Player == null)
                {
                    ErrorMessage = $"玩家 {id} 不存在！";
                    return Page();
                }
                return Page();
            }
            else
            {
                return RedirectToPage("/Player/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _playerAppService.Delete(id);
            if (!result.IsSuccess)
            {
                ErrorMessage = result.Message;
                return Page();
            }
            else
            {
                return RedirectToPage("/Player/Index");
            }

        }
    }
}
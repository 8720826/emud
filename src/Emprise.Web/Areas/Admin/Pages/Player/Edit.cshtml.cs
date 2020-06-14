using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Application.Player.Dtos;
using Emprise.Application.Player.Services;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Emprise.Web.Areas.Admin.Pages.Player
{
    public class EditModel : BaseAdminPageModel
    {
        private readonly IPlayerAppService _playerAppService;
        private readonly AppConfig _appConfig;
        private readonly IMapper _mapper;
        public EditModel(
            ILogger<EditModel> logger,
            IPlayerAppService playerAppService,
            IMapper mapper,
            IOptionsMonitor<AppConfig> appConfig)
            : base(logger)
        {
            _mapper = mapper;
            _playerAppService = playerAppService;
            _appConfig = appConfig.CurrentValue;

        }

        [BindProperty]
        public PlayerInput Player { get; set; }

        public string ErrorMessage { get; set; }



        public async Task<IActionResult> OnGetAsync(int id)
        {

            if (id > 0)
            {
                var player = await _playerAppService.Get(id);
                if (player == null)
                {
                    ErrorMessage = $"玩家 {id} 不存在！";
                    return Page();
                }
                Player = _mapper.Map<PlayerInput>(player);

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


            var result = await _playerAppService.Update(id, Player);
            if (!result.IsSuccess)
            {
                ErrorMessage = result.Message;
                return Page();
            }
            else
            {
                return RedirectToPage("/Ware/Index");
            }

            /*
            try
            {
                var player = await _db.Players.FindAsync(id);
                if (player == null)
                {
                    ErrorMessage = $"玩家 {id} 不存在！";
                    return Page();
                }
                var content = DifferenceComparison(player, Player);
                _mapper.Map(Player, player);

                await _db.SaveChangesAsync();

                await AddSuccess(new OperatorLog
                {
                    Type = OperatorLogType.修改玩家,
                    Content = $"Id = {id},Data = {content}"
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await AddError(new OperatorLog
                {
                    Type = OperatorLogType.修改玩家,
                    Content = $"Id = {id},Data={JsonConvert.SerializeObject(Player)},ErrorMessage={ErrorMessage}"
                });
                return Page();
            }

            return Redirect(UrlReferer);
            */
        }
    }
}
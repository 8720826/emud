using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Application.Player.Dtos;
using Emprise.Application.Player.Services;
using Emprise.Domain.Core.Authorization;
using Emprise.Domain.Core.Notifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Emprise.Web.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : BaseController
    {
        private readonly IPlayerAppService _playerService;
        private readonly IAccountContext _accountContext;
        public PlayerController(IPlayerAppService playerService,
            INotificationHandler<DomainNotification> notifications, IAccountContext accountContext) : base(notifications)
        {
            _playerService = playerService;
            _accountContext = accountContext;
        }
        /*
        
        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> Create(PlayerCreateDto dto)
        {
            var userId = _accountContext.UserId;
            await _playerService.Create(userId,dto);

            return MyResponse();
        }
        */
        /*
        [Route("delete")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _playerService.Delete(id);

            return MyResponse();
        }*/

        

        /*
        [Route("initGame")]
        [HttpPost]
        public async Task<IActionResult> InitGame()
        {
            var userId = _accountContext.UserId;
            var playerId = _accountContext.PlayerId;
            await _playerService.InitGame(userId, playerId);

            return MyResponse();
        }*/

            /*
        [Route("move")]
        [HttpPost]
        public async Task<IActionResult> Move([FromForm]int roomId)
        {
            var playerId = _accountContext.PlayerId;
            await _playerService.Move(playerId, roomId);

            return MyResponse();
        }

        [Route("me")]
        [HttpPost]
        public async Task<IActionResult> ShowMe()
        {
            var playerId = _accountContext.PlayerId;
            var me = await _playerService.GetPlayer(playerId);

            return MyResponse(me);
        }

        [Route("show")]
        [HttpPost]
        public async Task<IActionResult> Show(int id)
        {
            var player = await _playerService.GetPlayer(id);

            return MyResponse(player);
        }*/
    }
}
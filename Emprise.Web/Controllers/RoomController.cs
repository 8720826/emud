using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Application.User.Services;
using Emprise.Domain.Core.Authorization;
using Emprise.Domain.Core.Notifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Emprise.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : BaseController
    {
        private readonly IRoomAppService _roomService;
        private readonly IAccountContext _accountContext;
        public RoomController(IRoomAppService roomService,
            INotificationHandler<DomainNotification> notifications, IAccountContext accountContext) : base(notifications)
        {
            _roomService = roomService;
            _accountContext = accountContext;
        }

        /*
        [Route("current")]
        [HttpPost]
        public async Task<IActionResult> GetCurrent()
        {
            var playerId = _accountContext.PlayerId;
            var roomInfo = await _roomService.GetCurrent(playerId);

            return MyResponse(roomInfo);
        }*/
    }
}
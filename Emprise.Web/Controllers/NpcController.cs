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
    public class NpcController : BaseController
    {
        private readonly INpcAppService _npcService;
        private readonly IAccountContext _accountContext;
        public NpcController(INpcAppService npcService,
            INotificationHandler<DomainNotification> notifications, IAccountContext accountContext) : base(notifications)
        {
            _npcService = npcService;
            _accountContext = accountContext;
        }

        [Route("show")]
        [HttpPost]
        public async Task<IActionResult> Show(int id)
        {
            var playerId = _accountContext.PlayerId;
            var npc = await _npcService.GetNpc(playerId);

            return MyResponse(npc);
        }
    }
}
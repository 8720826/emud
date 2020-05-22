using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Npc.Entity;
using Emprise.Domain.Npc.Services;
using Emprise.Domain.Room.Entity;
using Emprise.Domain.Room.Services;
using Emprise.Infra.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Emprise.Web.Controllers
{
    [ApiAuthentication]
    [Route("api/event/[controller]")]
    [ApiController]
    public class NpcController : ControllerBase
    {
        
        private readonly ILogger<RoomController> _logger;
        private readonly INpcDomainService _npcDomainService;
        private readonly IMediatorHandler _bus;
        public NpcController(IMediatorHandler bus, INpcDomainService npcDomainService, ILogger<RoomController> logger)
        {
            _bus = bus;
            _npcDomainService = npcDomainService;
            _logger = logger;
        }

        [Route("edit")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id)
        {
            var npc = await _npcDomainService.Get(id);
            await _bus.RaiseEvent(new EntityUpdatedEvent<NpcEntity>(npc)).ConfigureAwait(false);
            return Ok();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Events;
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
    public class RoomController : ControllerBase
    {
        private readonly ILogger<RoomController> _logger;
        private readonly IRoomDomainService _roomDomainService;
        private readonly IMediatorHandler _bus;
        public RoomController(IMediatorHandler bus, IRoomDomainService roomDomainService, ILogger<RoomController> logger)
        {
            _bus = bus;
            _roomDomainService = roomDomainService;
            _logger = logger;
        }

        [Route("edit")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id)
        {
            var room = await _roomDomainService.Get(id);
            await _bus.RaiseEvent(new EntityUpdatedEvent<RoomEntity>(room)).ConfigureAwait(false);
            return Ok();
        }
    }
}
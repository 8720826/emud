using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    }
}
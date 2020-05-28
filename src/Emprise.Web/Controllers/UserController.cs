using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Application.User.Services;
using Emprise.Domain.Core.Authorization;
using Emprise.Domain.Core.Notifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Emprise.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {

        private readonly IUserAppService _userService;
        private readonly IAccountContext _accountContext;
        public UserController(IUserAppService userService,
            INotificationHandler<DomainNotification> notifications, IAccountContext accountContext) : base(notifications)
        {
            _userService = userService;
            _accountContext = accountContext;
        }
    }
}
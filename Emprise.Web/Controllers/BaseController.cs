using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Domain.Core.Notifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Emprise.Web.Controllers
{
    public class BaseController : ControllerBase
    {
        private readonly DomainNotificationHandler _notifications;

        public BaseController(INotificationHandler<DomainNotification> notifications)
        {
            _notifications = (DomainNotificationHandler)notifications;
        }

        public bool IsValidOperation()
        {
            return (!_notifications.HasNotifications());
        }

        protected IActionResult MyResponse(object result = null)
        {

            if (IsValidOperation())
            {
                return Ok(new
                {
                    status = true,
                    data = result
                });
            }

            return Ok(new
            {
                status = false,
                errorMessage = string.Join("；", _notifications.GetNotifications().Select(x => x.Content))
            });
        }


    }
}
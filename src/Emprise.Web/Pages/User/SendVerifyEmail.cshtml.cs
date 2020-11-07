using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Application.User.Services;
using Emprise.Domain.Core.Authorization;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Notifications;
using Emprise.Domain.User.Models;
using Emprise.MudServer.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Emprise.Web.Pages.User
{
    public class SendVerifyEmailModel : BasePageModel
    {
        private readonly IAccountContext _accountContext;
        private readonly IMediatorHandler _bus;
        private readonly DomainNotificationHandler _notifications;
        private readonly IUserAppService _userAppService;

        public SendVerifyEmailModel(IAccountContext accountContext,
            IOptionsMonitor<AppConfig> appConfig,
            INotificationHandler<DomainNotification> notifications,
            IUserAppService userAppService,
            IMediatorHandler bus) : base(appConfig)
        {
            _accountContext = accountContext;
            _notifications = (DomainNotificationHandler)notifications;
            _userAppService = userAppService;
            _bus = bus;
        }


        public string Email { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var player = await _userAppService.Get(_accountContext.UserId);

            Email = player.Email;

            if (player.HasVerifiedEmail)
            {
                return RedirectToPage("/User/Index");
            }


            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromBody] SendVerifyEmailDto dto)
        {
            if (!ModelState.IsValid)
            {
                return await Task.FromResult(new JsonResult(new
                {
                    Status = false,
                    ErrorMessage = ModelState.Where(e => e.Value.Errors.Count > 0).Select(e => e.Value.Errors.First().ErrorMessage).First()
                }));
            }

            var userId = _accountContext.UserId;

            var command = new SendVerifyEmailCommand(dto.Email);
            await _bus.SendCommand(command);

            if (_notifications.HasNotifications())
            {
                var errorMessage = string.Join("£»", _notifications.GetNotifications().Select(x => x.Content));
                return await Task.FromResult(new JsonResult(new
                {
                    status = false,
                    errorMessage
                }));
            }

            return await Task.FromResult(new JsonResult(new
            {
                status = true
            }));

        }
    }
}

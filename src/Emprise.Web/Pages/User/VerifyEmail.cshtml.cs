﻿using System;
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
using Emprise.MudServer.Commands.UserCommands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Emprise.Web.Pages.User
{
    public class VerifyEmailModel : BasePageModel
    {
        private readonly IAccountContext _accountContext;
        private readonly IMediatorHandler _bus;
        private readonly DomainNotificationHandler _notifications;
        private readonly IUserAppService _userAppService;

        public VerifyEmailModel(IAccountContext accountContext,
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

        public string Code { get; set; }

        public bool HasSend { get; set; }

        public async Task<IActionResult> OnGetAsync(string code)
        {
            var player = await _userAppService.Get(_accountContext.UserId);

            Code = code;
            Email = player.Email;

            if (player.HasVerifiedEmail)
            {
                return RedirectToPage("/User/Index");
            }

            if (!string.IsNullOrEmpty(code))
            {
                HasSend = true;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromBody]VerifyEmailDto dto)
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

            var command = new VerifyEmailCommand(dto.Email,dto.Code);
            await _bus.SendCommand(command);

            if (_notifications.HasNotifications())
            {
                var errorMessage = string.Join("；", _notifications.GetNotifications().Select(x => x.Content));
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
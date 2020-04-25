using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emprise.Application.User.Dtos;
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

        /*
        [Route("SendRegEmail")]
        [AllowAnonymous, HttpPost]
        public async Task<IActionResult> SendRegEmail(SendRegEmailDto dto)
        {
            await _userService.SendRegEmail(dto.Email);

            return MyResponse();
        }

        [Route("Reg")]
        [AllowAnonymous, HttpPost]
        public async Task<IActionResult> Reg(UserRegDto dto)
        {
            await _userService.Reg(dto);

            return MyResponse();
        }

        [Route("login")]
        [AllowAnonymous, HttpPost]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            await _userService.Login(dto);

            return MyResponse();
        }

        [Route("logout")]
        [AllowAnonymous, HttpPost]
        public async Task<IActionResult> Logout()
        {
            if (_accountContext.UserId > 0)
            {
                await _userService.Logout(_accountContext.UserId);
            }
            return MyResponse();
        }

        [Route("modifypassword")]
        [HttpPost]
        public async Task<IActionResult> ModifyPassword(ModifyPasswordDto dto)
        {
            await _userService.ModifyPassword(_accountContext.UserId, dto);

            return MyResponse();
        }

        [Route("resetpassword")]
        [AllowAnonymous, HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            await _userService.ResetPassword(dto);

            return MyResponse();
        }
        */
        
        /*
        [Route("sendVerifyEmail")]
        [HttpPost]
        public async Task<IActionResult> SendVerifyEmail([FromForm]SendVerifyEmailDto dto)
        {
            await _userService.SendVerifyEmail(_accountContext.UserId, dto);

            return MyResponse();
        }*/
    }
}
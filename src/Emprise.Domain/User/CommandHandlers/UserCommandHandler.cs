using AutoMapper;
using Emprise.Domain.Common.Modes;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.CommandHandlers;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Notifications;
using Emprise.Domain.User.Commands;
using Emprise.Domain.User.Entity;
using Emprise.Domain.User.Events;
using Emprise.Domain.User.Services;
using Emprise.Infra.Authorization;
using Emprise.Infra.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emprise.Domain.User.CommandHandlers
{
    
    public class UserCommandHandler : CommandHandler, 
        IRequestHandler<VisitCommand, Unit>, 
        IRequestHandler<RegCommand, Unit>,
        IRequestHandler<LoginCommand, Unit>,
        IRequestHandler<LogoutCommand, Unit>,
        IRequestHandler<ModifyPasswordCommand, Unit>,
        IRequestHandler<ResetPasswordCommand, Unit>,
        IRequestHandler<SendRegEmailCommand, Unit>,
        IRequestHandler<SendResetEmailCommand, Unit>
        
    {
        private readonly IMediatorHandler _bus;
        private readonly ILogger<UserCommandHandler> _logger;
        private readonly IUserDomainService _userDomainService;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly IMapper _mapper;
        private readonly IMail _mail;
        private readonly AppConfig _appConfig;
        private readonly IRedisDb _redisDb;

        public UserCommandHandler(
            IMediatorHandler bus,
            ILogger<UserCommandHandler> logger,
            IUserDomainService userDomainService,
            IHttpContextAccessor httpAccessor,
            IMapper mapper,
            IMail mail,
            IOptionsMonitor<AppConfig> appConfig,
            IRedisDb redisDb,
            INotificationHandler<DomainNotification> notifications,
            IUnitOfWork uow) : base(uow, bus, notifications)
        {

            _bus = bus;
            _logger = logger;
            _userDomainService = userDomainService;
            _httpAccessor = httpAccessor;
            _mapper = mapper;
            _mail = mail;
            _appConfig = appConfig.CurrentValue;
            _redisDb = redisDb;
        }

        public async Task<Unit> Handle(VisitCommand command, CancellationToken cancellationToken)
        {

            if (await Commit())
            {
                await _bus.RaiseEvent(new VisitedEvent());
            }


            return Unit.Value;
        }

        public async Task<Unit> Handle(RegCommand command, CancellationToken cancellationToken)
        {
            var email = command.Email;
            var password = command.Password;
            var code = command.Code;

            var user = await _userDomainService.Get(p => p.Email == email && p.HasVerifiedEmail);
            if (user != null)
            {
                await _bus.RaiseEvent(new DomainNotification("邮箱已被注册，请更改！"));
                return Unit.Value;
            }

            string key = string.Format(RedisKey.RegEmail, email);// $"regemail_{email}";
            long ttl = await _redisDb.KeyTimeToLive(key);
            if (ttl < 0)
            {
                await _bus.RaiseEvent(new DomainNotification($"注册验证码已超时，请重试"));
                return Unit.Value;
            }

            string emailCode = await _redisDb.StringGet<string>(key);

            if (string.Compare(emailCode, code, true) != 0)
            {
                await _bus.RaiseEvent(new DomainNotification($"注册验证码已失效，请重试"));
                return Unit.Value;
            }



            string ip = _httpAccessor.HttpContext.GetUserIp();

            user = new UserEntity
            {
                Email = email,
                LastDate = DateTime.Now,
                Password = password.ToMd5(),
                Status = UserStatusEnum.正常,
                RegDate = DateTime.Now,
                UserName = "",
                RegIp = ip,
                LastIp = ip, 
                HasVerifiedEmail = true
            };

            await _userDomainService.Add(user);

            var jwtAccount = new JwtAccount
            {
                UserId = user.Id,
                Email = user.Email
            };

            await _httpAccessor.HttpContext.SignIn(CookieAuthenticationDefaults.AuthenticationScheme, jwtAccount);

            await _redisDb.KeyDelete(key);

            if (await Commit())
            {
                await _bus.RaiseEvent(new SignUpEvent(user)).ConfigureAwait(false);
            }

            return Unit.Value;
        }

        public async Task<Unit> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            var email = command.Email;
            var password = command.Password;

            var user = await _userDomainService.Get(p => p.Email == email && p.HasVerifiedEmail);
            if (user == null)
            {
                await _bus.RaiseEvent(new DomainNotification("用户不存在"));
                return Unit.Value;
            }

            if (user.Password != password.ToMd5())
            {
                await _bus.RaiseEvent(new DomainNotification("密码错误"));
                return Unit.Value;
            }

            if (user.Status != UserStatusEnum.正常)
            {
                await _bus.RaiseEvent(new DomainNotification($"账号为“{user.Status}”状态"));
                return Unit.Value;
            }

            string ip = _httpAccessor.HttpContext.GetUserIp();

            user.LastDate = DateTime.Now;
            user.LastIp = ip;

            await _userDomainService.Update(user);
            var jwtAccount = new JwtAccount
            {
                UserId = user.Id,
                Email = user.Email
            };

            await _httpAccessor.HttpContext.SignIn(CookieAuthenticationDefaults.AuthenticationScheme, jwtAccount);

            if (await Commit())
            {
                await _bus.RaiseEvent(new SignInEvent(user)).ConfigureAwait(false);
            }

            return Unit.Value;
        }

        public async Task<Unit> Handle(LogoutCommand command, CancellationToken cancellationToken)
        {
            var id = command.Id;

            var user = await _userDomainService.Get(id);
            if (user == null)
            {
                await _bus.RaiseEvent(new DomainNotification("用户不存在"));
                return Unit.Value;
            }

            await _httpAccessor.HttpContext.SignOut(CookieAuthenticationDefaults.AuthenticationScheme);

            if (await Commit())
            {
                await _bus.RaiseEvent(new SignOutEvent(user)).ConfigureAwait(false);
            }

            return Unit.Value;
        }

        public async Task<Unit> Handle(ModifyPasswordCommand command, CancellationToken cancellationToken)
        {
            var userId = command.UserId;
            var password = command.Password;
            var newPassword = command.NewPassword;

            var user = await _userDomainService.Get(userId);
            if (user == null)
            {
                await _bus.RaiseEvent(new DomainNotification("用户不存在"));
                return Unit.Value;
            }

            if (user.Password != password.ToMd5())
            {
                await _bus.RaiseEvent(new DomainNotification("密码错误"));
                return Unit.Value;
            }

            user.Password = newPassword.ToMd5();

            await _userDomainService.Update(user);

            if (await Commit())
            {
                await _bus.RaiseEvent(new ModifiedPasswordEvent(user)).ConfigureAwait(false);
            }

            return Unit.Value;
        }

        public async Task<Unit> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            var email = command.Email;
            var code = command.Code;
            var password = command.Password;

            var user = await _userDomainService.Get(p => p.Email == email && p.HasVerifiedEmail);
            if (user == null)
            {
                await _bus.RaiseEvent(new DomainNotification("输入信息有误，请确认邮箱是否无误"));
                return Unit.Value;
            }

            string key = string.Format(RedisKey.ResetPassword, user.Id);// $"resetpassword_{user.Id}";
            long ttl = await _redisDb.KeyTimeToLive(key);
            if (ttl < 0)
            {
                await _bus.RaiseEvent(new DomainNotification($"找回密码验证码已超时，请重试"));
                return Unit.Value;
            }

            string emailCode = await _redisDb.StringGet<string>(key);

            if (string.Compare(emailCode ,code,true) != 0)
            {
                await _bus.RaiseEvent(new DomainNotification($"找回密码验证码已失效，请重试"));
                return Unit.Value;
            }

            await _redisDb.KeyDelete(key);

            user.Password = password.ToMd5();
            await _userDomainService.Update(user);

            if (await Commit())
            {
                await _bus.RaiseEvent(new ResetPasswordEvent(user)).ConfigureAwait(false);
            }

            return Unit.Value;
        }


        public async Task<Unit> Handle(SendRegEmailCommand command, CancellationToken cancellationToken)
        {
            var email = command.Email;

            var user = await _userDomainService.Get(x=> x.Email== email && x.HasVerifiedEmail);
            if (user != null)
            {
                await _bus.RaiseEvent(new DomainNotification("该邮箱已注册，请使用其他邮箱注册"));
                return Unit.Value;
            }

            int expiryMin = 30;

            string key = string.Format(RedisKey.RegEmail, email); //$"regemail_{email}";

            if (await _redisDb.KeyTimeToLive(key) > 0)
            {
                await _bus.RaiseEvent(new DomainNotification($"同一邮箱每{expiryMin}分钟只能发送一次，请稍后"));
                return Unit.Value;
            }

            string code = GenerateRandom(4);



            try
            {
                string url = $"{_appConfig.Site.Url}/user/reg?email={email}&code={code}";
                await _mail.Send(new MailModel
                {
                    Content = $"<p>您好，您正在使用该邮箱注册<a href='{_appConfig.Site.Url}'>{_appConfig.Site.Name}</a>账号，请在验证码输入框中输入此次验证码： {code}，验证码有效期{expiryMin}分钟。</p><p>您也可以<a href='{url}'>点击这里</a>或复制以下链接到浏览器打开</p><p>{url}</p>",
                    Address = email,
                    Subject = $"【{_appConfig.Site.Name}】请继续完成您的注册"
                });
            }
            catch(Exception ex)
            {
                await _bus.RaiseEvent(new DomainNotification($"邮件发送失败，请稍后重试"));
                return Unit.Value;
            }

            await _redisDb.StringSet(key, code, DateTime.Now.AddMinutes(expiryMin));


            if (await Commit())
            {
                //TODO
            }
            
            
            return Unit.Value;
        }

        public async Task<Unit> Handle(SendResetEmailCommand command, CancellationToken cancellationToken)
        {
            var email = command.Email;

            var user = await _userDomainService.Get(p => p.Email == email && p.HasVerifiedEmail);
            if (user == null)
            {
                await _bus.RaiseEvent(new DomainNotification("输入信息有误，请确认邮箱是否无误"));
                return Unit.Value;
            }

            int expiryMin = 30;

            string key = string.Format(RedisKey.ResetPassword, user.Id);// $"resetpassword_{user.Id}";
            if (await _redisDb.KeyTimeToLive(key) > 0)
            {
                await _bus.RaiseEvent(new DomainNotification($"找回密码操作需要间隔24小时"));
                return Unit.Value;
            }


            string code = GenerateRandom(4);

            try
            {
                string url = $"{_appConfig.Site.Url}/user/resetpassword?email={email}&code={code}";
                await _mail.Send(new MailModel
                {
                    Content = $"<p>您好，您正在使用找回密码功能，请在验证码输入框中输入此次验证码： {code}，验证码有效期{expiryMin}分钟。</p><p>您也可以<a href='{url}'>点击这里</a>或复制以下链接到浏览器打开</p><p>{url}</p>",
                    Address = email,
                    Subject = $"【{_appConfig.Site.Name}】找回密码"
                });
            }
            catch (Exception ex)
            {
                await _bus.RaiseEvent(new DomainNotification($"邮件发送失败，请稍后重试"));
                return Unit.Value;
            }

            await _redisDb.StringSet(key, code, DateTime.Now.AddMinutes(expiryMin));

            if (await Commit())
            {
                await _bus.RaiseEvent(new ResetPasswordEvent(user)).ConfigureAwait(false);
            }


            return Unit.Value;
        }


        
        #region 生成随机密码
        private string GenerateRandom(int Length)
        {
            char[] constant =
            {
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z'
            };

            StringBuilder newRandom = new StringBuilder(constant.Length);
            Random rd = new Random();
            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(constant[rd.Next(constant.Length)]);
            }
            return newRandom.ToString().ToUpper();
        }
        #endregion

    }
}

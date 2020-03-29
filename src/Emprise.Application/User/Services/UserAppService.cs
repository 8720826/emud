using AutoMapper;
using Emprise.Application.User.Dtos;
using Emprise.Application.User.Models;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.User.Commands;
using Emprise.Domain.User.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Emprise.Application.User.Services
{
    public class UserAppService : IUserAppService
    {
        private readonly IMediatorHandler _bus;
        private readonly IMapper _mapper;
        private readonly IUserDomainService _userDomainService;
        public UserAppService(IMediatorHandler bus, IMapper mapper, IUserDomainService userDomainService)
        {
            _bus = bus;
            _mapper = mapper;
            _userDomainService = userDomainService;
        }

        #region command
        public async Task Visit()
        {
            var command = new VisitCommand();
            await _bus.SendCommand(command);

        }

        public async Task SendRegEmail(string email)
        {
            var command = new SendRegEmailCommand(email);
            await _bus.SendCommand(command);
        }

        public async Task Reg(UserRegDto dto)
        {
            var command = new RegCommand(dto.Email, dto.Password, dto.Code);
            await _bus.SendCommand(command);
        }

        public async Task Login(UserLoginDto dto)
        {
            var command = new LoginCommand(dto.Email, dto.Password);
            await _bus.SendCommand(command);
        }

        public async Task Logout(int id)
        {
            var command = new LogoutCommand(id);
            await _bus.SendCommand(command);
        }

        public async Task ModifyPassword(int userId, ModifyPasswordDto dto)
        {
            var command = new ModifyPasswordCommand(userId, dto.Password, dto.NewPassword);
            await _bus.SendCommand(command);
        }

        public async Task ResetPassword(ResetPasswordDto dto)
        {
            var command = new ResetPasswordCommand(dto.Email);
            await _bus.SendCommand(command);
        }

        /*
        public async Task SendVerifyEmail(int userId, SendVerifyEmailDto dto)
        {
            var command = new SendVerifyEmailCommand(userId, dto.Email, dto.Password);
            await _bus.SendCommand(command);
        }*/

        #endregion


        public async Task<UserModel> GetUser(int id)
        {
            var user = await _userDomainService.Get(id);

            return _mapper.Map<UserModel>(user);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}

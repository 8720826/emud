using Emprise.Application.User.Dtos;
using Emprise.Application.User.Models;
using Emprise.Domain.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Application.User.Services
{
    public interface IUserAppService : IBaseService
    {
        Task Visit();

        Task Reg(UserRegDto dto);

        Task SendRegEmail(string email);

        Task Login(UserLoginDto dto);

        Task Logout(int id);

        Task ModifyPassword(int userId, ModifyPasswordDto dto);

        Task ResetPassword(ResetPasswordDto dto);

        //Task SendVerifyEmail(int userId, SendVerifyEmailDto dto);

        Task<UserModel> GetUser(int id);
    }
}

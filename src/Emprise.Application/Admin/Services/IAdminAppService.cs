using Emprise.Application.Admin.Dtos;
using Emprise.Domain.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Application.Admin.Services
{
    public interface IAdminAppService : IBaseService
    {
        Task<int> GetCount();

        Task<ResultDto> Login(LoginInput input);

        Task<ResultDto> Logout();

        Task<ResultDto> ModifyPassword(string name, ModifyPasswordInput input);
    }
}

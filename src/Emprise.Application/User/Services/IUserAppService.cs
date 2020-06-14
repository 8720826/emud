using Emprise.Application.User.Dtos;
using Emprise.Application.User.Models;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.User.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Application.User.Services
{
    public interface IUserAppService : IBaseService
    {
        Task<UserEntity> Get(int id);

        Task<ResultDto> SetEnabled(int id, bool enabled);

        Task<ResultDto> ModifyPassword(int id, ModifyPasswordInput modifyPasswordInput);

        Task<Paging<UserEntity>> GetPaging(string keyword, int pageIndex);

        Task<List<PlayerEntity>> GetPlayers(int id);
        Task<UserModel> GetUser(int id);
    }
}

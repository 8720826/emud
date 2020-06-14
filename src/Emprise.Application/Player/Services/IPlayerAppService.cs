using Emprise.Application.Player.Dtos;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.Player.Models;
using Emprise.Domain.User.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Application.Player.Services
{
    public interface IPlayerAppService : IBaseService
    {


        Task<PlayerEntity> GetUserPlayer(int userId);

        Task<PlayerEntity> Get(int id);

        Task<ResultDto> Update(int id, PlayerInput item);

        Task<ResultDto> Delete(int id);

        Task<UserEntity> GetUser(int playerId);

        Task<Paging<PlayerEntity>> GetPaging(string keyword, int pageIndex);
    }
}

using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.Player.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Application.Player.Services
{
    public interface IPlayerAppService : IBaseService
    {


        //Task InitGame(int playerId);



        Task<PlayerEntity> GetUserPlayer(int userId);

        Task<PlayerEntity> GetPlayer(int playerId);


    }
}

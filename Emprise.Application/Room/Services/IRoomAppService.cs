using Emprise.Application.Room.Models;
using Emprise.Application.User.Dtos;
using Emprise.Application.User.Models;
using Emprise.Domain.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Application.User.Services
{
    public interface IRoomAppService : IBaseService
    {
        Task<CurrentRoomInfo> GetCurrent(int playerId);
    }
}

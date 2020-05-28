using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Room.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Application.User.Services
{
    public interface IRoomAppService : IBaseService
    {
        Task<RoomModel> GetCurrent(int playerId);
    }
}

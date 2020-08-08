using Emprise.Application.Room.Models;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Room.Entity;
using Emprise.Domain.Room.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Application.User.Services
{
    public interface IRoomAppService : IBaseService
    {
        Task<RoomEntity> Get(int id);

        Task<RoomModel> GetCurrent(int playerId);

        Task<ResultDto> Add(RoomInput item, int mapId, int roomId, string position);

        Task<ResultDto> Update(int id, RoomInput item);

        Task<ResultDto> Delete(int id);

        Task<Paging<RoomEntity>> GetPaging(int mapId, string keyword, int pageIndex);
    }
}

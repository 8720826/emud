using Emprise.Domain.Core.Interfaces.Ioc;
using Emprise.Domain.Core.Models.Chat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Core.Interfaces
{
    public  interface IMudOnlineProvider: IScoped
    {
        Task<string> GetConnectionId(int playerId);

        Task SetConnectionId(int playerId, string connectionId);

        Task<List<PlayerOnlineModel>> GetPlayerList(int roomId);

        Task CheckOnline();

        Task<PlayerOnlineModel> GetPlayerOnline(int playerId);

        Task SetPlayerOnline(PlayerOnlineModel model);

        Task RemovePlayerOnline(int playerId);
    }
}

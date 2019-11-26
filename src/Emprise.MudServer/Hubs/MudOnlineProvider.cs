using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models.Chat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.MudServer.Hubs
{
    public class MudOnlineProvider: IMudOnlineProvider
    {
        private IRedisDb _db;

        public MudOnlineProvider(IRedisDb db)
        {
            _db = db;
        }

        public async Task<string> GetConnectionId(int playerId)
        {
            var key = "ConnectionId_" + playerId;
            var online = await _db.StringGet<string>(key);
            return online;
        }

        public async Task SetConnectionId(int playerId, string connectionId)
        {
            var key = "ConnectionId_" + playerId;
            await _db.StringSet(key, connectionId,DateTime.Now.AddDays(30));
        }

        public async Task<List<PlayerOnlineModel>> GetPlayerList(int roomId)
        {
            var onlineList = new List<PlayerOnlineModel>();

            var list = await _db.HashGetAll<PlayerOnlineModel>("onlinelist");

            foreach (var online in list)
            {
                if (online.Value.RoomId != roomId)
                {
                    continue;
                }
                online.Value.IsOnline = true;
                onlineList.Add(online.Value);
            }

            var offlineList = await _db.HashGetAll<PlayerOnlineModel>("offlinelist");

            foreach (var offline in offlineList)
            {
                if (offline.Value.RoomId != roomId)
                {
                    continue;
                }

                var online = new PlayerOnlineModel
                {
                    PlayerId = offline.Value.PlayerId,
                    LastDate = offline.Value.LastDate,
                    Level = offline.Value.Level,
                    PlayerName = offline.Value.PlayerName,
                    RoomId = offline.Value.RoomId,
                    Gender = offline.Value.Gender,
                    Title = offline.Value.Title
                };

                online.IsOnline = false;
                onlineList.Add(online);
            }

            return onlineList;

        }

        public async Task CheckOnline()
        {

            var onlineList = await _db.HashGetAll<PlayerOnlineModel>("onlinelist");

            foreach (var online in onlineList)
            {
                if (DateTime.Now.Subtract(online.Value.LastDate).TotalMinutes > 60)
                {
                    await _db.HashDelete("onlinelist", online.Value.PlayerId.ToString());
                }
            }

            var offlineList = await _db.HashGetAll<PlayerOnlineModel>("offlinelist");

            foreach (var offline in offlineList)
            {
                if (DateTime.Now.Subtract(offline.Value.LastDate).TotalHours > 24 * 2)
                {
                    await _db.HashDelete("offlinelist", offline.Value.PlayerId.ToString());
                }
            }

        }


        public async Task<PlayerOnlineModel> GetPlayerOnline(int playerId)
        {
            var model = await _db.HashGet<PlayerOnlineModel>("onlinelist", playerId.ToString());
            if (model == null)
            {
                model = await _db.HashGet<PlayerOnlineModel>("offlinelist", playerId.ToString());
            }
            return model;
        }

        public async Task SetPlayerOnline(PlayerOnlineModel model)
        {
            model.LastDate = DateTime.Now;
            await _db.HashSet("onlinelist", model.PlayerId.ToString(), model);
        }

        public async Task RemovePlayerOnline(int playerId)
        {
            await _db.HashDelete("onlinelist", playerId.ToString());
        }
    }
}

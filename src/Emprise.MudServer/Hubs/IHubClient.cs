using Emprise.Application.Npc.Models;
using Emprise.Application.Player.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Emprise.MudServer.Hubs.Models;
using Emprise.Domain.Player.Models;

namespace Emprise.MudServer.Hubs
{
    public interface IHubClient
    {
        Task ShowMessage(Message message);

        Task Offline();

        Task ShowPlayer(PlayerInfo playerInfo);

        Task ShowNpc(NpcInfo npcInfo);

        Task ShowMe(MyInfo myInfo);

        Task ShowMyStatus(MyInfo myInfo);

        Task ShowMyPack(MyPack myPack);

        Task Test(string msg);
    }
}

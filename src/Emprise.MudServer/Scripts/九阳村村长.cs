using Emprise.Domain.Npc.Entity;
using Emprise.Domain.Player.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Scripts
{
    public class 九阳村村长 : BaseScript
    {
        public 九阳村村长(NpcEntity npc, PlayerEntity player) : base(npc, player)
        {
            Actions = new List<string>
            {
                "测试",
            };

        }


    }
}

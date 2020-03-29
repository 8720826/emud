using Emprise.Application.Npc.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Hubs.Actions
{
    public class NpcCommandAction
    {
        public int NpcId { get; set; }

        public NpcAction Action { get; set; }
    }
}

using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands
{

    public class NpcScriptCommand : Command
    {
        public int NpcId { get; set; }

        public int CommandId { get; set; }


        public int PlayerId { get; set; }

        public int ScriptId { get; set; }

        public string Message { get; set; }



        public NpcScriptCommand(int playerId, int npcId, int scriptId, int commandId, string message)
        {
            NpcId = npcId;
            PlayerId = playerId;
            CommandId = commandId;
            ScriptId = scriptId;
            Message = message;
        }

    }
}

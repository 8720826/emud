using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands
{

    public class NpcActionCommand : Command
    {
        public int NpcId { get; set; }

        public int CommandId { get; set; }

        public string CommandName { get; set; }

        public int PlayerId { get; set; }

        public int ScriptId { get; set; }

        public string Message { get; set; }
        


        public NpcActionCommand(int playerId, int npcId, int scriptId, int commandId, string commandName, string message)
        {
            NpcId = npcId;
            PlayerId = playerId;
            CommandId = commandId;
            ScriptId = scriptId;
            CommandName = commandName;
            Message = message;
        }

    }
}

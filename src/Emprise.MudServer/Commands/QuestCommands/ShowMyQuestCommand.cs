using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands.QuestCommands
{
    public class ShowMyQuestCommand : Command
    {

        public int PlayerId { get; set; }


        public ShowMyQuestCommand(int playerId)
        {
            PlayerId = playerId;
        }

    }
}

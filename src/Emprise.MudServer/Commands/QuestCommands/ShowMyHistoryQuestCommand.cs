using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands.QuestCommands
{
    public class ShowMyHistoryQuestCommand : Command
    {

        public int PlayerId { get; set; }


        public ShowMyHistoryQuestCommand(int playerId)
        {
            PlayerId = playerId;
        }

    }
}

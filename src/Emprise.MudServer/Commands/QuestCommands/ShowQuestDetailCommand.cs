using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands.QuestCommands
{
    public class ShowQuestDetailCommand : Command
    {
        public int PlayerId { get; set; }

        public int QuestId { get; set; }
        public ShowQuestDetailCommand(int playerId, int questId)
        {
            PlayerId = playerId;
            QuestId = questId;
        }
    }
}

using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands
{
    public class CompleteQuestCommand : Command
    {
        public int PlayerId { get; set; }

        public int QuestId { get; set; }
        public CompleteQuestCommand(int playerId, int questId)
        {
            PlayerId = playerId;
            QuestId = questId;
        }
    }
}

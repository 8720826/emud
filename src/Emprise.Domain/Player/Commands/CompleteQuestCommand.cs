using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Player.Commands
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

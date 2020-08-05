using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands
{
    public class QuestCommand : Command
    {

        public int PlayerId { get; set; }

        public int QuestId { get; set; }

        public QuestCommand(int playerId, int questId)
        {
            PlayerId = playerId;
            QuestId = questId;
        }

    }
}

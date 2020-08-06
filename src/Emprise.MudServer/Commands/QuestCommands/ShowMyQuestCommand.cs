using Emprise.Domain.Core.Commands;
using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands.QuestCommands
{
    public class ShowMyQuestCommand : Command
    {

        public int PlayerId { get; set; }

        public QuestTypeEnum Type { set; get; }


        public ShowMyQuestCommand(int playerId, QuestTypeEnum type )
        {
            PlayerId = playerId;
            Type = type;
        }

    }
}

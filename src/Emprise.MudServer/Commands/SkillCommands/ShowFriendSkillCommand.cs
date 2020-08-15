using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands.SkillCommands
{

    public class ShowFriendSkillCommand : Command
    {
        public int PlayerId { get; set; }

        public int FriendId { get; set; }
        public ShowFriendSkillCommand(int playerId, int friendId)
        {
            PlayerId = playerId;
            FriendId = friendId;
        }
    }
}

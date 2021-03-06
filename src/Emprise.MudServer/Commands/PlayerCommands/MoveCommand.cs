﻿using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands
{
    public class MoveCommand : Command
    {
        public int RoomId { get; set; }

        public int PlayerId { get; set; }


        public MoveCommand(int playerId, int roomId)
        {
            RoomId = roomId;
            PlayerId = playerId;
        }

    }
}

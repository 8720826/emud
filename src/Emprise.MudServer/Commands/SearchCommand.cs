using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands
{
    public class SearchCommand : Command
    {

        public int PlayerId { get; set; }


        public SearchCommand(int playerId)
        {
            PlayerId = playerId;
        }

    }
}

using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands.EmailCommands
{
    public class ShowEmailCommand : Command
    {
        public int PlayerId { get; set; }
        public int PageIndex { get; set; }

        public ShowEmailCommand(int playerId, int pageIndex)
        {
            PlayerId = playerId;
            PageIndex = pageIndex;
        }
    }
}

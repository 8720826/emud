using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands
{
    public class DeleteCommand : Command
    {
        public int Id { get; set; }



        public DeleteCommand(int id)
        {
            Id = id;
        }

    }
}

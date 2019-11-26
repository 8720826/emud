using Emprise.Domain.Core.Commands;
using Emprise.Domain.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Player.Commands
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

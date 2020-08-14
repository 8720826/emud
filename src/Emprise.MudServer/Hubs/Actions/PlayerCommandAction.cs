using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Hubs.Actions
{
    public class PlayerCommandAction
    {
        public int TargetId { get; set; }

        public string CommandName { get; set; }
    }
}

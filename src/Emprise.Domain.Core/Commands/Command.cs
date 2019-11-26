using Emprise.Domain.Core.Events;
using System;

namespace Emprise.Domain.Core.Commands
{
    public abstract class Command : Message
    {
        public DateTime Timestamp { get; private set; }

        protected Command()
        {
            Timestamp = DateTime.Now;
        }

    }
}
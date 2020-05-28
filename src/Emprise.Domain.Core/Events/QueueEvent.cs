using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Core.Events
{
    public abstract class QueueEvent : Message
    {


        protected QueueEvent()
        {
            MessageType = GetType().Name;
        }
    }
}

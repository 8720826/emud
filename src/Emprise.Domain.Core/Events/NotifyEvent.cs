using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Core.Events
{
    public abstract class NotifyEvent : Message
    {

        protected NotifyEvent()
        {
            MessageType = GetType().Name;
        }
    }
}

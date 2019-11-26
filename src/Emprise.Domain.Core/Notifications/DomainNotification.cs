using Emprise.Domain.Core.Events;
using System;

namespace Emprise.Domain.Core.Notifications
{
    public class DomainNotification : Event
    {
        public Guid DomainNotificationId { get; private set; }
        public string Content { get; private set; }

        public DomainNotification(string content)
        {
            DomainNotificationId = Guid.NewGuid();
            Content = content;
        }
    }
}
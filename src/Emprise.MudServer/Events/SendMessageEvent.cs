using Emprise.Domain.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Events
{
    public class SendMessageEvent : Event
    {
        public int PlayerId { get; set; }

        public string Content { get; set; }

        public SendMessageEvent(int playerId, string content)
        {
            PlayerId = playerId;
            Content = content;
        }

    }
}

using Emprise.Domain.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Queues
{
    public class TestQueue : QueueEvent
    {
        public static TestQueue CreateSenderQueue(string msgId, string integrationId)
        {
            TestQueue queue = new TestQueue();
            queue.MessageId = msgId;
            queue.QueueType = "Test";
            queue.IntegrationId = integrationId;
            return queue;
        }



        public string QueueType { get; set; }
        public string MessageId { get; set; }
        public string IntegrationId { get; set; }

    }
}

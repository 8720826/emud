using DotNetCore.CAP;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Infra.Bus
{
    public class QueueCapBus : IQueueHandler
    {
        //(TBC) 需要处理异常情况
        //构造函数注入
        private readonly ICapPublisher _publisher;
        // private readonly ILogger<OrderPlacedEventHandler> _logger;


        public QueueCapBus(ICapPublisher publisher
          //   , ServiceFactory serviceFactory
          )
        {
            _publisher = publisher;
            //     _serviceFactory = serviceFactory;
        }

        public async Task SendQueueMessage<T>(T pubQueue) where T : QueueEvent
        {
            await _publisher.PublishAsync<T>(pubQueue.GetType().Name, pubQueue).ConfigureAwait(false);
        }

        ///// 引发事件的实现方法
        public async Task SendNofify<T>(T broadcast) where T : NotifyEvent
        {
            //not use
            // (tbc)
            await _publisher.PublishAsync<T>(broadcast.GetType().Name, broadcast).ConfigureAwait(false);//待处理，是否实现广播

        }
    }
}

using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Commands;
using Emprise.Domain.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Infra.Bus
{
    public class InMemoryBus : IMediatorHandler
    {
        //构造函数注入
        private readonly IMediator _mediator;
        private readonly ILogger _logger;
        public InMemoryBus(IMediator mediator, ILogger<InMemoryBus> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// 实现我们在IMediatorHandler中定义的接口
        /// 没有返回值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<Unit> SendCommand<T>(T command) where T : Command
        {
            _logger.LogDebug($"SendCommand:{JsonConvert.SerializeObject(command)}");

            return await _mediator.Send(command);//请注意 入参 的类型

        }

        /// <summary>
        /// 引发事件的实现方法
        /// </summary>
        /// <typeparam name="T">泛型 继承 Event：INotification</typeparam>
        /// <param name="event">事件模型，比如StudentRegisteredEvent</param>
        /// <returns></returns>
        public async Task RaiseEvent<T>(T @event) where T : Event
        {
            _logger.LogDebug($"RaiseEvent:{JsonConvert.SerializeObject(@event)}");

            await _mediator.Publish(@event);
            return;
        }

    }
}

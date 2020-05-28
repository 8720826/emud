using Emprise.Domain.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Core.Bus
{
    public interface IQueueHandler
    {
        /// <summary>
        /// 入队列
        /// </summary>
        /// <typeparam name="T"> 泛型 </typeparam>
        /// <param name="messaging">  </param>
        /// <returns></returns>
        Task SendQueueMessage<T>(T messaging) where T : QueueEvent;

        /// <summary>
        /// 广播队列
        /// </summary>
        /// <typeparam name="T"> 泛型 继承 Event：INotification</typeparam>
        /// <param name="notify"> 事件</param>
        /// <returns></returns>
        Task SendNofify<T>(T notify) where T : NotifyEvent;

    }
}

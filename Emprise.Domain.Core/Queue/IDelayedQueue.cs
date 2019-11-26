using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Interfaces.Ioc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emprise.Domain.Core.Bus
{
    /// <summary>
    /// 延迟队列
    /// </summary>
    public interface IDelayedQueue : IScoped
    {
        Task<bool> Publish<T>(int playerId, T t, int delayMin, int delayMax = 0);

        Task<List<T>> Subscribe<T>();
    }
}

using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Interfaces.Ioc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emprise.Domain.Core.Bus
{
    /// <summary>
    /// 重复性队列
    /// </summary>
    public interface IRecurringQueue : IScoped
    {
        Task<bool> Publish<T>(int playerId, T t, int delayMin, int delayMax = 0);

        //Task<bool> Publish<T>(T t, int delay, DateTime? endtime);

        Task<Dictionary<int, T>> Subscribe<T>();

        Task Remove<T>(int playerId);
    }
}

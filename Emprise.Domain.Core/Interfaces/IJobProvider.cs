using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Core.Interfaces
{
    public interface IJobProvider
    {
        /// <summary>
        /// Fire-and-forget jobs are executed only once and almost immediately after creation.
        /// </summary>
        /// <param name="methodCall"></param>
        /// <returns></returns>
        Task<string> Enqueue(Expression<Action> methodCall);

        /// <summary>
        /// Delayed jobs are executed only once too, but not immediately, after a certain time interval.
        /// </summary>
        /// <param name="methodCall"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        Task<string> Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay);

        /// <summary>
        /// Recurring jobs fire many times on the specified CRON schedule.
        /// </summary>
        /// <param name="methodCall"></param>
        /// <param name="cron"></param>
        /// <returns></returns>
        Task AddOrUpdate<T>(Expression<Func<T, Task>> methodCall, string cron);

        /// <summary>
        /// Continuations are executed when its parent job has been 
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="methodCall"></param>
        /// <returns></returns>
        Task<string> ContinueJobWith(string jobId, Expression<Action> methodCall);


        Task<string> MinuteInterval(int interval = 1);
    }
}

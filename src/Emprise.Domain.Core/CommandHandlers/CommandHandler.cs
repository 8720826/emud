using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Commands;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Notifications;
using MediatR;
using System.Threading.Tasks;

namespace Emprise.Domain.Core.CommandHandlers
{
    /// <summary>
    /// 领域命令处理程序
    /// 用来作为全部处理程序的基类，提供公共方法和接口数据
    /// </summary>
    public class CommandHandler
    {
        // 注入工作单元
        private readonly IUnitOfWork _uow;
        // 注入中介处理接口
        private readonly IMediatorHandler _bus;


        //private IMemoryCache _cache;
        private readonly DomainNotificationHandler _notifications;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="bus"></param>
        /// <param name="cache"></param>
        public CommandHandler(IUnitOfWork uow, IMediatorHandler bus, INotificationHandler<DomainNotification> notifications)
        {
            _uow = uow;
            _bus = bus;
            _notifications = (DomainNotificationHandler)notifications;
        }

        /*
        ////将领域命令中的验证错误信息收集
        protected void NotifyValidationErrors(Command message)
        {
            foreach (var error in message.ValidationResult.Errors)
            {
                //将错误信息提交到事件总线，派发出去
                _bus.RaiseEvent(new DomainNotification(message.MessageType, error.ErrorMessage));
            }
        }
        */

        //工作单元提交
        //如果有错误，下一步会在这里添加领域通知
        public async Task<bool> Commit()
        {
            await _uow.Commit();
            return true;
        }
    }
}

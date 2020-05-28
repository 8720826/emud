using Emprise.Domain.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.MudServer.Consumers
{
    public class BaseConsumer
    {
        // 注入工作单元
        private readonly IUnitOfWork _uow;



        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="bus"></param>
        /// <param name="cache"></param>
        public BaseConsumer(IUnitOfWork uow)
        {
            _uow = uow;
        }

        //工作单元提交
        //如果有错误，下一步会在这里添加领域通知
        public async Task<bool> Commit()
        {
            await _uow.Commit();
            return true;
        }
    }
}

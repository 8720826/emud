using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Core.EventHandlers
{
    public class MudEventHandler
    {       
        
        // 注入工作单元
        private readonly IUnitOfWork _uow;

        private readonly IMudProvider _mudProvider;


        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="bus"></param>
        /// <param name="cache"></param>
        public MudEventHandler(IUnitOfWork uow, IMudProvider mudProvider)
        {
            _uow = uow;
            _mudProvider = mudProvider;
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

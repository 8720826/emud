using Emprise.Domain.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Core.Services
{
    public class BaseAppService
    {
        private readonly IUnitOfWork _uow;
        public BaseAppService(IUnitOfWork uow)
        {
            _uow = uow;
        }


        public async Task<bool> Commit()
        {
            await _uow.Commit();
            return true;
        }
    }
}

using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Log.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Log.Services
{
    public interface IOperatorLogDomainService : IBaseService
    {
        Task AddSuccess(OperatorLogEntity item);

        Task AddError(OperatorLogEntity item);
    }
}

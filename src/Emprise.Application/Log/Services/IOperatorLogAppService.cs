using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Log.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Application.Log.Services
{
    public interface IOperatorLogAppService : IBaseService
    {
        Task ClearLog(DateTime dt);

        Task<Paging<OperatorLogEntity>> GetPaging(string keyword, int pageIndex);
    }
}

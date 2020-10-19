using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Ware.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Ware.Services
{

    public interface IPlayerWareDomainService : IBaseDomainService<PlayerWareEntity>
    {
        Task<List<PlayerWareEntity>> GetAll(int playerId);

    }
}

using Emprise.Domain.Core.Interfaces.Ioc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Emprise.Domain.Core.Data
{
    public interface IUnitOfWork : IScoped, IDisposable
    {

        Task<int> Commit();
    }
}

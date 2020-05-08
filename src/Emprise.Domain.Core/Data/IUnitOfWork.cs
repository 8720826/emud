using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Emprise.Domain.Core.Data
{
    public interface IUnitOfWork : IDisposable
    {

        Task<int> Commit();
    }
}

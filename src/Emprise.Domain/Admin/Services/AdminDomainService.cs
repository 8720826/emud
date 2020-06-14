using Emprise.Domain.Admin.Entity;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Map.Entity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Admin.Services
{
    public class AdminDomainService: IAdminDomainService
    {
        private readonly IRepository<AdminEntity> _adminRepository;
        private readonly IMemoryCache _cache;
        private readonly IMediatorHandler _bus;

        public AdminDomainService(IRepository<AdminEntity> adminRepository, IMemoryCache cache, IMediatorHandler bus)
        {
            _adminRepository = adminRepository;
            _cache = cache;
            _bus = bus;
        }


        public async Task<AdminEntity> Get(Expression<Func<AdminEntity, bool>> where)
        {
            return await _adminRepository.Get(where);
        }


        public async Task Add(AdminEntity entity)
        {
            await _adminRepository.Add(entity);
        }

        public async Task<IQueryable<AdminEntity>> GetAll()
        {
            return await _adminRepository.GetAll();
        }

        public async Task Update(AdminEntity entity)
        {
            await _adminRepository.Update(entity);
            await _bus.RaiseEvent(new EntityUpdatedEvent<AdminEntity>(entity)).ConfigureAwait(false);
        }

        public async Task Delete(AdminEntity entity)
        {
            await _adminRepository.Remove(entity);
            await _bus.RaiseEvent(new EntityDeletedEvent<AdminEntity>(entity)).ConfigureAwait(false);
        }
    }
}

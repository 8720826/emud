using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Skill.Entity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Skill.Services
{
    public  class SkillDomainService: ISkillDomainService
    {
        private readonly IRepository<SkillEntity> _skillRepository;
        private readonly IMemoryCache _cache;
        private readonly IMediatorHandler _bus;

        public SkillDomainService(IRepository<SkillEntity> skillRepository, IMemoryCache cache, IMediatorHandler bus)
        {
            _skillRepository = skillRepository;
            _cache = cache;
            _bus = bus;
        }

        public async Task<SkillEntity> Get(Expression<Func<SkillEntity, bool>> where)
        {
            return await _skillRepository.Get(where);
        }


        public async Task<SkillEntity> Get(int id)
        {
            var key = string.Format(CacheKey.Skill, id);
            return await _cache.GetOrCreateAsync(key, async p => {
                p.SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheKey.ExpireMinutes));
                return await _skillRepository.Get(id);
            });
        }

        public async Task Add(SkillEntity entity)
        {
            await _skillRepository.Add(entity);
            await _bus.RaiseEvent(new EntityInsertedEvent<SkillEntity>(entity)).ConfigureAwait(false);
        }

        public async Task<IQueryable<SkillEntity>> GetAll()
        {
            return await _skillRepository.GetAll();
        }

        public async Task Update(SkillEntity entity)
        {
            await _skillRepository.Update(entity);
            await _bus.RaiseEvent(new EntityUpdatedEvent<SkillEntity>(entity)).ConfigureAwait(false);
        }

        public async Task Delete(SkillEntity entity)
        {
            await _skillRepository.Remove(entity);
            await _bus.RaiseEvent(new EntityDeletedEvent<SkillEntity>(entity)).ConfigureAwait(false);
        }

        public async Task<List<SkillEntity>> GetAllBaseSkills()
        {
            var key = CacheKey.BaseSkills;
            return await _cache.GetOrCreateAsync(key, async p => {
                p.SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheKey.ExpireMinutes));
                return (await _skillRepository.GetAll(x => x.IsBase)).ToList();
            });
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}

using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Services;
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
    public  class SkillDomainService : BaseDomainService<SkillEntity>, ISkillDomainService
    {
        private readonly IRepository<SkillEntity> _skillRepository;
        private readonly IMemoryCache _cache;
        private readonly IMediatorHandler _bus;

        public SkillDomainService(IRepository<SkillEntity> skillRepository, IMemoryCache cache, IMediatorHandler bus) : base(skillRepository, cache, bus)
        {
            _skillRepository = skillRepository;
            _cache = cache;
            _bus = bus;
        }


        public async Task<List<SkillEntity>> GetAllBaseSkills()
        {
            var key = CacheKey.BaseSkills;
            return await _cache.GetOrCreateAsync(key, async p => {
                p.SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheKey.ExpireMinutes));
                return (await _skillRepository.GetAll(x => x.IsBase)).ToList();
            });
        }


    }
}

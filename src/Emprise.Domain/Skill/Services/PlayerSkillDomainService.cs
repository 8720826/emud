using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
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
    public class PlayerSkillDomainService : BaseDomainService<PlayerSkillEntity>,  IPlayerSkillDomainService
    {
        private readonly IRepository<PlayerSkillEntity> _skillRepository;
        private readonly IMemoryCache _cache;
        private readonly IMediatorHandler _bus;

        public PlayerSkillDomainService(IRepository<PlayerSkillEntity> skillRepository, IMemoryCache cache, IMediatorHandler bus) : base(skillRepository, cache, bus)
        {
            _skillRepository = skillRepository;
            _cache = cache;
            _bus = bus;
        }


        public async Task<List<PlayerSkillEntity>> GetAll(int playerId)
        {
            var query = await _skillRepository.GetAll(x => x.PlayerId == playerId);

            return query.ToList();
        }


        public async Task Delete(int id)
        {
            await _skillRepository.Remove(id);
        }

        
    }
}

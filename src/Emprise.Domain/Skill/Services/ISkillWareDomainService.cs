using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Skill.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Skill.Services
{

    public interface IPlayerSkillDomainService : IBaseService
    {
        Task<PlayerSkillEntity> Get(Expression<Func<PlayerSkillEntity, bool>> where);

        Task<List<PlayerSkillEntity>> GetAll(int playerId);

        Task<PlayerSkillEntity> Get(int id);

        Task Add(PlayerSkillEntity entity);

        Task Update(PlayerSkillEntity entity);

        Task Delete(int id);
    }
}

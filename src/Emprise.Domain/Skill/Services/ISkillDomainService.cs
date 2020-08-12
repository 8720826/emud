using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Skill.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Skill.Services
{

    public interface ISkillDomainService : IBaseService
    {
        Task<SkillEntity> Get(Expression<Func<SkillEntity, bool>> where);



        Task<IQueryable<SkillEntity>> GetAll();

        Task<SkillEntity> Get(int id);

        Task Add(SkillEntity entity);

        Task Update(SkillEntity entity);

        Task Delete(SkillEntity entity);
    }
}

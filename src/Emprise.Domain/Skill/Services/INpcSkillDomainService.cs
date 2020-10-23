using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Skill.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Skill.Services
{

    public interface INpcSkillDomainService : IBaseDomainService<NpcSkillEntity>
    {

        Task<List<NpcSkillEntity>> GetAll(int npcId);



        Task Delete(int id);
    }
}

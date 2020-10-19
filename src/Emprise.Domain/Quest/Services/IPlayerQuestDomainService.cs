using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Quest.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Quest.Services
{

    public interface IPlayerQuestDomainService :IBaseDomainService<PlayerQuestEntity>
    {
        Task<List<PlayerQuestEntity>> GetPlayerQuests(int playerId);


    }
}

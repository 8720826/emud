using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Quest.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Quest.Services
{

    public interface IPlayerQuestDomainService : IBaseService
    {
        Task<List<PlayerQuestEntity>> GetPlayerQuests(int playerId);

        Task<PlayerQuestEntity> Get(Expression<Func<PlayerQuestEntity, bool>> where);


        Task<PlayerQuestEntity> Get(int id);

        Task Add(PlayerQuestEntity user);

        Task Update(PlayerQuestEntity user);

    }
}

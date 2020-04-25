using Emprise.Domain.Core.Enum;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Npc.Entity;
using Emprise.Domain.Quest.Entity;
using Emprise.Domain.Quest.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Quest.Services
{
    public interface IQuestDomainService : IBaseService
    {
        Task<QuestEntity> Get(Expression<Func<QuestEntity, bool>> where);

        Task<List<QuestEntity>> GetAll();

        Task<QuestEntity> Get(int id);

        Task Add(QuestEntity user);

        Task Update(QuestEntity user);

        Task<QuestEntity> CheckTriggerCondition(QuestTriggerTypeEnum triggerTypeEnum,  QuestTriggerCheckModel checkModel);
    }
}

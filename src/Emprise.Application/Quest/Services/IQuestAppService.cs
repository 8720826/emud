using Emprise.Application.Quest.Dtos;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Quest.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Application.Quest.Services
{
    public interface IQuestAppService : IBaseService
    {
        Task<QuestEntity> Get(int id);

        Task<ResultDto> Add(QuestInput item);

        Task<ResultDto> Update(int id, QuestInput item);

        Task<ResultDto> Delete(int id);

        Task<Paging<QuestEntity>> GetPaging(string keyword, int pageIndex);
    }
}

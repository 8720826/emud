using Emprise.Application.Npc.Dtos;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Npc.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Application.Npc.Services
{
    public interface INpcAppService : IBaseService
    {

        Task<NpcEntity> Get(int id);
        //Task<NpcInfo> GetNpc(int playerId, int id);

        Task<ResultDto> Add(NpcInput item);

        Task<ResultDto> Update(int id, NpcInput item, List<int> scriptIds);

        Task<ResultDto> Delete(int id);

        Task<ResultDto> SetEnabled(int id, bool enabled);

        Task<List<ScriptEntity>> GetScripts(int npcId);


        Task<Paging<NpcEntity>> GetPaging(string keyword, int pageIndex);

        Task<Dictionary<int, List<ScriptEntity>>> GetScripts(List<int> ids);


    }
}

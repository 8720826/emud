
using Emprise.Application.Npc.Models;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Npc.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Application.User.Services
{
    public interface INpcAppService : IBaseService
    {

        Task<NpcEntity> Get(int id);
        Task<NpcInfo> GetNpc(int playerId, int id);
    }
}

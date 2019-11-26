using Emprise.Domain.Core.Data;
using Emprise.Domain.Npc.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Emprise.Domain.Npc.Services
{
    public class NpcDomainService : INpcDomainService
    {
        private readonly IRepository<NpcEntity> _npcRepository;

        public NpcDomainService(IRepository<NpcEntity> npcRepository)
        {
            _npcRepository = npcRepository;
        }

        public async Task<List<NpcEntity>> Query(Expression<Func<NpcEntity, bool>> where)
        {
            var query =  await _npcRepository.GetAll(where);
            return query.ToList();
        }

        public async Task<NpcEntity> Get(Expression<Func<NpcEntity, bool>> where)
        {
            return await _npcRepository.Get(where);
        }

        public async Task<NpcEntity> Get(int id)
        {
            return await _npcRepository.Get(id);
        }

        public async Task Add(NpcEntity user)
        {
            await _npcRepository.Add(user);
        }

        public async Task Update(NpcEntity user)
        {
             await _npcRepository.Update(user);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}

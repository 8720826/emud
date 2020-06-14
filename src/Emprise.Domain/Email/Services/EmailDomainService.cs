using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Email.Entity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Email.Services
{
    public class EmailDomainService : IEmailDomainService
    {
        private readonly IRepository<EmailEntity> _emailRepository;
        private readonly IMemoryCache _cache;
        private readonly IMediatorHandler _bus;
        public EmailDomainService(IRepository<EmailEntity> emailRepository, IMemoryCache cache, IMediatorHandler bus)
        {
            _emailRepository = emailRepository; 
            _cache = cache;
            _bus = bus;
        }

        public async Task<EmailEntity> Get(Expression<Func<EmailEntity, bool>> where)
        {
            return await _emailRepository.Get(where);
        }


        public async Task<EmailEntity> Get(int id)
        {
            var key = string.Format(CacheKey.Ware, id);
            return await _cache.GetOrCreateAsync(key, async p => {
                p.SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheKey.ExpireMinutes));
                return await _emailRepository.Get(id);
            });
        }

        public async Task Add(EmailEntity entity)
        {
            await _emailRepository.Add(entity);
        }

        public async Task<IQueryable<EmailEntity>> GetAll()
        {
            return await _emailRepository.GetAll();
        }

        public async Task Update(EmailEntity entity)
        {
            await _emailRepository.Update(entity);
            await _bus.RaiseEvent(new EntityUpdatedEvent<EmailEntity>(entity)).ConfigureAwait(false);
        }

        public async Task Delete(EmailEntity entity)
        {
            await _emailRepository.Remove(entity);
            await _bus.RaiseEvent(new EntityDeletedEvent<EmailEntity>(entity)).ConfigureAwait(false);
        }


        public async Task<List<EmailEntity>> GetMyEmails(int playerId, int factionId)
        {
            var query = await _emailRepository.GetAll(x => x.Type == EmailTypeEnum.公告 || x.Type == EmailTypeEnum.系统 || (x.Type == EmailTypeEnum.私信 && x.TypeId == playerId) || (factionId > 0 && x.Type == EmailTypeEnum.帮派 && x.TypeId == factionId));
            return query.ToList();
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}

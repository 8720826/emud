using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Services;
using Emprise.Domain.Log.Entity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Emprise.Domain.Log.Services
{
    public class ChatLogDomainService : BaseDomainService<ChatLogEntity>, IChatLogDomainService
    {
        private readonly IRepository<ChatLogEntity> _logRepository;

        public ChatLogDomainService(IRepository<ChatLogEntity> logRepository, IMemoryCache cache, IMediatorHandler bus) : base(logRepository, cache, bus)
        {
            _logRepository = logRepository;
        }

    }
}

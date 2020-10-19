using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Services;
using Emprise.Domain.Quest.Entity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Emprise.Domain.Quest.Services
{
    public class QuestDomainService : BaseDomainService<QuestEntity>, IQuestDomainService
    {
        private readonly IRepository<QuestEntity> _questRepository;
        private readonly IMemoryCache _cache;
        private readonly IMediatorHandler _bus;
        private readonly ILogger<QuestDomainService> _logger;
        public QuestDomainService(IRepository<QuestEntity> questRepository, IMemoryCache cache, IMediatorHandler bus, ILogger<QuestDomainService> logger) : base(questRepository, cache, bus)
        {
            _questRepository = questRepository;
            _cache = cache;
            _bus = bus;
            _logger = logger;
        }

    }
}

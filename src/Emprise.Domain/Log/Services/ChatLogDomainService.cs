using Emprise.Domain.Core.Data;
using Emprise.Domain.Log.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Emprise.Domain.Log.Services
{
    public class ChatLogDomainService : IChatLogDomainService
    {
        private readonly IRepository<ChatLogEntity> _logRepository;

        public ChatLogDomainService(IRepository<ChatLogEntity> logRepository)
        {
            _logRepository = logRepository;
        }


        public async Task Add(ChatLogEntity user)
        {
            await _logRepository.Add(user);
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}

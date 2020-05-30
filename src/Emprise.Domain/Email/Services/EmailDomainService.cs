using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Email.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Email.Services
{
    public class EmailDomainService : IEmailDomainService
    {
        private readonly IRepository<EmailEntity> _emailRepository;

        public EmailDomainService(IRepository<EmailEntity> emailRepository)
        {
            _emailRepository = emailRepository;
        }

        public async Task<IQueryable<EmailEntity>> Query()
        {
            return await _emailRepository.GetAll();
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

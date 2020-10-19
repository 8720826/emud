using Emprise.Domain.Admin.Entity;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Services;
using Emprise.Domain.Map.Entity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Domain.Admin.Services
{
    public class AdminDomainService: BaseDomainService<AdminEntity>, IAdminDomainService
    {
        private readonly IRepository<AdminEntity> _adminRepository;

        public AdminDomainService(IRepository<AdminEntity> adminRepository, IMemoryCache cache, IMediatorHandler bus) : base(adminRepository, cache, bus)
        {
            _adminRepository = adminRepository;
        }


    }
}

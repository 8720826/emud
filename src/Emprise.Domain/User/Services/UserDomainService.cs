using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Services;
using Emprise.Domain.User.Entity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Emprise.Domain.User.Services
{
    public class UserDomainService : BaseDomainService<UserEntity>, IUserDomainService
    {
        private readonly IRepository<UserEntity> _userRepository;

        public UserDomainService(IRepository<UserEntity> userRepository, IMemoryCache cache, IMediatorHandler bus) : base(userRepository, cache, bus)
        {
            _userRepository = userRepository;
        }

    }
}

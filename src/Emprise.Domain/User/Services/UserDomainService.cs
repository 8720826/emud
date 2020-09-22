using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Services;
using Emprise.Domain.User.Entity;
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

        public UserDomainService(IRepository<UserEntity> userRepository):base(userRepository)
        {
            _userRepository = userRepository;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}

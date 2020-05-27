using Emprise.Domain.Core.Data;
using Emprise.Domain.User.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Emprise.Domain.User.Services
{
    public class UserDomainService : IUserDomainService
    {
        private readonly IRepository<UserEntity> _userRepository;

        public UserDomainService(IRepository<UserEntity> userRepository)
        {
            _userRepository = userRepository;
        }


        public async Task<UserEntity> Get(Expression<Func<UserEntity, bool>> where)
        {
            return await _userRepository.Get(where);
        }

        public async Task<UserEntity> Get(int id)
        {
            return await _userRepository.Get(id);
        }

        public async Task Add(UserEntity user)
        {
            await _userRepository.Add(user);
        }

        public async Task Update(UserEntity user)
        {
             await _userRepository.Update(user);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}

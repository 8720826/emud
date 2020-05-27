using AutoMapper;
using Emprise.Application.User.Dtos;
using Emprise.Application.User.Models;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.User.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Emprise.Application.User.Services
{
    public class UserAppService : IUserAppService
    {
        private readonly IMapper _mapper;
        private readonly IUserDomainService _userDomainService;
        public UserAppService(IMapper mapper, IUserDomainService userDomainService)
        {
            _mapper = mapper;
            _userDomainService = userDomainService;
        }



        public async Task<UserModel> GetUser(int id)
        {
            var user = await _userDomainService.Get(id);

            return _mapper.Map<UserModel>(user);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}

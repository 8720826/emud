using AutoMapper;
using Emprise.Application.User.Dtos;
using Emprise.Application.User.Models;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Services;
using Emprise.Domain.Log.Entity;
using Emprise.Domain.Log.Services;
using Emprise.Domain.Player.Entity;
using Emprise.Domain.Player.Services;
using Emprise.Domain.User.Entity;
using Emprise.Domain.User.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Emprise.Application.User.Services
{
    public class UserAppService : BaseAppService, IUserAppService
    {
        private readonly IMapper _mapper;
        private readonly IUserDomainService _userDomainService;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly IOperatorLogDomainService _operatorLogDomainService;
        public UserAppService(
            IMapper mapper,
            IUserDomainService userDomainService,
            IPlayerDomainService playerDomainService,
            IUnitOfWork uow,
            IOperatorLogDomainService operatorLogDomainService)
            : base(uow)
        {
            _mapper = mapper;
            _userDomainService = userDomainService;
            _playerDomainService = playerDomainService;
            _operatorLogDomainService = operatorLogDomainService;
        }

        public async Task<UserEntity> Get(int id)
        {
            return await _userDomainService.Get(id);
        }

        public async Task<ResultDto> SetEnabled(int id, bool enabled)
        {
            var result = new ResultDto { Message = "" };

            var operatorLogType = enabled ? OperatorLogType.启用用户 : OperatorLogType.禁用用户;
            try
            {
                var user = await _userDomainService.Get(id);
                if (user == null)
                {
                    result.Message = $"用户 {id} 不存在！";
                    return result;
                }

                user.Status = enabled ? UserStatusEnum.正常 : UserStatusEnum.封禁;

                await _userDomainService.Update(user);

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = operatorLogType,
                    Content = $"Id = {id}"
                });

                await Commit();

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                await _operatorLogDomainService.AddError(new OperatorLogEntity
                {
                    Type = operatorLogType,
                    Content = $"Id = {id}"
                });
                await Commit();
            }
            return result;
        }

        public async Task<ResultDto> ModifyPassword(int id, ModifyPasswordInput modifyPasswordInput)
        {
            var result = new ResultDto { Message = "" };

            try
            {
                var user = await _userDomainService.Get(id);
                if (user == null)
                {
                    result.Message = $"用户 {id} 不存在！";
                    return result;
                }

                user.Password = modifyPasswordInput.NewPassword.ToMd5();

                await _userDomainService.Update(user);

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.修改用户密码,
                    Content = $"Id = {id}，{JsonConvert.SerializeObject(modifyPasswordInput)}"
                });

                await Commit();

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                await _operatorLogDomainService.AddError(new OperatorLogEntity
                {
                    Type = OperatorLogType.修改用户密码,
                    Content = $"Id = {id}，Data={JsonConvert.SerializeObject(modifyPasswordInput)},ErrorMessage={ex.Message}"
                });
                await Commit();
            }
            return result;
        }


        public async Task<Paging<UserEntity>> GetPaging(string keyword, int pageIndex)
        {

            var query = await _userDomainService.GetAll();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Email.Contains(keyword));
            }
            query = query.OrderBy(x => x.Id);

            return await query.Paged(pageIndex);
        }


        public async Task<List<PlayerEntity>> GetPlayers(int id)
        {
            var query = await _playerDomainService.GetAll();

            return query.Where(x => x.UserId == id).ToList();
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

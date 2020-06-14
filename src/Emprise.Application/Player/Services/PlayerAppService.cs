using AutoMapper;
using Emprise.Application.Player.Dtos;
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
using Emprise.Infra.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Emprise.Application.Player.Services
{
    public class PlayerAppService : BaseAppService, IPlayerAppService
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly IUserDomainService _userDomainService;
        private readonly IOperatorLogDomainService _operatorLogDomainService;
        public PlayerAppService(
            IMapper mapper, 
            IPlayerDomainService playerDomainService,
            IUserDomainService userDomainService,
            IOperatorLogDomainService operatorLogDomainService,
            IUnitOfWork uow,
            ILogger<PlayerAppService> logger)
            : base(uow)
        {
            _mapper = mapper;
            _playerDomainService = playerDomainService;
            _userDomainService = userDomainService;
            _operatorLogDomainService = operatorLogDomainService;
            _logger = logger;
        }



        public async Task<PlayerEntity> GetUserPlayer(int userId)
        {
            return await _playerDomainService.GetUserPlayer(userId);
        }

        public async Task<PlayerEntity> Get(int id)
        {
            return await _playerDomainService.Get(id);
        }

        public async Task<ResultDto> Update(int id, PlayerInput item)
        {
            var result = new ResultDto { Message = "" };
            try
            {
                var player = await _playerDomainService.Get(id);
                if (player == null)
                {
                    result.Message = $"玩家 {id} 不存在！";
                    return result;
                }
                var content = player.ComparisonTo(item);
                _mapper.Map(item, player);

                await _playerDomainService.Update(player);

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.修改玩家,
                    Content = $"Id = {id},Data = {content}"
                });

                await Commit();

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                await _operatorLogDomainService.AddError(new OperatorLogEntity
                {
                    Type = OperatorLogType.修改玩家,
                    Content = $"Data={JsonConvert.SerializeObject(item)},ErrorMessage={result.Message}"
                });
                await Commit();
            }
            return result;
        }

        public async Task<ResultDto> Delete(int id)
        {
            var result = new ResultDto { Message = "" };
            try
            {
                var player = await _playerDomainService.Get(id);
                if (player == null)
                {
                    result.Message = $"玩家 {id} 不存在！";
                    return result;
                }


                await _playerDomainService.Delete(player);

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.删除玩家,
                    Content = JsonConvert.SerializeObject(player)
                });

                await Commit();

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                await _operatorLogDomainService.AddError(new OperatorLogEntity
                {
                    Type = OperatorLogType.删除玩家,
                    Content = $"id={id}，ErrorMessage={result.Message}"
                });
                await Commit();
            }
            return result;
        }

        public async Task<UserEntity> GetUser(int playerId)
        {
            var player = await _playerDomainService.Get(playerId);
            if (player == null)
            {
                return null;
            }
            return await _userDomainService.Get(player.UserId);
        }

        public async Task<Paging<PlayerEntity>> GetPaging(string keyword, int pageIndex)
        {

            var query = await _playerDomainService.GetAll();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Name.Contains(keyword));
            }
            query = query.OrderBy(x => x.Id);

            return await query.Paged(pageIndex);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}

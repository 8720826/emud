using AutoMapper;
using Emprise.Application.Npc.Dtos;
using Emprise.Domain.Core.Authorization;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Services;
using Emprise.Domain.Log.Entity;
using Emprise.Domain.Log.Services;
using Emprise.Domain.Npc.Entity;
using Emprise.Domain.Npc.Services;
using Emprise.Domain.Player.Services;
using Emprise.Infra.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Emprise.Application.Npc.Services
{
    public class NpcAppService : BaseAppService, INpcAppService
    {
        private readonly IMediatorHandler _bus;
        private readonly IMapper _mapper;
        private readonly INpcDomainService _npcDomainService;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly IAccountContext _account;
        private readonly IScriptDomainService _scriptDomainService;
        private readonly INpcScriptDomainService _npcScriptDomainService;
        private readonly IMudProvider _mudProvider;
        private readonly ILogger<NpcAppService> _logger;
        private readonly IOperatorLogDomainService _operatorLogDomainService;

        public NpcAppService(IMediatorHandler bus, IMapper mapper, INpcDomainService npcDomainService, IPlayerDomainService playerDomainService, IAccountContext account, IScriptDomainService scriptDomainService, INpcScriptDomainService npcScriptDomainService,  IMudProvider mudProvider, ILogger<NpcAppService> logger,
            IUnitOfWork uow,
            IOperatorLogDomainService operatorLogDomainService) 
            : base(uow)
        {
            _bus = bus;
            _mapper = mapper;
            _npcDomainService = npcDomainService;
            _playerDomainService = playerDomainService;
            _account = account;
            _scriptDomainService = scriptDomainService;
            _npcScriptDomainService = npcScriptDomainService;
            _mudProvider = mudProvider;
            _logger = logger;
            _operatorLogDomainService = operatorLogDomainService;
        }

        public async Task<NpcEntity> Get(int id)
        {
            return await _npcDomainService.Get(id);
        }

        public async Task<ResultDto> Add(NpcInput item)
        {
            var result = new ResultDto { Message = "" };

            try
            {
                var npc = _mapper.Map<NpcEntity>(item);
                await _npcDomainService.Add(npc);

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.添加Npc,
                    Content = JsonConvert.SerializeObject(item)
                });

                await Commit();
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                await _operatorLogDomainService.AddError(new OperatorLogEntity
                {
                    Type = OperatorLogType.添加Npc,
                    Content = $"Data={JsonConvert.SerializeObject(item)},ErrorMessage={result.Message}"
                });
                await Commit();
            }
            return result;
        }

        public async Task<ResultDto> Update(int id, NpcInput item, List<int> scriptIds)
        {
            var result = new ResultDto { Message = "" };
            try
            {
                var npc = await _npcDomainService.Get(id);
                if (npc == null)
                {
                    result.Message = $"Npc {id} 不存在！";
                    return result;
                }
                var content = npc.ComparisonTo(item);
                _mapper.Map(item, npc);


                await _npcDomainService.Update(npc);

                var npcScripts = (await _npcScriptDomainService.GetAll()).Where(x => x.NpcId == id);
                foreach (var npcScript in npcScripts)
                {
                    if (!scriptIds.Contains(npcScript.ScriptId))
                    {
                        await _npcScriptDomainService.Delete(npcScript);
                    }
                    else
                    {
                        scriptIds.Remove(npcScript.ScriptId);
                    }
                }


                foreach (var scriptId in scriptIds)
                {
                    await _npcScriptDomainService.Add(new NpcScriptEntity { NpcId = id, ScriptId = scriptId });
                }


                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.修改Npc,
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
                    Type = OperatorLogType.修改Npc,
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
                var npc = await _npcDomainService.Get(id);
                if (npc == null)
                {
                    result.Message = $"Npc {id} 不存在！";
                    return result;
                }


                await _npcDomainService.Delete(npc);

                await _operatorLogDomainService.AddSuccess(new OperatorLogEntity
                {
                    Type = OperatorLogType.删除Npc,
                    Content = JsonConvert.SerializeObject(npc)
                });

                await Commit();

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                await _operatorLogDomainService.AddError(new OperatorLogEntity
                {
                    Type = OperatorLogType.删除Npc,
                    Content = $"id={id}，ErrorMessage={result.Message}"
                });
                await Commit();
            }
            return result;
        }

        public async Task<ResultDto> SetEnabled(int id, bool enabled)
        {
            var result = new ResultDto { Message = "" };

            var operatorLogType = enabled ? OperatorLogType.启用Npc : OperatorLogType.禁用Npc;
            try
            {
                var npc = await _npcDomainService.Get(id);
                if (npc == null)
                {
                    result.Message = $"Npc {id} 不存在！";
                    return result;
                }

                npc.IsEnable = enabled;

                await _npcDomainService.Update(npc);

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


        public async Task<List<ScriptEntity>> GetScripts(int npcId)
        {
            var npcScriptIds = (await _npcScriptDomainService.GetAll()).Where(x => x.NpcId == npcId).Select(x => x.ScriptId).ToList();

            return (await _scriptDomainService.GetAll()).Where(x => npcScriptIds.Contains(x.Id)).ToList();
        }


        public async Task<Paging<NpcEntity>> GetPaging(string keyword, int pageIndex)
        {

            var query = await _npcDomainService.GetAll();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Name.Contains(keyword));
            }
            var paging = await query.Paged(pageIndex);
            return paging;
        }

        public async Task<Dictionary<int, List<ScriptEntity>>> GetScripts(List<int> ids)
        {
            var dic = new Dictionary<int, List<ScriptEntity>>();

            var queryNpcScript = await _npcScriptDomainService.GetAll();
            var npcScripts = queryNpcScript.Where(x => ids.Contains(x.NpcId)).ToList();

            var scriptIds = npcScripts.Select(x => x.ScriptId);

            var queryScript = await _scriptDomainService.GetAll();
            var scripts = queryScript.Where(x => scriptIds.Contains(x.Id)).ToList();

            foreach (var id in ids)
            {
                scriptIds = npcScripts.Where(x => x.NpcId == id).Select(x => x.ScriptId);
                var myScripts = scripts.Where(x => scriptIds.Contains(x.Id)).ToList();

                dic.TryAdd(id, myScripts);
            }

            return dic;
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}

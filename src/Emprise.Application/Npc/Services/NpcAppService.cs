using AutoMapper;
using Emprise.Application.Npc.Models;
using Emprise.Domain.Core.Authorization;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Npc.Entity;
using Emprise.Domain.Npc.Services;
using Emprise.Domain.Player.Services;
using Emprise.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Emprise.Application.User.Services
{
    public class NpcAppService : INpcAppService
    {
        private readonly IMediatorHandler _bus;
        private readonly IMapper _mapper;
        private readonly INpcDomainService _npcDomainService;
        private readonly IPlayerDomainService _playerDomainService;
        private readonly IAccountContext _account;
        public NpcAppService(IMediatorHandler bus, IMapper mapper, INpcDomainService npcDomainService, IPlayerDomainService playerDomainService, IAccountContext account)
        {
            _bus = bus;
            _mapper = mapper;
            _npcDomainService = npcDomainService;
            _playerDomainService = playerDomainService;
            _account = account;
        }

        public async Task<NpcEntity> Get(int id)
        {
            return await _npcDomainService.Get(id);
        }

        public async Task<NpcInfo> GetNpc(int id)
        {
            var npcInfo = new NpcInfo()
            {
                Descriptions = new List<string>(), 
                Commands = new List<string>()
            };
            var npc = await _npcDomainService.Get(id);
            if (npc == null)
            {
                return npcInfo;
            }
            npcInfo.Id = id;
            npcInfo.Name = npc.Name;
            string genderStr = npc.Gender.ToGender();

            var player = await _playerDomainService.Get(_account.PlayerId);

            npcInfo.Descriptions.Add(npc.Description??"");
            npcInfo.Descriptions.Add($"{genderStr}{npc.Age.ToAge()}");
            npcInfo.Descriptions.Add($"{genderStr}{npc.Per.ToPer(npc.Age, npc.Gender)}");
            npcInfo.Descriptions.Add($"{genderStr}{npc.Exp.ToKunFuLevel(player.Exp)}");

            if (!string.IsNullOrEmpty(npc.Commands))
            {
                npcInfo.Commands = npc.Commands?.Replace(";", "\r").Replace("|", "\r").Replace(",", "\r").Replace("，", "\r").Replace(" ", "\r").Split('\r').ToList();
            }

            return npcInfo;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}

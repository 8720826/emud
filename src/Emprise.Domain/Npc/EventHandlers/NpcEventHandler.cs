using Emprise.Domain.Core.Enum;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Npc.Entity;
using Emprise.Domain.Npc.Events;
using Emprise.Domain.Quest.Models;
using Emprise.Domain.Quest.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emprise.Domain.Npc.EventHandlers
{
    public  class NpcEventHandler :
        INotificationHandler<EntityUpdatedEvent<NpcEntity>>,
        INotificationHandler<EntityInsertedEvent<NpcEntity>>,
        INotificationHandler<EntityDeletedEvent<NpcEntity>>,
        INotificationHandler<ChatWithNpcEvent>
    {
        private readonly IQuestDomainService _questDomainService;
        private readonly IMudProvider _mudProvider;
        private readonly ILogger<NpcEventHandler> _logger;
        public NpcEventHandler(ILogger<NpcEventHandler> logger, IMudProvider mudProvider, IQuestDomainService questDomainService)
        {
            _logger = logger;
            _mudProvider = mudProvider;
            _questDomainService = questDomainService;
        }

        public async Task Handle(EntityUpdatedEvent<NpcEntity> message, CancellationToken cancellationToken)
        {

        }

        public async Task Handle(EntityInsertedEvent<NpcEntity> message, CancellationToken cancellationToken)
        {
            await Task.Run(() => {

            });
        }

        public async Task Handle(EntityDeletedEvent<NpcEntity> message, CancellationToken cancellationToken)
        {

        }

        public async Task Handle(ChatWithNpcEvent message, CancellationToken cancellationToken)
        {
            var playerId = message.PlayerId;
            var npcId = message.NpcId;


            _logger.LogInformation($"CheckQuest  playerId={playerId},{npcId}");
            var questTriggerCheckModel = new QuestTriggerCheckModel { NpcId = npcId, PlayerId = playerId };
            var quest = await _questDomainService.CheckQuest(QuestTriggerTypeEnum.与Npc对话, questTriggerCheckModel);
            if (quest != null)
            {
                _logger.LogInformation($"CheckQuest questId= {quest.Id}");
                await _mudProvider.ShowMessage(playerId, quest.BeforeCreate);
            }

        }
    }
}

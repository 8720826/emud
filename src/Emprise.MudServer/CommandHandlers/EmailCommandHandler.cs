using AutoMapper;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.CommandHandlers;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Notifications;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emprise.MudServer.Commands.EmailCommands;
using Emprise.Domain.Email.Services;
using Emprise.Domain.Email.Models;
using Newtonsoft.Json;
using Emprise.MudServer.Events.EmailEvents;
using Emprise.Domain.Core.Enums;

namespace Emprise.MudServer.CommandHandlers
{
    
    public class EmailCommandHandler : CommandHandler, 
        IRequestHandler<ShowEmailCommand, Unit>,
        IRequestHandler<DeleteEmailCommand, Unit>,
        IRequestHandler<ReadEmailCommand, Unit>
    {
        private readonly IMediatorHandler _bus;
        private readonly ILogger<EmailCommandHandler> _logger;
        private readonly IEmailDomainService _emailDomainService;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly IMapper _mapper;
        private readonly IMail _mail;
        private readonly IPlayerEmailDomainService _playerEmailDomainService;
        private readonly IRedisDb _redisDb;
        private readonly IMudProvider _mudProvider;


        public EmailCommandHandler(
            IMediatorHandler bus,
            ILogger<EmailCommandHandler> logger,
            IEmailDomainService emailDomainService,
            IHttpContextAccessor httpAccessor,
            IMapper mapper,
            IMail mail,
            IPlayerEmailDomainService playerEmailDomainService,
            IRedisDb redisDb,
            IMudProvider mudProvider,
            INotificationHandler<DomainNotification> notifications,
            IUnitOfWork uow) : base(uow, bus, notifications)
        {

            _bus = bus;
            _logger = logger;
            _emailDomainService = emailDomainService;
            _httpAccessor = httpAccessor;
            _mapper = mapper;
            _mail = mail;
            _playerEmailDomainService = playerEmailDomainService;
            _redisDb = redisDb;
            _mudProvider = mudProvider;
        }

        public async Task<Unit> Handle(ShowEmailCommand command, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Handle ShowEmailCommand:{JsonConvert.SerializeObject(command)}");

            var playerId = command.PlayerId;
            var pageIndex = command.PageIndex;

            var queryEmail = await _emailDomainService.Query();
            var queryPlayerEmail = await _playerEmailDomainService.Query();

            var query = from email in queryEmail
                        join playerEmail in queryPlayerEmail
                              on email.Id equals playerEmail.EmailId
                        where playerEmail.PlayerId == playerId
                        select new PlayerEmailModel { Id = playerEmail.Id, PlayerId = playerEmail.PlayerId, Status = playerEmail.Status, CreateDate = playerEmail.CreateDate.ToFriendlyTime(), EmailId = playerEmail.EmailId, ExpiryDate = playerEmail.ExpiryDate, Title = email.Title, Content = email.Content};


            Paging<PlayerEmailModel> paging = await query.Paged(pageIndex);

            _logger.LogDebug($"Handle ShowEmailCommand Result:{JsonConvert.SerializeObject(paging)},{JsonConvert.SerializeObject(paging.Data)},{paging.Count},{paging.PageCount},{paging.PageIndex}");

            await  _mudProvider.ShowEmail(playerId, paging);

            return Unit.Value;
        }


        public async Task<Unit> Handle(DeleteEmailCommand command, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Handle DeleteEmailCommand:{JsonConvert.SerializeObject(command)}");

            var playerId = command.PlayerId;
            var playerEmailId = command.PlayerEmailId;

            var playerEmail = await _playerEmailDomainService.Get(playerEmailId);
            if(playerEmail==null|| playerEmail.PlayerId!= playerId)
            {
                return Unit.Value;
            }

            playerEmail.Status = EmailStatusEnum.删除;
            await _playerEmailDomainService.Update(playerEmail);

            if(await Commit())
            {
                //TODO
                await _mudProvider.RemoveEmail(playerId, playerEmailId);

                await _bus.RaiseEvent(new DeletedEmailEvent(playerId, playerEmailId)).ConfigureAwait(false);

            }

            return Unit.Value;
        }

        public async Task<Unit> Handle(ReadEmailCommand command, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Handle ReadEmailCommand:{JsonConvert.SerializeObject(command)}");

            var playerId = command.PlayerId;
            var playerEmailId = command.PlayerEmailId;

            var playerEmail = await _playerEmailDomainService.Get(playerEmailId);
            if (playerEmail == null || playerEmail.PlayerId != playerId)
            {
                return Unit.Value;
            }

            playerEmail.Status = EmailStatusEnum.已读;
            await _playerEmailDomainService.Update(playerEmail);

            if (await Commit())
            {
                //TODO
                await _mudProvider.RemoveEmail(playerId, playerEmailId);

                await _bus.RaiseEvent(new DeletedEmailEvent(playerId, playerEmailId)).ConfigureAwait(false);

            }

            return Unit.Value;
        }

        
    }
}

using AutoMapper;
using Emprise.Domain.Core.Bus;
using Emprise.Domain.Core.CommandHandlers;
using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Enums;
using Emprise.Domain.Core.Extensions;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Emprise.Domain.Core.Notifications;
using Emprise.MudServer.Commands;
using Emprise.Domain.User.Entity;
using Emprise.Domain.User.Services;
using Emprise.Infra.Authorization;
using Emprise.Infra.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emprise.MudServer.Events.UserEvents;
using Emprise.MudServer.Commands.EmailCommands;
using Emprise.Domain.Email.Services;
using Emprise.Domain.Email.Models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace Emprise.MudServer.CommandHandlers
{
    
    public class EmailCommandHandler : CommandHandler, 
        IRequestHandler<ShowEmailCommand, Unit>
        
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
                        select new PlayerEmailModel { Id = playerEmail.Id, PlayerId = playerEmail.PlayerId, Status = playerEmail.Status, CreateDate = playerEmail.CreateDate, EmailId = playerEmail.EmailId, ExpiryDate = playerEmail.ExpiryDate, Title = email.Title, Content = email.Content };


            var count = await query.CountAsync();

            bool hasMore = (count - pageIndex * 10 > 0);

            var list = query.Skip(pageIndex - 1).Take(10).ToList();

            _logger.LogDebug($"Handle ShowEmailCommand Result:{JsonConvert.SerializeObject(list)}");

            await  _mudProvider.ShowEmail(playerId, list, hasMore);

            return Unit.Value;
        }

      

    }
}

using Emprise.Domain.Core.Data;
using Emprise.Domain.Core.Entity;
using Emprise.Domain.Core.EventHandlers;
using Emprise.Domain.Core.Events;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.User.Entity;
using Emprise.MudServer.Events.UserEvents;
using MediatR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emprise.MudServer.EventHandlers
{
    public class UserEventHandler : MudEventHandler,
        INotificationHandler<EntityUpdatedEvent<UserEntity>>,
        INotificationHandler<EntityInsertedEvent<UserEntity>>,
        INotificationHandler<EntityDeletedEvent<UserEntity>>,
        INotificationHandler<VisitedEvent>,
        INotificationHandler<SignInEvent>,
        INotificationHandler<SignOutEvent>,
        INotificationHandler<SignUpEvent>


    {
        public const string User = "User_{0}";



        public UserEventHandler(IMudProvider mudProvider,
            IUnitOfWork uow) : base(uow, mudProvider)
        {

        }

        public async Task Handle(EntityUpdatedEvent<UserEntity> message, CancellationToken cancellationToken)
        {

        }

        public async Task Handle(EntityInsertedEvent<UserEntity> message, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {

            });
        }

        public async Task Handle(EntityDeletedEvent<UserEntity> message, CancellationToken cancellationToken)
        {

        }

        public async Task Handle(VisitedEvent message, CancellationToken cancellationToken)
        {

        }



        public async Task Handle(SignInEvent message, CancellationToken cancellationToken)
        {

        }

        public async Task Handle(SignUpEvent message, CancellationToken cancellationToken)
        {


        }


        public async Task Handle(SignOutEvent message, CancellationToken cancellationToken)
        {

        }

    }
}

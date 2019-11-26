using Emprise.Domain.Core.Entity;
using Emprise.Domain.Core.Events;
using Emprise.Domain.User.Entity;
using Emprise.Domain.User.Events;
using MediatR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emprise.Domain.User.EventHandlers
{
    public class UserEventHandler :
        INotificationHandler<EntityUpdatedEvent<UserEntity>>,
        INotificationHandler<EntityInsertedEvent<UserEntity>>,
        INotificationHandler<EntityDeletedEvent<UserEntity>>,
        INotificationHandler<VisitedEvent>
    /*
    INotificationHandler<SignInEvent>,
    INotificationHandler<SignOutEvent>,
    INotificationHandler<SignUpEvent>,
    INotificationHandler<UserInRoomEvent>,
    INotificationHandler<UserOutRoomEvent>*/
    {
        public const string User = "User_{0}";



        public UserEventHandler()
        {

        }

        public async Task Handle(EntityUpdatedEvent<UserEntity> message, CancellationToken cancellationToken)
        {

        }

        public async Task Handle(EntityInsertedEvent<UserEntity> message, CancellationToken cancellationToken)
        {
            await Task.Run(() => {
               
            });
        }

        public async Task Handle(EntityDeletedEvent<UserEntity> message, CancellationToken cancellationToken)
        {

        }

        public async Task Handle(VisitedEvent message, CancellationToken cancellationToken)
        {
            //System.IO.File.AppendAllText("d:/1.txt", $"{DateTime.Now}\n");
        }

        /*
        public async Task Handle(SignInEvent message, CancellationToken cancellationToken)
        {

        }

        public async Task Handle(SignUpEvent message, CancellationToken cancellationToken)
        {


        }


        public async Task Handle(SignOutEvent message, CancellationToken cancellationToken)
        {

        }


        public async Task Handle(UserInRoomEvent message, CancellationToken cancellationToken)
        {


        }

        public async Task Handle(UserOutRoomEvent message, CancellationToken cancellationToken)
        {

        }*/
    }
}

using Emprise.Domain.Core.Events;
using Emprise.Domain.User.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Events.UserEvents
{
    public class VerifyEmailEvent : Event
    {
        public UserEntity User { get; set; }


        public VerifyEmailEvent(UserEntity user)
        {
            User = user;
        }

    }
}

using Emprise.Domain.Core.Entity;
using Emprise.Domain.Core.Events;
using Emprise.Domain.User.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.User.Events
{
    public class SignOutEvent : Event
    {
        public UserEntity User { get; set; }


        public SignOutEvent(UserEntity user)
        {
            User = user;
        }
      
    }
}

using Emprise.Domain.Core.Entity;
using Emprise.Domain.Core.Events;
using Emprise.Domain.User.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.User.Events
{
    public class SignUpEvent : Event
    {
        public UserEntity User { get; set; }


        public SignUpEvent(UserEntity user)
        {
            User = user;
        }
      
    }
}

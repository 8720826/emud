using System;
using MediatR;

namespace Emprise.Domain.Core.Events
{
    public abstract class Message : IRequest
    {
        public string MessageType { get; protected set; }


        protected Message()
        {
            MessageType = GetType().Name;
        }
    }
}
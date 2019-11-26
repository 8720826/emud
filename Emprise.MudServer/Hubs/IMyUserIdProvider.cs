using Emprise.Domain.Core.Interfaces.Ioc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Hubs
{
    public interface IMyUserIdProvider: IUserIdProvider, ISingleton
    {
    }
}

using Emprise.Domain.Core.Bus.Models;
using Emprise.Domain.Core.Interfaces.Ioc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.MudServer.Handles
{
    public interface IExertHandler : IScoped
    {
        Task Execute(int playerId, ExertModel model);
    }
}

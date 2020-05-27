using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Emprise.MudServer.Hubs.Models;
using Emprise.Domain.Player.Models;
using Emprise.MudServer.Models;

namespace Emprise.MudServer.Hubs
{
    public interface IHubClient
    {
        Task ShowMessage(Message message);

        Task Offline();

    }
}

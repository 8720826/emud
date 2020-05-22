using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Emprise.Admin.Api
{
    public interface IMudClient
    {

        [Post("/api/event/room/edit")]
        Task<HttpResponseMessage> EditRoom(int id);


        [Post("/api/event/npc/edit")]
        Task<HttpResponseMessage> EditNpc(int id);
    }
}

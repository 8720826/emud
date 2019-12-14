using Emprise.Domain.Npc.Entity;
using Emprise.Domain.Player.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.MudServer.Scripts
{
    public  class BaseScript
    {
        protected PlayerEntity _user;
        protected NpcEntity _npc;

        public BaseScript(NpcEntity npc, PlayerEntity user)
        {
            _user = user;
            _npc = npc;
        }

        public List<string> Words { get; set; }

        public List<string> Actions { get; set; }

        public async Task<List<string>> GetActions()
        {
            return await Task.FromResult(Actions);
        }

        public async Task DoAction(string action, string input="")
        {
             await Task.CompletedTask;
        }
    }
}

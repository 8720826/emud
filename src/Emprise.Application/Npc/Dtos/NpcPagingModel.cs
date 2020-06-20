using Emprise.Domain.Npc.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Application.Npc.Dtos
{
    public class NpcPagingModel 
    {
        public NpcEntity Npc { get; set; }
        public List<NpcScriptEntity> NpcScripts { get; set; }
    }
}

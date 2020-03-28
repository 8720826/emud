using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Application.Npc.Models
{
    public class NpcInfo
    {
        public int Id { set; get; }

        /// <summary>
        /// 角色名
        /// </summary>
        public string Name { set; get; }


        /// <summary>
        /// 描述
        /// </summary>
        public List<string> Descriptions { set; get; }

        /// <summary>
        /// 操作
        /// </summary>
        public List<NpcAction> Actions { set; get; }
    }

    public class NpcAction
    {
        public int CommandId { set; get; }

        public string Name { set; get; }

        public int ScriptId { set; get; }
   
    }
}

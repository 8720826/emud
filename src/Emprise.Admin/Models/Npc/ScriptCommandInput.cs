using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Admin.Models.NpcScript
{
    public class ScriptCommandInput
    {
        /// <summary>
        /// 是否入口，入口将显示在Npc资料页
        /// </summary>
        public bool IsEntry { set; get; }

        /// <summary>
        /// 排序
        /// </summary>
        public int SortId { set; get; }

        /// <summary>
        /// 分支名称
        /// </summary>
        public string Name { set; get; }


        public string Description { set; get; }

        public string CaseIf { set; get; }


        public string CaseThen { set; get; }

        public string CaseElse { set; get; }

        public int ScriptId { set; get; }
    }
}

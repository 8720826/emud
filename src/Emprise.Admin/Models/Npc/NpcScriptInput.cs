using Emprise.Domain.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Admin.Models.Script
{
    public class NpcScriptInput
    {
        /// <summary>
        /// 脚本名称
        /// </summary>
        public string Name { set; get; }

        public string ActionName { set; get; }

        /// <summary>
        /// 默认对话
        /// </summary>
        public List<string> InitWords { set; get; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { set; get; }
    }
}

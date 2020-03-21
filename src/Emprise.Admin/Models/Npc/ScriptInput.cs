using Emprise.Domain.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Admin.Models.Script
{
    public class ScriptInput
    {
        /// <summary>
        /// 脚本名称
        /// </summary>
        public string Name { set; get; }

        public string Description { set; get; }


        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { set; get; }
    }
}

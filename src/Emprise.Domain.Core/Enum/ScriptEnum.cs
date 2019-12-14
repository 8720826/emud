using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Domain.Core.Enum
{
    public enum ScriptTypeEnum
    {
        /// <summary>
        /// Npc脚本
        /// </summary>
        Npc脚本 = 1,

        /// <summary>
        /// 技能脚本
        /// </summary>
        技能脚本 = 2,


        /// <summary>
        /// 物品脚本
        /// </summary>
        物品脚本 = 3,
    }

    public enum BranchTypeEnum
    {
        /// <summary>
        /// Npc脚本
        /// </summary>
        如果 = 1,

        /// <summary>
        /// 技能脚本
        /// </summary>
        技能脚本 = 2,


        /// <summary>
        /// 物品脚本
        /// </summary>
        物品脚本 = 3,
    }

    public enum RelateTypeEnum
    {
        无 = 0,
        关联Id = 1,
        关联名称 = 2
    }

    public enum CaseTypeEnum
    {
        升级 = 1,

        获得经验 = 2,

        移动 = 3,

        拥有物品 = 4,

        完成任务 = 5,

        与Npc对话 = 6,


        杀死Npc = 7,

        探索房间 = 8,



        获得金钱 = 9
    }
}

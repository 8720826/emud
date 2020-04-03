using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Domain.Core.Enum
{
    /// <summary>
    /// 条件类型
    /// </summary>
    public enum ConditionTypeEnum
    {
        /// <summary>
        /// 判断player的属性值，关联 （属性字段、逻辑关系、数值）
        /// </summary>
        角色属性 = 1,

        /// <summary>
        /// 判断拥有某件物品数量，关联 （物品名、逻辑关系、数量）
        /// </summary>
        拥有物品 = 2,

        /// <summary>
        /// 判断是否完成对应任务，关联 （任务id）
        /// </summary>
        完成任务 = 3,

        /// <summary>
        /// 判断近期是否进行某个操作，关联 （活动类型、相关id、数量）
        /// </summary>
        活动记录 = 4
    }


    /// <summary>
    /// 角色属性 字段
    /// </summary>
    public enum PlayerConditionFieldEnum
    {
        等级 = 1, 
        经验 = 2, 
        金钱 = 3,
        年龄=4,
        先天臂力 = 5,
        先天根骨 = 6,
        先天悟性 = 7,
        先天身法 = 8,
        后天臂力 = 9,
        后天根骨 = 10,
        后天悟性 = 11,
        后天身法 = 12
    }


    /// <summary>
    /// 逻辑关系
    /// </summary>
    public enum LogicalRelationTypeEnum
    {
        等于 = 1,
        不等于 = 2,
        大于 = 3,
        大于等于 = 4,
        小于 = 5,
        小于等于 = 6
    }

    /// <summary>
    /// 玩家活动类型
    /// </summary>
    public enum PlayerEventTypeEnum
    {
        /// <summary>
        /// 关联NpcId
        /// </summary>
        对话Npc = 1,

        /// <summary>
        /// 关联RoomId
        /// </summary>
        移动 = 2,

        /// <summary>
        /// 关联RoomId
        /// </summary>
        探索 = 3,

        /// <summary>
        /// 关联NpcId
        /// </summary>
        杀死Npc = 4,

        /// <summary>
        /// 无关联Id
        /// </summary>
        聊天 = 5
    }

    public enum CommandTypeEnum
    {
        播放对话 =1,
        对话选项 = 2,
        输入选项 = 3,
        跳转到分支 = 4
    }




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

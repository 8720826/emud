using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Core.Enum
{
    public enum NpcStatusEnum
    {
        /// <summary>
        /// 正常
        /// </summary>
        正常 = 0,

        /// <summary>
        /// 死亡
        /// </summary>
        死亡 = 1,

    }

    /// <summary>
    /// Npc类型
    /// </summary>
    public enum NpcTypeEnum
    {
        /// <summary>
        /// 无
        /// </summary>
        无 = 0,

        /// <summary>
        /// 人物
        /// </summary>
        人物 = 11,

        /// <summary>
        /// 怪物
        /// </summary>
        怪物 = 12,

        /// <summary>
        /// 倭寇
        /// </summary>
        倭寇 = 13,

        /// <summary>
        /// 世外高人
        /// </summary>
        世外高人 = 14,

        /// <summary>
        /// 门派守卫
        /// </summary>
        守卫 = 21,

        /// <summary>
        /// 镖车
        /// </summary>
        镖车 = 22,


        /// <summary>
        /// 劫匪
        /// </summary>
        劫匪 = 23,

        /// <summary>
        /// 宝藏守护者
        /// </summary>
        守护者 = 24,

        /// <summary>
        /// 木头人
        /// </summary>
        木头人 = 31,

        /// <summary>
        /// 毒物
        /// </summary>
        毒物 = 41,
    }
}

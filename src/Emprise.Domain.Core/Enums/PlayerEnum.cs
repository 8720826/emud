using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Core.Enums
{
    public enum PlayerStatusEnum
    {
        空闲 = 0,
        打工 = 1,
        打坐 = 2,
        疗伤 = 3,
        伐木 = 4,
        钓鱼 = 5,
        挖矿 = 6,
        采药 = 7,
        打猎 = 8,
        战斗 = 9,
        修练 =10
    }


    public enum WorkTypeEnum
    {
        打工 = 1,
        伐木 = 4,
        钓鱼 = 5,
        挖矿 = 6,
        采药 = 7,
        打猎 = 8,
    }


    public enum DropTypeEnum
    {
        打工 = 1,
        伐木 = 4,
        钓鱼 = 5,
        挖矿 = 6,
        采药 = 7,
        打猎 = 8,
    }

    public enum PlayerActionEnum
    {
        //闲聊 = 1,
        //给予 = 2,
        切磋 = 3,
        杀死 = 4,
        添加好友 = 5,
        拜师 = 6,
        割袍断义 =7,
        查看武功=8
    }
}

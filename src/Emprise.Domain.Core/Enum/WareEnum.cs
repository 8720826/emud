using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Core.Enum
{
    public enum WareCategoryEnum
    {
        丹药 = 101,//增加体力内力生命
        武器 = 102,//装备后增加攻击、防御
        服装 = 103,
        //暗器 = 4,//用来减少对方生命值
        宝物 = 105,//从商城购买，特殊用途
        材料 = 106,//挖矿、伐木、采药等获得
        //宝石 = 7,//由原材料合成，可以增加武器属性
        宝箱 = 108, //宝箱内由若干其他物品
        秘籍 = 109, //武功秘籍

        碎片 = 110,


        //工具 = 111,//用来挖矿、伐木、采药等
        //毒药=112
    }


    public enum WareTypeEnum
    {
        刀 = 102001,
        剑 = 102002,
        枪 = 102003,



        衣服 = 103001,
        帽 = 103002,
        鞋 = 103003,


        秘籍残卷 =110001,

    }

    public enum WareStatusEnum
    {
        装备 = 1,
        卸下 = 0,
        寄售 = 2
    }

    public enum WareEffectEnum
    {
        攻击 = 1,
        防御 = 2,
        内力 = 3,
        气血 = 4,

    }
}

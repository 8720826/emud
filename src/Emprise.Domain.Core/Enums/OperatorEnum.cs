using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Core.Enums
{
    public enum OperatorLogType
    {
        登录,
        退出登录,

        修改配置,

        添加地图,
        修改地图,
        删除地图,

        清除1年前记录,
        清除半年前记录,
        清除3个月前记录,
        清除1个月前记录,

        添加Npc,
        修改Npc,
        删除Npc,
        复制Npc,

        修改用户,
        修改玩家,
        删除玩家,

        添加任务,
        修改任务,
        删除任务,

        添加房间,
        修改房间,
        删除房间,

        添加脚本,
        修改脚本,
        删除脚本,

        添加脚本分支,
        修改脚本分支,
        删除脚本分支,

        添加物品,
        修改物品,
        删除物品,
        复制物品,

        清除1年前日志,
        清除半年前日志,
        清除3个月前日志,
        清除1个月前日志,
    }
}

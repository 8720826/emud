using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Core.Enums
{

    public enum QuestTypeEnum
    {
        新手 = 1,

        主线 = 2,

        支线 = 3,

        日常 = 4
    }



    /// <summary>
    /// 任务周期
    /// </summary>
    public enum QuestPeriodEnum
    {
        不可重复 = 1,

        每天一次 = 2,

        每周一次 = 3,

        每月一次 = 4,

        每年一次 = 5,

        无限制 =6
    }


    /// <summary>
    /// 领取方式
    /// </summary>
    public enum QuestTakeTypeEnum
    {
        /// <summary>
        /// 通常通过command命令等实现
        /// </summary>
        手动领取 = 1,


        自动领取 = 2
    }


    /// <summary>
    /// 交付方式
    /// </summary>
    public enum QuestDeliverTypeEnum
    {
        /// <summary>
        /// 通常通过command命令等实现
        /// </summary>
        手动交付 = 1,


        自动交付 = 2
    }

    /// <summary>
    /// 任务触发方式，与QuestTriggerConditionEnum对应
    /// </summary>
    public enum QuestTriggerTypeEnum 
    {
        无 = 0,//此项为任务不会自动触发

        升级 = 1,

        获得经验 = 2,

        移动 = 3,

        获得物品 = 4,

        完成任务 = 5,

        与Npc对话 = 6,


        杀死Npc = 7,

        探索房间 = 8,



        获得金钱 = 9
    }



    /// <summary>
    /// 任务触发条件，与QuestTriggerTypeEnum对应
    /// </summary>
    public enum QuestTriggerConditionEnum
    {
        /// <summary>
        /// 升级时触发
        /// </summary>
        角色等级达到 = 1,

        /// <summary>
        /// 获得经验时触发
        /// </summary>
        角色经验值达到 = 2,

        /// <summary>
        /// 移动后触发
        /// </summary>
        所在房间 = 3,

        /// <summary>
        /// 获得物品时触发
        /// </summary>
        拥有某件物品达到数量 = 4,

        /// <summary>
        /// 必须完成该任务才能被触发
        /// </summary>
        完成前置任务 = 5,

        与某个Npc对话 = 6,


        杀死某个Npc = 7,

        探索某个房间 = 8,

        拥有金钱 = 9
    }


    /// <summary>
    /// 任务领取条件
    /// </summary>
    public enum QuestTakeConditionEnum
    {
        /// <summary>
        /// 角色等级达到
        /// </summary>
        角色等级达到 = 1,

        /// <summary>
        /// 角色经验值达到
        /// </summary>
        角色经验值达到 = 2,

        /// <summary>
        /// 所在房间
        /// </summary>
        所在房间 = 3,

        /// <summary>
        /// 拥有某件物品达到数量
        /// </summary>
        拥有某件物品达到数量 = 4,

        /// <summary>
        /// 完成前置任务
        /// </summary>
        完成前置任务 = 5,

        与某个Npc对话 = 6,


        杀死某个Npc = 7,

        探索某个房间 = 8,

        拥有金钱 = 9
    }

    /// <summary>
    /// 任务目标，完成条件
    /// </summary>
    public enum QuestTargetEnum
    {

        /// <summary>
        /// 升级时触发
        /// </summary>
        角色等级达到 = 1,

        /// <summary>
        /// 获得经验时触发
        /// </summary>
        角色经验值达到 = 2,

        /// <summary>
        /// 移动后触发
        /// </summary>
        所在房间 = 3,

        /// <summary>
        /// 获得物品时触发
        /// </summary>
        拥有某件物品达到数量 = 4,


        与某个Npc对话 = 5,


        杀死某个Npc = 6,

        探索某个房间 =7,

    }

    /// <summary>
    /// 任务奖励
    /// </summary>
    public enum QuestRewardEnum
    {
        金钱 = 1,
        经验 = 2,
        物品 = 3
    }

    public enum QuestConsumeEnum
    {
        金钱 = 1,
        经验 = 2,
        物品 = 3
    }
    


    /// <summary>
    /// 任务状态
    /// </summary>
    public enum QuestStateEnum
    {
        未领取 = 0,
        进行中 = 1,
        已失败 = 2,
        完成未领奖 = 3,
        完成已领奖 = 4
    }
}

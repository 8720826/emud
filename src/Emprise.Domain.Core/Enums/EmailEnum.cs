using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Core.Enums
{
    /// <summary>
    /// 通知类型
    /// </summary>
    public enum EmailTypeEnum
    {
        /// <summary>
        /// 系统发送给所有人
        /// </summary>
        公告 = 0,

        /// <summary>
        /// 系统发送给单个人
        /// </summary>
        系统 = 1,

        /// <summary>
        /// 个人发送给个人
        /// </summary>
        私信 = 2,

        /// <summary>
        /// 发送给门派
        /// </summary>
        帮派 = 3,
    }


    /// <summary>
    /// 通知状态
    /// </summary>
    public enum EmailStatusEnum
    {
        未读 = 0,
        已读 = 1,
        删除 = 2
    }
}


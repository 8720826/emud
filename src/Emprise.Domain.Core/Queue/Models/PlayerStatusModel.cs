using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Core.Queue.Models
{
    public class PlayerStatusModel
    {
        public int PlayerId { get; set; }

        /// <summary>
        /// 玩家状态
        /// </summary>
        public PlayerStatusEnum Status { get; set; }
    }
}

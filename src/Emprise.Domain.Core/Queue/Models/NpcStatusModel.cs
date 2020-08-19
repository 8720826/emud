using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Core.Queue.Models
{
    public class NpcStatusModel
    {
        public int PlayerId { get; set; }

        /// <summary>
        /// Npc状态
        /// </summary>
        public NpcStatusEnum Status { get; set; }

        public TargetTypeEnum? TargetType { get; set; }

        public int TargetId { get; set; }
    }
}

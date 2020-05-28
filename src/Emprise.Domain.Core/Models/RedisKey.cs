using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Core.Models
{
    public class RedisKey
    {
        /// <summary>
        /// chatWithNpc_{playerId}_{npcId}
        /// </summary>
        public const string ChatWithNpc = "chatWithNpc_{0}_{1}";

        /// <summary>
        /// completeQuest_{playerId}_{questId}
        /// </summary>
        public const string CompleteQuest = "completeQuest_{0}_{1}";

        /// <summary>
        /// commandIds_{playerId}_{npcId}_{scriptId}
        /// </summary>
        public const string CommandIds = "commandIds_{0}_{1}_{2}";

        /// <summary>
        /// regemail_{email}
        /// </summary>
        public const string RegEmail = "regEmail_{0}";

        /// <summary>
        /// resetPassword_{userId}
        /// </summary>
        public const string ResetPassword = "resetPassword_{0}";
        

    }
}

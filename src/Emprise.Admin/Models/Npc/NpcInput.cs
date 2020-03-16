using Emprise.Domain.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Admin.Models.Npc
{
    public class NpcInput
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { set; get; }


        /// <summary>
        /// 描述
        /// </summary>
        public string Description { set; get; }


        /// <summary>
        /// 性别
        /// </summary>
        public GenderEnum Gender { set; get; }

        /// <summary>
        /// 类型
        /// </summary>
        public NpcTypeEnum Type { set; get; }

        /// <summary>
        /// 所在房间
        /// </summary>
        public int RoomId { set; get; }

        /// <summary>
        /// 是否可以攻击
        /// </summary>
        public bool CanFight { set; get; }


        /// <summary>
        /// 是否可以移动
        /// </summary>
        public bool CanMove { set; get; }

        /// <summary>
        /// 是否可以杀死
        /// </summary>
        public bool CanKill { set; get; }


        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { set; get; }

        /// <summary>
        /// 容貌
        /// </summary>
        public int Per { set; get; }

        /// <summary>
        /// 实战经验
        /// </summary>
        public int Exp { set; get; }

        /// <summary>
        /// 脚本
        /// </summary>
        public int ScriptId { set; get; }


        /// <summary>
        /// 脚本名
        /// </summary>
        public string ScriptName { set; get; }
    }
}

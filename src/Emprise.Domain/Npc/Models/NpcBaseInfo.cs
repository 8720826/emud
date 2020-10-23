using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Player.Models
{

    public class NpcBaseInfo
    {
        public int Id { set; get; }

        /// <summary>
        /// 角色名
        /// </summary>
        public string Name { set; get; }

        public GenderEnum Gender { set; get; }

        public int Level { set; get; }




    }
}

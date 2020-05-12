using Emprise.Domain.Core.Commands;
using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Player.Commands
{
    public class CreateCommand : Command
    {
        public int UserId { get; set; }
        public string Name { get; set; }

        public GenderEnum Gender { get; set; }

        /// <summary>
        /// 先天臂力（Strength）
        /// </summary>
        public int Str { set; get; }


        /// <summary>
        /// 根骨（Constitution）
        /// </summary>
        public int Con { set; get; }


        /// <summary>
        /// 悟性（Intelligence）
        /// </summary>
        public int Int { set; get; }


        /// <summary>
        /// 身法（Dexterity）
        /// </summary>
        public int Dex { set; get; }

        public CreateCommand(string name, GenderEnum gender, int userId, int str, int con, int dex, int @int)
        {
            Name = name;
            Gender = gender;
            UserId = userId;
            Str = str;
            Con = con;
            Dex = dex;
            Int = @int;
        }

    }
}

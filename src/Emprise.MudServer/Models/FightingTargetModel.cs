using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Models
{
    public class FightingTargetModel
    {
        public int TargetId { get; set; }

        public TargetTypeEnum? TargetType { get; set; }

        public string TargetName { get; set; }

        public int Hp { get; set; }

        public int Mp { get; set; }

        public int MaxHp { get; set; }

        public int MaxMp { get; set; }
    }
}

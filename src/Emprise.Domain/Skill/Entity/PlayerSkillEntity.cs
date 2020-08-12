using Emprise.Domain.Core.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Emprise.Domain.Skill.Entity
{
    [Table("PlayerSkill")]
    public class PlayerSkillEntity : BaseEntity
    {
        public int PlayerId { get; set; }

        public int SkillId { get; set; }

        public string SkillName { get; set; }

        public int Level { get; set; }

        public int Exp { get; set; }
    }
}

using Emprise.Domain.Core.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Admin.Models.Npc
{
    public class NpcInput
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Display(Name = "名称")]
        [Required(ErrorMessage = "请填写Npc名称")]
        [StringLength(20, ErrorMessage = "最长不能超过20个字符")]
        public string Name { set; get; }


        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "描述")]
        [Required(ErrorMessage = "请填写Npc描述")]
        [StringLength(500, ErrorMessage = "最长不能超过500个字符")]
        public string Description { set; get; }


        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        [Required(ErrorMessage = "请选择性别")]
        public GenderEnum Gender { set; get; }

        /// <summary>
        /// 类型
        /// </summary>
        [Display(Name = "类型")]
        [Required(ErrorMessage = "请选择类型")]
        public NpcTypeEnum Type { set; get; }

        /// <summary>
        /// 所在房间
        /// </summary>
        [Display(Name = "所在房间")]
        [Required(ErrorMessage = "请填写所在房间Id")]
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
        [Display(Name = "年龄")]
        [Required(ErrorMessage = "请填写年龄")]
        public int Age { set; get; }

        /// <summary>
        /// 容貌
        /// </summary>
        [Display(Name = "容貌")]
        [Required(ErrorMessage = "请填写容貌值")]
        public int Per { set; get; }

        /// <summary>
        /// 实战经验
        /// </summary>
        [Display(Name = "经验")]
        [Required(ErrorMessage = "请填写实战经验值")]
        public int Exp { set; get; }


        /// <summary>
        /// 是否启用
        /// </summary>
        [Display(Name = "是否启用")]
        [Required(ErrorMessage = "请选择是否启用")]
        public bool IsEnable { set; get; }
    }
}

using Emprise.Domain.Core.Enum;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Emprise.Application.Player.Dtos
{
    public class PlayerCreateDto
    {
        /// <summary>
        /// 角色名
        /// </summary>
        [RegularExpression("^[\u4e00-\u9fbb]{0,}$", ErrorMessage = "角色名必须为中文")]
        [MinLength(2, ErrorMessage = "角色名最少为2个字")]
        [MaxLength(4, ErrorMessage = "角色名最多为4个字")]

        [Required(ErrorMessage = "请输入角色名")]
        public string Name { set; get; }

        /// <summary>
        /// 性别
        /// </summary>
        [Required(ErrorMessage = "请选择性别")]
        public GenderEnum Gender { set; get; }


        /// <summary>
        /// 先天臂力（Strength）
        /// </summary>
        [Range(15,30, ErrorMessage = "你的先天臂力超出了15-30范围，请重新选择。")]
        public int Str { set; get; }


        /// <summary>
        /// 根骨（Constitution）
        /// </summary>
        [Range(15, 30, ErrorMessage = "你的先天根骨超出了15-30范围，请重新选择。")]
        public int Con { set; get; }


        /// <summary>
        /// 悟性（Intelligence）
        /// </summary>
        [Range(15, 30, ErrorMessage = "你的先天悟性超出了15-30范围，请重新选择。")]
        public int Int { set; get; }


        /// <summary>
        /// 身法（Dexterity）
        /// </summary>
        [Range(15, 30, ErrorMessage = "你的先天身法超出了15-30范围，请重新选择。")]
        public int Dex { set; get; }
    }
}

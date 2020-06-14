using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Application.Map.Dtos
{
    public class MapInput
    {
        /// <summary>
        /// 地图名
        /// </summary>
        [Display(Name = "名称")]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(32,ErrorMessage ="{0}长度最大为{1}字符")]

        public string Name { set; get; }

        /// <summary>
        /// 地图说明
        /// </summary>
        [Display(Name = "描述")]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(500, ErrorMessage = "{0}长度最大为{1}字符")]
        public string Description { set; get; }
    }
}

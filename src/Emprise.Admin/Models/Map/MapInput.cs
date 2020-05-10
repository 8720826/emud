using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Admin.Models.Map
{
    public class MapInput
    {
        /// <summary>
        /// 地图名
        /// </summary>
        [Display(Name = "名称")]
        [Required(ErrorMessage = "请输入地图名称")]
        [StringLength(32,ErrorMessage ="长度最大为32字符")]

        public string Name { set; get; }

        /// <summary>
        /// 地图说明
        /// </summary>
        [Display(Name = "描述")]
        [Required(ErrorMessage = "描述")]
        [StringLength(500, ErrorMessage = "长度最大为500字符")]
        public string Description { set; get; }
    }
}

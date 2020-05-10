using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Admin.Models.Room
{
    public class RoomInput
    {

        /// <summary>
        /// 房间名
        /// </summary>
        [Display(Name = "房间名")]
        [Required(ErrorMessage = "请输入房间名")]
        public string Name {  get; set; }


        /// <summary>
        /// 东
        /// </summary>
        public int East { get; set; }

        public string EastName { get; set; }

        /// <summary>
        /// 西
        /// </summary>
        public int West { get; set; }

        public string WestName { get; set; }

        /// <summary>
        /// 南
        /// </summary>
        public int South { get; set; }

        public string SouthName { get; set; }

        /// <summary>
        /// 北
        /// </summary>
        public int North { get; set; }

        public string NorthName { get; set; }

        /// <summary>
        /// 房间说明
        /// </summary>
        [Display(Name = "描述")]
        [Required(ErrorMessage = "请输入描述")]
        [StringLength(500)]
        public string Description {  get; set; }

        /// <summary>
        /// 是否可以战斗
        /// </summary>
        public bool CanFight {  get; set; }

        /// <summary>
        /// 是否可以挖矿
        /// </summary>
        public bool CanDig {  get; set; }

        /// <summary>
        /// 是否可以伐木
        /// </summary>
        public bool CanCut {  get; set; }

        /// <summary>
        /// 是否可以钓鱼
        /// </summary>
        public bool CanFish {  get; set; }

        /// <summary>
        /// 是否可以采药
        /// </summary>
        public bool CanCollect {  get; set; }

        /// <summary>
        /// 是否可以打猎
        /// </summary>
        public bool CanHunt {  get; set; }

    }
}

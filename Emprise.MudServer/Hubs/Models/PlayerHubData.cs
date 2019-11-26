using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Hubs.Models
{
    /// <summary>
    /// 接收 - 个人的信息
    /// </summary>
    public  class PlayerHubData : HubData
    {

        /// <summary>
        /// 接收者
        /// </summary>
        public int PlayerId { set; get; }

    }
}

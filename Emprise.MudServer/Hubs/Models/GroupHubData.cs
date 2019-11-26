using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Hubs.Models
{
    /// <summary>
    /// 客户端接收到的消息
    /// </summary>
    public class GroupHubData : HubData
    {

        /// <summary>
        /// 接收者
        /// </summary>
        public string GroupId { set; get; }


    }

}

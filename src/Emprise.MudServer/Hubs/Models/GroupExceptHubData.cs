using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Hubs.Models
{
    /// <summary>
    /// 客户端接收到的消息
    /// </summary>
    public class GroupExceptHubData : HubData
    {

        /// <summary>
        /// 接收GroupId
        /// </summary>
        public string GroupId { set; get; }

        /// <summary>
        /// 不接收消息者PlayerId
        /// </summary>
        public int PlayerId { set; get; }

    }

}

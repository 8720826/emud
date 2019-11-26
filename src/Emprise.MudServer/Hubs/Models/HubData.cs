using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Hubs.Models
{
    /// <summary>
    /// 客户端接收到的消息
    /// </summary>
    public class HubData
    {
        /// <summary>
        /// 方法名
        /// </summary>
        public string Action { set; get; }


        /// <summary>
        /// 消息内容
        /// </summary>
        public object Data { set; get; }
    }

}

using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Hubs.Models
{
    /// <summary>
    /// 接收到的消息
    /// </summary>
    public  class PlayerMessage: Message
    {
        /// <summary>
        /// 发送者id
        /// </summary>
        public int PlayerId { set; get; }

        /// <summary>
        /// 发送者
        /// </summary>
        public string Sender { set; get; }

        /// <summary>
        /// 频道
        /// </summary>
        public string Channel { set; get; }


    }
}

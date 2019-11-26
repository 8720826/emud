using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Hubs.Models
{
    /// <summary>
    /// 消息
    /// </summary>
    public class Message
    {
        /// <summary>
        /// 时间
        /// </summary>
        public string PostDate { get { return DateTime.Now.ToString("HH:mm:ss"); } }

        /// <summary>
        /// 内容
        /// </summary>

        public string Content { set; get; }
    }
}

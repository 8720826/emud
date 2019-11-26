using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Hubs.Actions
{
    public class SendAction
    {

        /// <summary>
        /// 频道(默认闲聊)
        /// </summary>
        public string Channel { set; get; }

        /// <summary>
        /// 内容
        /// </summary>

        public string Content { set; get; }
    }
}

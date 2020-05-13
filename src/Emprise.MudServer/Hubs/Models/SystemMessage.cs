using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Hubs.Models
{
    /// <summary>
    /// 接收到的消息
    /// </summary>
    public  class SystemMessage: Message
    {
        public MessageTypeEnum Type { get; set; }
    }
}

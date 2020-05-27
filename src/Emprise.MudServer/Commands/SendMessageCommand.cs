using Emprise.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Commands
{
    public class SendMessageCommand : Command
    {
        /// <summary>
        /// 频道(默认闲聊)
        /// </summary>
        public string Channel { set; get; }

        /// <summary>
        /// 内容
        /// </summary>

        public string Content { set; get; }

        public int PlayerId { get; set; }


        public SendMessageCommand(int playerId, string channel, string content)
        {
            Channel = channel;
            Content = content;
            PlayerId = playerId;
        }

    }
}

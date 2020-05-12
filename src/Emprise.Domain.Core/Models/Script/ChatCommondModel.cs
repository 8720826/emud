using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Core.Models.Script
{
    public class ChatCommandModel: BaseCommandModel
    {
        public new CommandTypeEnum Type { get; } = CommandTypeEnum.播放对话;

        public string Content { get; set; }
    }
}

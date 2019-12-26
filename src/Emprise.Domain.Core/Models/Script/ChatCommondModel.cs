using Emprise.Domain.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Core.Models.Script
{
    public class ChatCommondModel: BaseCommondModel
    {
        public new CommondTypeEnum Type { get; } = CommondTypeEnum.播放对话;

        public string Content { get; set; }
    }
}

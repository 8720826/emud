using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Hubs.Actions
{
    public  class QuestAction
    {

        public QuestTypeEnum Type { get; set; }
    }

    public class QuestDetailAction
    {
        public int QuestId { get; set; }

    }
}

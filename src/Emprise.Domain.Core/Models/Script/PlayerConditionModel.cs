using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Core.Models.Script
{
    public class PlayerConditionModel: BaseConditionModel
    {
        public PlayerConditionFieldEnum Field { get; set; }

        public LogicalRelationTypeEnum Relation { get; set; }

        public object Value { get; set; }
    }
}

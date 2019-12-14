using Emprise.Domain.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Admin.Models
{
    public class TriggerConditionCaseModel : BaseCaseModel
    {
        public TaskTriggerConditionEnum Type { set; get; }
    }
}

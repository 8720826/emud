using Emprise.Domain.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Admin.Models
{
    public class TriggerTypeCaseModel : BaseCaseModel
    {
        public TaskTriggerTypeEnum Type { set; get; }
    }
}

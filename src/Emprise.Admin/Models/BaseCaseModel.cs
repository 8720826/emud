using Emprise.Domain.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Admin.Models
{
    public class BaseCaseModel
    {
        public RelateTypeEnum RelateType { set; get; }

        public int RelateId { set; get; }

        public string RelateName { set; get; }

        public int RelateValue { set; get; }
    }
}

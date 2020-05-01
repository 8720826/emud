using Emprise.Domain.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Core.Attributes
{

    public class WareEffectFieldAttribute : Attribute
    {
        public WareEffectEnum effectEnum { get; set; }
        public WareEffectFieldAttribute(WareEffectEnum effectEnum)
        {

        }
    }
}

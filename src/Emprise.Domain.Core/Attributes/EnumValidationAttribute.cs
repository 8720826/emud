using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Emprise.Domain.Core.Attributes
{
    public class EnumValidationAttribute: ValidationAttribute
    {
        private Type _type;
        public EnumValidationAttribute(Type type)
        {
            _type = type;
        }

        public override bool IsValid(object value)
        {
            return Enum.IsDefined(_type, value);
        }

    }
}

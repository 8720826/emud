﻿using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Core.Attributes
{
    public class ConditionFieldAttribute : Attribute
    {
        public PlayerConditionFieldEnum FieldEnum { get; set; }
        public ConditionFieldAttribute(PlayerConditionFieldEnum  playerConditionFieldEnum) 
        {
        
        }
    }
}

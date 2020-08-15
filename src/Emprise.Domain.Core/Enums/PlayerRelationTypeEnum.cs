using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Core.Enums
{
    public enum PlayerRelationTypeEnum
    {
        好友 = 1, 
        仇人 = 2, 
        师父 = 3, 
        徒弟 = 4, 
        夫妻 = 5
    }


    public enum PlayerRelationStatusEnum
    {
        申请 = 1,
        同意 = 2,
        黑名单 = 3
    }
}

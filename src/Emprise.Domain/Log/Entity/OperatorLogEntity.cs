using Emprise.Domain.Core.Entity;
using Emprise.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Emprise.Domain.Log.Entity
{
    [Table("OperatorLog")]
    public class OperatorLogEntity : BaseEntity
    {

        public string AdminName { set; get; }

        public string Ip { set; get; }

        public DateTime CreateDate { set; get; }

        public OperatorLogType Type { set; get; }

        public bool IsSuccess { set; get; }

        public string Content { set; get; }


  

    }
}

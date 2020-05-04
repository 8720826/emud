using Emprise.Domain.Core.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Emprise.Domain.Log.Entity
{
    [Table("SystemLog")]
    public class SystemLogEntity : BaseEntity
    {

        [Timestamp]
        public DateTime Timestamp { set; get; }


        public string Level { set; get; }

        public string Message { set; get; }

        public string Exception { set; get; }
        public string Properties { set; get; }

        [Timestamp]
        public DateTime _ts { set; get; }
    }
}

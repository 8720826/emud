using Emprise.Domain.Core.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Emprise.Domain.Log.Entity
{
    [Table("ChatLog")]
    public class ChatLogEntity : BaseEntity
    {

        public int PlayerId { set; get; }



        public string Content { set; get; }


        public DateTime PostDate { set; get; }

    }
}

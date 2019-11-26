using Emprise.Domain.Core.Queue.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Jobs.Models
{
    public class MessageModel: BaseModel
    {

        public string Content { get; set; }
    }
}

using Emprise.Domain.Player.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.MudServer.Models
{
    public class MyFriendModel
    {
        public List<PlayerBaseInfo> Friends { get; set; }

        public List<PlayerBaseInfo> Requests { get; set; }

        public List<PlayerBaseInfo> FriendMes { get; set; }
        
    }
}

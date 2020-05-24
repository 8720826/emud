using Emprise.Domain.Ware.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Domain.Player.Models
{
    public class MyPack
    {
        public string Money { get; set; }

        public List<WareModel> Wares { get; set; }
    }
}

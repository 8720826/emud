using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Admin.Models.Npc
{
    public class CaseIf
    {
        public string Condition { get; set; }

        public List<Attribute> Attrs { get; set; }
    }

    public class CaseThen
    {
        public string Commond { get; set; }

        public List<Attribute> Attrs { get; set; }
    }

    public class CaseElse
    {
        public string Commond { get; set; }

        public List<Attribute> Attrs { get; set; }
    }

    public class Attribute
    {
        public string Attr { get; set; }

        public string Val { get; set; }
    }
}

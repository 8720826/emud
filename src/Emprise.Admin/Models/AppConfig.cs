using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Admin.Models
{
    public class AppConfig
    {

        public ConnectionStringConfig ConnectionStrings { get; set; }

        public SiteConfig Site { get; set; }
        
    }


    public class ConnectionStringConfig
    {
        public string Redis { get; set; }

        public string Mysql { get; set; }

        public string MsSql { get; set; }
    }

    public class SiteConfig
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public bool IsApiEnable { get; set; }

        public string ApiKey { get; set; }

        
    }
}

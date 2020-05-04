using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emprise.Admin.Models
{
    public class OssTokenModel
    {
        public string Key { get; set; }

        public string Bucket { get; set; }

        public string OSSAccessKeyId { get; set; }

        public string Policy { get; set; }

        public string Signature { get; set; }

        public string Endpoint { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Emprise.Infra.Queue
{
    public class QueueData<T>
    {
        public int DelayMin { get; set; }

        public int DelayMax { get; set; }

        public DateTimeOffset DelayTime { get; set; }


        public T Data { get; set; }
    }
}

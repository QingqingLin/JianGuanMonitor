using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorPorts
{
    class IPHead
    {
        public byte[] IPHeadData { get; set; }
        public uint IPHeaderLength { get; set; }
        public string SourceIP { get; set; }
        public string DestIP { get; set; }
    }
}

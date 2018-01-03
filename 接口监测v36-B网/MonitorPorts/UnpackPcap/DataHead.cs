using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorPorts
{
    class DataHead
    {
        public DateTime Time{ get; set; }
        public int GetDataLength { get; set; }
        public int ActualLength { get; set; }

    }
}

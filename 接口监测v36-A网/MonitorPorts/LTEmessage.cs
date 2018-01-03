using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonitorPorts
{
    public struct LTEmessage
    {
        public string name;
        public int direction;
        public string protocol;
        public byte[] frame;
        public string src_ip;
        public string des_ip;
        public string face;
        public double time;
        public string absTime;
    }  
}

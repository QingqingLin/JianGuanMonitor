using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorPorts
{
    class TAU_request1
    {
        public int Length = 0;
        public int Index = 0;
     /// <summary>
     /// 该方法用于解析TAU request信令中的用户标识tmsi
     /// </summary>
        /// <param name="TauRequest">TauRequest信令编码</param>
     /// <returns>tmsi</returns>
        public string guti(byte[] TauRequest)
        {
            string tmsi = "";
            byte[] tmsi1 = new byte[4];
            Length = 4;
            Index = 11;
            Array.Copy(TauRequest, 11, tmsi1, 0, 4);
            tmsi = attachAccept.convert(tmsi1);
            return tmsi;
        }
    }
}

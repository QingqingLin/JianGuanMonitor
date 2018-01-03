using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorPorts
{
    class guti_reall
    {
        public int Index = 0;
        public int Length = 0;
        /// <summary>
        /// 该方法用于解析guti reallocation command信令中的tmsi
        /// </summary>
        /// <param name="s1ap">guti reallocation command信令编码</param>
        /// <returns>返回tmsi</returns>
       public string guti(byte[] s1ap)
        {
            Index = 10;
            Length = 4;
            string tmsi = "";
            byte[] tmsi1 = new byte[4];
            Array.Copy(s1ap, 10, tmsi1, 0, 4);
            tmsi = attachAccept.convert(tmsi1);
            return tmsi;
        }
    }
}

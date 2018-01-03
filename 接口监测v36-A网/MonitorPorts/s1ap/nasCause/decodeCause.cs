using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitorPorts;

namespace nasCause
{
    class cause
    {
        public int Index = 0;
        public int Length = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content">不包含完整性保护头部和加密头部的nas编码</param>
        /// <returns>nas cause</returns>
        public string NasCause(byte[]content)
        {           
            decodeNasCause Cause = new decodeNasCause();
            string str = "";
            Cause.Nas_Cause(content);
            Length = Cause.Length;
            Index = Cause.Index;
            str=Cause.causeStr;
            return str;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonitorPorts
{
    /// <summary>
    /// diameter中一条CDR的信息结构体
    /// </summary>
    public struct DiameterCDRStruct
    {
        public List<DiameterStruct> CDRbuffer;//本条CDR包含的各项信息
        public string CDRIMSI;//本条CDR对应的IMSI号
    }
}

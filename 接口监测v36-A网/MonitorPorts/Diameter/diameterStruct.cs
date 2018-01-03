using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonitorPorts
{
    /// <summary>
    /// CDR中一条信令包含的信息
    /// </summary>
    public struct DiameterStruct
    {
        public int ID_diamter;//本条信令在离线界面中对应的ID编号
        public string time; //本条信令的时间
        public byte[] diameterFrame;//信令数据帧，从IP层开始
        public byte re; //本条信令的方向
        public string command; //本条信令的命令消息
        public string protocol;   //本条信令的的上层承载协议
    }
}

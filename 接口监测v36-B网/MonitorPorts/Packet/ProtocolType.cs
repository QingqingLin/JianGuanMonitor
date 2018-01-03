using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonitorPorts.Packet
{
    class ProtocolID
    {
        public int src_port = 0;
        public int des_port = 0;
        public string protocol;
        int protocolid;
        public int protocolID(byte[] frame)
        {

            protocolid = 0;
            int ip_flag = frame[9];          
            des_port = frame[22] * 256 + frame[23];//目的Port
            src_port = frame[20] * 256 + frame[21];//源Port

            if (ip_flag == 17 && (src_port == 2123 || des_port == 2123))//将TCP协议数据按不同形式整理出来-GTP
            {
                protocolid = 3;
            }

            else if (ip_flag == 132) //将STCP协议数据按不同形式整理出来-S1ap或diameter
            {
                protocolid = 4;
            }
            else if (ip_flag == 6 && (src_port == 3868 || des_port == 3868))//将TCP协议数据按不同形式整理出来-Diameter
            {
                protocolid = 2;
            }
            else
            {
                if (ip_flag == 6)
                {
                    protocol = "TCP";
                }
                else if (ip_flag == 17)
                {
                    protocol = "UDP";
                }
                else if (ip_flag == 1)
                {
                    protocol = "ICMP";
                }
                else
                {
                    protocol = "unknown";
                }
                protocolid = 0;
            }
            return protocolid;
        }
    }
}

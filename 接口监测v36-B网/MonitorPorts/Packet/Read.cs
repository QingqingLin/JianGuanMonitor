using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTP_V2Decoder
{

    class Read
    {
        public string stringList;
        public byte[] ScChunk;
        public byte[] allMessageList;
        public byte[] TcChunk;
        public byte[] UnChunk;
        public byte[] S1apChunk;
        public string ScS;
        public string S1apS;
        public string TcT;
        public int sourceIP = 0;
        public int desIP = 0;
        public int src_port = 0;
        public int des_port = 0;
        public string protocol;
        int protocolid;
        public string time;
        public List<string> TIME = new List<string>();//给所有的时间以此加入到该list中
        public int RRead(string[] st)
        {

            protocolid = 0;
            time = st[0];
            TIME.Add(time);
            int ip_flag = Convert.ToInt16(st[10], 16);            //UDP=1
            des_port = Convert.ToInt32(st[23] + st[24], 16);//目的Port
            src_port = Convert.ToInt32(st[21] + st[22], 16);//源Port

            if ( ip_flag == 17 && (src_port == 2123 || des_port == 2123))//将TCP协议数据按不同形式整理出来-GTP
            {
                allMessageList = new byte[st.Length-1];
                for (int i = 0; i < st.Length-1; i++)
                {
                    allMessageList[i] = Convert.ToByte(st[i+1], 16);

                }
                protocolid = 3;
            }

            else if ( ip_flag == 132) //将STCP协议数据按不同形式整理出来-S1ap或diameter
            {
                S1apChunk = new byte[st.Length-1];
                for (int i = 0; i < st.Length-1; i++)
                {

                    S1apChunk[i] = Convert.ToByte(st[i+1], 16);
                }
                //ScClone.Add(S1apChunk);
                protocolid = 4;
            }
            else if (ip_flag == 6 && (src_port == 3868 || des_port == 3868))//将TCP协议数据按不同形式整理出来-Diameter
            {
                TcChunk = new byte[st.Length-1];
                for (int i = 0; i < st.Length-1; i++)
                {
                    TcChunk[i] = Convert.ToByte(st[i+1], 16);
                }
                protocolid = 2;
            }
            else
            {
                UnChunk = new byte[st.Length-1];
                for (int i = 0; i < st.Length-1; i++)
                {
                    UnChunk[i] = Convert.ToByte(st[i+1], 16);
                }
                if ( ip_flag == 6)
                {
                    protocol = "TCP";
                }
                else if ( ip_flag == 17)
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

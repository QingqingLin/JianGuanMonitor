using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorPorts
{
    class TAU_accept1
    {
        public int Length = 0;
        public int Index = 0;
        /// <summary>
        /// 用于解析TAU accept信令中的用户标识TMSI
        /// </summary>
        /// <param name="TauAccept">TAU accept 信令编码</param>
        /// <returns>TMSI</returns>
        public string guti(byte[] TauAccept)
        {
            byte id1 = 0, id2 = 0;//指示信元类型
            string tmsi = "";
            if (TauAccept.Length <= 5)
                tmsi = "";
            else
            {
                id1 = TauAccept[3];
                if (id1 == 0x5a)//T3412 value
                {
                    id2 = TauAccept[5];
                    if (id2 == 0x50)//guti
                    {
                        Index = 14;
                        Length = 4;
                        byte[] tmsi1 = new byte[4];
                        Array.Copy(TauAccept, 14, tmsi1, 0, 4);
                        tmsi = attachAccept.convert(tmsi1);
                    }
                    else//信令中不含GUTI
                        ;
                }
                else if (id1 == 0x50)//信令中没有T3412 value信元
                {
                    Index = 12;
                    Length = 4;
                    byte[] tmsi1 = new byte[4];
                    Array.Copy(TauAccept, 12, tmsi1, 0, 4);
                    tmsi = attachAccept.convert(tmsi1);
                }
                else//信令中没有T3412 value与GUTI           
                    ;
            }
            return tmsi;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorPorts
{
    class Imsi_Tmsi
    {
        public int Length = 0;//该信元在整体信令编码中的位置
        public int Index = 0;//信元的编码长度
        /// <summary>
        /// 该方法用于解析initial ue message信令中的用户标识
        /// </summary>
        /// <param name="s1ap">initial ue message信令编码</param>
        /// <returns>用户标识tmsi</returns>
        public string identity(byte[] s1ap)
        {
            string tmsi = "";
            int id = 0;
            int length = 0;//信元长度
            for (int i = 7; i <s1ap.Length; )
            {
                id =s1ap[i] * 256 +s1ap[i + 1];
                length =s1ap[i + 3];
                if (id == 96)
                {
                    Length = 4;
                    Index = i + 6;
                    byte[] tmsi1 = new byte[4];
                  Array.Copy(s1ap, i + 6, tmsi1, 0, 4);
                    tmsi = attachAccept.convert(tmsi1);
                    break;
                }
                else
                    i = i + 4 + length;
            }
            return tmsi;
        }
    }
}

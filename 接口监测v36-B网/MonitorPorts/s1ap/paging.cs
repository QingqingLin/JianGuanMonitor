using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorPorts
{
    public class IMSI_TMSI
    {
        public int Length = 0;//该信元在整体信令编码中的位置
        public int Index = 0;//信元的编码长度
     /// <summary>
     /// 该方法用于解析paging信令中的用户标识
     /// </summary>
        /// <param name="paging">paging信令编码</param>
     /// <returns>用户标识</returns>
       public string identity(byte[] paging)
        {
           string identity = "";  //用户标识         
           int id=0;//信元类型
           int length = 0;//信元长度
           for (int i = 7; i < paging.Length; )
            {
                id = paging[i] * 256 + paging[i + 1];
                length = paging[i + 3];
                if (id == 43)
                {
                    #region
                    byte type = 0;//指示用户标识是IMSI还是TMSI
                    type = (byte)(paging[i + 4] & 0x40);//type表示选择的表示是imsi还是m-tmsi
                    if (type == 0x00)
                    {
                        Index = i + 6;
                        Length = 4;
                        byte[] tmsi1 = new byte[4];
                        Array.Copy(paging, i + 6, tmsi1, 0, 4);
                        identity = attachAccept.convert(tmsi1);
                        break;
                    }
                    else if (type == 0x40)
                    {
                        Index = i + 5;
                        byte imsi_length = (byte)(paging[i + 4] & 0x38);//取s1ap[i+4]的中间3bits,它们表示IMSI长度
                        imsi_length = (byte)((imsi_length >> 3) + 3);//因为imsi为OCTET STRING (SIZE (3..8))，当length=n时，只对n-3编码为x，所以真实长度解码结果x+3
                        byte[] imsi1 = new byte[imsi_length];
                        Array.Copy(paging, i + 5, imsi1, 0, imsi_length);
                        Length = imsi_length;
                        identity = attachAccept.convert(imsi1);
                        break;
                    }
                    #endregion
                }
                else
                {
                    i = i + 4 + length;
                }
            }
            return identity;
        }
    }
}

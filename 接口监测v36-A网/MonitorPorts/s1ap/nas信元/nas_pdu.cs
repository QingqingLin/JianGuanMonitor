using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MonitorPorts
{
    public class nasPdu1
    {
        public byte direction = 0;
        public int LengthS1ap = 0;//指示nas前面的s1ap的长度
        /// <summary>
        /// 该方法将从含有nas信令的s1ap信令编码中取出nas编码
        /// </summary>
        /// <param name="s1ap">s1ap信令编码</param>
        /// <returns>nas编码</returns>
        public byte[] nasShuzu(byte[] s1ap)
        {
            ushort tlength = 0;//s1ap信令的所有信元编码总长度
            ushort slength = 0;//单个信元的编码长度
            ushort len1 = 0;//用于判定总长度所占字节的变量
            ushort len2 = 0;//用于判定单个信元长度所占字节的变量
            ushort id = 0;//用于表示信元的类型
            byte headLength = 0, headLength1 = 0;
            byte[] Nasshuzu = new byte[1];//用于接收解析出的nas数据流（注意：使用此种数组时一定要考虑所有情况中都需要将其初始化，否则就会出现“未初始化的数组”）
            s1ap_name dir = new s1ap_name();//用来获得direction

            dir.s1ap_decode1(s1ap);//获取信令的direction
            direction = (byte)dir.direction;
            len1 = s1ap[3];
            if (len1 >= 128)//s1ap编码总长度超过128，则长度编码为2字节
            {
                tlength = (ushort)((s1ap[3] & 0x0f) * 256 + s1ap[4]);
                headLength = 8;//一条s1ap消息的头部长度
            }
            else
            {
                tlength = s1ap[3];
                headLength = 7;
            }
            byte[] b = new byte[s1ap.Length - headLength];
            Array.Copy(s1ap, headLength, b, 0, b.Length);
            if (tlength >= 128)
            {
                #region
                for (int i = 0; (b.Length != 0) && (i < b.Length); )//每进行一次循环i都指向信元的首部
                {
                    id = (ushort)(b[i] * 256 + b[i + 1]);
                    len2 = b[i + 3];
                    if (len2 >= 128)
                    {
                        slength = (ushort)((b[i + 3] & 0x0f) * 256 + b[i + 4]);
                        headLength1 = 5;//表示一个信元的头部长度
                    }
                    else
                    {
                        slength = b[i + 3];
                        headLength1 = 4;
                    }
                    if (id == 26)//该信元是NAS-pdu
                    {
                        LengthS1ap = headLength + i + 5;
                        byte nasLength = 0;
                        nasLength = b[i + 4];
                        byte[] nas_pdu = new byte[nasLength];
                        Array.Copy(b, 5 + i, nas_pdu, 0, nas_pdu.Length);//nas_pdu存放的数据是nas从headerType开始到nas结束的部分（即NAS-PDU除去type，临界，两个长度值的部分）
                        Nasshuzu = nas_pdu;
                        break;
                    }
                    else
                    {
                        i = (ushort)(i + slength + headLength1);
                    }
                }
                #endregion
            }
            else//s1ap编码总长度不超过128，则长度编码为1字节
            {
                #region

                for (int i = 0; (b.Length != 0) && (i < b.Length); )
                {
                    id = (ushort)(b[i] * 256 + b[i + 1]);
                    slength = b[i + 3];
                    if (id == 26)//假设NAS-PDU的长度不超过128
                    {
                        LengthS1ap = headLength + i + 5;
                        byte nasLength = 0;
                        nasLength = b[i + 4];
                        byte[] nas_pdu = new byte[nasLength];
                        Array.Copy(b, i + 5, nas_pdu, 0, nas_pdu.Length);//nas_pdu存放的数据是nas从headerType开始到nas结束的部分（即NAS-PDU除去type，临界，两个长度值的部分）
                        Nasshuzu = nas_pdu;
                        break;
                    }
                    else
                    {
                        i = (ushort)(i + slength + 4);
                    }
                }
                #endregion
            }
            return Nasshuzu;
        }
    }
}

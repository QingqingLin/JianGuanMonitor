using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorPorts
{
    /// <summary>
    /// 该类用于解析attach request信令中的用户标识
    /// </summary>
    public class attachRequest
    {
        public int Length = 0;
        public int Index = 0;
        /// <summary>
        /// 将数组a中的数据转化成16进制，然后将前后两部分换位，用于解析BCD编码；例如：(10进制)16->10(16z进制)，
        /// 0A经过函数inversion之后返回为A0。
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static byte[] inversion(byte[] a)
        {
            for (int i = 0; i < a.Length; i++)
            {
                byte former = 0, latter = 0;
                former = (byte)(a[i] / 16);
                latter = (byte)(a[i] % 16);
                a[i] = (byte)(latter * 16 + former);
            }
            return a;
        }
        /// <summary>
        /// 将字符串a截取最开始的一个字符和最后的t个字符
        /// </summary>
        /// <param name="a"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string trunc(string a, byte t)
        {
            char[] c = a.ToCharArray();
            char[] d = new char[a.Length - t];
            Array.Copy(c, 1, d, 0, a.Length - t);
            string b = new string(d);
            return b;
        }
        /// <summary>
        /// 该方法解析信令attach request中的用户标识
        /// </summary>
        /// <param name="a"><数组a是nas信令明文编码且内容时nas编码的铭文头部（plain message）至编码结束>
        /// <returns><返回值 dignity是IMSI或TMSI>
        public string identity(byte[] a)
        {
            byte type = 0;//指示信元类型
            byte indic = 0;
            string dignity = "";//存储解析出的用户标识
            byte len = 0;//如果用户标识是IMSI，则len表示IMSI编码长度

            len = a[3];
            type = (byte)(a[4] & 0x07);//指示该用户标识是IMSI还是TMSI
            indic = (byte)((a[4] & 0x08) >> 3);//指示identity digit的数量是奇数还是偶数，0为偶数，1为奇数

            if (type == 6)//guti
            {
                Length = 4;
                Index = 11;
                byte[] tmsi = new byte[4];
                Array.Copy(a, 11, tmsi, 0, 4);
                dignity = attachAccept.convert(tmsi);
            }
            else if (type == 1)//IMSI
            {
                Length = a[3];
                Index = 4;
                byte[] tmsi = new byte[len];
                byte fill = 0;//当identity digit个数为偶数时，最后一个identity digit为1111
                Array.Copy(a, 4, tmsi, 0, len);
                fill = (byte)((tmsi[len - 1] & 0xf0) >> 4);
                if ((indic == 0) && (fill == 15))//偶数个identity digit
                {
                    byte t = 2;
                    tmsi = inversion(tmsi);//每个数组元素前后半部分倒置
                    dignity = attachAccept.convert(tmsi);//将byte数组转为16进制string
                    dignity = trunc(dignity, t);//将string中多余的第一个字符和最后一个字符去掉
                }
                else if (indic == 1)//奇数个identity digit
                {
                    byte t = 1;
                    tmsi = inversion(tmsi);
                    dignity = attachAccept.convert(tmsi);
                    dignity = trunc(dignity, t);
                }
                else
                    return "error";
            }
            else
                return "error";
            return dignity;//imsi or tmsi
        }
    }
}

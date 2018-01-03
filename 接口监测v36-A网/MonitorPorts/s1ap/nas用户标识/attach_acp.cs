using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorPorts
{
    /// <summary>
    /// 该类解析attach accept信令中的用户标识
    /// </summary>
    class attachAccept
    {
        public int Length = 0;
        public int Index = 0;
        /// <summary>
        /// 该方法的作用是将byte数组转化为16进制字符串
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string convert(byte[] a)
        {
            string str = "";
            if (a.Length != 0)
            {
                for (int i = 0; i < a.Length; i++)
                {
                    str += a[i].ToString("X2");
                }
            }
            return str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"><数组a是nas信令明文编码且内容时nas编码的铭文头部（plain message）至编码结束>
        /// <returns><返回用户标识tmsi>
        public string Guti(byte[] a)//数组a为从明文的头部至nas结束
        {
            string tmsi = "";
            int tai_len = 0, container_len = 0, total = 0;
            byte id = 0;//判断IE是否为guti
            tai_len = a[4];
            container_len = a[4 + 1 + tai_len] * 256 + a[4 + 1 + tai_len + 1];
            total = 4 + 1 + tai_len + 2 + container_len;//排在Guti信元前面的信元长度
            if (a.Length > total)
            {
                id = a[total];
                if (id == 0x50)
                {
                    Index = total + 9;
                    Length = 4;
                    byte[] tmsi1 = new byte[4];
                    Array.Copy(a, total + 9, tmsi1, 0, 4);
                    tmsi = convert(tmsi1);
                }
                else
                    ;
            }
            else
                ;
            return tmsi;
        }
    }
}

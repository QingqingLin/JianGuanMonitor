using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorPorts
{
    using System;
    using System.Runtime.InteropServices;
    //本程序中动态链接库的使用方法有待验证！！
    class EEA2
    {
        public byte[] aes(byte[] ciper, byte[] Knas, byte[] IK)//Knas是Knasenc的后128位，IK是COUNT，方向，bearer identity组合而成，它只有8字节
        {
            //List<byte[]> cip = new List<byte[]>();
            int len = 0;
            long T = 0;
            byte[] half_IK = new byte[8];//用于获取IK的低128bit，作为T值
            //EEA2是分段进行加密，每段16bytes
            if (ciper.Length % 16 == 0)
                len = ciper.Length / 16;
            else
                len = ciper.Length / 16 + 1;
            //for (int i = 0,j=0; i < len; i++,j=j+16)
            //    Array.Copy(ciper,j, cip[i],0, 16);
            //获得T值（T0，T1...Tn）
            //T表示是aes中的T0的意思，8个字节初始值是0，而不是IK的一部分
            Array.Copy(IK, 8, half_IK, 0, 8);
            Array.Reverse(half_IK);//将half_IK倒序，以便将其转换为long型
            T = BitConverter.ToInt64(half_IK, 0);

            //逐段进行解密
            byte[] OutText = new byte[16];
            byte[] ExpKey = new byte[176];//ExpKey[4 * Nb*(Nr + 1)];Nb=4,Nr=10
            byte[] Key_stream = new byte[len * 16];//用于存放密钥流（每次循环产生的密钥流块都是128bits,len为循环次数）byte[] Key_stream = new byte[len * 8]
            byte[] buffer = new byte[8];//将T值拆成byte数组buffer（8bytes）
            for (int k = 0, i = 0; i < len; i++, k = k + 16, T++)//原来为for (int k = 0, i = 0; i < len; i++, k = k + 8, T++)
            {
                buffer = BitConverter.GetBytes(T);
                Array.Reverse(buffer);//将buffer倒序放置才是所需的数组
                Array.Copy(buffer, 0, IK, 8, 8);//将buffer插入到IK的低128bits,每循环一次，T自加一
                ExpandKey(Knas, ExpKey);
                Encrypt(IK, ExpKey, OutText);
                Array.Copy(OutText, 0, Key_stream, k, 16);//原来为Array.Copy(OutText, 0, Key_stream, k, 8);
            }
            for (int i = 0; i < ciper.Length; i++)
                ciper[i] = (byte)(ciper[i] ^ Key_stream[i]);
            return ciper;
        }
        [DllImport("aes.dll", EntryPoint = "ExpandKey", CallingConvention = CallingConvention.Cdecl)]
        static extern void ExpandKey(byte[] a, byte[] b);
        [DllImport("aes.dll", EntryPoint = "Encrypt", CallingConvention = CallingConvention.Cdecl)]
        static extern void Encrypt(byte[] a, byte[] b, byte[] c);
    }
}
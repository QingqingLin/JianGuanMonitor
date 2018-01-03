using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
namespace MonitorPorts
{
    using System;
    using System.Runtime.InteropServices;
    class EEA1
    {
        public byte[] snow(byte[] ciper, byte[] Knas, UInt32[] IK)//Knas是Knasenc的后128位，IK是COUNT，方向，bearer identity组合而成
        {
            UInt32[] Key_nas = new UInt32[4];
            UInt32[] Key_nas1 = new UInt32[4];

            uint len = 0;
            for (int i = 0, j = 0; i < 16; i = i + 4, j++)
                Key_nas[j] = (UInt32)(Knas[i] * Math.Pow(256, 3) + Knas[i + 1] * Math.Pow(256, 2) + Knas[i + 2] * Math.Pow(256, 1) + Knas[i + 3]);
            for (int i = 0; i < 4; i++)
                Key_nas1[i] = Key_nas[3 - i];
            if (ciper.Length % 4 == 0)
                len = (uint)ciper.Length / 4;
            else
                len = (uint)ciper.Length / 4 + 1;//若密文长度不为4的倍数，则密钥流长度多加1
            UInt32[] Key_stream = new UInt32[len];
            byte[] Key_stream1 = new byte[4 * len];
            Initialize(Key_nas1, IK);
            GenerateKeystream(len, Key_stream);
            for (int i = 0; i < len; i++)
            {
                Key_stream1[i * 4 + 3] = (byte)(Key_stream[i] & 0xFF);
                Key_stream1[i * 4 + 2] = (byte)((Key_stream[i] & 0xFF00) >> 8);
                Key_stream1[i * 4 + 1] = (byte)((Key_stream[i] & 0xFF0000) >> 16);
                Key_stream1[i * 4 + 0] = (byte)((Key_stream[i] >> 24) & 0xFF);
            }
            for (int i = 0; i < ciper.Length; i++)
                ciper[i] = (byte)(ciper[i] ^ Key_stream1[i]);//密文与密钥流异或得到明文
            return ciper;
        }
        //static string pathdll = FilterForm.pathDll + "\\snow1.dll";
        //[DllImport(pathdll, EntryPoint = "Initialization", CallingConvention = CallingConvention.Cdecl)]
        [DllImport("snow1.dll", EntryPoint = "Initialize", CallingConvention = CallingConvention.Cdecl)]//@"C:\Users\hp\Desktop\接口监测v27\MonitorPorts\bin\Debug\snow1.dll"
        static extern void Initialize(UInt32[] a, UInt32[] b);
        [DllImport("snow1.dll", EntryPoint = "GenerateKeystream", CallingConvention = CallingConvention.Cdecl)]
        static extern void GenerateKeystream(UInt32 a, UInt32[] b);
    }
}

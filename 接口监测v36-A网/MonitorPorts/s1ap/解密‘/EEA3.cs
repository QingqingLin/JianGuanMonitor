using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorPorts
{
    using System;
    using System.Runtime.InteropServices;
using System.Windows.Forms;
    class EEA3
    {
        public byte[] zuc(byte[] ciper, byte[] Knas, byte[] IK)
        {
            int KeystreamLen = 0;
            //确定密钥字的长度
            if (ciper.Length % 4 == 0)
                KeystreamLen = ciper.Length / 4;
            else
                KeystreamLen = ciper.Length / 4 + 1;
            UInt32[] Keystream = new UInt32[KeystreamLen];//存放密钥流
            byte[] stream = new byte[KeystreamLen * 4];
            //生成密钥流
            Initialization(Knas, IK);
            GenerateKeystream(Keystream, KeystreamLen);
            for (int i = 0; i < KeystreamLen; i++)
            {
                stream[3 + i * 4] = (byte)(Keystream[i] & 0xFF);
                stream[2 + i * 4] = (byte)((Keystream[i] & 0xFF00) >> 8);
                stream[1 + i * 4] = (byte)((Keystream[i] & 0xFF0000) >> 16);
                stream[0 + i * 4] = (byte)((Keystream[i] >> 24) & 0xFF);
            }
            for (int i = 0; i < ciper.Length; i++)
                ciper[i] = (byte)(ciper[i] ^ stream[i]);
            return ciper;
        }
        //static string path = Application.StartupPath + "\\ZUC1.dll";
        //[DllImport(path, EntryPoint = "Initialization", CallingConvention = CallingConvention.Cdecl)]
        [DllImport("ZUC1.dll", EntryPoint = "Initialization", CallingConvention = CallingConvention.Cdecl)]
        static extern void Initialization(byte[] a, byte[] b);
        [DllImport("ZUC1.dll", EntryPoint = "GenerateKeystream", CallingConvention = CallingConvention.Cdecl)]
        static extern void GenerateKeystream(UInt32[] a, int b);
    }
}

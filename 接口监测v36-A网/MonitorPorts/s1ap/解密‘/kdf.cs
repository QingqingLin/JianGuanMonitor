using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
namespace MonitorPorts
{
    class KDF
    {
        /// <summary>
        /// 该方法将根密钥转化为nas加密密钥Knasenc
        /// </summary>
        /// <param name="Kasme">根密钥</param>
        /// <param name="str">特定字符串(与加密算法有关)</param>
        /// <returns>Knasenc</returns>
        public byte[] kdf_decode(byte[] Kasme, byte[] str)
        {
            byte[] hash = new byte[32];
            using (HMAC hmac = new HMACSHA256(Kasme))
            {
                hash = hmac.ComputeHash(str);
            }
            return hash;
        }
    }
}

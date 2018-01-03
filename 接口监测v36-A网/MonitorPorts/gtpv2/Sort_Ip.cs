using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTP_V2Decoder
{
    class Sort_Ip
    {
        /// <summary>
        /// 找出Ip对，输入参数为：gtp数据包、包备份、分类结果列表
        /// </summary>
        /// <param name="gtp"></param>
        /// <param name="g"></param>
        /// <param name="sortedIp"></param>
        public static void FindIpPair(List<gtpStruct> gtp, List<gtpStruct> g, List<List<gtpStruct>> sortedIp)
        {
            int numberOfIpPair = 0;
            while (gtp.Count > 0)
            {
                int n = g.Count;
                List<gtpStruct> l = new List<gtpStruct>();
                sortedIp.Add(l);
                string SorIp, DesIp;
                //不包含数据链路层，但是第一位加了时间，从13开始，不加时间从12开始，包含数据链路层从28开始
                SorIp = Convert.ToString(gtp[0].gtpFrame[12]) + "." + Convert.ToString(gtp[0].gtpFrame[13]) + "." + Convert.ToString(gtp[0].gtpFrame[14]) + "." + Convert.ToString(gtp[0].gtpFrame[15]);
                DesIp = Convert.ToString(gtp[0].gtpFrame[16]) + "." + Convert.ToString(gtp[0].gtpFrame[17]) + "." + Convert.ToString(gtp[0].gtpFrame[18]) + "." + Convert.ToString(gtp[0].gtpFrame[19]);
                for (int i = 0; i < n; i++)
                {
                    byte[] chunk = g[i].gtpFrame;
                    string chunk_sorIp, chunk_DesIp;
                    chunk_sorIp = Convert.ToString(chunk[12]) + "." + Convert.ToString(chunk[13]) + "." + Convert.ToString(chunk[14]) + "." + Convert.ToString(chunk[15]);
                    chunk_DesIp = Convert.ToString(chunk[16]) + "." + Convert.ToString(chunk[17]) + "." + Convert.ToString(chunk[18]) + "." + Convert.ToString(chunk[19]);
                    if ((chunk_sorIp == DesIp && chunk_DesIp == SorIp) || (chunk_DesIp == DesIp && chunk_sorIp == SorIp))
                    {
                        sortedIp[numberOfIpPair].Add(g[i]);
                        gtp.Remove(g[i]);
                    }
                }
                numberOfIpPair = numberOfIpPair + 1;
            }
        }
    }
}

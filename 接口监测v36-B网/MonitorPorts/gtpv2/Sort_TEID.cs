using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTP_V2Decoder
{
    class Sort_TEID
    {
        /// <summary>
        /// 判断这条Gtp消息的TEID是否存在
        /// </summary>
        /// <param name="arraySameIPPair"></param>
        /// <returns></returns>
        public static Boolean TEID_Exist(gtpStruct arraySameIPPair)
        {
            //不包含数据链路层从28开始  减了16
            int TeidFlag = arraySameIPPair.gtpFrame[28] / 8 % 2;
            if (TeidFlag == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
            
        }
        /// <summary>
        /// 在一个IP对下，将消息先分成两类，有无TEID号
        /// </summary>
        /// <param name="sameIPPair"></param>
        /// <param name="TEIDExist"></param>
        /// <param name="nullTEID"></param>
        public static void Exist_sort(List<gtpStruct> sameIPPair, List<gtpStruct> TEIDExist, List<gtpStruct> nullTEID)
        {
            for (int i=0; i < sameIPPair.Count; i++)
            {
                if (TEID_Exist(sameIPPair[i]))
                {
                    TEIDExist.Add(sameIPPair[i]);
                }
                else
                {
                    nullTEID.Add(sameIPPair[i]);
                }
            }
        }
        /// <summary>
        /// 在存在TEID时，将同一IP对下面的TEID进行分类
        /// </summary>
        /// <param name="TEIDExist"></param>
        /// <param name="TEIDExistClone"></param>
        /// <param name="sortedTEID"></param> 
        public static void Sort(List<gtpStruct> TEIDExist, List<gtpStruct> TEIDExistClone, List<List<gtpStruct>> sortedTEID)
        {
            int numberOfTEID = 0;
            while (TEIDExist.Count > 0)
            {
                
                int n = TEIDExistClone.Count;
                List<gtpStruct> l = new List<gtpStruct>();
                sortedTEID.Add(l);
                string TEID;
                //第一位是时间，从33开始，不是时间从32开始，包含数据链路层从48开始
                TEID = Convert.ToString(TEIDExist[0].gtpFrame[32]) + Convert.ToString(TEIDExist[0].gtpFrame[33]) + Convert.ToString(TEIDExist[0].gtpFrame[34]) + Convert.ToString(TEIDExist[0].gtpFrame[35]);
                for (int i = 0; i < n; i++)
                {
                    byte[] chunk = TEIDExistClone[i].gtpFrame;
                    string chunk_TEID;
                    chunk_TEID = Convert.ToString(chunk[32]) + Convert.ToString(chunk[33]) + Convert.ToString(chunk[34]) + Convert.ToString(chunk[35]);   
                    if (chunk_TEID == TEID)
                    {
                        sortedTEID[numberOfTEID].Add(TEIDExistClone[i]);
                        TEIDExist.Remove(TEIDExistClone[i]);
                    }
                }
                numberOfTEID = numberOfTEID + 1;
            }
        }
    }
}

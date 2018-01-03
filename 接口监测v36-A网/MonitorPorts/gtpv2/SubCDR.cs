using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTP_V2Decoder
{
    class SubCDR
    {
        /// <summary>
        /// 同一个IP对下面的gtp消息，已经分类好了TEID，根据SequenceNumber号确定一个CDR
        /// </summary>
        /// <param name="sortedTEID"></param>
        /// <returns></returns>
        public static List<List<gtpStruct>> CDRFunction(List<List<gtpStruct>> sortedTEID)
        {
            List<List<gtpStruct>> sortedTEID_Clone = new List<List<gtpStruct>>();
            foreach (List<gtpStruct> a in sortedTEID)
            {
                sortedTEID_Clone.Add(a);
            }
            List<List<gtpStruct>> CDR_list = new List<List<gtpStruct>>();
            int number = 0;
            while (sortedTEID.Count > 0)
            {
                int n = sortedTEID_Clone.Count;
                List<gtpStruct> l = new List<gtpStruct>();
                CDR_list.Add(l);
                string sn1 = "", sn2 = "", sn3 = "";
                //减了16，第一位是时间，从37开始
                sn1 = Convert.ToString(sortedTEID[0][0].gtpFrame[36]) + Convert.ToString(sortedTEID[0][0].gtpFrame[37]) + Convert.ToString(sortedTEID[0][0].gtpFrame[38]);
                if (sortedTEID[0].Count > 1)                       //序列号匹配，数量判断
                {
                    sn2 = Convert.ToString(sortedTEID[0][1].gtpFrame[36]) + Convert.ToString(sortedTEID[0][1].gtpFrame[37]) + Convert.ToString(sortedTEID[0][1].gtpFrame[38]);
                }
                if (sortedTEID[0].Count > 2)
                {
                    sn3 = Convert.ToString(sortedTEID[0][2].gtpFrame[36]) + Convert.ToString(sortedTEID[0][2].gtpFrame[37]) + Convert.ToString(sortedTEID[0][2].gtpFrame[38]);
                }
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < sortedTEID_Clone[i].Count; j++)
                    {
                        byte[] chunk = sortedTEID_Clone[i][j].gtpFrame;
                        string chunk_sn;
                        chunk_sn = Convert.ToString(chunk[36]) + Convert.ToString(chunk[37]) + Convert.ToString(chunk[38]);
                        if ((chunk_sn == sn1) || (chunk_sn == sn2) || (chunk_sn == sn3))
                        //if ((chunk_sn == sn1) || (chunk_sn == sn2))
                        //if (chunk_sn == sn1)
                        {
                            if (sortedTEID.Count > 0)
                            {
                                foreach (gtpStruct a in sortedTEID_Clone[i])
                                {
                                    CDR_list[number].Add(a);
                                }
                                sortedTEID.Remove(sortedTEID_Clone[i]);
                                break;
                            }
                        }
                    }
                }
                number = number + 1;
            }
            return CDR_list;
        }
    }
}

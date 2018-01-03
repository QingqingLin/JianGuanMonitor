using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorPorts
{

    public class process
    {
        /// <summary>
        /// 该结构体代表一条信令的6元组
        /// </summary>
        public struct key6
        {
            public string sourceIP;
            public string desIP;
            public int sourceport;
            public int desport;
            public uint mmeUeS1apId;
            public int enbUeS1apId;
        }
        /// <summary>
        /// 该结构体代表一条信令中解析出的内容
        /// </summary>
        public struct S1apContent
        {
            public int id;//包序号
            public string src_ip;
            public string des_ip;
            public string time1;//信令时间
            public string timeNas;
            public byte[] chunk;
            public string s1ap_name;
            public uint MmeUeS1apId;
            public int EnbUeS1apId;
            public string cause_kind;
            public string cause_name;
            public string s1ap_identity;
            public int direction;
            public byte[] NasChunk;
            public string nas_name;
            public string nas_cause;
            public string nas_identity;
            public string ue_ip;
        }
        /// <summary>
        /// 该结构体表示唯一确定一个S1-C逻辑链接的一对ID
        /// </summary>
        public struct IdPair
        {
            public uint MmeUeS1apId;
            public int EnbUeS1apId;
        }
        /// <summary>
        /// CdrList存放最终的分类结果，即它的每个单元为一个CDR
        /// </summary>
        public List<List<S1apContent>> CdrList = new List<List<S1apContent>>();
        /// <summary>
        /// CdrId存放的是对应与CdrList的CDR单元的ID Pair
        /// </summary>
        public List<IdPair> CdrId = new List<IdPair>();
        /// <summary>
        /// 该变量指示每个CDR单元对应的ID pair
        /// </summary>
        IdPair SingleCdrId = new IdPair();

        /// <summary>
        /// 该方法用于判断两条s1ap消息是否属于同一个CDR单元内
        /// </summary>
        /// <param name="a">一个CDR的第一条信令对应的6元组</param>
        /// <param name="b">后续的信令所对应的6元组</param>
        /// <returns>如果是true，则两条信令属于同一个CDR；否则，不属于同一个CDR内</returns>
        public bool Classfy(ref key6 CdrFirst, key6 CdrBehind)//结构体CdrFirst表示缓存中的6元组，结构体b表示消息中的6元组
        {
            if ((CdrFirst.sourceIP == CdrBehind.sourceIP) && (CdrFirst.desIP == CdrBehind.desIP) && (CdrFirst.sourceport == CdrBehind.sourceport) && (CdrFirst.desport == CdrBehind.desport))//同向相等
            {
                //if (CdrFirst.mmeUeS1apId == 0 && CdrFirst.enbUeS1apId == 0)
                //{
                //    CdrFirst.mmeUeS1apId = CdrBehind.mmeUeS1apId;
                //    CdrFirst.enbUeS1apId = CdrBehind.enbUeS1apId;
                //    return true;
                //}
                if (CdrFirst.mmeUeS1apId == 0 && (CdrFirst.enbUeS1apId == CdrBehind.enbUeS1apId))
                {
                    CdrFirst.mmeUeS1apId = CdrBehind.mmeUeS1apId;
                    return true;
                }
                if (CdrFirst.enbUeS1apId == 0 && (CdrFirst.mmeUeS1apId == CdrBehind.mmeUeS1apId))
                {
                    CdrFirst.enbUeS1apId = CdrBehind.enbUeS1apId;
                    return true;
                }
                if ((CdrFirst.enbUeS1apId == CdrBehind.enbUeS1apId) && (CdrFirst.mmeUeS1apId == CdrBehind.mmeUeS1apId))
                    return true;
                else
                    return false;
            }

            if ((CdrFirst.sourceIP == CdrBehind.desIP) && (CdrFirst.desIP == CdrBehind.sourceIP) && (CdrFirst.sourceport == CdrBehind.desport) && (CdrFirst.desport == CdrBehind.sourceport))//反向相等
            {
                //if (CdrFirst.mmeUeS1apId == 0 && CdrFirst.enbUeS1apId == 0)
                //{
                //    CdrFirst.mmeUeS1apId = CdrBehind.mmeUeS1apId;
                //    CdrFirst.enbUeS1apId = CdrBehind.enbUeS1apId;
                //    return true;
                //}
                if (CdrFirst.mmeUeS1apId == 0 && (CdrFirst.enbUeS1apId == CdrBehind.enbUeS1apId))
                {
                    CdrFirst.mmeUeS1apId = CdrBehind.mmeUeS1apId;
                    return true;
                }
                if (CdrFirst.enbUeS1apId == 0 && (CdrFirst.mmeUeS1apId == CdrBehind.mmeUeS1apId))
                {
                    CdrFirst.enbUeS1apId = CdrBehind.enbUeS1apId;
                    return true;
                }
                if ((CdrFirst.enbUeS1apId == CdrBehind.enbUeS1apId) && (CdrFirst.mmeUeS1apId == CdrBehind.mmeUeS1apId))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        /// <summary>
        /// 该方法将离线文件中所有的s1ap信令根据Classfy方法归类到CDR单元中，
        /// 每个CDR单元就是一个终端进行的一次通信过程，
        /// </summary>
        /// <param name="PcapSignalKey6">存放所有信令6元组的列表</param>
        /// <param name="PcapSignalContent">存放所有信令解析内容的列表</param>
        public void CdrCreate(List<key6> PcapSignalKey6, List<S1apContent> PcapSignalContent)//注意实际应用中实际的流程数目会大于3，此处以后需要修改
        {
            CdrList.Clear();
            CdrId.Clear();
            key6 SingleCdrFirst = new key6();//
            for (int i = 0; PcapSignalKey6.Count != 0 && Classfy(ref SingleCdrFirst, PcapSignalKey6[i]) == false; i = 0)//此处i=0,a是上个流程的关键字，但是此时PcapSignalKey6已经没有上个流程的信令了那么func（a,PcapSignalKey6[i]）=false,故正好更新关键字a
            //此处判断 PcapSignalKey6.Count!=0，是当PcapSignalKey6为空时，说明所有的信令都已归类，故应结束循环，也避免再次进行keyfunc判断。
            {
                List<key6> SingleCdrKey6 = new List<key6>();//该变量存放属于一个CDR的信令的6 元组
                List<S1apContent> SingleCdrContent = new List<S1apContent>();//该变量存放属于一个CDR的信令的解析内容
                List<key6> UnCdrKey6 = new List<key6>();//该变量存放与指定信令不属于同一个CDR的信令的6元组
                List<S1apContent> UnCdrContent = new List<S1apContent>();//该变量存放与指定信令不属于同一个CDR的信令的解析内容

                for (; i < PcapSignalKey6.Count && PcapSignalKey6.Count > 0; )
                {
                    if ((PcapSignalContent[i].s1ap_name == "initialUEMessage") || (PcapSignalContent[i].s1ap_name == "HandoverRequest"))
                    {
                        SingleCdrFirst = PcapSignalKey6[i];
                        for (; i < PcapSignalKey6.Count && PcapSignalKey6.Count > 0; i++)
                        {
                            if (Classfy(ref SingleCdrFirst, PcapSignalKey6[i]) == true)//当后续的信令与制定的信令SingleCdrFirst属于同一个CDR时
                            {
                                SingleCdrKey6.Add(PcapSignalKey6[i]);
                                SingleCdrContent.Add(PcapSignalContent[i]);
                            }
                            else//与指定信令不属于同一个CDR时
                            {
                                UnCdrKey6.Add(PcapSignalKey6[i]);
                                UnCdrContent.Add(PcapSignalContent[i]);
                            }
                        }
                        break;
                    }
                    else
                        i++;
                }

                SingleCdrId.MmeUeS1apId = SingleCdrFirst.mmeUeS1apId;
                SingleCdrId.EnbUeS1apId = SingleCdrFirst.enbUeS1apId;//Z代表一个CDR的ID-PAIRS
                CdrId.Add(SingleCdrId);//所有CDR的ID-PAIRS
                CdrList.Add(SingleCdrContent);//CdrList存放所有完结的CDR
                PcapSignalKey6 = UnCdrKey6;
                PcapSignalContent = UnCdrContent;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using read;
using sctp;
using MonitorPorts;

namespace MonitorPorts
{
    class s1apDec
    {
        Readdata read1 = new Readdata();
        public List<byte[]> Sc = new List<byte[]>();//存储的是sctp层数据     

        SctpDecode sctp1 = new SctpDecode();
        public List<byte[]> s1ap = new List<byte[]>();//存储当前的s1ap消息（即刚接收到的数据中的S1ap消息）

        s1ap_name s1ap_name1 = new s1ap_name();
       

        List<List<byte[]>> s1ap1 = new List<List<byte[]>>();//有s1ap消息，则存放s1ap消息；没有则存放0xff表示不是s1ap报文

        Id1 id1 = new Id1();
        public uint MmeUeS1apId = 0;

        Id2 id2 = new Id2();
        public int EnbUeS1apId = 0;

        causeOnline cause1 = new causeOnline();
        public string str1 = "";//s1ap信令名称
        public string str2 = "";//cause类型
        public string str3 = "";//cause名称
        //下面所定义的变量都是用于辅助显示信元对应的编码的
        public int indexMme;
        public int LengthMme;
        public int IndexEnb;
        public int LengthEnb;
        public int IndexCause;
        public int LengthCause;
       /// <summary>
       /// 该方法调用其他方法解析一条s1ap报文中的cause及信令名称，id pair
       /// </summary>
        /// <param name="s1ap">s1ap报文</param>
        public void s1apIeDec(byte[] s1ap)
        {
            str1 = s1ap_name1.s1ap_decode1(s1ap);
            MmeUeS1apId = id1.s1ap_id(s1ap);
            LengthMme = id1.Length;
            indexMme = id1.Index;
            EnbUeS1apId = id2.s1ap_id(s1ap);
            LengthEnb = id2.Length;
            IndexEnb = id2.Index;
            cause1.decodeCause(s1ap);
            str2 = cause1.str1;//cause类型
            str3 = cause1.str2;//cause名称
            IndexCause = cause1.Index;
            LengthCause = cause1.Length;
        }
    }
}



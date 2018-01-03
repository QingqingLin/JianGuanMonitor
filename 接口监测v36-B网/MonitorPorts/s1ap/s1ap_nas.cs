using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitorPorts.Packet;//diameter类，从其中获得Kasme

namespace MonitorPorts
{
    public class s1ap_nas1
    {
       //用于辅助显示用户所点击的节点对应的编码
        public int IndexEmm = 0;
        public int LengthEmm = 0;
        public int IndexEsm = 0;
        public int LengthEsm = 0;
        public int IndexCause = 0;
        public int LengthCause = 0;
        public int IndexIdentity = 0;
        public int LengthIdentity = 0;
        public int IndexIP = 0;
        public int LengthIP = 0;
        public string EmmMessage1 = " ";
        public string EsmMessage1 = " ";
        public string identity = "";//UE的标识IMSI或TMSI
        public string cause_str = "";//nas cause
        public string nas_name = " ";//nas 名称
        public string ue_ip = "";//UE IP
        public byte algorithm = 0;//用于多段关联
        public byte[] NasChunk = new byte[1];
        /// <summary>
        /// 该方法用于解析s1ap信令中所含nas信令的名称，cause，用户标识，ue ip及加密所选用的算法
        /// </summary>
        /// <param name="s1ap">s1ap编码</param>
        public void nas_decode(byte[] s1ap)//输入为s1ap数据流
        {
            int LengthS1ap = 0;//指示nas前面的s1ap的长度
            byte type = 0;//s1ap信令类型（初始化、成功回复、失败回复）
            byte procedure = 0;//s1ap信令所属过程
            int key = 0;//指示s1ap具体信令名称
            byte[] nas_pdu = new byte[1];//用于存储从s1ap中截取出的nas编码
            byte direction = 2;//direction的合法值为0,1
            List<byte[]> Kasme = new List<byte[]>();//接收从diameter信令中获得的KASME

            identity = "";//UE的标识IMSI或TMSI
            cause_str = "";//nas cause
            nas_name = " ";//nas 名称;
            ue_ip = "";
            nasPdu1 nas1 = new nasPdu1();
            setup1 setup2 = new setup1();
            ErabModifyReq modify2 = new ErabModifyReq();
            initial_ue initial_ue1 = new initial_ue();
            nasDecrypt1 nasDecrypt2 = new nasDecrypt1();

            //获取Kasme     2017.6.7（黄刚）
            //Kasme = Diameter.KASMEValue;      //在这里不去获得Kasme，防止在“打开”离线文件事件和点击listview_off事件中改变decode_decrypt.cs中的Kasme_i的值，
            //而影响到密钥的轮换，因为"打开"事件,“点击listview_off”事件和多段关联事件，都使用的是同一个Kasme_i值，但是其实在前两个事件中是本来不需要解密的，
            //因为在多个终端下是不能分清当前密钥是属于哪个终端的


            ////获取s1ap的信令名称
            type = s1ap[0];
            procedure = s1ap[1];
            key = (int)(procedure + type * 256);

            //从s1ap中获取nas-pdu
            if ((key == 7) || (key == 11) || (key == 12) || (key == 13) || (key == 16))//nas-pdu为1阶信元
            {
                nas_pdu = nas1.nasShuzu(s1ap);
                LengthS1ap = nas1.LengthS1ap;
                direction = nas1.direction;
                NasChunk = nas_pdu;
            }
            else if (key == 5)//E-rab setup request
            {
                nas_pdu = setup2.setup_list(s1ap);
                LengthS1ap = setup2.LengthS1ap;
                direction = setup2.direction;
                NasChunk = nas_pdu;
            }
            else if (key == 6)//E-rab modify request
            {
                nas_pdu = modify2.nasIE(s1ap);
                LengthS1ap = modify2.LengthS1ap;
                direction = modify2.direction;
                NasChunk = nas_pdu;
            }
            else if (key == 9)//initail context setup 
            {
                nas_pdu = initial_ue1.initialUe(s1ap);
                LengthS1ap = initial_ue1.LengthS1ap;
                direction = initial_ue1.direction;
                NasChunk = nas_pdu;
            }
            else//不含有nas-pdu
            {
                NasChunk = nas_pdu;
            }
            //解析解码nas-pdu
            if (nas_pdu.Length > 1)//只有当有nas消息时才进行解析，因为定义nas_pdu时分配了1个空间
            {
                nasDecrypt2.decrypt(nas_pdu, direction, Kasme);
                identity = nasDecrypt2.identity;
                cause_str = nasDecrypt2.cause_str;
                nas_name = nasDecrypt2.EmmMessage + " " + nasDecrypt2.EsmMessage;
                EmmMessage1 = nasDecrypt2.EmmMessage;
                EsmMessage1 = nasDecrypt2.EsmMessage;
                ue_ip = nasDecrypt2.ue_ip;

                IndexEmm = nasDecrypt2.IndexEmm+LengthS1ap;
                IndexEsm = nasDecrypt2.IndexEsm + LengthS1ap;
                IndexCause = nasDecrypt2.IndexCause + LengthS1ap;
                IndexIdentity = nasDecrypt2.IndexIdentity + LengthS1ap;
                IndexIP = nasDecrypt2.IndexIP + LengthS1ap;
                LengthEmm = nasDecrypt2.LengthEmm;
                LengthEsm = nasDecrypt2.LengthEsm;
                LengthCause = nasDecrypt2.LengthCause;
                LengthIdentity = nasDecrypt2.LengthIdentity;
                LengthIP = nasDecrypt2.LengthIP;
            }
            else //该s1ap消息不包含nas消息
            { 
             identity = "";//UE的标识IMSI或TMSI
             cause_str = "";//nas cause
             nas_name = " ";//nas 名称;
             ue_ip = "";
             EmmMessage1 = " ";
             EsmMessage1 = " ";
            }
        }
    }
}

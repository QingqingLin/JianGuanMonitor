using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nasCause;
namespace MonitorPorts.nas
{
    public class NAS_DEC
    {
        public string identity = "";//UE的标识IMSI或TMSI
        public string cause_str = "";//nas cause
        public string nas_name = " ";//nas 名称
        public string ue_ip = "";//UE IP
        public byte algorithm = 0;//用于多段关联
        public void nas_decode1(byte[] s1ap, List<byte[]> kasme, int direction)//输入为s1ap数据流
        {
            byte type = 0;
            byte procedure = 0;
            int key = 0;
            byte[] nas_pdu = new byte[1];
            identity = "";//UE的标识IMSI或TMSI
            cause_str = "";//nas cause
            nas_name = " ";//nas 名称;
            ue_ip = "";
            nasPdu1 nas1 = new nasPdu1();
            setup1 setup2 = new setup1();
            ErabModifyReq modify2 = new ErabModifyReq();
            initial_ue initial_ue1 = new initial_ue();
            nasDecrypt0 nasDecrypt2 = new nasDecrypt0();
            type = s1ap[0];
            procedure = s1ap[1];
            key = (int)(procedure + type * 256);
            //从s1ap中获取nas-pdu
            if ((key == 7) || (key == 11) || (key == 12) || (key == 13) || (key == 16))//nas-pdu为1阶信元
            {
                nas_pdu = nas1.nasShuzu(s1ap);
                //direction = nas1.direction;       //方向由输入的参数给出  （黄刚）2017.6.7
            }
            else if (key == 5)//E-rab setup request
            {
                nas_pdu = setup2.setup_list(s1ap);
                //direction = setup2.direction;
            }
            else if (key == 6)
            {
                nas_pdu = modify2.nasIE(s1ap);
                //direction = modify2.direction;
            }
            else if (key == 9)
            {
                nas_pdu = initial_ue1.initialUe(s1ap);
                //direction = initial_ue1.direction;
            }
            else//不含有nas-pdu
            { ;}
            //解析解码nas-pdu
            if (nas_pdu.Length > 1)//只有当有nas消息时才进行解析，因为定义nas_pdu时分配了1个空间
            {
                nasDecrypt2.decrypt(nas_pdu, direction, kasme);
                identity = nasDecrypt2.identity;
                cause_str = nasDecrypt2.cause_str;
                nas_name = nasDecrypt2.str2 + " " + nasDecrypt2.str1;
                ue_ip = nasDecrypt2.ue_ip;
            }
            else //该s1ap消息不包含nas消息
            {
                identity = "";//UE的标识IMSI或TMSI
                cause_str = "";//nas cause
                nas_name = " ";//nas 名称;
                ue_ip = "";
            }
        }
    }
    public class nasDecrypt0
    {
        public byte esmType = 0, emmType = 0;
        static byte algorithm = 0;//Nas所采用的加密算法由algorithm指示，所以从信令中获得之后一直要保持到下一次有新的algorithm出现为止
        //public byte algorithm1 = 0;//用于在多段关联中使用
        public string str1 = " ", str2 = " ";//str1为esmtype,str2为emmtype
        public string identity = "";//UE的标识IMSI或TMSI
        public string cause_str = "";//nas cause
        //static int Kasme_i = -1;//此变量专门由于更改Kasme的数值使用
        public string ue_ip = "";
        //下面得两行的变量需要保存上一次运行的值故设为全局变量
        static ushort down_overflow = 0, up_overflow = 0;
        static byte re_down_sqn = 0, re_up_sqn = 0;//用于计算count，进行解密使用
        KDF kdf = new KDF();
        EEA1 eea1 = new EEA1();
        EEA2 eea2 = new EEA2();
        EEA3 eea3 = new EEA3();
        hash hash1 = new hash();
        cause cause1 = new cause();
        public static byte[] uintToBytes(uint value)//uint与byte数组的转换 
        {
            #region
            byte[] src = new byte[4];
            src[0] = (byte)((value >> 24) & 0xFF);
            src[1] = (byte)((value >> 16) & 0xFF);
            src[2] = (byte)((value >> 8) & 0xFF);
            src[3] = (byte)(value & 0xFF);
            return src;
            #endregion
        }
        static private bool field(byte a)
        {
            byte[] b = new byte[12] { 0x02, 0x52, 0x62, 0x72, 0x82, 0x92, 0xa2, 0xb2, 0xc2, 0xd2, 0xe2, 0xf2 };//EPS bearer identity的取值+ESM消息标识
            bool c = false;
            for (int i = 0; i < 12; i++)
            {
                if (a == b[i])
                {
                    c = true;
                    break;
                }
                if (i >= 12)
                    c = false;
            }
            return c;
        }
        public void plain(byte[] a)//注意当此函数被调用时，传进来的int i是不同的    
        {
            str1 = " ";
            str2 = " ";
            identity = "";
            cause_str = "";
            ue_ip = "";
            byte proDiscriminator = 0;
            proDiscriminator = (byte)(a[0] & 0x0f);
            if (proDiscriminator == 0x07)//头部为security header identity   
            {
                emmType = a[1];
                str1 = hash1.name(emmType);
                if (emmType == 0x41)//attach request
                {
                    attachRequest req = new attachRequest();
                    byte length1 = 0, length2 = 0;
                    length1 = a[3];
                    length2 = a[length1 + 4];
                    esmType = a[length1 + length2 + 9];
                    str2 = hash1.name(esmType);
                    identity = req.identity(a);
                }
                else if (emmType == 0x42)//attach accept
                {
                    attachAccept acp = new attachAccept();
                    byte length1 = 0, length2 = 0, length3 = 0, length4 = 0, type = 0; int length5 = 0, length6 = 0;
                    length1 = a[4];
                    esmType = a[length1 + 9];
                    length2 = a[length1 + 10];//QOS 长度
                    length3 = a[length1 + 11 + length2];//APN 长度
                    length4 = a[length1 + 12 + length2 + length3];
                    type = a[length1 + 13 + length2 + length3];
                    length5 = length1 + 13 + length2 + length3 + 1;
                    int leng1 = 0;
                    for (leng1 = length5; leng1 < length5 + length4 - 2; leng1++)
                        ue_ip += a[leng1].ToString() + ".";//终端的IP地址    BitConverter.ToString(a,length5,length4-1)
                    ue_ip += a[leng1].ToString();
                    str2 = hash1.name(esmType);
                    identity = acp.Guti(a);
                }
                else if (emmType == 0x43)//attach complete
                {
                    esmType = a[6];//esm message container的编码长度为（5...n）
                    str2 = hash1.name(esmType);
                }
                else if (emmType == 0x44)//attach reject
                {
                    esmType = a[8];//注意attach reject的esm message container的编码与其他的不同（5...n）,它是（6...n）
                    str2 = hash1.name(esmType);
                }
                else if (emmType == 0x45)//detach request(由于detach request的IMSI、TMSI解析函数与attach request完全一致)
                {
                    detachRequest req = new detachRequest();
                    identity = req.identity(a);
                }
                else if (emmType == 0x48)//TAU request
                {
                    TAU_request1 tau_req = new TAU_request1();
                    identity = tau_req.guti(a);
                }
                else if (emmType == 0x49)//TAU accept
                {
                    TAU_accept1 tau_acp = new TAU_accept1();
                    identity = tau_acp.guti(a);
                }
                else if (emmType == 0x50)//GUTI allocation command
                {
                    guti_reall reall = new guti_reall();
                    identity = reall.guti(a);
                }
                else if (emmType == 0x5d)//security mode command
                {
                    algorithm = (byte)((a[2] & 0x70) >> 4);//从该信令中获得所选择的加密算法
                }
            }
            if (proDiscriminator == 0x02)//头部为EPS bearer identity
                esmType = a[2];
            str2 = hash1.name(esmType);
        }
        public void decrypt(byte[] nas_pdu, int direction, List<byte[]> kasme)//数组nas_pdu 是nas数据流
        {
            cause_str = "";
            byte proDiscriminator = 0, EBI = 0;
            byte headType1 = 0, headType = 0;
            byte sqn = 0;
            byte header = 0;
            bool ciper_or = false;
            byte down_sqn = 0, up_sqn = 0;
            uint count = 0;
            List<byte[]> Kasme1 = new List<byte[]>();
            byte[] Kasme = new byte[32];


            byte[] Knas1 = new byte[32];
            byte[] Knas = new byte[16];

            proDiscriminator = (byte)(nas_pdu[0] & 0x0f);
            if (proDiscriminator == 0x07)
                headType1 = (byte)((nas_pdu[0] & 0xf0) >> 4);
            else if (proDiscriminator == 0x02)
                EBI = (byte)((nas_pdu[0] & 0xf0) >> 4);



            //用于判断security header Type=2,4时，加密开关是否关闭，若关闭，则不需要进行解密。 
            if ((headType1 == 1) || (headType1 == 2) || (headType1 == 3) || (headType1 == 4))//当headerType=1，2，3，4时才有nasPlain,sqn,header,ciper_or.
            {
                byte[] nasPlain = new byte[nas_pdu.Length - 6];//nasPlain表示加密的nas部分，但是这只适宜于headertype=1,2,3,4的情况，不适于type=0.          
                sqn = nas_pdu[5];//获得每条信令的序列号(对于service request有特殊情况，该表达式不是其序列号，故需要单独处理)                
                Array.Copy(nas_pdu, 6, nasPlain, 0, nasPlain.Length);
                header = nasPlain[0];
                ciper_or = field(header);
                if (headType1 == 1)
                {
                    #region
                    if (direction == 0)
                    {
                        down_sqn = sqn;
                        if (System.Math.Abs(down_sqn - re_down_sqn) >= 128)
                            down_overflow++;
                        re_down_sqn = down_sqn;
                        count = (uint)(down_overflow << 8 | down_sqn);//把溢出值与序列号拼接
                    }
                    if (direction == 1)
                    {
                        up_sqn = sqn;
                        if (System.Math.Abs(up_sqn - re_up_sqn) >= 128)
                            down_overflow++;
                        re_up_sqn = up_sqn;
                        count = (uint)(up_overflow << 8 | up_sqn);
                    }
                    cause_str = cause1.NasCause(nasPlain);
                    plain(nasPlain);
                    #endregion
                }
                else if (headType1 == 2)
                {
                    #region
                    if (direction == 0)
                    {
                        down_sqn = sqn;
                        if (System.Math.Abs(down_sqn - re_down_sqn) >= 128)
                            down_overflow++;
                        re_down_sqn = down_sqn;
                        count = (uint)(down_overflow << 8 | down_sqn);//把溢出值与序列号拼接
                    }
                    if (direction == 1)
                    {
                        up_sqn = sqn;
                        if (System.Math.Abs(up_sqn - re_up_sqn) >= 128)
                            down_overflow++;
                        re_up_sqn = up_sqn;
                        count = (uint)(up_overflow << 8 | up_sqn);
                    }
                    if ((header == 0x07) || (ciper_or == true))//虽然安全头类型为2，但是不加密
                        plain(nasPlain);
                    else
                    {
                        if (kasme.Count > 0)
                        {
                            //Kasme1 = convert(Kasme_1);//将string的Kasme转为byte数组
                            Kasme = kasme[MainForm.kasmeCount];//在此处获得Kasme数组中合适的Kasme
                            //解密模块，注意三种算法的IK是不同的
                            if (algorithm == 1)
                            {
                                #region
                                UInt32[] IK1 = new UInt32[4];//存放解密所需的IK值（count，方向，identity组成）
                                if (direction == 0)
                                {
                                    IK1[0] = 0;
                                    IK1[1] = count;
                                    IK1[2] = 0;
                                    IK1[3] = count;
                                }
                                if (direction == 1)
                                {
                                    IK1[0] = 0x04000000;
                                    IK1[1] = count;
                                    IK1[2] = 0x04000000;
                                    IK1[3] = count;
                                }
                                byte[] str = new byte[7] { 0x15, 0x01, 0x00, 0x01, 0x01, 0x00, 0x01 };
                                Knas1 = kdf.kdf_decode(Kasme, str);
                                Array.Copy(Knas1, 16, Knas, 0, 16);//获得Knas                               
                                nasPlain = eea1.snow(nasPlain, Knas, IK1);//解析出明文                
                                #endregion
                            }
                            else if (algorithm == 2)
                            {
                                #region
                                byte[] count1 = new byte[4];
                                count1 = uintToBytes(count);
                                byte[] diret = new byte[4];//用于表示direction部分
                                byte[] IK = new byte[16];
                                //EEA2的IK与EEA1不同
                                if (direction == 0)
                                {
                                    diret[0] = 0;
                                    diret[1] = 0;
                                    diret[2] = 0;
                                    diret[3] = 0;
                                }
                                if (direction == 1)
                                {
                                    diret[0] = 0x04;
                                    diret[1] = 0;
                                    diret[2] = 0;
                                    diret[3] = 0;
                                }
                                Array.Copy(count1, 0, IK, 0, 4);
                                Array.Copy(diret, 0, IK, 4, 4);
                                byte[] str = new byte[7] { 0x15, 0x01, 0x00, 0x01, 0x02, 0x00, 0x01 };
                                Knas1 = kdf.kdf_decode(Kasme, str);
                                Array.Copy(Knas1, 16, Knas, 0, 16);//获得Knas
                                nasPlain = eea2.aes(nasPlain, Knas, IK);//解析明文
                                #endregion
                            }
                            else if (algorithm == 3)
                            {
                                #region
                                byte[] count1 = new byte[4];
                                count1 = uintToBytes(count);
                                byte[] diret = new byte[4];//用于表示direction部分
                                byte[] IK = new byte[32];
                                if (direction == 0)
                                {
                                    diret[0] = 0;
                                    diret[1] = 0;
                                    diret[2] = 0;
                                    diret[3] = 0;
                                }
                                if (direction == 1)
                                {
                                    diret[0] = 0x04;
                                    diret[1] = 0;
                                    diret[2] = 0;
                                    diret[3] = 0;
                                }
                                //注意三种算法的IK计算方法不同（程序中的算法有待实际验证）
                                Array.Copy(diret, 0, IK, 0, 4);
                                Array.Copy(count1, 0, IK, 4, 4);
                                Array.Copy(diret, 0, IK, 8, 4);
                                Array.Copy(count1, 0, IK, 12, 4);
                                byte[] str = new byte[7] { 0x15, 0x01, 0x00, 0x01, 0x03, 0x00, 0x01 };
                                Knas1 = kdf.kdf_decode(Kasme, str);
                                Array.Copy(Knas1, 16, Knas, 0, 16);//获得Knas
                                nasPlain = eea3.zuc(nasPlain, Knas, IK);
                                #endregion
                            }
                            plain(nasPlain);
                            cause_str = cause1.NasCause(nasPlain);
                        }
                        else//没有获得密钥
                        {
                            identity = "";
                            cause_str = "";
                            ue_ip = "";
                            str2 = "ciperd message";
                        }
                    }

                    #endregion
                }
                else if (headType1 == 3)
                {
                    #region
                    down_sqn = 0;
                    down_overflow = 0;
                    count = (uint)(down_overflow << 8 | down_sqn);
                    cause_str = cause1.NasCause(nasPlain);
                    plain(nasPlain);
                    #endregion
                }
                else if (headType1 == 4)
                {
                    #region

                    up_sqn = 0;
                    up_overflow = 0;
                    count = (uint)(up_overflow << 8 | up_sqn);
                    if ((header == 0x07) || (ciper_or == true))//虽然安全头类型为4，但是不加密
                        plain(nasPlain);
                    else
                    //解密模块，注意三种算法的IK是不同的
                    {
                        //只有需要解密时，才能更改或获得Kasme，因为即使headType==2,4也不一定会加密，也与加密开关有关系
                        MainForm.kasmeCount++;//当headertype==4时，要更新Kasme
                        if (kasme.Count > 0)
                        {
                            //Kasme1 = convert(Kasme_1);//将string的Kasme转为byte数组
                            Kasme = kasme[MainForm.kasmeCount];//在此处获得Kasme数组中合适的Kasme
                            #region
                            if (algorithm == 1)
                            {
                                #region
                                UInt32[] IK1 = new UInt32[4];//存放解密所需的IK值（count，方向，identity组成）
                                if (direction == 0)
                                {
                                    IK1[0] = 0;
                                    IK1[1] = count;
                                    IK1[2] = 0;
                                    IK1[3] = count;
                                }
                                if (direction == 1)
                                {
                                    IK1[0] = 0x04000000;
                                    IK1[1] = count;
                                    IK1[2] = 0x04000000;
                                    IK1[3] = count;
                                }
                                byte[] str = new byte[7] { 0x15, 0x01, 0x00, 0x01, 0x01, 0x00, 0x01 };
                                Knas1 = kdf.kdf_decode(Kasme, str);
                                Array.Copy(Knas1, 16, Knas, 0, 16);//获得Knas
                                nasPlain = eea1.snow(nasPlain, Knas, IK1);//解析出明文                
                                #endregion
                            }
                            else if (algorithm == 2)
                            {
                                #region
                                byte[] count1 = new byte[4];
                                count1 = uintToBytes(count);
                                byte[] diret = new byte[4];//用于表示direction部分
                                byte[] IK = new byte[16];
                                //EEA2的IK与EEA1不同
                                if (direction == 0)
                                {
                                    diret[0] = 0;
                                    diret[1] = 0;
                                    diret[2] = 0;
                                    diret[3] = 0;
                                }
                                if (direction == 1)
                                {
                                    diret[0] = 0x04;
                                    diret[1] = 0;
                                    diret[2] = 0;
                                    diret[3] = 0;
                                }
                                Array.Copy(count1, 0, IK, 0, 4);
                                Array.Copy(diret, 0, IK, 4, 4);
                                byte[] str = new byte[7] { 0x15, 0x01, 0x00, 0x01, 0x02, 0x00, 0x01 };
                                Knas1 = kdf.kdf_decode(Kasme, str);
                                Array.Copy(Knas1, 16, Knas, 0, 16);//获得Knas
                                nasPlain = eea2.aes(nasPlain, Knas, IK);//解析明文
                                #endregion
                            }
                            else if (algorithm == 3)
                            {
                                #region
                                byte[] count1 = new byte[4];
                                count1 = uintToBytes(count);
                                byte[] diret = new byte[4];//用于表示direction部分
                                byte[] IK = new byte[32];
                                if (direction == 0)
                                {
                                    diret[0] = 0;
                                    diret[1] = 0;
                                    diret[2] = 0;
                                    diret[3] = 0;
                                }
                                if (direction == 1)
                                {
                                    diret[0] = 0x04;
                                    diret[1] = 0;
                                    diret[2] = 0;
                                    diret[3] = 0;
                                }
                                //注意三种算法的IK计算方法不同（程序中的算法有待实际验证）
                                Array.Copy(diret, 0, IK, 0, 4);
                                Array.Copy(count1, 0, IK, 4, 4);
                                Array.Copy(diret, 0, IK, 8, 4);
                                Array.Copy(count1, 0, IK, 12, 4);
                                byte[] str = new byte[7] { 0x15, 0x01, 0x00, 0x01, 0x03, 0x00, 0x01 };
                                Knas1 = kdf.kdf_decode(Kasme, str);
                                Array.Copy(Knas1, 16, Knas, 0, 16);//获得Knas
                                nasPlain = eea3.zuc(nasPlain, Knas, IK);
                                #endregion
                            }
                            //decrypt2();//解密函数（密钥更新）
                            #endregion
                            plain(nasPlain);
                            cause_str = cause1.NasCause(nasPlain);
                        }
                        else //没有从S6a口获得密钥
                        {
                            identity = "";
                            cause_str = "";
                            ue_ip = "";
                            str2 = "ciperd message";
                        }
                    }
                    #endregion
                }
            }
            else if (headType1 == 0)//不加密
            {
                plain(nas_pdu);//注意函数中的参数是nas_pdu，与上面的nasPlain不同
                cause_str = cause1.NasCause(nas_pdu);
            }
            else //此处表示其余的情况全部视为service request
            {
                #region
                sqn = (byte)(nas_pdu[1] & 0x1f);//service request 的序列号获取
                if (direction == 0)
                {
                    down_sqn = sqn;
                    if (System.Math.Abs(down_sqn - re_down_sqn) >= 128)
                        down_overflow++;
                    re_down_sqn = down_sqn;
                    count = (uint)(down_overflow << 8 | down_sqn);//把溢出值与序列号拼接
                }
                if (direction == 1)
                {
                    up_sqn = sqn;
                    if (System.Math.Abs(up_sqn - re_up_sqn) >= 128)
                        down_overflow++;
                    re_up_sqn = up_sqn;
                    count = (uint)(up_overflow << 8 | up_sqn);
                }
                str2 = "service request";
                esmType = 255;//由于service request既不是EMM消息也不是ESM消息，故没有其编号，暂定为255.
                #endregion
            }
        }
    }
}

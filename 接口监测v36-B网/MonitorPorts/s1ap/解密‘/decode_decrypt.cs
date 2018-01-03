using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nasCause;
namespace MonitorPorts
{
    public class nasDecrypt1
    {
        //辅助显示信元对应的编码
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

        public byte esmType = 0, emmType = 0;
        static byte algorithm = 0;//Nas所采用的加密算法由algorithm指示，所以从信令中获得之后一直要保持到下一次有新的algorithm出现为止       
        public string EmmMessage = " ", EsmMessage = " ";//EmmMessage为esmtype,EsmMessage为emmtype
        public string identity = "";//UE的标识IMSI或TMSI
        public string cause_str = "";//nas cause
        static int Kasme_i = -1;//此变量专门由于更改Kasme的数值使用
        public string ue_ip = "";//NAS信令中分配的终端的IP地址
        //下面得两行的变量需要保存上一次运行的值故设为全局变量
        static ushort down_overflow = 0, up_overflow = 0;//用于辅助计算count值
        static byte re_down_sqn = 0, re_up_sqn = 0;//用于计算count，进行解密使用
        /// <summary>
        /// 加密算法的第一步，根密钥Kasme经过kdf算法加密转换为nas加密密钥Knasenc
        /// </summary>
        KDF kdf = new KDF();
        /// <summary>
        /// 加密算法第二步，Knasenc经过下面三种算法中其中一种算法加密转换成密钥流
        /// </summary>
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
        /// <summary>
        /// 判断nas消息是不是ESM message
        /// </summary>
        /// <param name="a">特征字节</param>
        /// <returns>返回true，表示该字节所在的信令属于ESM message；否则，是EMM message</returns>
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
        /// <summary>
        /// 该方法用于解析明文的nas消息，该方法解析出nas信令中所包含的esm信令名称，emm信令名称，
        /// 用户标识，终端IP等信息
        /// </summary>
        /// <param name="plainMessage">nas编码（从plain NAS mnessage到整个nas编码结束）</param>       
        public void plain(byte[] plainMessage)
        {
            //清空变量
            EmmMessage = " ";
            EsmMessage = " ";
            identity = "";
            cause_str = "";
            ue_ip = "";

            byte proDiscriminator = 0;//指示安全头类型（security header type)及nas信令类型(ESM message或Emm message)
            proDiscriminator = (byte)(plainMessage[0] & 0x0f);
            if (proDiscriminator == 0x07)//头部为security header identity   
            {
                LengthEmm = 1;
                IndexEmm = 1;

                emmType = plainMessage[1];//特征字节，指示该nas信令的具体名称
                EmmMessage = hash1.name(emmType);//EmmMessage具体的emm message名称
                if (emmType == 0x41)//attach request
                {
                    attachRequest req = new attachRequest();
                    byte length1 = 0, length2 = 0;//length1表示EPS mobile identity的编码长度，length2表示UE network capability的编码长度
                    length1 = plainMessage[3];
                    length2 = plainMessage[length1 + 4];
                    IndexEsm = length1 + length2 + 9;
                    LengthEsm = 1;
                    esmType = plainMessage[length1 + length2 + 9];
                    EsmMessage = hash1.name(esmType);
                    identity = req.identity(plainMessage);//attach request 中的用户标识
                    IndexIdentity=req.Index;
                    LengthIdentity=req.Length;
                }
                else if (emmType == 0x42)//attach accept
                {
                    attachAccept acp = new attachAccept();
                    byte length1 = 0, length2 = 0, length3 = 0,
                            length4 = 0;
                    int length5 = 0;
                    length1 = plainMessage[4];//TAI list的编码长度
                    IndexEsm = 4;
                    LengthEsm = 1;
                    esmType = plainMessage[length1 + 9];
                    length2 = plainMessage[length1 + 10];//　ESM　message　container中QOS 长度
                    length3 = plainMessage[length1 + 11 + length2];//ESM　message　container中APN 长度
                    length4 = plainMessage[length1 + 12 + length2 + length3];//ESM　message　container中PDN address
                    length5 = length1 + 13 + length2 + length3 + 1;//UE ip的编码长度
                    int leng1 = length5;
                    LengthIP =length4 - 1;
                    IndexIP = length5;
                    for (; leng1 < length5 + length4 - 2; leng1++)
                        ue_ip += plainMessage[leng1].ToString() + ".";//终端的IP地址    BitConverter.ToString(a,length5,length4-1)
                    ue_ip += plainMessage[leng1].ToString();
                    EsmMessage = hash1.name(esmType);
                    identity = acp.Guti(plainMessage);
                    IndexIdentity = acp.Index;
                    LengthIdentity = acp.Length;
                }
                else if (emmType == 0x43)//attach complete
                {
                    IndexEsm = 6;
                    LengthEsm = 1;
                    esmType = plainMessage[6];//esm message container的编码长度为（5...n）
                    EsmMessage = hash1.name(esmType);
                }
                else if (emmType == 0x44)//attach reject
                {
                    if (plainMessage.Length > 9)
                    {
                        IndexEsm = 8;
                        LengthEsm = 1;
                        esmType = plainMessage[8];//注意attach reject的esm message container的编码与其他的不同（5...n）,它是（6...n）
                    }                       
                    else
                        ;
                    EsmMessage = hash1.name(esmType);
                }
                else if (emmType == 0x45)//detach request(由于detach request的IMSI、TMSI解析函数与attach request完全一致)
                {
                    detachRequest req = new detachRequest();
                    identity = req.identity(plainMessage);
                    IndexIdentity = req.Index;
                    LengthIdentity = req.Length;
                }
                else if (emmType == 0x48)//TAU request
                {
                    TAU_request1 tau_req = new TAU_request1();
                    identity = tau_req.guti(plainMessage);
                    IndexIdentity = tau_req.Index;
                    LengthIdentity = tau_req.Length;
                }
                else if (emmType == 0x49)//TAU accept
                {
                    TAU_accept1 tau_acp = new TAU_accept1();
                    identity = tau_acp.guti(plainMessage);
                    IndexIdentity = tau_acp.Index;
                    LengthIdentity = tau_acp.Length;
                }
                else if (emmType == 0x50)//GUTI allocation command
                {
                    guti_reall reall = new guti_reall();
                    identity = reall.guti(plainMessage);
                    IndexIdentity = reall.Index;
                    LengthIdentity = reall.Length;
                }
                else if (emmType == 0x5d)//security mode command
                {
                    algorithm = (byte)((plainMessage[2] & 0x70) >> 4);//从该信令中获得所选择的加密算法                   
                }
            }
            if (proDiscriminator == 0x02)//头部为EPS bearer identity
            {               
                if (plainMessage.Length > 2)
                {
                    IndexEsm = 2;
                    LengthEsm = 1;
                    esmType = plainMessage[2];
                }
                    
                else //数据不完整
                    ;
            }
            EsmMessage = hash1.name(esmType);
        }
        /// <summary>
        /// 该方法先判断输入的nas信令是否加密，如果没有加密，则直接调用明文解析方法plain()进行解析；
        /// 否则先进行解密，再进行明文解析
        /// </summary>
        /// <param name="nas_pdu">nas信令编码</param>
        /// <param name="direction">该nas信令所在的s1ap信令的方向</param>
        /// <param name="Kasme_1">从diameter信令中获得的Kasme list</param>
        public void decrypt(byte[] nas_pdu, byte direction, List<byte[]> Kasme_1)
        {
            // 清空数据
            cause_str = "";
            byte proDiscriminator = 0, EBI = 0;
            byte headType = 0;
            byte sqn = 0;
            byte header = 0;
            bool ciper_or = false;
            byte down_sqn = 0, up_sqn = 0;
            uint count = 0;
            List<byte[]> Kasme1 = new List<byte[]>();
            byte[] Kasme = new byte[32];
            byte[] Knas1 = new byte[32];
            byte[] Knas = new byte[16];

            proDiscriminator = (byte)(nas_pdu[0] & 0x0f);//指示nas信令类型是ESM or EMM
            if (proDiscriminator == 0x07)//EMM 消息
                headType = (byte)((nas_pdu[0] & 0xf0) >> 4);
            else if (proDiscriminator == 0x02) //ESM消息
                EBI = (byte)((nas_pdu[0] & 0xf0) >> 4); //EPS bear identity

            //用于判断security header Type=2,4时，加密开关是否关闭，若关闭，则不需要进行解密。 
            if ((headType == 1) || (headType == 2) || (headType == 3) || (headType == 4))//当headerType=1，2，3，4时才有nasPlain,sqn,header,ciper_or.
            {
                byte[] nasPlain = new byte[nas_pdu.Length - 6];//nasPlain表示加密的nas部分，但是这只适宜于headertype=1,2,3,4的情况，不适于type=0.          
                sqn = nas_pdu[5];//获得每条信令的序列号(对于service request有特殊情况，该表达式不是其序列号，故需要单独处理)
                Array.Copy(nas_pdu, 6, nasPlain, 0, nasPlain.Length);//当nas不是plain message时（或者完整性保护，或者加密），去掉nas_pdu安全头部
                //把剩余部分放在nasPlain中。
                header = nasPlain[0];//特征字节，指示nas信令是否属于nas  esm 消息
                ciper_or = field(header);//用于判断是否打开加密开关，如果是true，则加密开关关闭，不加密
                if (headType == 1)//只进行完整性保护，不加密
                {
                    #region
                    //用于计算加密所使用的参数COUNT
                    if (direction == 0)//方向下行（MME->eNB）
                    {
                        down_sqn = sqn;
                        if (System.Math.Abs(down_sqn - re_down_sqn) >= 128)
                            down_overflow++;
                        re_down_sqn = down_sqn;
                        count = (uint)(down_overflow << 8 | down_sqn);//把溢出值与序列号拼接
                    }
                    if (direction == 1)//方向上行（eNB->MME）
                    {
                        up_sqn = sqn;
                        if (System.Math.Abs(up_sqn - re_up_sqn) >= 128)
                            down_overflow++;
                        re_up_sqn = up_sqn;
                        count = (uint)(up_overflow << 8 | up_sqn);
                    } 

                    cause_str = cause1.NasCause(nasPlain);//解析nas信令中的 cause
                    IndexCause = cause1.Index;
                    LengthCause = cause1.Length;
                    plain(nasPlain);
                    #endregion
                }
                else if (headType == 2)//完整性保护并加密(是否真的加密还需要判断加密开关是否打开)
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
                    if ((header == 0x07) || (ciper_or == true))//虽然安全头类型为2，但是加密开关关闭故不加密
                    {
                        plain(nasPlain);
                        IndexCause = cause1.Index;
                        LengthCause = cause1.Length;
                    }
                    else
                    {
                        if (Kasme_1.Count > 0)//diameter信令分配有根密钥
                        {                          
                            Kasme = Kasme_1[Kasme_i];//在此处获得Kasme数组中合适的Kasme
                            //解密模块，注意三种算法的IK是不同的
                            if (algorithm == 1)//加密算法选择EEA1，即snow 3G，下面的解密方法具体见EEA1和KDF算法的介绍
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
                                byte[] str = new byte[7] { 0x15, 0x01, 0x00, 0x01, 0x01, 0x00, 0x01};
                                Knas1 = kdf.kdf_decode(Kasme, str);
                                Array.Copy(Knas1, 16, Knas, 0, 16);//获得Knas                               
                                nasPlain = eea1.snow(nasPlain, Knas, IK1);//解析出明文                
                                #endregion
                            }
                            else if (algorithm == 2)//加密算法选择EEA2，即AES，下面的解密方法具体见EEA2和KDF算法的介绍
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
                            else if (algorithm == 3)//加密算法选择EEA2，即ZUC，下面的解密方法具体见EEA3和KDF算法的介绍
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
                            IndexCause = cause1.Index;
                            LengthCause = cause1.Length;
                            cause_str = cause1.NasCause(nasPlain);
                        }
                        else//没有从S6a口获得密钥
                        {
                            identity = "";
                            cause_str = "";
                            ue_ip = "";
                            EsmMessage = "ciperd message";
                        }
                    }

                    #endregion
                }
                else if (headType == 3)//只进行完整性保护
                {
                    #region
                    down_sqn = 0;
                    down_overflow = 0;
                    count = (uint)(down_overflow << 8 | down_sqn);
                    cause_str = cause1.NasCause(nasPlain);
                    plain(nasPlain);
                    IndexCause = cause1.Index;
                    LengthCause = cause1.Length;
                    #endregion
                }
                else if (headType == 4)//使用新的安全上下文中的KASME加密，故在这里需要更新加密密钥
                {
                    #region
                    up_sqn = 0;
                    up_overflow = 0;
                    count = (uint)(up_overflow << 8 | up_sqn);
                    if ((header == 0x07) || (ciper_or == true))//虽然安全头类型为4，但是不加密
                    {
                        plain(nasPlain);
                        cause_str = cause1.NasCause(nasPlain);
                        IndexCause = cause1.Index;
                        LengthCause = cause1.Length;
                    }                      
                    else
                    //解密模块，注意三种算法的IK是不同的
                    {
                        //只有需要解密时，才能更改或获得Kasme，因为即使headType==2,4也不一定会加密，也与加密开关有关系
                        Kasme_i++;//当headertype==4时，要更新Kasme
                        if (Kasme_1.Count > 0)
                        {                         
                            Kasme = Kasme_1[Kasme_i];//在此处获得Kasme数组中合适的Kasme
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
                            IndexCause = cause1.Index;
                            LengthCause = cause1.Length;
                        }
                        else //没有从S6a口获得密钥
                        {
                            identity = "";
                            cause_str = "";
                            ue_ip = "";
                            EsmMessage = "ciperd message";
                        }
                    }
                    #endregion
                }
                //因为此处还要包括安全头部的6字节
               IndexEsm=IndexEsm +6;
               IndexEmm = IndexEmm + 6;
               IndexCause = IndexCause + 6;
               IndexIP = IndexIP + 6;
               IndexIdentity = IndexIdentity + 6;
            }
            else if (headType == 0)//不加密，也不进行完整性保护
            {
                plain(nas_pdu);
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
                IndexEsm = 0;
                LengthEmm = 1;
                EsmMessage = "service request";
                esmType = 255;//由于service request既不是EMM消息也不是ESM消息，故没有其编号，暂定为255.
                #endregion
            }
        }
    }
}

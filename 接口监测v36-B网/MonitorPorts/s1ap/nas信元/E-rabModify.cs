using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MonitorPorts
{
    /// <summary>
    /// 该类是为了解析在信令E-rab modify  request中的nas编码
    /// </summary>
    public class ErabModifyReq
    {
        public byte direction = 1;//E-rab modify request 信令方向下行
        public int LengthS1ap = 0;//指示nas前面的s1ap的长度
        /// <summary>
        /// 该方法将输入的E-rab modify  request信令编码中截取nas编码
        /// </summary>
        /// <param name="s1ap">s1ap信令编码</param>
        /// <returns>nas信令编码</returns>
        public byte[] nasIE(byte[] s1ap)
        {
            #region//变量声明
            ushort tlength = 0;//s1ap信令的所有信元编码总长度
            ushort slength = 0;//单个信元的编码长度
            ushort listLength = 0;
            ushort itemLength = 0;
            ushort nasLength = 1;
            ushort nasLength1 = 0;
            ushort len1 = 0;//用于判定总长度所占字节的变量
            ushort len2 = 0;//用于判定单个信元长度所占字节的变量
            ushort len3 = 0;
            ushort len4 = 0;
            ushort id = 0;            
            byte Gbr_or = 0;//用来判定e-rab level qos parameter中是否有GBR（16byte）            
            List<byte> nas = new List<byte>();
            byte[] nasShuzu = new byte[nasLength];//用于接收nas的内容
            #endregion
            s1ap_name dir = new s1ap_name();

            dir.s1ap_decode1(s1ap);//获取信令的direction
            direction = (byte)dir.direction;

            len1 = s1ap[3];//用于判断当前信元的长度
            if (len1 >= 128)
                tlength = (ushort)((s1ap[3] & 0x0f) * 256 + s1ap[4]);
            else
                tlength = s1ap[3];
            if (tlength >= 128)//信令编码总长度大于128
            {
                #region
                for (int i = 8; i < s1ap.Length; )
                {
                    id = (ushort)(s1ap[i] * 256 + s1ap[i + 1]);
                    if (id == 30)
                    {
                        #region
                        len2 = s1ap[i + 3];//用来判断list信元的长度
                        if (len2 >= 128)//list信元的长度大于128
                        {
                            #region
                            slength = (ushort)((s1ap[i + 3] & 0x0f) * 256 + s1ap[i + 4]);
                            listLength = 2;//该信元长度域占2字节
                            len3 = s1ap[i + 9];//用来判断item信元的长度

                            if (len3 >= 128)//item的长度                         
                            {
                                #region
                                itemLength = (ushort)((s1ap[i + 9] & 0x0f) * 256 + s1ap[i + 10]);
                                Gbr_or = s1ap[i+12];//该字节包含e-rab level qos parameter的标志位，若为80则，含有Gbr信元；若为00则不含GBR
                                
                                if (Gbr_or == 0x00)
                                {
                                    #region
                                    len4 = s1ap[i + 15];//用于判断nas长度
                                    if (len4 >= 128)//nas长度
                                    {
                                        nasLength = (ushort)((s1ap[i + 15] & 0x0f) * 256 + s1ap[i + 16]);
                                        nasLength1 = 2;//nas 长度域所占字节
                                        byte[] b1 = new byte[nasLength];
                                        Array.Copy(s1ap, i + 17, b1, 0, nasLength);
                                        LengthS1ap = i + 17;
                                        nasShuzu = b1;
                                        break;
                                    }
                                    else//nas长度
                                    {
                                        nasLength = len4;
                                        nasLength1 = 1;
                                        byte[] b1 = new byte[nasLength];
                                        Array.Copy(s1ap, i + 16, b1, 0, nasLength);
                                        LengthS1ap = i + 16;
                                        nasShuzu = b1;
                                        break;
                                    }
                                    #endregion
                                }
                                else if (Gbr_or == 0x80)//该字节包含e-rab level qos parameter的标志位，若为80则，含有Gbr信元；若为00则不含GBR
                                {
                                    #region    
                               
                                    int bitrate1_len = ((s1ap[i + 15] & 0x38) >> 3)+2;                                   
                                    int bitrate2_len = ((s1ap[i + 15 + bitrate1_len] & 0xe0) >> 5 )+ 2;                                    
                                    int bitrate3_len = ((s1ap[i + 15 + bitrate1_len + bitrate2_len] & 0xe0) >> 5) + 2;                                   
                                    int bitrate4_len = ((s1ap[i + 15 + bitrate1_len + bitrate2_len + bitrate3_len] & 0xe0) >> 5) + 2;
                                    int bitrate_len = bitrate1_len + bitrate2_len + bitrate3_len + bitrate4_len;
                                    len4 = s1ap[i + 15 + bitrate_len];//用于判断nas长度，GBR占16字节
                                    if (len4 >= 128)//nas长度
                                    {
                                        nasLength = (ushort)((s1ap[i + 15 + bitrate_len] & 0x0f) * 256 + s1ap[i + 16 + bitrate_len]);
                                        nasLength1 = 2;//nas 长度域所占字节
                                        byte[] b1 = new byte[nasLength];
                                        Array.Copy(s1ap, i + 17 + bitrate_len, b1, 0, nasLength);
                                        LengthS1ap = i + 17 + bitrate_len;
                                        nasShuzu = b1;
                                        break;
                                    }
                                    else//nas长度
                                    {
                                        nasLength = len4;
                                        byte[] b1 = new byte[nasLength];
                                        Array.Copy(s1ap, i + 16 + bitrate_len, b1, 0, nasLength);
                                        nasShuzu = b1;
                                        LengthS1ap = i + 16 + bitrate_len;
                                        break;
                                    }
                                    #endregion
                                }
                              
                                #endregion
                            }
                            else//item的长度小于128
                            {
                                #region
                                Gbr_or = s1ap[i+11];
                                if (Gbr_or == 0x80)
                                {
                                    int bitrate1_len = ((s1ap[i + 14] & 0x38) >> 3) + 2;                            
                                    int bitrate2_len = ((s1ap[i + 14 + bitrate1_len] & 0xe0) >> 5) + 2;
                                    int bitrate3_len = ((s1ap[i + 14 + bitrate1_len + bitrate2_len] & 0xe0) >> 5) + 2;
                                    int bitrate4_len = ((s1ap[i + 14 + bitrate1_len + bitrate2_len + bitrate3_len] & 0xe0) >> 5) + 2;
                                    int bitrate_len = bitrate1_len + bitrate2_len + bitrate3_len + bitrate4_len;
                                    len4 = s1ap[i + 14 + bitrate_len];//用于判断nas长度，GBR占16字节
                                    nasLength = s1ap[i + 14 + bitrate_len];
                                    byte[] b1 = new byte[nasLength];
                                    Array.Copy(s1ap, i + 15 + bitrate_len, b1, 0, nasLength);
                                    nasShuzu = b1;
                                    LengthS1ap = i + 15 + bitrate_len;
                                    break;
                                }
                                else if (Gbr_or == 0x00)
                                {
                                    nasLength = s1ap[i+14];
                                    byte[] b1 = new byte[nasLength];
                                    Array.Copy(s1ap, i + 15, b1, 0, nasLength);
                                    nasShuzu = b1;
                                    LengthS1ap = i + 15;
                                    break;
                                }                               
                                #endregion
                            }
                            #endregion
                        }
                        else//list的长度小于128
                        {
                            #region
                            Gbr_or = s1ap[i+10];
                            if (Gbr_or == 0x80)
                            {
                                int bitrate1_len = ((s1ap[i + 13] & 0x38) >> 3) + 2;
                                int bitrate2_len = ((s1ap[i + 13 + bitrate1_len] & 0xe0) >> 5) + 2;
                                int bitrate3_len = ((s1ap[i + 13 + bitrate1_len + bitrate2_len] & 0xe0) >> 5) + 2;
                                int bitrate4_len = ((s1ap[i + 13 + bitrate1_len + bitrate2_len + bitrate3_len] & 0xe0) >> 5) + 2;
                                int bitrate_len = bitrate1_len + bitrate2_len + bitrate3_len + bitrate4_len;
                                nasLength = s1ap[i + 13 + bitrate_len];
                                byte[] b1 = new byte[nasLength];
                                Array.Copy(s1ap, i + 14 + bitrate_len, b1, 0, nasLength);
                                nasShuzu = b1;
                                LengthS1ap = i + 14 + bitrate_len;
                                break;
                            }
                            else if (Gbr_or == 0x00)
                            {
                                nasLength = s1ap[i + 13];
                                byte[] b1 = new byte[nasLength];
                                Array.Copy(s1ap, i + 14, b1, 0, nasLength);
                                nasShuzu = b1;
                                LengthS1ap = i + 14;
                                break;
                            }
                         
                            #endregion
                        }
                        #endregion
                    }
                    else//id !=30
                    {
                        #region
                        slength = s1ap[i + 3];
                        if (slength < 128)
                            i = (ushort)(i + slength + 4);
                        else
                        {
                            slength = (ushort)((s1ap[i + 3] & 0x0f) * 256 + s1ap[i + 4]);
                            i = (ushort)(i + slength + 5);//当信元的编码长度超过128时，则长度域的编码为0x80yy（2个字节）,且yy才是实际的编码长度
                        }
                        #endregion
                    }
                }
                #endregion
            }
            else//tlength < 128
            {
                #region
                for (int i = 7; i < s1ap.Length; )
                {
                    id = (ushort)(s1ap[i] * 256 + s1ap[i + 1]);
                    slength = s1ap[i + 3];
                    if (id == 30)
                    {
                        #region                       
                        listLength = 1;//该信元长度域占1字节
                        Gbr_or = s1ap[i + 10];
                        if (Gbr_or == 0x80)
                        {
                            int bitrate1_len = ((s1ap[i + 13] & 0x38) >> 3) + 2;
                            int bitrate2_len = ((s1ap[i + 13 + bitrate1_len] & 0xe0) >> 5) + 2;
                            int bitrate3_len = ((s1ap[i + 13 + bitrate1_len + bitrate2_len] & 0xe0) >> 5) + 2;
                            int bitrate4_len =((s1ap[i + 13 + bitrate1_len + bitrate2_len + bitrate3_len] & 0xe0) >> 5) + 2;
                            int bitrate_len = bitrate1_len + bitrate2_len + bitrate3_len + bitrate4_len;
                            nasLength = s1ap[i + 13 + bitrate_len];
                            byte[] b1 = new byte[nasLength];
                            Array.Copy(s1ap, i + 14 + bitrate_len, b1, 0, nasLength);
                            nasShuzu = b1;
                            LengthS1ap = i + 14 + bitrate_len;
                            break;//找到该信元以后，解析之后直接跳出循环
                        }
                        else if (Gbr_or == 0x00)
                        {
                            nasLength = s1ap[i + 13];
                            byte[] b1 = new byte[nasLength];
                            Array.Copy(s1ap, i + 14, b1, 0, nasLength);
                            nasShuzu = b1;
                            LengthS1ap = i + 14;
                            break;//找到该信元以后，解析之后直接跳出循环
                        }                        
                        #endregion
                    }
                    if (id != 30)
                    {
                        i = (ushort)(i + slength + 4);
                    }
                }
                #endregion
            }
            return nasShuzu;
        }
    }
}

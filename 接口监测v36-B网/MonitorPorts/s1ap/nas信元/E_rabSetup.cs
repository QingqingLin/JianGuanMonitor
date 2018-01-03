using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MonitorPorts
{
    public class setup1
    {
        public int LengthS1ap = 0;//指示nas前面的s1ap的长度
        public byte direction = 1;//E-rab setuo request的方向是
        public byte[] setup_list(byte[] a)
        {
            ushort tlength = 0;//s1ap信令的所有信元编码总长度
            ushort slength = 0;//单个信元的编码长度
            ushort listLength = 0;
            ushort itemLength = 0;
            ushort itemLength1 = 0;
            ushort nasLength = 1;
            ushort nasLength1 = 0;
            ushort len1 = 0;//用于判定总长度所占字节的变量
            ushort len2 = 0;//用于判定单个信元长度所占字节的变量
            ushort len3 = 0;
            ushort len4 = 0;
           
            ushort id = 0;
            byte Gbr_or = 0;//用来判定e-rab level qos parameter中是否有GBR（16byte）            

            byte[] nasShuzu = new byte[nasLength];

            s1ap_name dir = new s1ap_name();

            dir.s1ap_decode1(a);//获取信令的direction
            direction = (byte)dir.direction;

            len1 = a[3];//用于判断当前信元的长度
            if (len1 >= 128)
                tlength = (ushort)((a[3] & 0x0f) * 256 + a[4]);
            else
                tlength = a[3];
            if (tlength >= 128)//信令编码总长度大于128
            {
                for (int i = 8; i < a.Length; )
                {
                    id = (ushort)(a[i] * 256 + a[i + 1]);
                    if (id == 16)
                    {
                        #region
                        len2 = a[i + 3];//用来判断list信元的长度
                        if (len2 >= 128)//list的长度大于128
                        {
                            #region
                            slength = (ushort)((a[i + 3] & 0x0f) * 256 + a[i + 4]);
                            listLength = 2;//该信元长度域占2字节
                            len3 = a[i + 9];//用来判断item信元的长度
                            if (len3 >= 128)//item的长度                         
                            {
                                #region
                                itemLength = (ushort)((a[i + 9] & 0x0f) * 256 + a[i + 10]);
                                Gbr_or = a[i+12];//该字节包含e-rab level qos parameter的标志位，若为80则，含有Gbr信元；若为00则不含GBR
                              
                                if (Gbr_or == 0x00)
                                {
                                    #region
                                    len4 = a[i + 25];//用于判断nas长度
                                    if (len4 >= 128)//nas长度
                                    {
                                        nasLength = (ushort)((a[i + 25] & 0x0f) * 256 + a[i + 26]);
                                        nasLength1 = 2;//nas 长度域所占字节 
                                        byte[] b1 = new byte[nasLength];
                                        Array.Copy(a, i + 27,b1, 0, nasLength);
                                        nasShuzu = b1;
                                        LengthS1ap = i + 27;
                                        break;
                                    }
                                    else//nas长度
                                    {
                                        nasLength = len4;
                                        nasLength1 = 1;
                                        byte[] b1 = new byte[nasLength];
                                        Array.Copy(a, i + 26, b1, 0, nasLength);
                                        nasShuzu = b1;
                                        LengthS1ap = i + 26;
                                        break;
                                    }
                                    #endregion
                                }
                                else if (Gbr_or == 0x80)//该字节包含e-rab level qos parameter的标志位，若为80则，含有Gbr信元；若为00则不含GBR
                                {
                                    #region
                                    int bitrate1_len = ((a[i + 15] & 0x38) >> 3) + 2;
                                    //byte bitrate2 = a[i + 15 + bitrate1_len];
                                    int bitrate2_len = ((a[i + 15 + bitrate1_len] & 0xe0) >> 5) + 2;
                                    //byte bitrate3 = a[i + 15 + bitrate1_len+bitrate2_len];
                                    int bitrate3_len = ((a[i + 15 + bitrate1_len + bitrate2_len] & 0xe0) >> 5) + 2;
                                    //byte bitrate4 = a[i + 15 + bitrate1_len + bitrate2_len + bitrate3_len];
                                    int bitrate4_len = ((a[i + 15 + bitrate1_len + bitrate2_len + bitrate3_len] & 0xe0)>> 5) + 2;
                                    int bitrate_len = bitrate1_len + bitrate2_len + bitrate3_len + bitrate4_len;
                                    len4 = a[i + 15 + bitrate_len+10];//用于判断nas长度，GBR占16字节
                                    if (len4 >= 128)//nas长度
                                    {
                                        nasLength = (ushort)((a[ i + 15 + bitrate_len + 10] & 0x0f) * 256 + a[ i + 15 + bitrate_len + 11]);
                                        nasLength1 = 2;//nas 长度域所占字节
                                        byte[] b1 = new byte[nasLength];
                                        Array.Copy(a, i + 15 + bitrate_len + 12, b1, 0, nasLength);
                                        nasShuzu = b1;
                                        LengthS1ap = i + 15 + bitrate_len + 12;
                                        break;
                                    }
                                    else//nas长度
                                    {
                                        nasLength = len4;
                                        byte[] b1 = new byte[nasLength];
                                        Array.Copy(a, i + 15 + bitrate_len + 11, b1, 0, nasLength);
                                        nasShuzu = b1;
                                        LengthS1ap = i + 15 + bitrate_len + 11;
                                        break;
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            else//item的长度小于128
                            {
                                #region
                                Gbr_or = a[i+11];
                                if (Gbr_or == 0x80)
                                {
                                    int bitrate1_len = ((a[i + 14] & 0x38) >> 3) + 2;
                                    int bitrate2_len = ((a[i + 14 + bitrate1_len] & 0xe0) >> 5) + 2;
                                    int bitrate3_len = ((a[i + 14 + bitrate1_len + bitrate2_len] & 0xe0) >> 5) + 2;
                                    int bitrate4_len = ((a[i + 14 + bitrate1_len + bitrate2_len + bitrate3_len] & 0xe0) >> 5) + 2;
                                    int bitrate_len = bitrate1_len + bitrate2_len + bitrate3_len + bitrate4_len;
                                    len4 = a[i + 14 + bitrate_len+10];//用于判断nas长度，GBR占16字节
                                    nasLength = a[i + 14 + bitrate_len + 10];
                                    byte[] b1 = new byte[nasLength];
                                    Array.Copy(a, i + 14 + bitrate_len + 11, b1, 0, nasLength);
                                    LengthS1ap = i + 14 + bitrate_len + 11;
                                    nasShuzu = b1;
                                    break;
                                }
                                else if (Gbr_or == 0x00)
                                {
                                    nasLength = a[i+24];
                                    byte[] b1 = new byte[nasLength];
                                    Array.Copy(a, i + 25, b1, 0, nasLength);
                                    nasShuzu = b1;
                                    LengthS1ap = i + 25;
                                    break;
                                }
                                #endregion
                            }
                            #endregion
                        }
                        else//list的长度小于128
                        {
                            #region
                            Gbr_or = a[i+10];
                            if (Gbr_or == 0x80)
                            {
                                int bitrate1_len = ((a[i + 13] & 0x38) >> 3) + 2;
                                int bitrate2_len = ((a[i + 13 + bitrate1_len] & 0xe0)>> 5)+ 2;
                                int bitrate3_len = ((a[i + 13 + bitrate1_len + bitrate2_len] & 0xe0) >> 5) + 2;
                                int bitrate4_len = ((a[i + 13 + bitrate1_len + bitrate2_len + bitrate3_len] & 0xe0) >> 5) + 2;
                                int bitrate_len = bitrate1_len + bitrate2_len + bitrate3_len + bitrate4_len;
                                nasLength = a[i + 13 + bitrate_len+10];                               
                                byte[] b1 = new byte[nasLength];
                                Array.Copy(a, i + 13 + bitrate_len + 11, b1, 0, nasLength);
                                nasShuzu = b1;
                                LengthS1ap = i + 13 + bitrate_len + 11;
                                break;
                            }
                            else if (Gbr_or == 0x00)
                            {
                                nasLength = a[i + 23];
                                byte[] b1 = new byte[nasLength];
                                Array.Copy(a, i + 24, b1, 0, nasLength);
                                nasShuzu = b1;
                                LengthS1ap = i + 24;
                                break;
                            }
                            #endregion
                        }
                        #endregion
                    }
                    if (id != 16)
                    {
                        #region
                        slength = a[i + 3];
                        if (slength < 128)
                            i = (ushort)(i + slength + 4);
                        else
                            i = (ushort)(i + slength + 5);//当信元的编码长度超过128时，则长度域的编码为0x80yy（2个字节）,且yy才是实际的编码长度                         
                        #endregion
                    }
                }
            }
            if (tlength < 128)
            {
                #region
                for (int i = 7; i < a.Length; )
                {
                    id = (ushort)(a[i] * 256 + a[i + 1]);
                    slength = a[i + 3];
                    if (id == 16)
                    {
                        #region                     
                        listLength = 1;//该信元长度域占1字节
                        Gbr_or = a[i + 10];
                        if (Gbr_or == 0x80)
                        {
                            int bitrate1_len = ((a[i + 13] & 0x38) >> 3) + 2;
                            int bitrate2_len = ((a[i + 13 + bitrate1_len] & 0xe0) >> 5 )+ 2;
                            int bitrate3_len =( (a[i + 13 + bitrate1_len + bitrate2_len] & 0xe0) >> 5) + 2;
                            int bitrate4_len = ((a[i + 13 + bitrate1_len + bitrate2_len + bitrate3_len] & 0xe0) >> 5)+ 2;
                            int bitrate_len = bitrate1_len + bitrate2_len + bitrate3_len + bitrate4_len;
                            nasLength = a[i + 13 + bitrate_len+10];                            
                            byte[] b1 = new byte[nasLength];
                            Array.Copy(a, i +13 + bitrate_len + 11, b1, 0, nasLength);
                            LengthS1ap = i + 13 + bitrate_len + 11;
                            nasShuzu = b1;
                            break;//找到该信元以后，解析之后直接跳出循环
                        }
                        else if (Gbr_or == 0x00)
                        {
                            nasLength = a[i + 23];
                            byte[] b1 = new byte[nasLength];
                            Array.Copy(a, i + 24, b1, 0, nasLength);
                            LengthS1ap = i + 24;
                            nasShuzu = b1;
                            break;//找到该信元以后，解析之后直接跳出循环
                        }
                        #endregion
                    }
                    if (id != 16)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MonitorPorts
{
    /// <summary>
    /// 该类是为了解析在信令initial context setup request中的nas编码
    /// </summary>
    public class initial_ue
    {
        public byte direction = 1;//initial context setup request信令的方向是下行
        
        public int LengthS1ap = 0;//指示nas前面的s1ap的长度
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a">s1ap的信令编码</param>
        /// <returns>nas的信令编码</returns>
        public byte[] initialUe(byte[] a)
        {
            byte item_length2 = 0, itemctxt_length2=0;
            ushort len1,tlength, item_length;
            int k;
            ushort id = 0;
            ushort nasLength = 1;
            byte[] nasShuzu = new byte[nasLength];          
            byte nas_or = 0;
            byte gbr_or = 0;
            ushort itemctxt_length = 0;
            len1 = a[3];//用于判断整条s1ap消息的长度
            if (len1 >= 128)
            {
               tlength = (ushort)((a[3] & 0x0f) * 256 + a[4]);// tlength是整条s1ap消息的实际长度
               k = 8;//k表示id的位置
            }
            else
            {
                tlength = a[3];
                k = 7;
            }
            for (; k < a.Length;)
            {
                #region
                id = (ushort)(a[k] * 256 + a[k + 1]);
                item_length = a[k + 3];
                if (item_length >= 128)
                {
                    item_length = (ushort)((a[k + 3] & 0x0f) * 256 + a[k + 4]);//信元长度
                    item_length2 = 2;//信元长度编码的长度
                }
                else
                {
                    item_length = a[k + 3];
                    item_length2 = 1;
                }
                if (id == 24)
                {
                    #region
                    byte[]  itemctxt = new byte[item_length-1];
                    Array.Copy(a, k + 3 + item_length2 + 1, itemctxt, 0, item_length-1);//从listctxt信元中获取itemctxt的内容
                    for (int p = 0; p < itemctxt.Length;)
                    {
                        itemctxt_length = itemctxt[p+3];
                        if (itemctxt_length >= 128)
                        {
                            itemctxt_length = (ushort)((itemctxt[p + 3] & 0x0f) * 256 + itemctxt[p + 4]);//一个itemctxt信元的长度
                            itemctxt_length2 = 2;//itemctxt信元长度的编码长度
                        }
                        else
                        {
                            itemctxt_length = itemctxt[p + 3];
                            itemctxt_length2 = 1;
                        }
                        nas_or = itemctxt[p + 2 + itemctxt_length2+1];
                        gbr_or = itemctxt[p + 2 + itemctxt_length2+2];
                        nas_or = (byte)(nas_or & 0x40);//用于判断item中是否有nas-pdu
                        if (nas_or == 0x40 && gbr_or == 0x00)//有nas_pdu，没有gbr(注这里的程序只获取第一个item里的nas)
                        {
                            nasLength = itemctxt[p + 2 + itemctxt_length2 + 15];
                            if (nasLength >= 128)
                            {
                                nasLength = (ushort)((itemctxt[p + 2 + itemctxt_length2 + 15] & 0x0f) * 256 + itemctxt[p + 2 + itemctxt_length2 + 16]);
                                byte[] b1 = new byte[nasLength];
                                Array.Copy(itemctxt, p + 2 + itemctxt_length2 + 17, b1, 0, nasLength);
                                LengthS1ap = k + 3 + item_length2 + 1 + p + 2 + itemctxt_length2 + 17;
                                nasShuzu = b1;
                            }
                            else
                            {
                                byte[] b1 = new byte[nasLength];
                                Array.Copy(itemctxt, p + 2 + itemctxt_length2 + 16, b1, 0, nasLength);
                                nasShuzu = b1;
                                LengthS1ap = k + 3 + item_length2 + 1 + p + 2 + itemctxt_length2 + 16;
                            }
                                break;
                        }
                        if (nas_or == 0x40 && gbr_or == 0x40)//有nas_pdu，有gbr(注这里的程序只获取第一个item里的nas)
                        {
                            int bitrate1_len = ((itemctxt[p + 2 + itemctxt_length2 + 5] & 0x38) >> 3) + 2;
                            int bitrate2_len = ((itemctxt[p + 2 + itemctxt_length2 + 5 + bitrate1_len] & 0xe0) >> 5) + 2;
                            int bitrate3_len = ((itemctxt[p + 2 + itemctxt_length2 + 5 + bitrate1_len + bitrate2_len] & 0xe0) >> 5) + 2;
                            int bitrate4_len = ((itemctxt[p + 2 + itemctxt_length2 + 5 + bitrate1_len + bitrate2_len + bitrate3_len] & 0xe0) >> 5) + 2;
                            int bitrate_len = bitrate1_len + bitrate2_len + bitrate3_len + bitrate4_len;
                            nasLength = (ushort)(itemctxt[p + 2 + itemctxt_length2 + 5 + bitrate_len + 11]);
                            if (nasLength >= 128)
                            {
                                nasLength = (ushort)((itemctxt[p + 2 + itemctxt_length2 + 5 + bitrate_len + 11] & 0x0f) * 256 + itemctxt[p + 2 + itemctxt_length2 + 5 + bitrate_len + 12]);
                                byte[] b1 = new byte[nasLength];
                                Array.Copy(itemctxt, p + 2 + itemctxt_length2 + 5 + bitrate_len + 13, b1, 0, nasLength);
                                nasShuzu = b1;
                                LengthS1ap = k + 3 + item_length2 + 1 + p + 2 + itemctxt_length2 + 5 + bitrate_len + 13;
                            }
                            else
                            {
                                byte[] b1 = new byte[nasLength];
                                Array.Copy(itemctxt, p + 2 + itemctxt_length2 + 5 + bitrate_len + 12, b1, 0, nasLength);
                                nasShuzu = b1;
                                LengthS1ap = k + 3 + item_length2 + 1 + p + 2 + itemctxt_length2 + 5 + bitrate_len + 12;
                            }
                                break;
                        }
                        else 
                        {
                            p = p + itemctxt_length + 3 + itemctxt_length2;
                        }
                    }
                    #endregion
                    break;
                }
                else//不是所要解析的信元
                {
                    k = (ushort)(k + item_length + 3 + item_length2);
                }
                #endregion
            }
            return nasShuzu;
        }
    }
}

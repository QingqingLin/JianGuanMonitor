﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonitorPorts
{
    class Id1
    {
        public int Index = 0;//MmeUeS1apId编码在整个s1ap编码中的位置
        public int Length = 0;//MmeUeS1apId编码长度
        /// <summary>
        /// 该方法用于解析s1ap信令中的MmeUeS1apId信元
        /// </summary>
        /// <param name="s1ap">s1ap信令编码</param>
        /// <returns>MmeUeS1apId</returns>
        public uint s1ap_id(byte[] s1ap)
        {
            ushort tlength = 0;//s1ap信令的所有信元编码总长度
            ushort slength = 0;//单个信元的编码长度
            ushort len1 = 0;//用于判定总长度所占字节的变量
            ushort len2 = 0;//用于判定单个信元长度所占字节的变量
            ushort id = 0;//指示信元类型
            uint MmeUeS1apId = 0;
            byte length2 = 0;//用于求解mmes1apId;

            len1 = s1ap[3];
            if (len1 >= 128)
                tlength = (ushort)((s1ap[3] & 0x0f) * 256 + s1ap[4]);
            else
                tlength = s1ap[3];

            if (tlength >= 128)
            {
                for (int i = 8; i < s1ap.Length; )
                {
                    id = (ushort)(s1ap[i] * 256 + s1ap[i + 1]);
                    len2 = s1ap[i + 3];
                    if (len2 >= 128)
                        slength = (ushort)((s1ap[i + 3] & 0x0f) * 256 + s1ap[i + 4]);
                    else
                        slength = s1ap[i + 3];
                    if (id == 0)//此处不用考虑该信元的编码长度超过128，因为 MmeUeS1apId信元长度较短
                    {
                        Index = i+4;
                        Length = slength;//(type+criticality++length+content)
                        for (int j = slength, k = 0; j > 1; j--, k++)//j>1是因为编码的最高一个字节并不表示数值
                            MmeUeS1apId += (uint)(s1ap[i + 3 + j] * System.Math.Pow(256, k));
                        break;//找到该信元以后，解析之后直接跳出循环
                    }
                    if (id == 99)///此处不用考虑该信元的编码长度超过128，因为 MmeUeS1apId信元长度较短
                    {
                        Index = i+4;
                        length2 = (byte)((s1ap[i + 4] & 0x0c) >> 2);
                        Length =1 + length2+1;//(type+criticality+content)
                        for (byte m = length2, l = 0; l < length2 + 1; l++, m--)

                            MmeUeS1apId += (uint)(s1ap[i + 5 + l] * Math.Pow(256, m));
                        break;
                    }
                    if (id != 0 && id != 99)
                    {
                        if (slength < 128)
                            i = (ushort)(i + slength + 4);
                        else
                        {
                            i = (ushort)(i + slength + 5);//当信元的编码长度超过128时，则长度域的编码为0x80yy（2个字节）,且yy才是实际的编码长度
                        }
                    }
                }
            }
            if (tlength < 128)
            {
                for (int i = 7; i < s1ap.Length; )
                {
                    id = (ushort)(s1ap[i] * 256 + s1ap[i + 1]);
                    slength = s1ap[i + 3];
                    if (id == 0)
                    {
                        Index = i+4;
                        Length = slength;//(type+criticality+length+content)
                        for (int j = slength, k = 0; j > 1; j--, k++)//j>1是因为编码的最高一个字节并不表示数值
                            MmeUeS1apId += (uint)(s1ap[i + 3 + j] * System.Math.Pow(256, k));
                        break;//找到该信元以后，解析之后直接跳出循环
                    }
                    if (id == 99)
                    {
                        Index = i+4;
                        length2 = (byte)((s1ap[i + 4] & 0x0c) >> 2);
                        Length = 1+length2+1;//(length+content)
                        for (byte m = length2, l = 0; l < length2 + 1; l++, m--)
                            MmeUeS1apId += (uint)(s1ap[i + 5 + l] * Math.Pow(256, m));
                        break;
                    }
                    if (id != 0 && id != 99)
                    {
                        i = (ushort)(i + slength + 4);
                    }
                }
            }
            return MmeUeS1apId;
        }
    }
}

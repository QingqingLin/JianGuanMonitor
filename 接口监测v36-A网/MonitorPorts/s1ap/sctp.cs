using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;
namespace sctp
{
    class SctpDecode
    {
        public string protocol = string.Empty;
        public string sourIP = string.Empty;//源IP地址
        public string sourPort = string.Empty;//源端口号
        public string destIP = string.Empty;//目的IP
        public string destPort = string.Empty;//目的端口号
        public int sourPort1 = 0;
        public int desPort1 = 0;
        byte chunkType = 0;//用于指示chunk的类型，只有chunk是data chunk时才是信令数据
        int chunkLength = 0;//chunk的长度
        static int dataLength;//data chunk中s1ap的编码长度
        public int paylaodProtocolIdentifier;//指示sctp之上的应用层协议      
        public List<byte[]> s1ap = new List<byte[]>();//用于存储一条报文中的s1ap信令编码（注意：一条报文中可以含有两条s1ap信令）
        /// <summary>
        /// 该方法将带有IP层，sctp层头部的s1ap报文进行解析
        /// </summary>
        /// <param name="buf">带有IP层，SCTP层头部的s1ap报文</param>
        public List<byte[]> DeScChunk(byte[] buf)
        {
            s1ap.Clear();
            int i = 0;
            paylaodProtocolIdentifier = 0;
            int padding = 0;
            for (i = 12; i < buf.Length; )
            {
                chunkType = buf[i];
                chunkLength = buf[i + 2] * 256 + buf[i + 3];
                //sctp的chunk长度若不为4的整数倍，则需要在整个编码之后补0
                int padding0 = chunkLength % 4;
                if (padding0 > 0 && padding0 < 4)
                    padding = 4 - padding0;
                else
                    padding = 0;//padding为补0的个数
                if (chunkType == 0) // DATA块  
                {
                    dataLength = chunkLength - 16;
                    paylaodProtocolIdentifier = BitConverter.ToInt32(buf, i + 12);
                    paylaodProtocolIdentifier = IPAddress.NetworkToHostOrder(paylaodProtocolIdentifier);
                    if (paylaodProtocolIdentifier == 18)//应用层是s1ap
                    {
                        byte[] messageBuffer;
                        messageBuffer = new byte[dataLength];//存放不带有头部的s1ap信令编码
                        Array.Copy(buf, i + 16, messageBuffer, 0, dataLength);
                        s1ap.Add(messageBuffer);
                    }
                    i = i + chunkLength + padding;
                }
                else if ((chunkType == 11) || (chunkType == 6))//异常的sctp 块
                    break;
                else  //非data块  
                {
                    i = i + chunkLength + padding;
                    paylaodProtocolIdentifier = -1;
                }                
            }
            return s1ap;
        }
        /// <summary>
        /// 该方法用于解析s1ap信令中的源IP，目的IP，源端口和目的端口，报文长度和协议类型
        /// </summary>
        /// <param name="Chunk"></param>
        public void decoder_All(byte[] Chunk)
        {
            sourIP = Convert.ToString(Chunk[12]) + "." + Convert.ToString(Chunk[13]) + "." + Convert.ToString(Chunk[14]) + "." + Convert.ToString(Chunk[15]);
            destIP = Convert.ToString(Chunk[16]) + "." + Convert.ToString(Chunk[17]) + "." + Convert.ToString(Chunk[18]) + "." + Convert.ToString(Chunk[19]);
            sourPort = Convert.ToString(Chunk[20] * 256 + Chunk[21]);
            destPort = Convert.ToString(Chunk[22] * 256 + Chunk[23]);
            protocol = "S1AP";
        }
    }
}


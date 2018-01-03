using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace MonitorPorts
{
    class PcapDecoder
    {
        public PcapFile PcapFile;       // pcap文件数据结构
        public int isFinish = 1;       //文件解析完成标志

        /// 构造函数
        /// 构造函数
        /// <param name="filepath"></param>需要解析的文件路径
        public PcapDecoder(string filepath)
        {
            PcapFile = new PcapFile();
        }
        
        ///解析数据内容
        /// 解析数据内容
        /// </summary>
        /// <param name="fs"></param>流文件
        public void ReadPcapFile(FileStream fs)
        {
            GetDataLength(fs);
            GetEthernetData(fs);
            GetUpToIPData(fs);
        }

        /// pcap文件头
        /// 读取pcap文件头，24字节
        /// </summary>
        /// <param name="fs"></param>
        public void GetFileHead(FileStream fs)
        {
            PcapFile.PcapFileHead = new byte[24];
            fs.Read(PcapFile.PcapFileHead, 0, 24);
        }

        /// 读取一个数据帧头部时间 8字节
        /// 读取一个数据帧头部时间 8字节
        /// </summary>
        /// <param name="fs"></param>
        public void GetDataHead(FileStream fs)
        {
            byte[] DateSecond = new byte[4];
            isFinish = fs.Read(DateSecond, 0, 4);
            int Second = System.BitConverter.ToInt32(DateSecond, 0);
            DateTime dt = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            string StrSecond = Second.ToString();
            long lTime = long.Parse(StrSecond);
            PcapFile.DataHead.Time = new DateTime();
            PcapFile.DataHead.Time = dt.AddSeconds(lTime);
            fs.Read(DateSecond, 0, 4);
            Second = System.BitConverter.ToInt32(DateSecond, 0);
            PcapFile.DataHead.Time = PcapFile.DataHead.Time.AddMilliseconds(Second / 1000);          
        }

        /// <读取数据帧长度>
        /// 读取数据帧长度 8字节
        /// </summary>
        /// <param name="fs"></param>
        public void GetDataLength(FileStream fs)
        {
            byte[] Length = new byte[4];
            fs.Read(Length, 0, 4);
            PcapFile.DataHead.GetDataLength = System.BitConverter.ToInt32(Length, 0);
            fs.Read(Length, 0, 4);
            PcapFile.DataHead.ActualLength = System.BitConverter.ToInt32(Length, 0);
        }

        public void GetEthernetData(FileStream fs)
        {
            fs.Read(PcapFile.EthernetData, 0, 14);
        }

        public void GetUpToIPData(FileStream fs)
        {
            PcapFile.UpToIPData = new byte[PcapFile.DataHead.GetDataLength - 14];
            fs.Read(PcapFile.UpToIPData, 0, PcapFile.DataHead.GetDataLength - 14);
            Receive(PcapFile.UpToIPData, PcapFile.UpToIPData.Length);
            
        }

        unsafe private void Receive(byte[] buf, int len)
        {
            byte protocol = 0;
            uint version = 0;
            uint ipSourceAddress = 0;
            uint ipDestinationAddress = 0;
            int sourcePort = 0;
            int destinationPort = 0;
            IPAddress ip;
            //PcapFile.UpToIPData = buf;
            //e.BufLength = len;
            fixed (byte* FixedBuf = buf)
            {
                HandlePacket.IPHeader* head = (HandlePacket.IPHeader*)FixedBuf;
                PcapFile.IPHead.IPHeaderLength = (uint)((head->versionAndLength & 0x0f) << 2);
                protocol = head->protocol;
                switch (protocol)
                {
                    case 1:
                        PcapFile.Protocol = "ICMP";
                        break;
                    case 2:
                        PcapFile.Protocol = "IGMP";
                        break;
                    case 6:
                        PcapFile.Protocol = "TCP";
                        break;
                    case 17:
                        PcapFile.Protocol = "UDP";
                        break;
                    case 132:
                        PcapFile.Protocol = "SCTP";
                        break;
                    default:
                        PcapFile.Protocol = "UNKNOWN";
                        break;
                }
                version = (uint)((head->versionAndLength & 0xf0) >> 4);
                ipSourceAddress = head->sourceAddress;
                ipDestinationAddress = head->destinationAdress;
                ip = new IPAddress(ipSourceAddress);
                PcapFile.IPHead.SourceIP = ip.ToString();
                ip = new IPAddress(ipDestinationAddress);
                PcapFile.IPHead.DestIP = ip.ToString();
                sourcePort = buf[PcapFile.IPHead.IPHeaderLength] * 256 + buf[PcapFile.IPHead.IPHeaderLength + 1];
                destinationPort = buf[PcapFile.IPHead.IPHeaderLength + 2] * 256 + buf[PcapFile.IPHead.IPHeaderLength + 3];
                PcapFile.UDPHead.SourcePort = sourcePort.ToString();
                PcapFile.UDPHead.DestPort = destinationPort.ToString(); 
                PcapFile.DataLength = (uint)len - PcapFile.IPHead.IPHeaderLength - 8;
                PcapFile.Data = new byte[PcapFile.DataLength];
                Array.Copy(buf, (int)PcapFile.IPHead.IPHeaderLength + 8, PcapFile.Data, 0, PcapFile.Data.Length);

            }
        }
    }
    
}

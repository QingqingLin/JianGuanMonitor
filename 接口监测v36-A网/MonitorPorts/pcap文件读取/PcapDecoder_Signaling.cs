using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace MonitorPorts
{
    class PcapDecoder_Signaling
    {
        public PcapFile PcapFile;       // pcap文件数据结构
        public int isFinish = 1;       //文件解析完成标志
        public bool isLinux = false;        //数据链路层判断

        /// 构造函数
        /// 构造函数
        /// <param name="filepath"></param>需要解析的文件路径
        public PcapDecoder_Signaling(string filepath)
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
            if (PcapFile.PcapFileHead[20] == 113)
            {
                isLinux = true;
            }
        }

        /// 读取一个数据帧头部时间 8字节
        /// 读取一个数据帧头部时间 8字节
        /// </summary>
        /// <param name="fs"></param>
        public void GetDataHead(FileStream fs)
        {
            byte[] DateSecond = new byte[4];
            isFinish = fs.Read(DateSecond, 0, 4);                                                  //  判断当前pcap文件是否读取完毕
            decimal Second = System.BitConverter.ToInt32(DateSecond, 0);                            //
            DateTime dt = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            string StrSecond = Second.ToString();
            long lTime = long.Parse(StrSecond);
            fs.Read(DateSecond, 0, 4);                                                                              /////////////        有问题         /////////////
            decimal Second1 = System.BitConverter.ToInt32(DateSecond, 0);
            int secondInt1 = System.BitConverter.ToInt32(DateSecond, 0);
            decimal s = Second1 / 1000000;
            PcapFile.DataHead_Signaling.Time = Second + s;
            PcapFile.DataHead_Signaling.TimeAll = dt.AddSeconds(lTime).AddMilliseconds(secondInt1 / 1000).ToString("yyyy-MM-dd HH:mm:ss:fff");
        }

        /// <读取数据帧长度>
        /// 读取数据帧长度 8字节
        /// </summary>
        /// <param name="fs"></param>
        public void GetDataLength(FileStream fs)
        {
            byte[] Length = new byte[4];
            fs.Read(Length, 0, 4);
            PcapFile.DataHead_Signaling.GetDataLength = System.BitConverter.ToInt32(Length, 0);
            fs.Read(Length, 0, 4);
            PcapFile.DataHead_Signaling.ActualLength = System.BitConverter.ToInt32(Length, 0);
        }

        public void GetEthernetData(FileStream fs)
        {
            if (isLinux)
            {
                fs.Read(PcapFile.LinuxnetData, 0, 16);
            }
            else
            {
                fs.Read(PcapFile.EthernetData, 0, 14);
            }

        }

        public void GetUpToIPData(FileStream fs)
        {
            if (isLinux)
            {
                PcapFile.UpToIPData = new byte[PcapFile.DataHead_Signaling.GetDataLength - 16];
                fs.Read(PcapFile.UpToIPData, 0, PcapFile.DataHead_Signaling.GetDataLength - 16);
            }
            else
            {
                PcapFile.UpToIPData = new byte[PcapFile.DataHead_Signaling.GetDataLength - 14];
                fs.Read(PcapFile.UpToIPData, 0, PcapFile.DataHead_Signaling.GetDataLength - 14);
            }


        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MonitorPorts
{

    public class GeneratePcapFile
    {
        public const uint TCPDUMP_MAGIC = 0xa1b2c3d4;
        public const ushort PCAP_VERSION_MAJOR = 2;
        public const ushort PCAP_VERSION_MINOR = 4;

        //文件流
        FileStream fs;

        //文件路径
        private string _pcapFileSavePath = "";

        //文件名
        private string _pcapFileName = "";

        //数据包头16位
        byte[] m_pcapPktHead = new byte[16];

        //头文件字节数据
        byte[] fileHeadBuff = new byte[24];

        //构建以太网头部
        byte[] ethernetHeader = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00 };
        #region 属性
        public string PcapFileSavePath
        {
            get { return _pcapFileSavePath; }
            set { _pcapFileSavePath = value; }
        }

        public string PcapFileName
        {
            get { return _pcapFileName; }
            set { _pcapFileName = value; }
        }
        #endregion
        //构造函数
        public GeneratePcapFile()
        {
            //生成pcap文件头部
            GetFileHeadBuff();
        }

        /// <summary>
        /// 生成pcap文件头部
        /// </summary>
        unsafe private void GetFileHeadBuff()
        {
            //生成pcap文件头部
            PcapFileHeader pcapFHeader;
            pcapFHeader.Magic = TCPDUMP_MAGIC;
            pcapFHeader.MajorVersion = PCAP_VERSION_MAJOR;
            pcapFHeader.MinorvVersion = PCAP_VERSION_MINOR;
            pcapFHeader.ThisZone = 0;

            //抓包最大长度 如果要抓全，设为0x0000ffff（65535），
            pcapFHeader.SnapLen = 0x0000ffff;
            pcapFHeader.SigFigs = 0;
            pcapFHeader.LinkType = 0x00000001;
            byte* pktPtr = (byte*)(&pcapFHeader);
            fixed (byte* pktBufPtr = fileHeadBuff)
            {

                //byte* pp = pktPtr;
                byte* pk = pktBufPtr;
                for (int i = 0; i < 24; i++)
                {
                    *pk = *pktPtr;
                    pk++;
                    pktPtr++;
                }
            }
        }
        /// <summary>
        /// 建立pcap文件
        /// </summary>
        /// <param name="fileSavePath">文件路径</param>
        /// <param name="FileName">文件名</param>
        /// <returns>若成功生成文件返回true,若失败返回false</returns>
        public bool CreatPcap(string fileSavePath, string FileName)
        {
            _pcapFileSavePath = fileSavePath;

            //生成保存数据文件夹
            if (!Directory.Exists(_pcapFileSavePath))
            {
                Directory.CreateDirectory(_pcapFileSavePath);
            }

            _pcapFileName = FileName;
            string suffix = ".pcap";
            string filepath = _pcapFileSavePath + @"\" + _pcapFileName + suffix;
            //try
            //{
                if (fs != null)
                {
                    fs.Close();
                }

                fs = new FileStream(filepath, FileMode.Append,FileAccess.Write, FileShare.ReadWrite);
                //写入头文件
                fs.Write(fileHeadBuff, 0, fileHeadBuff.Length);

                //保存文件
                fs.Flush();
             

            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("文件建立失败：" + ex.Message);
            //}
            return true;

        }
        /// <summary>
        /// 写入数据包
        /// </summary>
        /// <param name="dt">数据包捕获时间</param>
        /// <param name="packetData">IP数据包</param>
        unsafe public void WritePacketData(DateTime dt, byte[] packetData, int packetLenth)
        {
            if (fs != null)
            {
                //将时间转化成时间戳
                Timestamp timestamp = DateTimeToTimestamp(dt);

                //实例化包头
                //写入包头，共16字节
                PacapPacketHeader pcapPktHad;

                //写入时间戳
                pcapPktHad.Ts = timestamp;

                //帧长度
                pcapPktHad.CapLen = (uint)packetLenth + 14;

                //数据包长度
                pcapPktHad.Len = (uint)packetLenth + 14;

                fixed (byte* pktHdPtr = m_pcapPktHead)
                {
                    byte* wph = pktHdPtr;
                    byte* pcapPktPtr = (byte*)(&pcapPktHad);
                    for (int i = 0; i < 16; i++)
                    {
                        *wph = *pcapPktPtr;
                        wph++;
                        pcapPktPtr++;
                    }
                }
                //写入数据包头
                fs.Write(m_pcapPktHead, 0, m_pcapPktHead.Length);


                //写入以太网帧头ethernetHeader,共14个字节
                fs.Write(ethernetHeader, 0, ethernetHeader.Length);

                //写入数据
                fs.Write(packetData, 0, Convert.ToInt16(packetLenth));


                fs.Flush();
            }
        }

        private Timestamp DateTimeToTimestamp(DateTime dt)
        {
            //时间戳是指格林威治时间1970年01月01日00时00分00秒(北京时间1970年01月01日08时00分00秒)起至现在的总秒数
            DateTime startTime = new DateTime(1970, 1, 1, 8, 0, 0, 0);

            TimeSpan ts = dt - startTime;

            uint u1 = (uint)ts.TotalSeconds;

            uint u2 = (uint)(ts.Ticks / 10 - u1 * 1000000);

            Timestamp timestamp = new Timestamp();
            timestamp.Timestamp_S = u1;
            timestamp.Timestamp_MS = u2;
            return timestamp;
        }


        public void ClosePcapFile()
        {
            if (fs != null)
            {
                fs.Close();
            }

        }
    }
}

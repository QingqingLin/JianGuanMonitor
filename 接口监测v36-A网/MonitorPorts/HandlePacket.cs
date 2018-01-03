using GTP_V2Decoder;
using read;
using sctp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace MonitorPorts
{
    class HandlePacket
    {
        PacketProperties PacketProperties;
        PacketProperties CBTCPacketProperties;

        CatchSocket Pcap;
        SctpDecode C = new SctpDecode();
        SCTPDiameter B = new SCTPDiameter();
        public Readdata readData = new Readdata();
        static Dictionary<string, DateTime> DicInterval = new Dictionary<string, DateTime>();
        public delegate void PacketArrivedEventHandler(object Handle, PacketProperties Properties, bool showSignal, bool showSignaling);
        public static event PacketArrivedEventHandler PacketArrival;
        bool showSignal = false;//本条消息是否为信号消息。若是，需执行信号消息显示界面更新
        bool showSignaling = false;//本条消息是否为信号消息。若是，需执行信号消息显示界面更新
        public static bool Pause = false;            //抓包在线显示标志
        public static List<PacketProperties> CBTCDataCache = new List<PacketProperties>();      //林庆庆
        public static List<PacketProperties> LTEDataCache = new List<PacketProperties>();       //林庆庆

        public HandlePacket()
        {
        }

        public void Unpack(byte[] buf, int len, CatchSocket Pcap,bool bStatus_Signal, bool bStatus_Signaling)
        {
            showSignal = false;
            showSignaling = false;
            if (bStatus_Signal)//解析信号数据包
            {
                this.Pcap = Pcap;
                CBTCPacketProperties = new PacketProperties(len);
                CBTCPacketProperties.CaptureTime = DateTime.Now;
                Unpack(buf, len);
            }
            if (bStatus_Signaling)//解析信令数据包
            {
                this.Pcap = Pcap;
                byte protocol = 0;
                uint version = 0;
                int sourcePort = 0;
                int destinationPort = 0;
                protocol = buf[9];
                version = (uint)((buf[0] & 0xf0) >> 4);
                sourcePort = buf[20] * 256 + buf[21];
                destinationPort = buf[22] * 256 + buf[23];
                if ((protocol == 6) && (sourcePort == 3868 || destinationPort == 3868))//TCP-diameter  (黄罡)
                {
                    PacketProperties = new PacketProperties(len);
                    Array.Copy(buf, PacketProperties.Buf, len);
                    PacketProperties.BufLength = len;
                    PacketProperties.IPHeaderLength = (uint)((buf[0] & 0x0f) << 2);
                    PacketProperties.Version = version.ToString();
                    PacketProperties.OriginationPort = sourcePort.ToString();
                    PacketProperties.DestinationPort = destinationPort.ToString();
                    PacketProperties.DestinationAddress = buf[16].ToString() + "." + buf[17].ToString() + "." + buf[18].ToString() + "." + buf[19].ToString();
                    PacketProperties.OriginationAddress = buf[12].ToString() + "." + buf[13].ToString() + "." + buf[14].ToString() + "." + buf[15].ToString();
                    PacketProperties.PacketLength = (uint)len;
                    PacketProperties.MessageLength = PacketProperties.PacketLength - PacketProperties.IPHeaderLength;                  
                    PacketProperties.MessagePacketHex = readData.RRead(buf);//用于展示抓获报文的名称(readData.RRead(PacketBuffer))//yao
                    PacketProperties.PacketBuffer = readData.SourceData;
                    PacketProperties.protocolid = readData.protocolid;
                    showSignaling = true;
                    if (!Pause)
                    {
                        UpdateForm(PacketProperties, showSignal, showSignaling);
                    }
                    SaveToPcap_Signaling(PacketProperties);
                }
                else if ((protocol == 17) && (sourcePort == 2123 || destinationPort == 2123))//UDP-GTPv2 (黄罡)
                {
                    PacketProperties = new PacketProperties(len);
                    Array.Copy(buf, PacketProperties.Buf, len);
                    PacketProperties.BufLength = len;
                    PacketProperties.IPHeaderLength = (uint)((buf[0] & 0x0f) << 2);
                    PacketProperties.Version = version.ToString();
                    PacketProperties.OriginationPort = sourcePort.ToString();
                    PacketProperties.DestinationPort = destinationPort.ToString();
                    PacketProperties.DestinationAddress = buf[16].ToString() + "." + buf[17].ToString() + "." + buf[18].ToString() + "." + buf[19].ToString();
                    PacketProperties.OriginationAddress = buf[12].ToString() + "." + buf[13].ToString() + "." + buf[14].ToString() + "." + buf[15].ToString();
                    PacketProperties.PacketLength = (uint)len;
                    PacketProperties.MessageLength = PacketProperties.PacketLength - PacketProperties.IPHeaderLength;                   
                    PacketProperties.MessagePacketHex = readData.RRead(buf);//用于展示抓获报文的名称(readData.RRead(PacketBuffer))//yao
                    PacketProperties.protocolid = readData.protocolid;
                    PacketProperties.PacketBuffer = readData.SourceData;//SourceData是去掉头部之后的数据（gtpv2去掉ip和udp头部，其他两个只去掉ip头）
                    showSignaling = true;
                    if (!Pause)
                    {
                        UpdateForm(PacketProperties, showSignal, showSignaling);
                    }
                    SaveToPcap_Signaling(PacketProperties);
                }
                else if (protocol == 132)
                {
                    byte[] sctp = new byte[len - 20];
                    Array.Copy(buf, 20, sctp, 0, len - 20);//去除IP头部20字节 
                    C.DeScChunk(sctp);
                    int PPID = C.paylaodProtocolIdentifier;
                    if (PPID == 18)//sctp-s1ap
                    {
                        PacketProperties = new PacketProperties(len);
                        Array.Copy(buf, PacketProperties.Buf, len);
                        if (buf[3] == PacketProperties.Buf[3])
                        {
                            byte a = 1;
                        }
                        PacketProperties.BufLength = len;
                        PacketProperties.IPHeaderLength = (uint)((buf[0] & 0x0f) << 2);
                        PacketProperties.Version = version.ToString();
                        PacketProperties.OriginationPort = sourcePort.ToString();
                        PacketProperties.DestinationPort = destinationPort.ToString();
                        PacketProperties.DestinationAddress = buf[16].ToString() + "." + buf[17].ToString() + "." + buf[18].ToString() + "." + buf[19].ToString();
                        PacketProperties.OriginationAddress = buf[12].ToString() + "." + buf[13].ToString() + "." + buf[14].ToString() + "." + buf[15].ToString();
                        PacketProperties.PacketLength = (uint)len;
                        
                        PacketProperties.MessagePacketHex = readData.RRead(buf);//用于展示抓获报文的名称(readData.RRead(PacketBuffer))//yao
                        PacketProperties.protocolid = readData.protocolid;
                        PacketProperties.MessageLength = readData.s1apLength;//消息长度使用纯s1ap信令长度
                        PacketProperties.PacketBuffer = readData.SourceData;
                        showSignaling = true;
                        if (!Pause)
                        {
                            UpdateForm(PacketProperties, showSignal, showSignaling);    
                        }
                        
                        SaveToPcap_Signaling(PacketProperties);
                    }
                    else if ((PPID == 0 || PPID == 46 || PPID == 47 || PPID == 132) && ((sourcePort == 3868) || (destinationPort == 3868) || (sourcePort == 60000) || (destinationPort == 60000)))//sctp-diameter(黄罡)
                    {
                        B.DecodeSctpChunk(sctp);
                        if (B.IsDiameter == 1)
                        {
                            PacketProperties = new PacketProperties(len);
                            Array.Copy(buf, PacketProperties.Buf, len);
                            PacketProperties.BufLength = len;
                            PacketProperties.IPHeaderLength = (uint)((buf[0] & 0x0f) << 2);
                            PacketProperties.Version = version.ToString();
                            PacketProperties.OriginationPort = sourcePort.ToString();
                            PacketProperties.DestinationPort = destinationPort.ToString();
                            PacketProperties.DestinationAddress = buf[16].ToString() + "." + buf[17].ToString() + "." + buf[18].ToString() + "." + buf[19].ToString();
                            PacketProperties.OriginationAddress = buf[12].ToString() + "." + buf[13].ToString() + "." + buf[14].ToString() + "." + buf[15].ToString();
                            PacketProperties.PacketLength = (uint)len;
                            PacketProperties.MessageLength = PacketProperties.PacketLength - PacketProperties.IPHeaderLength;
                            PacketProperties.MessagePacketHex = readData.RRead(buf);//用于展示抓获报文的名称(readData.RRead(PacketBuffer))//ya
                            PacketProperties.protocolid = readData.protocolid;
                            PacketProperties.PacketBuffer = readData.SourceData;
                            showSignaling = true;
                            if (Pause)
                            {
                                UpdateForm(PacketProperties, showSignal, showSignaling);
                            }
                            SaveToPcap_Signaling(PacketProperties);
                        }
                    }
                }
            }//黄罡2017.4.29
            
        }

        unsafe private void Unpack(byte[] buf, int len)//信号解码
        {
            byte protocol = 0;
            uint version = 0;
            uint ipSourceAddress = 0;
            uint ipDestinationAddress = 0;
            int sourcePort = 0;
            int destinationPort = 0;
            IPAddress ip;

            Array.Copy(buf, CBTCPacketProperties.Buf, len);
            CBTCPacketProperties.BufLength = len;
            fixed (byte* FixedBuf = buf)
            {
                try
                {
                    IPHeader* head = (IPHeader*)FixedBuf;
                    CBTCPacketProperties.IPHeaderLength = (uint)((head->versionAndLength & 0x0f) << 2);
                    protocol = head->protocol;
                    version = (uint)((head->versionAndLength & 0xf0) >> 4);
                    CBTCPacketProperties.Version = version.ToString();
                    ipSourceAddress = head->sourceAddress;
                    ipDestinationAddress = head->destinationAdress;
                    ip = new IPAddress(ipSourceAddress);
                    CBTCPacketProperties.OriginationAddress = ip.ToString();
                    ip = new IPAddress(ipDestinationAddress);
                    CBTCPacketProperties.DestinationAddress = ip.ToString();
                    sourcePort = buf[CBTCPacketProperties.IPHeaderLength] * 256 + buf[CBTCPacketProperties.IPHeaderLength + 1];
                    destinationPort = buf[CBTCPacketProperties.IPHeaderLength + 2] * 256 + buf[CBTCPacketProperties.IPHeaderLength + 3];
                    CBTCPacketProperties.OriginationPort = sourcePort.ToString();
                    CBTCPacketProperties.DestinationPort = destinationPort.ToString();
                    CBTCPacketProperties.PacketLength = (uint)len;
                    CBTCPacketProperties.MessageLength = CBTCPacketProperties.PacketLength - CBTCPacketProperties.IPHeaderLength;
                    CBTCPacketProperties.PacketBuffer = buf;
                    Array.Copy(buf, (int)CBTCPacketProperties.IPHeaderLength, CBTCPacketProperties.MessageBuffer, 0, (int)CBTCPacketProperties.MessageLength);
                    if (protocol == 17)
                    {
                        if (CBTCPacketProperties.DestinationPort == "2152")
                        {
                            int UDP_HEADER_LENGTH = 8;
                            //int GTP_MUST_CHOICE_LENGTH = 8;
                            int GTP_LENGTH_FLAG = 2;
                            int gtpMessegePayloadLength = buf[CBTCPacketProperties.IPHeaderLength + UDP_HEADER_LENGTH + GTP_LENGTH_FLAG] * 256 + buf[CBTCPacketProperties.IPHeaderLength + UDP_HEADER_LENGTH + GTP_LENGTH_FLAG + 1];
                            //byte[] ActualBuf = new byte[len - PacketProperties.IPHeaderLength - UDP_HEADER_LENGTH - GTP_MUST_CHOICE_LENGTH - gtpMessegePayloadLength];
                            //Array.Copy(buf, PacketProperties.IPHeaderLength + UDP_HEADER_LENGTH + GTP_MUST_CHOICE_LENGTH + gtpMessegePayloadLength, ActualBuf, 0, len - PacketProperties.IPHeaderLength - UDP_HEADER_LENGTH - GTP_MUST_CHOICE_LENGTH - gtpMessegePayloadLength);
                            byte[] ActualBuf = new byte[len - CBTCPacketProperties.IPHeaderLength - UDP_HEADER_LENGTH - 12];
                            Array.Copy(buf, CBTCPacketProperties.IPHeaderLength + UDP_HEADER_LENGTH + 12, ActualBuf, 0, len - CBTCPacketProperties.IPHeaderLength - UDP_HEADER_LENGTH - 12);
                            Unpack(ActualBuf, ActualBuf.Length);
                        }
                        else
                        {
                            DataFilterByIPAndPort(CBTCPacketProperties);
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
        }

        private void DataFilterByIPAndPort(PacketProperties Properties)
        {
            bool IsSourceMeet = false;
            bool isDestMeet = false;
            ConfigurationProperties SourceDevice = null;
            ConfigurationProperties DestDevice = null;
            foreach (var Source in IPList.ConfigProperties)
            {
                if (Source.IsSourceChoose == true && Properties.OriginationAddress==Source.IP && Properties.OriginationPort == Source.Port)
                {
                    IsSourceMeet = true;
                    SourceDevice = Source;
                    break;
                }
            }
            if (IsSourceMeet)
            {
                foreach (var Dest in IPList.ConfigProperties)
                {
                    if (Dest.IsDestChoose == true && Properties.DestinationAddress==Dest.IP && Properties.DestinationPort == Dest.Port)
                    {
                        isDestMeet = true;
                        DestDevice = Dest;
                        break;
                    }
                }
                if (isDestMeet)
                {
                    if (SetSendDir(SourceDevice, DestDevice, Properties))
                    {
                        if (!Pause)
                        {
                            SaveToShowStorage(Properties);
                        }
                        SaveToPcap(Properties);
                    }
                }
            }
        }

        private void SaveToShowStorage(PacketProperties properties)
        {
            ShowThread.ShowStorate.Add(properties);
        }
        
        private bool SetSendDir(ConfigurationProperties SourceDevice, ConfigurationProperties DestDevice, PacketProperties Properties)
        {
            if (SourceDevice.Type == "VOBC" && DestDevice.Type == "ZC")
            {
                Properties.SendDir = SendDir.VOBCToZC;
                return true;
            }
            else if (SourceDevice.Type == "ZC" && DestDevice.Type == "VOBC")
            {
                Properties.SendDir = SendDir.ZCToVOBC;
                return true;
            }
            else if (SourceDevice.Type == "CI" && DestDevice.Type == "VOBC")
            {
                Properties.SendDir = SendDir.CIToVOBC;
                return true;
            }
            else if (SourceDevice.Type == "VOBC" && DestDevice.Type == "CI")
            {
                Properties.SendDir = SendDir.VOBCToCI;
                return true;
            }
            else if (SourceDevice.Type == "ATS" && DestDevice.Type == "VOBC")
            {
                Properties.SendDir = SendDir.ATSToVOBC;
                return true;
            }
            else if (SourceDevice.Type == "VOBC" && DestDevice.Type == "ATS")
            {
                Properties.SendDir = SendDir.VOBCToATS;
                return true;
            }
            else
            {
                return false;
            }
        }

        protected virtual void UpdateForm(PacketProperties Properties,bool showSignal,bool showSignaling)
        {
            if (PacketArrival != null)
            {
                PacketArrival(this, Properties,showSignal,showSignaling);
            }
        }

        private void SaveToPcap_Signaling(PacketProperties Properties)      //修改林庆庆
        {
            CatchSocket.LTETemporaryStorage.Add(Properties);
        }

        private void SaveToPcap(PacketProperties Properties)      //修改林庆庆
        {
            CatchSocket.CBTCTemporaryStorage.Add(Properties);
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct IPHeader
        {
            [FieldOffset(0)]
            public byte versionAndLength;
            [FieldOffset(1)]
            public byte typeOfServices;
            [FieldOffset(2)]
            public ushort totalLength;
            [FieldOffset(4)]
            public ushort identifier;
            [FieldOffset(6)]
            public ushort flagsAndOffset;
            [FieldOffset(8)]
            public byte timeToLive;
            [FieldOffset(9)]
            public byte protocol;
            [FieldOffset(10)]
            public ushort checksum;
            [FieldOffset(12)]
            public uint sourceAddress;
            [FieldOffset(16)]
            public uint destinationAdress;
        }
    }
}

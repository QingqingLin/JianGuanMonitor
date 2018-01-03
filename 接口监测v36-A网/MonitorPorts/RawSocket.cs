using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Linq;
using read;
using sctp;
using System.IO;
using GTP_V2Decoder;
namespace MonitorPorts
{
    [StructLayout(LayoutKind.Explicit)]
    public struct IPHeader
    {
        [FieldOffset(0)]
        public byte versionAndLength;//4位ip号+4位包头长度
        [FieldOffset(1)]
        public byte typeOfServices;//8位服务类型
        [FieldOffset(2)]
        public ushort totalLength;//16位总长度（字节）
        [FieldOffset(4)]
        public ushort identifier;//16位标识
        [FieldOffset(6)]
        public ushort flagsAndOffset;//3位标志位+13位报片偏移
        [FieldOffset(8)]
        public byte timeToLive;//8位生存时间
        [FieldOffset(9)]
        public byte protocol;//8位协议（TCP，UDP或其他）
        [FieldOffset(10)]
        public ushort checksum;//16位ip包头校验和
        [FieldOffset(12)]
        public uint sourceAddress;//32位ip源地址
        [FieldOffset(16)]
        public uint destinationAdress;//32位目的ip地址
    }
    public class RawSocket
    {
        private bool keepRunning;//是否继续捕获标志
        private static int receiveBufferLength;//捕获的数据流的长度
        private byte[] receiveBufferBytes;//收到的字节流
        private Socket socket = null;//套接字对象

        private List<byte[]> listBytes;
        private List<DateTime> listRecvTimes;
        private List<int> listRecvLenths;

        private List<IAsyncResult> listAsync;
        private DateTime Recv;
        MainForm Main;




        //构造函数
        public RawSocket(MainForm Main)
        {
            this.Main = Main;
            receiveBufferLength = 1500;//设置捕获数据包的最大长度
            receiveBufferBytes = new byte[receiveBufferLength];

            listBytes = new List<byte[]>();
            listRecvTimes = new List<DateTime>();
            listRecvLenths = new List<int>();
            listAsync = new List<IAsyncResult>();
        }
        //创建并绑定套接字对象
        public void CreateAndBindSocket(string ip)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);//SocketType.Raw 设置成为原始套接字
            socket.Blocking = false;//socket是否处于阻塞模式
            socket.Bind(new IPEndPoint(IPAddress.Parse(ip), 0));
            SetSocketOption();
        }

        //设置socket选项
        private void SetSocketOption()
        {
            try
            {
                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, 1);
                //操作需要输入的数据
                byte[] inValue = new byte[4] { 1, 0, 0, 0 };
                //操作输出的数据
                byte[] outValue = new byte[4];
                int ioControlCode = unchecked((int)0x98000001);
                //返回outValue中的字节数
                int returnCode = socket.IOControl(ioControlCode, inValue, outValue);
                returnCode = outValue[0] + outValue[1] + outValue[2] + outValue[3];//returnCode=0 成功
                if (returnCode != 0)
                {
                    throw new SnifferSocketException("command excute error!");
                }
            }
            catch (SocketException ex)
            {
                throw new SnifferSocketException("socket error!", ex);
            }
        }

        public void Start()
        {
            keepRunning = true;
            BeginReceive();
        }

        public void Stop()
        {
            keepRunning = false;
        }


        //开始从连接的socket中异步接收数据
        public void BeginReceive()
        {
            if (socket != null)
            {
                object state = null;
                state = socket;
                //socket开始异步监听数据包，并利用委托AsynccallBack在相应异步完成时调用CallReceive进行处理
                IAsyncResult ar = socket.BeginReceive(receiveBufferBytes, 0, receiveBufferLength, SocketFlags.None, new AsyncCallback(CallReceive), state);
            }
        }

        /// <summary>  
        /// 异步操作完成时调用的方法 异步回调
        /// </summary>
        /// <param name="ar">异步操作状态接口参数</param>
        private void CallReceive(IAsyncResult ar)
        {
            if (receiveBufferBytes[1499] > 0)
            {
                ;
            }

            int receivedBytes = socket.EndReceive(ar);
            Receive(receiveBufferBytes, receivedBytes);
            receiveBufferBytes = new byte[receiveBufferLength];       //清空接收数组
            if (keepRunning == true)
            {
                BeginReceive();
            }
        }
        Sctp1 C = new Sctp1();
        Sctp B = new Sctp();
        private void Receive(byte[] buf, int len)
        {
            byte protocol = 0;
            uint version = 0;
            int sourcePort = 0;
            int destinationPort = 0;
            PacketArrivedEventArgs e = new PacketArrivedEventArgs();
            protocol = buf[9];
            switch (protocol)
            {
                case 6:
                    e.Protocol = "TCP";
                    break;
                case 17:
                    e.Protocol = "UDP";
                    break;
                case 132:
                    e.Protocol = "SCTP";
                    break;
                default:
                    e.Protocol = "UNKNOWN";
                    break;
            }
            version = (uint)((buf[0] & 0xf0) >> 4);
            sourcePort = buf[20] * 256 + buf[21];
            destinationPort = buf[22] * 256 + buf[23];
            if ((protocol == 6) && (sourcePort == 3868 || destinationPort == 3868))//TCP-diameter  (黄罡)
            {
                e.IPHeaderLength = (uint)((buf[0] & 0x0f) << 2);
                e.Version = version.ToString();
                e.OriginationPort = sourcePort.ToString();
                e.DestinationPort = destinationPort.ToString();
                e.DestinationAddress = buf[16].ToString() + "." + buf[17].ToString() + "." + buf[18].ToString() + "." + buf[19].ToString();
                e.OriginationAddress = buf[12].ToString() + "." + buf[13].ToString() + "." + buf[14].ToString() + "." + buf[15].ToString();
                e.PacketLength = (uint)len;
                e.MessageLength = e.PacketLength - e.IPHeaderLength;
                e.PacketBuffer = buf;
                OnPacketArrival(e);//引发PacketArrival事件
                SaveToPcap(e);
            }
            else if ((protocol == 17) && (sourcePort == 2123 || destinationPort == 2123))//UDP-GTPv2 (黄罡)
            {
                e.IPHeaderLength = (uint)((buf[0] & 0x0f) << 2);
                e.Version = version.ToString();
                e.OriginationPort = sourcePort.ToString();
                e.DestinationPort = destinationPort.ToString();
                e.DestinationAddress = buf[16].ToString() + "." + buf[17].ToString() + "." + buf[18].ToString() + "." + buf[19].ToString();
                e.OriginationAddress = buf[12].ToString() + "." + buf[13].ToString() + "." + buf[14].ToString() + "." + buf[15].ToString();
                e.PacketLength = (uint)len;
                e.MessageLength = e.PacketLength - e.IPHeaderLength;
                e.PacketBuffer = buf;
                OnPacketArrival(e);//引发PacketArrival事件
                SaveToPcap(e);


            }
            else if (protocol == 132)
            {
                byte[] sctp = new byte[len - 20];
                Array.Copy(buf, 20, sctp, 0, len - 20);//去除IP头部20字节 
                C.DeScChunk(sctp);
                int PPID = C.paylaodProtocolIdentifier;
                if (PPID == 18)//sctp-s1ap
                {
                    e.IPHeaderLength = (uint)((buf[0] & 0x0f) << 2);
                    e.Version = version.ToString();
                    e.OriginationPort = sourcePort.ToString();
                    e.DestinationPort = destinationPort.ToString();
                    e.DestinationAddress = buf[16].ToString() + "." + buf[17].ToString() + "." + buf[18].ToString() + "." + buf[19].ToString();
                    e.OriginationAddress = buf[12].ToString() + "." + buf[13].ToString() + "." + buf[14].ToString() + "." + buf[15].ToString();
                    e.PacketLength = (uint)len;
                    e.MessageLength = e.PacketLength - e.IPHeaderLength;
                    e.PacketBuffer = buf;
                    OnPacketArrival(e);//引发PacketArrival事件
                    SaveToPcap(e);
                }
                else if ((PPID == 0 || PPID == 46 || PPID == 47 || PPID == 132) && ((sourcePort == 3868) || (destinationPort == 3868) || (sourcePort == 60000) || (destinationPort == 60000)))//sctp-diameter(黄罡)
                {
                    B.DecodeSctpChunk(sctp);
                    if (B.IsDia == 1)
                    {
                        e.IPHeaderLength = (uint)((buf[0] & 0x0f) << 2);
                        e.Version = version.ToString();
                        e.OriginationPort = sourcePort.ToString();
                        e.DestinationPort = destinationPort.ToString();
                        e.DestinationAddress = buf[16].ToString() + "." + buf[17].ToString() + "." + buf[18].ToString() + "." + buf[19].ToString();
                        e.OriginationAddress = buf[12].ToString() + "." + buf[13].ToString() + "." + buf[14].ToString() + "." + buf[15].ToString();
                        e.PacketLength = (uint)len;
                        e.MessageLength = e.PacketLength - e.IPHeaderLength;
                        e.PacketBuffer = buf;
                        OnPacketArrival(e);//引发PacketArrival事件
                        SaveToPcap(e);
                    }
                }
            }
        }//黄罡2017.4.29
        //定义封包数据的事件参数类
        private void SaveToPcap(PacketArrivedEventArgs e)
        {
            FileInfo File = new FileInfo(Main.FileSavePath + "\\" + Main.FileName + ".pcap");
            if (File.Length < 50 * 1024 * 1024)
            {
                BuiltPcapFile(e.PacketBuffer, e.PacketLength);
            }
            else
            {
                Main.CreatPcapFile();
                BuiltPcapFile(e.PacketBuffer, e.PacketLength);
            }
        }
        private void BuiltPcapFile(byte[] packet, uint packetlenth)
        {
            Main.genePcapFile.WritePacketData(System.DateTime.Now, packet, packetlenth);
        }

        public class PacketArrivedEventArgs : EventArgs
        {
            private string protocol;//协议
            private string destinationPort;//目标端口
            private string originationPort;//源端口
            private string destinationAddress;//目标地址
            private string originationAddress;//源地址
            private string version;//ip版本号
            private uint packetLength;//IP数据包总长度
            private uint messageLength;//IP数据包中消息长度
            private uint ipHeaderLength;//IP数据包包头长度
            private byte[] packetBuffer = null;//数据包中数据字节流
            private byte[] ipHeaderBuffer = null;//数据包头部字节流
            private byte[] messageBuffer = null;//数据包消息字节流
            private DateTime date = DateTime.Now;//捕获时间

            public PacketArrivedEventArgs()
            {
                protocol = "";
                destinationPort = "";
                originationPort = "";
                destinationAddress = "";
                originationAddress = "";
                version = "";
                packetLength = 0;
                messageLength = 0;
                ipHeaderLength = 0;
                packetBuffer = new byte[receiveBufferLength];
                ipHeaderBuffer = new byte[receiveBufferLength];
                messageBuffer = new byte[receiveBufferLength];

            }

            public string Protocol
            {
                get { return protocol; }
                set { protocol = value; }
            }

            public string DestinationPort
            {
                get { return destinationPort; }
                set { destinationPort = value; }
            }

            public string OriginationPort
            {
                get { return originationPort; }
                set { originationPort = value; }
            }

            public string DestinationAddress
            {
                get { return destinationAddress; }
                set { destinationAddress = value; }
            }

            public string OriginationAddress
            {
                get { return originationAddress; }
                set { originationAddress = value; }
            }

            public string Version
            {
                get { return version; }
                set { version = value; }
            }

            public uint PacketLength
            {
                get { return packetLength; }
                set { packetLength = value; }
            }

            public uint MessageLength
            {
                get { return messageLength; }
                set { messageLength = value; }
            }


            public uint IPHeaderLength
            {
                get { return ipHeaderLength; }
                set { ipHeaderLength = value; }
            }

            public byte[] PacketBuffer
            {
                get { return packetBuffer; }
                set { packetBuffer = value; }
            }

            public byte[] IPHeaderBuffer
            {
                get { return ipHeaderBuffer; }
                set { ipHeaderBuffer = value; }
            }

            public byte[] MessageBuffer
            {
                get { return messageBuffer; }
                set { messageBuffer = value; }
            }

            public DateTime Date
            {
                get { return date; }
                set { date = value; }
            }
        }



        //定义处理包的委托及事件对象
        public delegate void PacketArrivedEventHandler(object Sender, PacketArrivedEventArgs args);
        public event PacketArrivedEventHandler PacketArrival;

        //触发事件
        protected virtual void OnPacketArrival(PacketArrivedEventArgs e)
        {
            if (PacketArrival != null)
            {
                PacketArrival(this, e);
            }
        }

        //自定义异常类
        public class SnifferSocketException : Exception
        {
            public SnifferSocketException()
                : base()
            {

            }
            public SnifferSocketException(string message)
                : base(message)
            {

            }
            public SnifferSocketException(string message, Exception innerException)
                : base(message, innerException)
            {

            }
        }
    }
}

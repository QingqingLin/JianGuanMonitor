using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonitorPorts
{
    public class PacketProperties
    {
        private string protocol;//协议
        private int _BufLength;
        private int _protocolid;
        private string messagePacketHex;//协议解析
        private string destinationPort;//目标端口
        private string originationPort;//源端口
        private string destinationAddress;//目标地址
        private string originationAddress;//源地址
        private string version;//ip版本号
        private uint packetLength;//IP数据包总长度
        private uint messageLength;//IP数据包中消息长度
        private uint ipHeaderLength;//IP数据包包头长度
        private byte[] packetBuffer = null;//数据包中数据字节流
        private byte[] messageBuffer = null;//数据包消息字节流
        private DateTime date = DateTime.Now;//捕获时间
        //signal
        private double interval;
        private SendDir _sendDir;
        private DateTime _captureTime = DateTime.Now;


        public PacketProperties(int receiveBufferLength)
        {
            packetBuffer = new byte[receiveBufferLength];
            messageBuffer = new byte[receiveBufferLength];
            _buf = new byte[receiveBufferLength];
        }

        public string Protocol
        {
            get { return protocol; }
            set { protocol = value; }
        }
        public int protocolid
        {
            get { return _protocolid; }
            set { _protocolid = value; }
        }
        public string MessagePacketHex
        {
            get { return messagePacketHex; }
            set { messagePacketHex = value; }
        }

        public int BufLength
        {
            get { return _BufLength; }
            set { _BufLength = value; }
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
        //signal
        public double Interval
        {
            get { return interval; }
            set { interval = value; }
        }
        public DateTime CaptureTime
        {
            get { return _captureTime; }
            set { _captureTime = value; }
        }
        public SendDir SendDir
        {
            get { return _sendDir; }
            set { _sendDir = value; }
        }

        private byte[] _buf;
        public byte[] Buf
        {
            get { return _buf; }
            set
            {
                if (_buf != value)
                {
                    _buf = value;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorPorts
{
    class PcapFile
    {
        private DataHead_Signaling _DataHead_Signaling = new DataHead_Signaling();
        private DataHead _DataHead = new DataHead();
        private byte[] _PcapFileHead = new byte[24];
        private byte[] _EthernetData = new byte[14];   //   两种不同类型的数据链路层
        private byte[] _LinuxnetData = new byte[16];  //
        private IPHead _IPHead = new IPHead();
        private UDPHead _UDPHead = new UDPHead();

        public byte[] UpToIPData { get; set; }

        public byte[] PcapFileHead
        {
            get { return _PcapFileHead; }
            set { _PcapFileHead = value; }
        }

        public DataHead_Signaling DataHead_Signaling
        {
            get { return _DataHead_Signaling; }
            set { _DataHead_Signaling = value; }
        }


        public byte[] EthernetData
        {
            get { return _EthernetData; }
            set { _EthernetData = value; }
        }
        public byte[] LinuxnetData
        {
            get { return _LinuxnetData; }
            set { _LinuxnetData = value; }
        }
        public IPHead IPHead
        {
            get { return _IPHead; }
            set { _IPHead = value; }
        }
        public UDPHead UDPHead
        {
            get { return _UDPHead; }
            set { _UDPHead = value; }
        }
        public byte[] Data { get; set; }
        public string Protocol { get; set; }
        public uint DataLength { get; set; }
        public uint PacketLength { get; set; }
        //signal
        public DataHead DataHead
        {
            get { return _DataHead; }
            set { _DataHead = value; }
        }
    }
}

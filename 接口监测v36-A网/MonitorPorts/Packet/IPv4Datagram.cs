using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Collections;

namespace MonitorPorts
{
	public class IPv4Fragment 
    {
		private byte[] Data_ = null;
		private IPv4Fragment Next_ = null;
		private int Offset_ = 0;
		private bool MoreFlag_ = false;
		private int Length_ = 0;
		private int TTL_ = 0;

		public byte[] Data 
        { 
			get { return Data_; }
		}

		public IPv4Fragment Next 
        { 
			get { return Next_; } 
			set { Next_ = value; }
		}

		public int Offset 
        { 
			get { return Offset_; }
			set { Offset_ = value; }
		}

		public bool MoreFlag 
        { 
			get { return MoreFlag_; }
			set { MoreFlag_ = value; }
		}

		public int Length 
        { 
			get { return Length_; }
			set { Length_ = value; }
		}

		public int TTL 
        { 
			get { return TTL_; }
			set { TTL_ = value; }
		}

		public IPv4Fragment() 
        { 

		}

		public void SetData(byte[] Data,int offset, int length) 
        { 
			Data_ = new byte[length];
			Length_ = length;
			Array.Copy(Data,(int)offset,Data_,0,(int)length);
		}
	}


	/// <summary>
	/// Summary description for IpPacket.
    /// IP数据包的描述
	/// </summary>
	public class IPv4Datagram
	{
		private byte[] Header_ = null;
		private int IHL_;//IP数据包头长
		private int TypeOfService_ = 0;//服务类型
		private int Length_ = 0;//IP数据包总长度
		private int Identification_ = 0;//16位标识
        private int R_;//3位标志位
		private int DF_;
		private int MF_;
        private int FragmentOffset_;//13位报片偏移
		private int TTL_;
        private int Protocol_ = 0;
		private String UpperProtocol_ = null;
		private int Checksum_;
		private IPAddress Source_ = null;
		private string SourceName_ = null;
		private IPAddress Destination_ = null;
		private string DestinationName_ = null;
		private int Options_;
		private byte[] Data_ = null;

		private int Proc_ =0;
		private string ProcStr_ ="";
		private int SPort_= 0;
		private int DPort_= 0;
		
		private bool   Complete_ = false;
		private IPv4Fragment FragmentHead_ = null;
		
		public byte[] Header
		{ 
			get { return Header_; }
			set { Header_ = value; }
		}

		public int IHL
		{ 
			get { return IHL_; }
			set { IHL_ = value; }
		}

		public int TypeOfService { 
			get { return TypeOfService_; }
			set { TypeOfService_ = value; }
		}

		public int Length { 
			get { return Length_; }
			set { Length_ = value; }
		}

		public int Identification { 
			get { return Identification_; }
			set { Identification_ = value; }
		}

		public int Protocol { 
			get { return Protocol_; }
			set { SetUpperProtocol(value); }
		}

		public int ReservedFlag
		{ 
			get { return R_; }
			set { R_ = value; }
		}
		
		public int DontFragmentFlag
		{ 
			get { return DF_; }
			set { DF_ = value; }
		}
		
		public int MoreFlag
		{ 
			get { return MF_; }
			set { MF_ = value; }
		}

		public int FragmentOffset
		{ 
			get { return FragmentOffset_; }
			set { FragmentOffset_ = value; }
		}

		public int TTL
		{ 
			get { return TTL_; }
			set { TTL_ = value; }
		}
		
		public string UpperProtocol
		{ 
			get { return UpperProtocol_; }
			set { UpperProtocol_ = value; }
		}

		public int Checksum
		{ 
			get { return Checksum_; }
			set { Checksum_ = value; }
		}

		public IPAddress Source { 
			get { return Source_; }
			set { Source_ = value; }
		}

		public string SourceName 
		{ 
			get { return SourceName_; }
			set { SourceName_ = value; }
		}

		public IPAddress Destination 
		{ 
			get { return Destination_; }
			set { Destination_ = value; }
		}

		public string DestinationName 
		{ 
			get { return DestinationName_; }
			set { DestinationName_ = value; }
		}

		public int SPort
		{ 
			get { return SPort_; }
			set { SPort_ = value; }
		}

		public int DPort
		{ 
			get { return DPort_; }
			set { DPort_ = value; }
		}

		public int Options
		{ 
			get { return Options_; }
			set { Options_ = value; }
		}

		public byte[] Data 
		{ 
			get { return Data_; }
		}

		public bool Complete 
        { 
			get { return Complete_; }
		}

		public IPv4Fragment FragmentList
        { 
			get { return FragmentHead_; }
		}

		public String SourceIP 
        {
			get { return this.Source.ToString(); }
		}

		public String DestinationIP { 
			get { return this.Destination.ToString(); }
		}

		public void SetHeader(byte[] Data) 
		{ 
			Header_ = new byte[IHL];		
			Array.Copy(Data,0,Header_,0,IHL);
		}
		public int Proc
		{ 
			get { return Proc_; }
			set { Proc_ = value; }
		}
		public string ProcStr
		{ 
			get { return ProcStr_; }
			set { ProcStr_ = value; }
		}

		public IPv4Datagram()
		{

		}

		public void AddFragment(IPv4Fragment fragment) 
        { 
			if ( FragmentHead_ == null ) 
            { 
				FragmentHead_ = fragment;
			} 
            else if ( fragment.Offset < FragmentHead_.Offset ) 
            { 
				fragment.Next = FragmentHead_;
				FragmentHead_ = fragment;
			} else { 
				IPv4Fragment temp = FragmentHead_;
				IPv4Fragment last = temp.Next;

				while ( temp != null && fragment.Offset > temp.Offset ) 
                { 
					last = temp;
					temp = temp.Next;
				}

				last.Next = fragment;
				fragment.Next = temp;
			}

			TestComplete();
		}

		// TODO("Code TestComplete()")
		private void TestComplete() 
        {
			bool done = false; 
			IPv4Fragment fragment = FragmentHead_; 
			int current = 0;

			while ( fragment != null ) { 
				if ( fragment.Next != null ) { 
					if ( fragment.Offset == current ) { 
						fragment = fragment.Next;
						current += fragment.Length;
					} else { 
						fragment = null;
					}
				} else { 
					if ( fragment.Offset == current && fragment.MoreFlag == false ) {
						fragment = null;
						done = true;
					} else { 
						fragment = null;
					}
				}
			}

			if ( done ) 
				CombineData();
			this.Complete_ = done;
		}

		private void CombineData() 
        { 
			int length = 0;
			int offset = 0;
			IPv4Fragment temp = FragmentHead_;

			while ( temp != null ) { 
				length += temp.Length;
				temp = temp.Next;
			}

			Data_ = new byte[length];
			Length_ = length;
			temp = FragmentHead_;

			while ( temp != null ) { 
				Array.Copy(temp.Data,0,this.Data_,offset,temp.Length);
				offset += temp.Length;
				temp = temp.Next;
			}
		}

		public static String GetIPString(uint addr) 
        { 
			uint a = addr >> 24;
			uint b = (addr >> 16) & 0xFF;
			uint c = (addr >> 8) & 0xFF;
			uint d = addr & 0xFF;
			String format = "{0}.{1}.{2}.{3}";

			return String.Format(format,a,b,c,d);
		}

		public String GetHashString() 
        { 
			if (this.Protocol==6 ||this.Protocol==17)
			{
				String format1 = "IPv4:{0}:{1}:{2}:{3}:{4}";
				return String.Format(format1,this.GetUpperProtocol(),this.SourceIP,this.DestinationIP,this.SPort.ToString(),this.DPort.ToString());
			}
			String format2 = "IPv4:{0}:{1}:{2}:{3}:{4}";
			return String.Format(format2,this.GetUpperProtocol(),this.SourceIP,this.DestinationIP,"0","0");
		}

//		public String GetPacketHashString() 
//		{ 
//			String format = "{0}:{1}:{2}:{3}";
//			return String.Format(format,this.GetUpperProtocol(),this.SourceIP,this.DestinationIP,this.ProcStr);
//		}
		public String GetPacketHashString() 
		{ 
			String format = "{0}:{1}:{2}:{3}";
			return String.Format(format,this.GetUpperProtocol(),this.SourceName,this.DestinationName,this.ProcStr);
		}

		public bool WasFragmented()
        { 
			Debug.Assert(this.Complete,"The datagram is not complete so this won't be correct");
			return this.FragmentList.Next != null;
		}

		private void SetUpperProtocol(int protocol)
        { 
			this.Protocol_ = protocol;
			switch ( this.Protocol ) { 
				case 1:	this.UpperProtocol_ = "ICMP"; break;
				case 6: this.UpperProtocol_ = "Tcp"; break;
				case 17: this.UpperProtocol_ = "Udp"; break;
				default:
					this.UpperProtocol_ = Protocols.getProtocolName(this.Protocol.ToString());
					break; 
			}
		}

		public String GetUpperProtocol() 
        { 
			return this.UpperProtocol_;
		}

		public void SetPorts()
        {
			if (this.Protocol==6 ||this.Protocol==17)
			{
				SPort = HeaderParser.ToInt(this.Data,0,16);
				DPort = HeaderParser.ToInt(this.Data,16,16);
			}
		}

		public TcpPacket HandleTcpPacket()
		{	
			TcpPacket packet = new TcpPacket();
			int source_port = HeaderParser.ToInt(this.Data,0,16);
			int dest_port = HeaderParser.ToInt(this.Data,16,16);
			int offset    = HeaderParser.ToInt(this.Data,96,4)*4;
			packet.Source = new IPEndPoint(this.Source,source_port);
			packet.Destination = new IPEndPoint(this.Destination,dest_port);
			packet.Sequence = HeaderParser.ToUInt(this.Data,32,32);
			packet.Acknowledgement = HeaderParser.ToUInt(this.Data,64,32);
			packet.DataOffset= offset;
			packet.Urgent = HeaderParser.ToByte(this.Data,106,1) != 0;
			packet.Ack    = HeaderParser.ToByte(this.Data,107,1) != 0;
			packet.Push   = HeaderParser.ToByte(this.Data,108,1) != 0;
			packet.Reset  = HeaderParser.ToByte(this.Data,109,1) != 0;
			packet.Syn    = HeaderParser.ToByte(this.Data,110,1) != 0;
			packet.Fin    = HeaderParser.ToByte(this.Data,111,1) != 0;


			packet.WindowSize = HeaderParser.ToInt(this.Data,112,16);
			packet.Checksum = HeaderParser.ToInt(this.Data,128,16);
			packet.UrgentPointer = HeaderParser.ToInt(this.Data,144,16);

			if(offset>20){
				packet.SetData(this.Data,20,offset-20);
			}

			packet.SetData(this.Data,offset,this.Data.Length - offset);
			return packet;
		}


		public UdpDatagram HandleUdpDatagram()
		{
			UdpDatagram packet = new UdpDatagram();
			int source_port = HeaderParser.ToInt(this.Data,0,16);
			int dest_port = HeaderParser.ToInt(this.Data,16,16);
			int length = HeaderParser.ToInt(this.Data,32,16) - 8;
			packet.Source = new IPEndPoint(this.Source,source_port);
			packet.Destination = new IPEndPoint(this.Destination,dest_port);
			packet.SetData(this.Data,8,length);
			return packet;
		}


		public IcmpPacket HandleIcmpPacket()
		{
			IcmpPacket packet = new IcmpPacket();
			packet.Source = new IPEndPoint(this.Source,0);
			packet.Destination = new IPEndPoint(this.Destination,0);
			packet.Type = HeaderParser.ToByte(this.Data,0,8);
			packet.Code = HeaderParser.ToByte(this.Data,8,8);
			packet.Checksum = HeaderParser.ToUShort(this.Data,16,16);
			packet.SetData(this.Data,4,this.Data.Length-4);
			return packet;
		}

		public ProtocolTemplate HandleOthers(){
			if (Protocols.ContainsProtocol(this.Protocol.ToString())){
				ProtocolTemplate ret =new ProtocolTemplate(Protocols.GetProcFields(this.Protocol.ToString()));
				ret.SetFields(Data_);
				ret.Source_=this.Source_;
				ret.Destination_=this.Destination_;
				return ret;
			}
			return null;
		}

		public byte[] GetBinaryPacket()
        {
			byte [] ret= new byte[this.Header_.Length+this.Data_.Length];
			Array.Copy(this.Header_,0,ret,0,this.Header_.Length);
			Array.Copy(this.Data_,0,ret,this.Header_.Length,this.Data_.Length);
			return ret;
		}

		public int GetInnerDataLength()
		{
			switch ( this.Protocol ) 
			{ 
				case 1:	return (this.Data.Length-8);
				case 6:	return (this.Data.Length - HeaderParser.ToInt(this.Data,96,4)*4);
				case 17: return (HeaderParser.ToInt(this.Data,32,16) - 8);
				default: return 0; 
			}
		}

	}
}

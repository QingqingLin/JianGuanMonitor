using System;
using System.Net;
using System.Net.Sockets;

namespace MonitorPorts 
{
	/// <summary>
	/// 
	/// </summary>
	public class TcpPacket 
	{
		private IPEndPoint Source_ = null;
		private IPEndPoint Destination_ = null;
		private uint Sequence_ = 0;
		private uint Acknowledgement_ = 0;
		private int  DataOffset_ = 0;//Header Length
		private bool Urgent_ = false;
		private bool Push_ = false;
		private bool Ack_ = false;
		private bool Fin_ = false;
		private bool Syn_ = false;
		private bool Reset_ = false;
		private int WindowSize_ = 0;
		private int Checksum_ = 0;
		private int UrgentPointer_ = 0;
		private byte[] Data_ = null;
		private byte[] Options_ = null;
		private int  Length_ = 0;//Data Length

		public int SourcePort 
		{ 
			get { return Source_.Port; }
		}

		public int DestinationPort 
		{ 
			get { return Destination_.Port; }
		}

		public uint Sequence 
		{ 
			get { return Sequence_; }
			set { Sequence_ = value; }
		}

		public uint Acknowledgement 
		{ 
			get { return Acknowledgement_; } 
			set { Acknowledgement_ = value; }
		}

		public int DataOffset 
		{ 
			get { return DataOffset_; }
			set { DataOffset_ = value; }
		}

		public bool Urgent 
		{ 
			get { return Urgent_; }
			set { Urgent_ = value; }
		}

		public bool Push 
		{ 
			get { return Push_; } 
			set { Push_ = value; }
		}

		public bool Ack 
		{ 
			get { return Ack_; }
			set { Ack_ = value; }
		}

		public bool Reset 
		{ 
			get { return Reset_; }
			set { Reset_ = value; }
		}

		public bool Syn 
		{ 
			get { return Syn_; }
			set { Syn_ = value; }
		}

		public bool Fin 
		{ 
			get { return Fin_; }
			set { Fin_ = value; }
		}

		public int WindowSize 
		{ 
			get { return WindowSize_; } 
			set { WindowSize_ = value; }
		}

		public int Checksum 
		{ 
			get { return Checksum_; } 
			set { Checksum_ = value; }
		}

		public int UrgentPointer 
		{ 
			get { return UrgentPointer_; } 
			set { UrgentPointer_ = value; }
		}

		public byte[] Options
		{ 
			get { return Options_; }
			set { Options_ = value; }
		}

		public byte[] Data 
		{ 
			get { return Data_; }
			set { Data_ = value; }
		}

		public int Length 
		{
			get { return Length_; }
			set { Length_ = value; }
		}

		public IPEndPoint Source 
		{ 
			get { return Source_; }
			set { Source_ = value; }
		}

		public IPEndPoint Destination 
		{ 
			get { return Destination_; }
			set { Destination_ = value; }
		}

		public String DestinationIP 
		{ 
			get { return this.Destination.Address.ToString(); }
		}

		public String SourceIP 
		{ 
			get { return this.Source.Address.ToString(); }
		}

		public TcpPacket()
		{
		}

		public void SetData(byte[] data, int offset, int length) 
		{
			Data_ = new byte[length];
			Array.Copy(data,offset,Data_,0,length);
		}

		public void SetOptions(byte[] data, int offset, int length) 
		{
			Data_ = new byte[length];
			Array.Copy(data,offset,Options_,0,length);
		}

		public String GetHashString() 
		{ 
			String format = "Tcp:{0}:{1}";
			return String.Format(format,this.SourceIP,this.DestinationIP);
		}

	}
}

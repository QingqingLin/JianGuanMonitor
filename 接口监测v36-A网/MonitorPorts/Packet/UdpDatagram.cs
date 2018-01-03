using System;
using System.Net;
using System.Net.Sockets;

namespace MonitorPorts
{
	/// <summary>
	/// 
	/// </summary>
	public class UdpDatagram
	{
		private IPEndPoint Source_ = null;
		private IPEndPoint Destination_ = null;
		private byte[]     Data_ = null;

		public int SourcePort 
		{ 
			get { return Source_.Port; }
		}

		public int DestinationPort 
		{ 
			get { return Destination_.Port; }
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

		public byte[] Data 
		{ 
			get { return Data_; }
		}

		public UdpDatagram()
		{
		}

		public void SetData(byte[] data, int offset, int length ) 
		{ 
			Data_ = new byte[length];
			Array.Copy(data,offset,Data_,0,length);
		}

		public String GetHashString() 
		{ 
			String format = "Udp:{0}:{1}";			
			return String.Format(format,this.SourceIP,this.DestinationIP);
		}
	}
}

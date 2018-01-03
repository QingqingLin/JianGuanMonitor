using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;

namespace MonitorPorts
{
	/// <summary>
	/// Summary description for ProtocolTemplate.
	/// </summary>
	public class ProtocolTemplate
	{
		public IPAddress Source_ = null;
		public IPAddress Destination_ = null;
		private byte[] Data_ = null;
		private int  Length_ = 0;
		private int  HeaderLength_ = 0;

		private string ProtocolName_="";
		private int Protocol_=0;
		private Hashtable FieldLengths_;
		private Hashtable Fields_;
		private ArrayList Keys_;


		public ProtocolTemplate(Hashtable fieldLengths)
		{
			int [] x=new int[2];
			int max=0;
			Fields_=new Hashtable();
			FieldLengths_=new Hashtable();
			Keys_=new ArrayList();
			Hashtable temp = new Hashtable();
			ArrayList arr = new ArrayList();
			foreach (string key in fieldLengths.Keys)
			{
				if (key=="name")
					this.ProtocolName=(string)fieldLengths[key];
				else if (key=="id")
					this.Protocol=Convert.ToInt32(fieldLengths[key]);
				else{
					x=(int[])fieldLengths[key];
					arr.Add(x[0]);
					temp.Add(x[0].ToString(),key);
				} 
			}

			arr.Sort();

			for (int i=0; i<arr.Count;i++)
			{
				Keys_.Add(temp[arr[i].ToString()]);
			}

			for (int i=0;i<Keys_.Count;i++){
				FieldLengths_.Add(Keys_[i],fieldLengths[Keys_[i]]);
				x=(int[])FieldLengths_[Keys_[i]];
				if (x[0]>=max)
				{
					max=x[0];
					this.HeaderLength_=(max+x[1])/8;
				}
			}
		}

		public bool SetFields(Byte[] buffer)
		{
			if (buffer.Length>=this.HeaderLength_)
			{
				int [] x=new int[2];
				for (int i=0;i<Keys_.Count;i++)
				{
					x=(int[])FieldLengths_[Keys_[i]];
					if(x[1]<=8)
						Fields_.Add(Keys_[i],HeaderParser.ToByte(buffer,x[0],x[1]));
					else if (x[1]<=16)
						Fields_.Add(Keys_[i],HeaderParser.ToInt(buffer,x[0],x[1]));
					else if (x[1]<=32)
						Fields_.Add(Keys_[i],HeaderParser.ToUInt(buffer,x[0],x[1]));
				}
				this.SetData(buffer,this.HeaderLength_,buffer.Length-this.HeaderLength_);
				Length_=buffer.Length;
				return true;
			}
			return false;
		}

		private void SetData(byte[] data, int offset, int length) 
		{
			Data_ = new byte[length];
			Array.Copy(data,offset,Data_,0,length);
		}

		public uint GetField(int offset)
		{
			return GetField((string)Keys_[offset]);
		}

		private uint GetField(string key)
		{
			return Convert.ToUInt32(Fields_[key]);
		}
		public int GetFieldCount()
		{
			return Keys_.Count;
		}
		public string GetFieldName(int offset)
		{
			return (string)Keys_[offset];
		}

		public int[] GetFieldLength(int offset)
		{
			return (int[])FieldLengths_[(string)Keys_[offset]];
		}


		public int Protocol
		{ 
			get { return Protocol_; }
			set {Protocol_=value;}
		}
		public String ProtocolName 
		{ 
			get { return ProtocolName_; }
			set {ProtocolName_=value;}
		}
//		public IPEndPoint Source 
//		{ 
//			get { return Source_; }
//			set { Source_ = value; }
//		}
//
//		public IPEndPoint Destination 
//		{ 
//			get { return Destination_; }
//			set { Destination_ = value; }
//		}

		public String DestinationIP 
		{ 
			get { return this.Destination_.ToString(); }
		}

		public String SourceIP 
		{ 
			get { return this.Source_.ToString(); }
		}

		public String GetHashString() 
		{ 
			String format = "{0}:{1}:{2}";
			return String.Format(format,this.ProtocolName,this.SourceIP,this.DestinationIP);
		}
	}
}

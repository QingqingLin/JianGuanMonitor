using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;

namespace MonitorPorts
{
	/// <summary>
	/// Summary description for IcmpPacket.
	/// </summary>
	public class IcmpPacket
	{
		private IPEndPoint Source_ = null;
		private IPEndPoint Destination_ = null;
		private byte Type_;
		private byte Code_;
        private UInt16 Checksum_;
		private int DataSize_;
		private byte[] Data_ = new byte[1024];
		private bool dataIsIp_=false;
		private bool hasNextField_=false;

		/// <summary>
		/// optionals
		/// </summary>
		/// 

		private ArrayList FieldNames_;
		private ArrayList FieldLength_;
		private Hashtable FieldValues_;


		public IcmpPacket()
		{
			
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
			get { return this.Destination_.Address.ToString(); }
		}

		public String SourceIP 
		{ 
			get { return this.Source_.Address.ToString(); }
		}

		public byte Type
		{ 
			get { return Type_; }
			set {Type_=value;}
		}

		public byte Code
		{ 
			get { return Code_; }
			set {Code_=value;}
		}

		public UInt16 Checksum
		{ 
			get { return Checksum_; }
			set {Checksum_=value;}
		}

		public int DataSize
		{ 
			get { return DataSize_; }
			set {DataSize_=value;}
		}

		public Byte[] Data
		{ 
			get { return Data_; }
			set {Data_=value;}
		}

		public bool dataIsIp
		{ 
			get { return dataIsIp_; }
			set {dataIsIp_=value;}
		}

		public bool hasNextField
		{ 
			get { return hasNextField_; }
			set {hasNextField_=value;}
		}

		public void SetData(byte[] data, int offset, int length) 
		{
			int ret;
			ret=CheckNextFields(data);
			if (ret!=0) hasNextField=true;
			Data_ = new byte[length-ret];
			Array.Copy(data,ret,Data_,0,length-ret);
		}

		public String GetHashString() 
		{ 
			String format = "ICMP:{0}:{1}";
			return String.Format(format,this.SourceIP,this.DestinationIP);
		}


		public IcmpPacket(byte[] data, int size)
		{
			Type_ = data[20];
			Code_ = data[21];
			Checksum_ = BitConverter.ToUInt16(data, 22);
			DataSize_ = size - 24;
			Buffer.BlockCopy(data, 24, Data_, 0, DataSize_);
		}
		public byte[] getBytes()
		{
			byte[] data = new byte[DataSize_ + 9];
			Buffer.BlockCopy(BitConverter.GetBytes(Type_), 0, data, 0, 1);
			Buffer.BlockCopy(BitConverter.GetBytes(Code_), 0, data, 1, 1);
			Buffer.BlockCopy(BitConverter.GetBytes(Checksum_), 0, data, 2, 2);
			Buffer.BlockCopy(Data_, 0, data, 4, DataSize_);
			return data;
		}
		public UInt16 getChecksum()
		{
			UInt32 chcksm = 0;
			byte[] data = getBytes();
			int packetsize = DataSize_ + 8;
			int index = 0;
			while ( index < packetsize)
			{
				chcksm += Convert.ToUInt32(BitConverter.ToUInt16(data, index));
				index += 2;
			}
			chcksm = (chcksm >> 16) + (chcksm & 0xffff);
			chcksm += (chcksm >> 16);
			return (UInt16)(~chcksm);
		}

		public int CheckNextFields(byte [] data){
			int ret =0;
			switch(this.Type){
				case 0:
				case 8:
				case 13:
				case 14:
				case 15:
				case 16:
				case 17:
				case 18:
				case 37:
				case 38:
					FieldNames_=new ArrayList();
					FieldLength_=new ArrayList();
					FieldValues_=new Hashtable();
					this.FieldLength_.Add(16);
					this.FieldNames_.Add("Identifier");
					this.FieldValues_.Add("Identifier",HeaderParser.ToUInt(data,32,16));
					this.FieldLength_.Add(16);
					this.FieldNames_.Add("Sequence Number");
					this.FieldValues_.Add("Sequence Number",HeaderParser.ToUInt(data,48,16));
					ret=8;
					this.dataIsIp=false;					
					break;
				case 4:
				case 11:
					FieldNames_=new ArrayList();
					FieldLength_=new ArrayList();
					FieldValues_=new Hashtable();
					this.FieldLength_.Add(32);
					this.FieldNames_.Add("Reserved");
					this.FieldValues_.Add("Reserved",HeaderParser.ToUInt(data,32,32));
					ret=8;
					this.dataIsIp=true;
					break;
				case 10:
					FieldNames_=new ArrayList();
					FieldLength_=new ArrayList();
					FieldValues_=new Hashtable();
					this.FieldLength_.Add(32);
					this.FieldNames_.Add("Reserved");
					this.FieldValues_.Add("Reserved",HeaderParser.ToUInt(data,32,32));
					ret=8;
					this.dataIsIp=false;
					break;
				case 3:
					FieldNames_=new ArrayList();
					FieldLength_=new ArrayList();
					FieldValues_=new Hashtable();
					this.FieldLength_.Add(16);
					this.FieldNames_.Add("Unused");
					this.FieldValues_.Add("Unused",HeaderParser.ToUInt(data,32,16));
					this.FieldLength_.Add(16);
					this.FieldNames_.Add("Next Hop MTU");
					this.FieldValues_.Add("Next Hop MTU",HeaderParser.ToUInt(data,48,16));
					ret=8;
					this.dataIsIp=true;					
					break;
				case 5:
					FieldNames_=new ArrayList();
					FieldLength_=new ArrayList();
					FieldValues_=new Hashtable();
					this.FieldLength_.Add(32);
					this.FieldNames_.Add("IP Address");
					this.FieldValues_.Add("IP Address",HeaderParser.ToUInt(data,32,32));
					ret=8;
					this.dataIsIp=true;
					break;
				case 31:
					FieldNames_=new ArrayList();
					FieldLength_=new ArrayList();
					FieldValues_=new Hashtable();
					this.FieldLength_.Add(32);
					this.FieldNames_.Add("Offset");
					this.FieldValues_.Add("Offset",HeaderParser.ToUInt(data,32,32));
					ret=8;
					this.dataIsIp=false;
					break;

				case 12:
					FieldNames_=new ArrayList();
					FieldLength_=new ArrayList();
					FieldValues_=new Hashtable();
					this.FieldLength_.Add(8);
					this.FieldNames_.Add("Pointer");
					this.FieldValues_.Add("Pointer",HeaderParser.ToUInt(data,32,8));
					this.FieldLength_.Add(24);
					this.FieldNames_.Add("Unused");
					this.FieldValues_.Add("Unused",HeaderParser.ToUInt(data,40,24));
					ret=8;
					this.dataIsIp=true;
					break;

				case 9:
					FieldNames_=new ArrayList();
					FieldLength_=new ArrayList();
					FieldValues_=new Hashtable();
					this.FieldLength_.Add(8);
					this.FieldNames_.Add("Advertisement Count");
					this.FieldValues_.Add("Advertisement Count",HeaderParser.ToUInt(data,32,8));
					this.FieldLength_.Add(8);
					this.FieldNames_.Add("Address Entry Size");
					this.FieldValues_.Add("Address Entry Size",HeaderParser.ToUInt(data,40,8));
					this.FieldLength_.Add(16);
					this.FieldNames_.Add("Life Time");
					this.FieldValues_.Add("Life Time",HeaderParser.ToUInt(data,48,16));
					ret=8;
					this.dataIsIp=false;
					break;
				case 30:
					FieldNames_=new ArrayList();
					FieldLength_=new ArrayList();
					FieldValues_=new Hashtable();
					this.FieldLength_.Add(16);
					this.FieldNames_.Add("Identifier");
					this.FieldValues_.Add("Identifier",HeaderParser.ToUInt(data,32,16));
					this.FieldLength_.Add(16);
					this.FieldNames_.Add("Unused");
					this.FieldValues_.Add("Unused",HeaderParser.ToUInt(data,48,16));
					this.FieldLength_.Add(16);
					this.FieldNames_.Add("Out Bount Hop Count");
					this.FieldValues_.Add("Out Bount Hop Count",HeaderParser.ToUInt(data,64,16));
					this.FieldLength_.Add(16);
					this.FieldNames_.Add("Return Hop Count");
					this.FieldValues_.Add("Return Hop Count",HeaderParser.ToUInt(data,80,16));
					this.FieldLength_.Add(32);
					this.FieldNames_.Add("Output Link Speed");
					this.FieldValues_.Add("Output Link Speed",HeaderParser.ToUInt(data,96,32));
					this.FieldLength_.Add(32);
					this.FieldNames_.Add("Output Link MTU");
					this.FieldValues_.Add("Output Link MTU",HeaderParser.ToUInt(data,128,32));
					ret=20;
					this.dataIsIp=false;
					break;
				case 40:
					FieldNames_=new ArrayList();
					FieldLength_=new ArrayList();
					FieldValues_=new Hashtable();
					this.FieldLength_.Add(16);
					this.FieldNames_.Add("Reserved");
					this.FieldValues_.Add("Reserved",HeaderParser.ToUInt(data,32,16));
					this.FieldLength_.Add(16);
					this.FieldNames_.Add("Pointer");
					this.FieldValues_.Add("Pointer",HeaderParser.ToUInt(data,48,16));
					ret=8;
					this.dataIsIp=false;
					break;
				default: break;
			}
			return ret;

		}


		public string GetFieldName(int index)
		{
			return (string)this.FieldNames_[index];
		}

		public int GetFieldCount()
		{
			return this.FieldNames_.Count;
		}

		public int GetFieldLengt(int index)
		{
			return (int)this.FieldLength_[index];
		}

		public uint GetField(string key)
		{
			return (uint)this.FieldValues_[key];
		}

		public uint GetField(int index)
		{
			return (uint)this.FieldValues_[GetFieldName(index)];
		}


	}
}

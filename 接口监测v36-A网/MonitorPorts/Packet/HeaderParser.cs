using System;
using System.Diagnostics;

namespace MonitorPorts
{
	/// <summary>
	/// Summary description for HeaderParser.
	/// </summary>
	public class HeaderParser 
    {
		public static uint ToUInt(byte[] datagram, int offset, int length)
        {
			uint total = 0;
			int byte_index;
			int bit_offset;
			int bit;
			byte b;
			
			for ( int i = 0; i < length; i++ ) {
				bit_offset = (offset+i) % 8;
				byte_index = (offset+i-bit_offset) / 8;
				b = datagram[byte_index];
				bit = (int)(b >> (7 - bit_offset));
				bit = bit & 0x0001;

				if ( bit > 0 ) { 
					total += (uint)Math.Pow(2,length-i-1);
				}
			}

			return total;
		}

		public static int ToInt(byte[] datagram, int offset, int length ) {
			return (int)ToUInt(datagram,offset,length);
		}

		public static ushort ToUShort(byte[] datagram, int offset, int length) {
			return (ushort)(ToUInt(datagram,offset,length));
		}

		public static byte ToByte(byte[] datagram,int offset, int length) { 
			return (byte)(ToUInt(datagram,offset,length));
		}
	}
}

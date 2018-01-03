using System;

namespace MonitorPorts

{
	/// <summary>
	/// Summary description for consts.
	/// </summary>
	public class consts
	{
		static consts()
		{
			//
			// TODO: Add constructor logic here
			//
		}


		#region ip

		public static string GetTOSForIP(int TOS)
		{
			switch(TOS)
			{
				case 0: return "Routine";
				case 1: return "Priority";
				case 2: return "Immediate";
				case 3: return "Flash";
				case 4: return "Flash Override";
				case 5: return "CRITIC/ECP";
				case 6: return "Internetwork Control";
				case 7: return "Network Control";
			}
			return "unknown";
		}		

		#endregion

		#region tcp

		public static string GetTcpPorts(int Port)
		{
			switch(Port)
			{
				case 7: return "Echo";
				case 13: return "Daytime";
				case 17: return "Quote of the day";
				case 20: return "FTP (data channel)";
				case 21: return "FTP (control channel)";
				case 22: return "SSH";
				case 23: return "Telnet";
				case 25: return "SMTP";
				case 37: return "Time";
				case 80: return "HTTP";
				case 110: return "POP3";
				case 119: return "NNTP";
				case 123: return "Network Time Protocol (NTP)";
				case 137: return "NETBIOS name service";
				case 138: return "NETBIOS datagram service";
				case 139: return "NETBIOS Session";
				case 143: return "Internet Message Access Protocol (IMAP)";
				case 161: return "SNMP";
				case 389: return "Lightweight Directory Access Protocol (LDAP)";
				case 443: return "Secure HTTP (HTTPS)";
				case 993: return "Secure IMAP";
				case 995: return "Secure POP3";
				case 6000: return "XWINDOWS";
				case 7070: return "RA_HANDSHAKE";
				default : return "Unknown";
			}
		} 

		#endregion

		#region udp

		public static string GetUdpPorts(int Port)
		{
			switch(Port)
			{
				case 53: return "Domain Name System";
				case 69: return "Trivial File Transfer Protocol";
				case 111: return "Remote Procedure Call";
				case 137: return "NetBIOS name service";
				case 138: return "NetBIOS datagram";
				case 161: return "Simple Network Management Protocol";
				case 1021: return "NFS";
				default : return "Unknown";
			}
		} 

		#endregion

		#region icmp

		public static string GetTypeForIcmp(int type)
		{
			switch(type)
			{
				case 0: return "Echo Reply";
				case 3: return "Destination Unreachable";
				case 4: return "Source Quench";
				case 5: return "Redirect";
				case 6: return "Alternate Host Address";
				case 8: return "Echo Request";
				case 9: return "Router Advertisement";
				case 10: return "Router Selection";
				case 11: return "Time Exceeded";
				case 12: return "Parameter Problem";
				case 13: return "Time Stamp";
				case 14: return "Time Stamp Reply";
				case 15: return "Information Request";
				case 16: return "Information Reply";
				case 17: return "Address Mask Request";
				case 18: return "Address Mask Reply";
				case 19: return "Reserved (for Security)";
				case 20:
				case 21:
				case 22:
				case 23:
				case 24:
				case 25:
				case 26:
				case 27:
				case 28:
				case 29: return "Reserved (for Robustness Experiments)";
				case 30: return "Traceroute";
				case 31: return "Datagram Conversion Error";
				case 32: return "Mobile Host Redirect";
				case 35: return "Mobile Registration Request";
				case 36: return "Mobile Registration Reply";
				case 39: return "SKIP";
				case 40: return "Photuris";
			}
			return "unknown";
		}

		public static string GetCodeForIcmp(int type, int code)
		{
			switch(type)
			{
				//Error Messages

				case 3://Destination Unreachable
				
				switch(code)
				{
					case 0: return "Network Unreachable";
					case 1: return "Host Unreachable";
					case 2: return "Protocol Unreachable";
					case 3: return "Port Unreachable";
					case 4: return "Fragmentation needed but Dont Fragment Flag is set";
					case 5: return "Source Route Failed";
					case 6: return "Destionation Network Unknown";
					case 7: return "Destination Host Unknown";
					case 8: return "Source Host Isolated";
					case 9: return "Communication with Destination Host Administratively Prohibited";
					case 10: return "Communication with Destination Host Administratively Prohibited";
					case 11: return "Network Unreachable for Type of Service";
					case 12: return "Host Unreachable for Type of Service";
					case 13: return "Communication Administratively Prohibited";
					case 14: return "Host Precedence Violation";
					case 15: return "Precedence cutoff in effect";
					default: return "Invalid code";
				}
				
				case 4://Source Quench
				
				switch(code)
				{
					case 0: return "Valid code";
					default: return "Invalid code";
				}

				case 5://Redirect
				
				switch(code)
				{
					case 0: return "Redirect Datagram for the Network";
					case 1: return "Redirect Datagram for the Host";
					case 2: return "Redirect Datagram for the Type of Service and Network";
					case 3: return "Redirect Datagram for the Type of Service and Host";
					default : return "Invalid code";
				}

				case 11://Time Exceeded
				
				switch(code)
				{
					case 0: return "Time-to-Live Exceeded in Transit";
					case 1: return "Fragment Reassembly Time Exceeded";
					default: return "Invalid code";
				}

				case 12://Parameter Problem
				
				switch(code)
				{
					case 0: return "Pointer indicates the location of the problem";
					case 1: return "Missing a Required Option";
					case 2: return "Bad Length";
					default: return "Invalid code";
				}

				//Quary Messages
				
				case 8://Echo Request
				case 0://Echo Reply
				case 10://Router Selection
				case 6://Alternate Host Address
				case 13://Time Stamp Request
				case 14://Time Stamp Reply
				case 15://Information Request
				case 16://Information Reply
				case 17://Address Mask Request
				case 18://Address Mask Reply
				case 19://Reserved (for Security)

				switch(code)
				{
					case 0: return "Valid";
					default: return "Invalid code";
				}


				case 40://Photuris
				
				switch(code)
				{
					case 0: return "Bad SPI";
					case 1: return "Authentication Failed";
					case 2: return "Decompression Failed";
					case 3: return "Decryption Failed";
					case 4: return "Need Authentication";
					case 5: return "Need Authorization";
					default: return "Invalid code";
				}

				case 9://Router Advertisement
				
				switch(code)
				{
					case 0: return "Normal router advertisement";
					case 1: return "Does not route common traffic";
					default: return "Invalid code";
				}

				default:
					return "No code defined";

			}
		}
		#endregion
	}
}

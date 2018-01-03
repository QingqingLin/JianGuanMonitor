using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using sctp;
using MonitorPorts;
using GTP_V2Decoder;
using MonitorPorts.Packet;
namespace read
{
    class Readdata
    {
        SctpDecode sctp1 = new SctpDecode();

        s1ap_name s1ap_name1 = new s1ap_name();

        //diamter
        TCPDiameter tcp1 = new TCPDiameter();
        SCTPDiameter sctp_dia = new SCTPDiameter();

        //gtpv2
        gtpv2 Gtpv2 = new gtpv2();

        public int protocolid = 0;
        public uint s1apLength=0;
        //nas解密解码
        s1ap_nas1 s1ap_nas2 = new s1ap_nas1();
        //传递去除头部以后的数据（gtpv2去除IP头及UDP头，其他两个只去除IP头）
        public byte[] SourceData = new byte[1];
        public string RRead(byte[] st)
        {
            protocolid = 0;
            string str = "";
            //int st.Length = 0;
            int IP_length = 0;
            //List<byte[]> arg = new List<byte[]>();
            IP_length = st[2] * 256 + st[3];
            //string SourceData = GetDataHex(st, 0, IP_length);//从10000字节中取出有用的报文（IP报）
            int ip_flag = st[9];//UDP=1
            int src_port = st[20] * 256 + st[21];//源Port
            int port_flag = st[22] * 256 + st[23];//目的Port   
            //data1.Add(SourceData);
            if (ip_flag == 17 && ((port_flag == 2123) || (src_port == 2123))) //将TCP/GTP（源或目的端口都有可能为2123）
            {
                //nas解密解码（没有nas的地方也需要填充，保证listview中的No与 Nasclass下标对应）
                //nas_class nas_class1 = new nas_class();
                //List<nas_class> Nasclass = new List<nas_class>();//用于存放所有解析出来的nas信息

                //nas_class1.cause_str = " ";
                //nas_class1.identity = " ";
                //nas_class1.nas_name = " ";
                //Nasclass.Add(nas_class1);
                //Nasclass1.Add(Nasclass);

                byte[] arf = new byte[IP_length - 28];
                Array.Copy(st, 28, arf, 0, IP_length - 28);
                //arg.Add(arf);
                //data.Add(arg);
                Gtpv2.decoder(arf);
                str = Gtpv2.mt;
                protocolid = 3;
                Array.Copy(arf, 0, SourceData, 0, arf.Length);
            }
            else if (ip_flag == 132 && ((port_flag == 3868) || (src_port == 3868))) //将STCP/Diameter（源或目的端口都有可能为3868）
            {
                //nas解密解码（没有nas的地方也需要填充，保证listview中的No与 Nasclass下标对应）
                //nas_class nas_class1 = new nas_class();
                //List<nas_class> Nasclass = new List<nas_class>();//用于存放所有解析出来的nas信息

                //nas_class1.cause_str = " ";
                //nas_class1.identity = " ";
                //nas_class1.nas_name = " ";
                //Nasclass.Add(nas_class1);
                //Nasclass1.Add(Nasclass);

                byte[] arf = new byte[IP_length - 20];
                Array.Copy(st, 20, arf, 0, IP_length - 20);
                //arg.Add(arf);
                //data.Add(arg);

                sctp_dia.DecodeSctpChunk(arf);
                if (sctp_dia.paylaodProtocolIdentifier == 0 || sctp_dia.paylaodProtocolIdentifier == 46 || sctp_dia.paylaodProtocolIdentifier == 47 || sctp_dia.paylaodProtocolIdentifier == 132)
                {
                    str = sctp_dia.diameterContent;
                    protocolid = 1;
                }
                else
                {
                    protocolid = 5;
                    str = "DATA";
                }
                SourceData = arf;
            }
            else if (ip_flag == 132 && (port_flag != 3868) && (src_port != 3868)) //将STCP/S1ap（源或目的端口都有可能为3868）( && (port_flag != 3868) && (src_port != 3868))
            {
                List<byte[]> s1ap2 = new List<byte[]>();//用于存储当前的s1ap消息
                byte[] arf = new byte[IP_length - 20];
                Array.Copy(st, 20, arf, 0, IP_length - 20);
                s1apLength = 0;
                s1ap2 = sctp1.DeScChunk(arf);//arf为s1ap数据包或DATA块,data是包含0xff与S1ap消息的,包含所有抓到的数据，用于解码树解码
                //if (s1ap2.Count > 0)
                //{
                //    data.Add(s1ap2);
                //}
                //else
                //{
                //    List<byte[]> brg = new List<byte[]>();
                //    byte[] brf = new byte[1] { 0xff };
                //    brg.Add(brf);
                //    data.Add(brg);
                //}
                if (s1ap2.Count == 0)//该消息是sctp消息，但不含S1AP消息
                {
                    //nas解密解码（没有nas的地方也需要填充，保证listview中的No与 Nasclass下标对应）
                    //nas_class nas_class1 = new nas_class();
                    //List<nas_class> Nasclass = new List<nas_class>();//用于存放所有解析出来的nas信息

                    //nas_class1.cause_str = " ";
                    //nas_class1.identity = " ";
                    //nas_class1.nas_name = " ";
                    //Nasclass.Add(nas_class1);
                    //Nasclass1.Add(Nasclass);

                    str = "DATA";
                    protocolid = 5;
                }
                else
                {
                    str = s1ap_name1.s1ap_decode(s1ap2);
                    protocolid = 4;
                    for (int No = 0; No < s1ap2.Count; No++)
                        s1apLength += (uint)s1ap2[No].Length;
                    //nas解密解码
                    //List<nas_class> Nasclass = new List<nas_class>();//用于存放所有解析出来的nas信息
                    //for (int j = 0; j < s1ap2.Count; j++)
                    //{
                    //    //nas_class nas_class1 = new nas_class();
                    //    s1ap_nas2.nas_decode(s1ap2[j]);
                    //    //nas_class1.identity = s1ap_nas2.identity;
                    //    //nas_class1.cause_str = s1ap_nas2.cause_str;
                    //    str = str + s1ap_nas2.nas_name;
                    //    //Nasclass.Add(nas_class1);
                    //}
                    //Nasclass1.Add(Nasclass);
                }
                SourceData = arf;
            }
            else if (ip_flag == 6 && ((port_flag == 3868) || (src_port == 3868)))//将TCP/Diameter（源或目的端口都有可能为3868）
            {
                //nas解密解码（没有nas的地方也需要填充，保证listview中的No与 Nasclass下标对应）
                //nas_class nas_class1 = new nas_class();
                //List<nas_class> Nasclass = new List<nas_class>();//用于存放所有解析出来的nas信息

                //nas_class1.cause_str = " ";
                //nas_class1.identity = " ";
                //nas_class1.nas_name = " ";
                //Nasclass.Add(nas_class1);
                //Nasclass1.Add(Nasclass);

                byte[] arf = new byte[IP_length - 20];
                Array.Copy(st, 20, arf, 0, IP_length - 20);
                //arg.Add(arf);
                //data.Add(arg);
                tcp1.DecodeTcpChunk(arf);
                str = tcp1.diameterContent;
                protocolid = 2;
                SourceData = arf;
            }
            else//非gtpv2,diameter,s1ap消息
            {
                //nas解密解码（没有nas的地方也需要填充，保证listview中的No与 Nasclass下标对应）
                //nas_class nas_class1 = new nas_class();
                //List<nas_class> Nasclass = new List<nas_class>();//用于存放所有解析出来的nas信息



                //nas_class1.cause_str = " ";
                //nas_class1.identity = " ";
                //nas_class1.nas_name = " ";
                //Nasclass.Add(nas_class1);
                //Nasclass1.Add(Nasclass);

                //byte[] brf = new byte[1] { 0xff };
                //arg.Add(brf);
                //data.Add(arg);
                str = "DATA";
            }

            return str;
        }
        public static string GetDataHex(Byte[] Data, int index, int count)
        {
            string DataHex = "";
            for (int i = index; i < index + count; i++)
            {
                if (i > index && (i - index) % 16 == 0)
                {
                    DataHex += "\r\n";
                }
                if (Data[i].ToString("X").Length != 1)
                {
                    DataHex += Data[i].ToString("X") + " ";
                }
                else
                {
                    DataHex += "0" + Data[i].ToString("X") + " ";
                }
            }

            return DataHex;
        }
        public static byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
    }
}

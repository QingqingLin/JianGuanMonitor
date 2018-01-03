using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using MonitorPorts.Packet;

namespace MonitorPorts
{
    class SCTPDiameter
    {
        public string protocol = string.Empty;
        public string sourIP = string.Empty;
        public string sourPort = string.Empty;
        public string destIP = string.Empty;
        public string destPort = string.Empty;
        public string allLength = string.Empty;
        public string messageBodyLen = string.Empty;
        public string messageBodyHex = string.Empty;
        public string version;
        public string length;
        public string flag;
        public string commandCODE;
        public string applicationID;
        public string H2HIdentifier;
        public string E2EIdentifier;
        public string userNameCode, userNameFlag, userNameLength, userNameStr, avp_ResultcodeCode, avp_ResultcodeFlag,
            avp_ResultcodeLength, avp_ResultcodeStr, avp_AuthenticationCode, avp_AuthenticationFlag, avp_AuthenticationLength,
            avp_AuthenticationVendorID, avp_SessionID1, avp_SessionID2, avp_SessionID3, avp_SessionID4;
        public List<string> avp_EUTRANVectorCode = new List<String>();
        public List<string> avp_EUTRANVectorFlag = new List<String>();
        public List<string> avp_EUTRANVectorLength = new List<String>();
        public List<string> avp_EUTRANVectorVendorID = new List<String>();
        public List<string> avp_KASMECode = new List<String>();
        public List<string> avp_KASMEFlag = new List<String>();
        public List<string> avp_KASMELength = new List<String>();
        public List<string> KASMEStr = new List<String>();
        public List<byte[]> KASMEValue = new List<byte[]>();
        public char[] ascii;
        public string protocol_Treeview;//treeview上显示的协议名称
        public string diameterContent;//diameter解析后概要内容
        public string command;//信令（内容+方向）
        public byte diameterDirection;//diameter信令方向
        static byte chunckType = 0;//消息块类型
        static byte chunckFlag = 0;//消息块标志位
        static int chunckLength = 0;//消息块长度
        public int paylaodProtocolIdentifier;//PPID
        public uint chunckCommandCODE;//消息块的命令代码
        public int IsDiameter;//该消息是否为diameter消息
        Diameter Diameter = new Diameter();
        public int diameterLocation;//diameter在buf中的起始位置
        public int diameterLength;//diameter长度
        //各项消息的位置和长度记录
        public int avpLocation_UserName, avpLocation_Result, avpLocation_Authentication, avpLocation_SessionID;//记录各avp的起始位置信息
        public int avpLength_UserName, avpLength_Result, avpLength_Authentication, avpLength_SessionID;//记录各avp的长度信息
        public int location_UserNameStr, location_ResultCodeStr, location_SessionID4, length_UserNameStr, length_SessionID4;//记录各项avp中详细信息的位置，长度参数
        public List<int> avpLocation_EUTRANVector = new List<int>();//记录各EUTRANVector avp的起始位置信息
        public List<int> avpLength_EUTRANVector = new List<int>();//记录各EUTRANVector avp的长度信息
        public List<int> avpLocation_KASME = new List<int>();//记录各KASME avp的起始位置信息
        public List<int> avpLength_KASME = new List<int>();//记录各KASME avp的长度信息
        public List<int> location_KASMEStr = new List<int>();//记录各KASMEStr的起始位置信息

        //解码SCTP层chunk块获得数据
        public void DecodeSctpChunk(byte[] buf)//buf指的是去除IP头以后的数据
        {
            diameterContent = string.Empty;
            protocol_Treeview = "SCTP";
            protocol = "SCTP";
            version = string.Empty;
            length = string.Empty;
            flag = string.Empty;
            commandCODE = string.Empty;
            applicationID = string.Empty;
            H2HIdentifier = string.Empty;
            E2EIdentifier = string.Empty;
            command = string.Empty;
            userNameCode = string.Empty;
            userNameFlag = string.Empty;
            userNameLength = string.Empty;
            userNameStr = string.Empty;
            avp_ResultcodeCode = string.Empty;
            avp_ResultcodeFlag = string.Empty;
            avp_ResultcodeLength = string.Empty;
            avp_ResultcodeStr = string.Empty;
            avp_AuthenticationCode = string.Empty;
            avp_AuthenticationFlag = string.Empty;
            avp_AuthenticationLength = string.Empty;
            avp_AuthenticationVendorID = string.Empty;
            avp_EUTRANVectorCode = null;
            avp_EUTRANVectorFlag = null;
            avp_EUTRANVectorLength = null;
            avp_EUTRANVectorVendorID = null;
            avp_KASMECode = null;
            avp_KASMEFlag = null;
            avp_KASMELength = null;
            KASMEStr = null;
            avp_SessionID1 = string.Empty;
            avp_SessionID2 = string.Empty;
            avp_SessionID3 = string.Empty;
            avp_SessionID4 = string.Empty;
            ascii = null;
            diameterDirection = 3;
            int padding = 0;//chunck块填充（即补0）长度
            int dataLength = 0;//diameter消息长度
            //各项消息的位置和长度记录
            avpLocation_UserName = 0; avpLocation_Result = 0; avpLocation_Authentication = 0; avpLocation_SessionID = 0;
            avpLength_UserName = 0; avpLength_Result = 0; avpLength_Authentication = 0; avpLength_SessionID = 0;
            location_UserNameStr = 0; location_ResultCodeStr = 0; location_SessionID4 = 0; length_UserNameStr = 0; length_SessionID4 = 0;
            avpLocation_EUTRANVector.Clear();
            avpLength_EUTRANVector.Clear();
            avpLocation_KASME.Clear();
            avpLength_KASME.Clear();
            location_KASMEStr.Clear();

            for (int i = 12; i < buf.Length; )//循环遍历所有chuck块，获得diameter数据
            {
                chunckType = buf[i];
                chunckLength = (int)(((0x00) << 24)
             | ((0x00 & 0xFF) << 16)
             | ((buf[i + 2] & 0xFF) << 8)
             | (buf[i + 3] & 0xFF));
                if (chunckType == 0)//若是diameter所在的data块，则提取diameter数据并对其进行解析
                {
                    IsDiameter = 1;
                    diameterLength = chunckLength - 16;
                    diameterLocation = i + 16;
                    dataLength = chunckLength - 16;
                    byte[] diameterData = new byte[dataLength];//用于存储Diameter数据
                    Array.Copy(buf, i + 16, diameterData, 0, dataLength);
                    diameterContent = Diameter.DecodeDiameterChunk(diameterData);
                    protocol_Treeview = "diameter";
                    protocol = "DIAMETER-SCTP";
                    version = Diameter.version;
                    length = Diameter.lengthStr;
                    flag = Diameter.flag;
                    commandCODE = Diameter.commandCODE;
                    chunckCommandCODE = Diameter.chunckCommandCODE;
                    applicationID = Diameter.applicationID;
                    H2HIdentifier = Diameter.H2HIdentifier;
                    E2EIdentifier = Diameter.E2EIdentifier;
                    command = Diameter.command;
                    userNameCode = Diameter.userNameCode;
                    userNameFlag = Diameter.userNameFlag;
                    userNameLength = Diameter.userNameLength;
                    userNameStr = Diameter.userNameStr;
                    avp_ResultcodeCode = Diameter.resultcodeCode;
                    avp_ResultcodeFlag = Diameter.resultcodeFlag;
                    avp_ResultcodeLength = Diameter.resultcodeLength;
                    avp_ResultcodeStr = Diameter.resultcodeStr;
                    avp_AuthenticationCode = Diameter.authenticationCode;
                    avp_AuthenticationFlag = Diameter.authenticationFlag;
                    avp_AuthenticationLength = Diameter.authenticationLength;
                    avp_AuthenticationVendorID = Diameter.authenticationVendorId;
                    avp_EUTRANVectorCode = Diameter.EUTRANVectorCode;
                    avp_EUTRANVectorFlag = Diameter.EUTRANVectorFlag;
                    avp_EUTRANVectorLength = Diameter.EUTRANVectorLength;
                    avp_EUTRANVectorVendorID = Diameter.EUTRANVectorVendorId;
                    avp_KASMECode = Diameter.KASMECode;
                    avp_KASMEFlag = Diameter.KASMEFlag;
                    avp_KASMELength = Diameter.KASMELength;
                    KASMEStr = Diameter.KASMEStr;
                    KASMEValue = Diameter.KASMEValue;
                    avp_SessionID1 = Diameter.SessionID1;
                    avp_SessionID2 = Diameter.SessionID2;
                    avp_SessionID3 = Diameter.SessionID3;
                    avp_SessionID4 = Diameter.SessionID4;
                    ascii = Diameter.ascii;
                    diameterDirection = Diameter.direction;
                    //各项消息的位置和长度记录
                    avpLocation_UserName = Diameter.avpLocation_UserName;
                    avpLocation_Result = Diameter.avpLocation_Result;
                    avpLocation_Authentication = Diameter.avpLocation_Authentication;
                    avpLocation_SessionID = Diameter.avpLocation_SessionID;
                    avpLength_UserName = Diameter.avpLength_UserName;
                    avpLength_Result = Diameter.avpLength_Result;
                    avpLength_Authentication = Diameter.avpLength_Authentication;
                    avpLength_SessionID = Diameter.avpLength_SessionID;
                    location_UserNameStr = Diameter.location_UserNameStr;
                    location_ResultCodeStr = Diameter.location_ResultCodeStr;
                    location_SessionID4 = Diameter.location_SessionID4;
                    length_UserNameStr = Diameter.length_UserNameStr;
                    length_SessionID4 = Diameter.length_SessionID4;
                    avpLocation_EUTRANVector = Diameter.avpLocation_EUTRANVector;
                    avpLength_EUTRANVector = Diameter.avpLength_EUTRANVector;
                    avpLocation_KASME = Diameter.avpLocation_KASME;
                    avpLength_KASME = Diameter.avpLength_KASME;
                    location_KASMEStr = Diameter.location_KASMEStr;
                }
                else if ((chunckType == 11) || (chunckType == 6))//异常的sctp 块
                {
                    IsDiameter = 0;
                    break;
                }
                else
                {
                    IsDiameter = 0;
                }

                if (chunckLength % 4 != 0)//判别该chunck块是否需要填充
                {
                    padding = 4 - (chunckLength % 4);
                }
                else
                {
                    padding = 0;
                }

                i = i + chunckLength + padding;//指向下一个chunck块的头部
            }
        }
        public void decoder_All(byte[] Chunk)
        {
            sourIP = Convert.ToString(Chunk[12]) + "." + Convert.ToString(Chunk[13]) + "." + Convert.ToString(Chunk[14]) + "." + Convert.ToString(Chunk[15]);
            destIP = Convert.ToString(Chunk[16]) + "." + Convert.ToString(Chunk[17]) + "." + Convert.ToString(Chunk[18]) + "." + Convert.ToString(Chunk[19]);
            sourPort = Convert.ToString(Chunk[20] * 256 + Chunk[21]);
            destPort = Convert.ToString(Chunk[22] * 256 + Chunk[23]);
            allLength = Convert.ToString(Chunk.Length);
            protocol = "DIAMETER-SCTP";
        }
    }
}


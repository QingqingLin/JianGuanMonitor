using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonitorPorts
{
    class TCPDiameter
    {
        public string protocol = string.Empty;
        public string sourIP = string.Empty;
        public string sourPort = string.Empty;
        public string destIP = string.Empty;
        public string destPort = string.Empty;
        public string allLength = string.Empty;
        public string messageBodyLen = string.Empty;
        public string messageBodyHex = string.Empty;
        public string Version;
        public string Length;
        public string Flag;
        public string CommandCODE;
        public string ApplicationID;
        public string H2HIdentifier;
        public string E2EIdentifier;
        public string userNameCode, userNameFlag, userNameLength, userNameStr, avp_ResultcodeCode, avp_ResultcodeFlag, avp_ResultcodeLength,
            avp_ResultcodeStr, avp_AuthenticationCode, avp_AuthenticationFlag, avp_AuthenticationLength, avp_AuthenticationVendorID,
            avp_SessionID1, avp_SessionID2, avp_SessionID3, avp_SessionID4;
        public List<string> avp_EUTRANVectorCode = new List<String>();
        public List<string> avp_EUTRANVectorFlag = new List<String>();
        public List<string> avp_EUTRANVectorLength = new List<String>();
        public List<string> avp_EUTRANVectorVendorID = new List<String>();
        public List<string> avp_KASMECode = new List<String>();
        public List<string> avp_KASMEFlag = new List<String>();
        public List<string> avp_KASMELength = new List<String>();
        public List<string> KASMEStr = new List<String>();
        public List<byte[]> KASMEValue = new List<byte[]>();
        public uint chunckCommandCODE;
        public char[] ascii;
        public string protocol_Treeview;//treeview上显示的协议名称
        public string diameterContent = "";//diameter解析后概要内容
        public string command;//信令（内容+方向）
        public byte diameterDirection;//diameter信令方向
        Diameter Diameter = new Diameter();
        //各项消息的位置和长度记录
        public int diameterLength;
        public int avpLocation_UserName, avpLocation_Result, avpLocation_Authentication, avpLocation_SessionID;//记录各avp的起始位置信息
        public int avpLength_UserName, avpLength_Result, avpLength_Authentication, avpLength_SessionID;//记录各avp的长度信息
        public int location_UserNameStr, location_ResultCodeStr, location_SessionID4, length_UserNameStr, length_SessionID4;//记录各项avp中详细信息的位置，长度参数
        public List<int> avpLocation_EUTRANVector = new List<int>();//记录各EUTRANVector avp的起始位置信息
        public List<int> avpLength_EUTRANVector = new List<int>();//记录各EUTRANVector avp的长度信息
        public List<int> avpLocation_KASME = new List<int>();//记录各KASME avp的起始位置信息
        public List<int> avpLength_KASME = new List<int>();//记录各KASME avp的长度信息
        public List<int> location_KASMEStr = new List<int>();//记录各KASMEStr的起始位置信息

        //解码TCP层chunk块获得数据
        public void DecodeTcpChunk(byte[] buf)
        {
            diameterContent = string.Empty;
            protocol_Treeview = "TCP";
            protocol = "TCP";
            Version = string.Empty;
            Length = string.Empty;
            Flag = string.Empty;
            CommandCODE = string.Empty;
            ApplicationID = string.Empty;
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
            //各项消息的位置和长度记录
            avpLocation_UserName = 0; avpLocation_Result = 0; avpLocation_Authentication = 0; avpLocation_SessionID = 0;
            avpLength_UserName = 0; avpLength_Result = 0; avpLength_Authentication = 0; avpLength_SessionID = 0;
            location_UserNameStr = 0; location_ResultCodeStr = 0; location_SessionID4 = 0; length_UserNameStr = 0; length_SessionID4 = 0;
            avpLocation_EUTRANVector.Clear();
            avpLength_EUTRANVector.Clear();
            avpLocation_KASME.Clear();
            avpLength_KASME.Clear();
            location_KASMEStr.Clear();

            if (buf.Length > 32)
            {
                byte[] diameterData = new byte[buf.Length - 32];
                for (int k = 0; k < buf.Length - 32; k++)
                {
                    diameterData[k] = buf[k + 32];
                }
                diameterLength = buf.Length - 32;
                diameterContent = Diameter.DecodeDiameterChunk(diameterData);
                protocol_Treeview = "diameter";
                protocol = "DIAMETER-TCP";
                Version = Diameter.version;
                Length = Diameter.lengthStr;
                Flag = Diameter.flag;
                CommandCODE = Diameter.commandCODE;
                chunckCommandCODE = Diameter.chunckCommandCODE;
                ApplicationID = Diameter.applicationID;
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
                //EUTRANNumber = A.EUTRANNumber;
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
        }
        public void decoder_All(byte[] Chunk)
        {
            sourIP = Convert.ToString(Chunk[12]) + "." + Convert.ToString(Chunk[13]) + "." + Convert.ToString(Chunk[14]) + "." + Convert.ToString(Chunk[15]);
            destIP = Convert.ToString(Chunk[16]) + "." + Convert.ToString(Chunk[17]) + "." + Convert.ToString(Chunk[18]) + "." + Convert.ToString(Chunk[19]);
            sourPort = Convert.ToString(Chunk[20] * 256 + Chunk[21]);
            destPort = Convert.ToString(Chunk[22] * 256 + Chunk[23]);
            allLength = Convert.ToString(Chunk.Length);
            protocol = "DIAMETER-TCP";
        }
    }
}

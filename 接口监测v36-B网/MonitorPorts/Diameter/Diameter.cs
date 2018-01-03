using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Collections;
using MonitorPorts.Packet;


namespace MonitorPorts
{
    [StructLayout(LayoutKind.Explicit)]
    public struct DiameterChunkHeader //SCTP中Chunk块头
    {
        //diameter消息结构
        [FieldOffset(0)]
        public byte Version;//8位版本号
        [FieldOffset(1)]
        public uint Length;//24位长度
        [FieldOffset(4)]
        public byte Flag;//8位标志位
        [FieldOffset(5)]
        public uint CommandCODE;//24位命令代码
        [FieldOffset(8)]
        public uint ApplicationID;//32位应用ID
        [FieldOffset(12)]
        public int HopbyHopIdentifier;//32位跳到跳标志
        [FieldOffset(16)]
        public int EndtoEndIdentifier;//32位端到端标志
        [FieldOffset(20)]
        public uint AVPCODE;//32位属性值对
        [FieldOffset(24)]
        public uint AVPFlag;//8位avp标志位
        [FieldOffset(25)]
        public uint AVPLength;//24位avp长度
        [FieldOffset(28)]
        public uint ResultCode;
        [FieldOffset(37)]
        public uint AVPVendorID;
    }
    public class Diameter
    {
        Hashtable diameterHash = HashTable.GetDiaHashtable();//引用hash表中的command-hash表
        Hashtable avpHash = HashTable.GetAvpHashtable();//引用hash表中的avp-hash表
        Hashtable resultHash = HashTable.GetResultHashtable();//引用hash表中的result-hash表
        public string version;
        public string lengthStr;//消息长度语句
        public string flag;
        public string commandCODE;
        public string applicationID;
        public string H2HIdentifier;
        public string E2EIdentifier;
        public string command;
        public string appl;//ApplicationID的简要信息
        DiameterChunkHeader diameterChunkHeader;
        byte chunckVersion = 0;
        uint chunckLength = 0;
        byte chunckFlag = 0;
        public uint chunckCommandCODE = 0;
        uint chunckApplicationID = 0;
        uint chunckAVPCode;
        uint chunckAVPFlag;
        uint chunckAVPLength;
        uint chunckResultCode;
        uint chunckAVPVendorID;
        byte[] messageBuffer = new byte[200];//数据包消息字节流
        string applicationIDString;
        public string userNameCode, userNameFlag, userNameLength, userNameStr, resultcodeCode, resultcodeFlag, resultcodeLength,
            resultcodeStr, authenticationCode, authenticationFlag, authenticationLength, authenticationVendorId, SessionID1,
            SessionID2, SessionID3, SessionID4;
        public int avpLocation_UserName, avpLocation_Result, avpLocation_Authentication, avpLocation_SessionID;//记录各avp的起始位置信息
        public int avpLength_UserName, avpLength_Result, avpLength_Authentication, avpLength_SessionID;//记录各avp的长度信息
        public int location_UserNameStr, location_ResultCodeStr, location_SessionID4, length_UserNameStr, length_SessionID4;//记录各项avp中详细信息的位置，长度参数
        public List<string> EUTRANVectorCode;
        public List<string> EUTRANVectorFlag = new List<String>();
        public List<string> EUTRANVectorLength = new List<String>();
        public List<string> EUTRANVectorVendorId = new List<String>();
        public List<int> avpLocation_EUTRANVector = new List<int>();//记录各EUTRANVector avp的起始位置信息
        public List<int> avpLength_EUTRANVector = new List<int>();//记录各EUTRANVector avp的长度信息
        public List<string> KASMECode;
        public List<string> KASMEFlag = new List<String>();
        public List<string> KASMELength = new List<String>();
        public List<string> KASMEVendorID = new List<String>();
        public List<string> KASMEStr = new List<String>();
        public static List<byte[]> KASMEValue = new List<byte[]>();
        public List<int> avpLocation_KASME=new List<int>();//记录各KASME avp的起始位置信息
        public List<int> avpLength_KASME = new List<int>();//记录各KASME avp的长度信息
        public List<int> location_KASMEStr = new List<int>();//记录各KASMEStr的起始位置信息
        public byte direction;
        public char[] ascii;
        string kasme = "";
        string user = "";
        string diameterContent = "";
        int padding = 0;//填充
        int avplength1, avplength2, avplength3;//用于消息长度判断的变量
        public string DecodeDiameterChunk(byte[] buf)
        {
            userNameCode = "";
            resultcodeCode = "";
            authenticationCode = "";
            EUTRANVectorCode = new List<String>();
            KASMECode = new List<String>();
            SessionID1 = "";
            resultcodeCode = string.Empty;
            resultcodeFlag = string.Empty;
            resultcodeLength = string.Empty;
            resultcodeStr = string.Empty;
            authenticationCode = string.Empty;
            authenticationFlag = string.Empty;
            authenticationLength = string.Empty;
            authenticationVendorId = string.Empty;
            SessionID1 = string.Empty;
            SessionID2 = string.Empty;
            SessionID3 = string.Empty;
            SessionID4 = string.Empty;
            List<String> strs = new List<String>();
            avpLocation_UserName = 0; avpLocation_Result = 0; avpLocation_Authentication = 0; avpLocation_SessionID = 0;
            avpLength_UserName = 0; avpLength_Result = 0; avpLength_Authentication = 0; avpLength_SessionID = 0;
            location_UserNameStr = 0; location_ResultCodeStr = 0; location_SessionID4 = 0; length_UserNameStr = 0; length_SessionID4 = 0;
            avpLocation_EUTRANVector.Clear();
            avpLength_EUTRANVector.Clear();
            avpLocation_KASME.Clear();
            avpLength_KASME.Clear();
            location_KASMEStr.Clear();
           

            //获取消息的版本号
            diameterChunkHeader.Version = buf[0];
            chunckVersion = diameterChunkHeader.Version;
            if (chunckVersion == 1)
            {
                //获取消息长度
                int length;
                length = (int)(((0x00) << 24)
                 | ((buf[1] & 0xFF) << 16)
                 | ((buf[2] & 0xFF) << 8)
                 | (buf[3] & 0xFF));
                diameterChunkHeader.Length = (uint)length;
                chunckLength = diameterChunkHeader.Length;
                //获取消息的标志位
                diameterChunkHeader.Flag = buf[4];
                chunckFlag = diameterChunkHeader.Flag;
                //获取消息中的commandcode
                int commandcode;
                commandcode = (int)(((0x00) << 24)
                 | ((buf[5] & 0xFF) << 16)
                 | ((buf[6] & 0xFF) << 8)
                 | (buf[7] & 0xFF));
                diameterChunkHeader.CommandCODE = (uint)commandcode;
                chunckCommandCODE = diameterChunkHeader.CommandCODE;
                //匹配-找出commandcode在协议中规定的具体含义
                string commandstr = (string)diameterHash[commandcode];
                if ((chunckFlag & 128) == 128)
                {
                    command = commandstr + " Request";
                    direction = 1;
                }
                else
                {
                    command = commandstr + " Answer";
                    direction = 0;
                }
                //取出数据中的应用ID
                uint applicationid;
                applicationid = (uint)(((buf[8] & 0xFF) << 24)
                 | ((buf[9] & 0xFF) << 16)
                 | ((buf[10] & 0xFF) << 8)
                 | (buf[11] & 0xFF));
                diameterChunkHeader.ApplicationID = applicationid;
                chunckApplicationID = diameterChunkHeader.ApplicationID;
                //匹配-找出捕获数据的applicationID所对应的具体xml协议
                if (applicationid == 0)
                {
                    applicationIDString = "Diameter Common Messages(" + applicationid + ")";
                    appl = "appl= Diameter Common Messages(" + applicationid + ")";
                }
                else if (applicationid == 3)
                {
                    applicationIDString = "Diameter Base Accounting(" + applicationid + ")";
                    appl = "appl=Diameter Base Accounting(" + applicationid + ")";
                }
                else if (applicationid == 16777251)
                {
                    applicationIDString = "3GPP S6a/S6d(" + applicationid + ")";
                    appl = "appl=3GPP S6a/S6d(" + applicationid + ")";
                }
                //获取跳到跳标志
                int hopbyhopidentifier;
                hopbyhopidentifier = (int)(((buf[12] & 0xFF) << 24)
                 | ((buf[13] & 0xFF) << 16)
                 | ((buf[14] & 0xFF) << 8)
                 | (buf[15] & 0xFF));
                diameterChunkHeader.HopbyHopIdentifier = hopbyhopidentifier;
                string chunckHopbyHopIdentifier = (diameterChunkHeader.HopbyHopIdentifier).ToString("X8");
                //获取端到端标志
                int endtoendidentifier;
                endtoendidentifier = (int)(((buf[16] & 0xFF) << 24)
                 | ((buf[17] & 0xFF) << 16)
                 | ((buf[18] & 0xFF) << 8)
                 | (buf[19] & 0xFF));
                diameterChunkHeader.EndtoEndIdentifier = endtoendidentifier;
                string chunckEndtoEndIdentifier = (diameterChunkHeader.EndtoEndIdentifier).ToString("X8");
                //定义所有之前avp的长度之和为i
                int i = 0, ivendor1 = 0, ivendor2 = 0, ivendor3 = 0;
                while (length - 20 - i > 0)
                {
                    //取第一层avp的code即avpcode1和length即avplength1
                    uint avpcode1;
                    avpcode1 = (uint)(((buf[20 + i] & 0xFF) << 24)
                     | ((buf[21 + i] & 0xFF) << 16)
                     | ((buf[22 + i] & 0xFF) << 8)
                     | (buf[23 + i] & 0xFF));

                    avplength1 = (int)(((0x00) << 24)
                     | ((buf[25 + i] & 0xFF) << 16)
                     | ((buf[26 + i] & 0xFF) << 8)
                     | (buf[27 + i] & 0xFF));
                    if (avpcode1 == 1)
                    {
                        int ivendor_User = 0;//vendorID的数据长度
                        avpLocation_UserName = 20 + i;//username avp的起始位置（相对于buf）
                        avpLength_UserName = avplength1;//username avp的长度
                        diameterChunkHeader.AVPCODE = avpcode1;
                        chunckAVPCode = diameterChunkHeader.AVPCODE;
                        diameterChunkHeader.AVPFlag = buf[24 + i];
                        chunckAVPFlag = diameterChunkHeader.AVPFlag;
                        diameterChunkHeader.AVPLength = (uint)avplength1;
                        chunckAVPLength = diameterChunkHeader.AVPLength;

                        if ((chunckAVPFlag & 0x80) >> 7 == 1)//是否包含vendorID
                        {
                            ivendor_User = 4;
                        }//对应if ((chunckAVPFlag & 0x80) >> 7 == 1)

                        byte[] userName = new byte[avplength1 - 8 - ivendor_User];
                        System.Array.Copy(buf, (28 + i), userName, 0, (avplength1 - 8 - ivendor_User));
                        user = "";
                        foreach (byte ab in userName)
                        {

                            string temp = Convert.ToString(ab, 16);
                            user += Convert.ToInt32(temp) - 30;
                        }
                        location_UserNameStr = 28 + i;
                        length_UserNameStr = avplength1 - 8 - ivendor_User;
                        userNameCode = "User-Name AVP Code:" + chunckAVPCode + "\r\n";
                        userNameFlag = "User-Name AVP Flag:" + chunckAVPFlag + "\r\n";
                        userNameLength = "User-Name AVP Length:" + chunckAVPLength + "\r\n";
                        userNameStr = user;

                    }//if (avpcode1 == 1)
                    else if (avpcode1 == 268)
                    {
                        int ivendor_Result = 0;//vendorID的数据长度
                        avpLocation_Result = 20 + i;//Result avp的起始位置（相对于buf）
                        avpLength_Result = avplength1;//Result avp的长度
                        diameterChunkHeader.AVPCODE = avpcode1;
                        chunckAVPCode = diameterChunkHeader.AVPCODE;
                        diameterChunkHeader.AVPFlag = buf[24 + i];
                        chunckAVPFlag = diameterChunkHeader.AVPFlag;
                        diameterChunkHeader.AVPLength = (uint)avplength1;
                        chunckAVPLength = diameterChunkHeader.AVPLength;

                        if ((chunckAVPFlag & 0x80) >> 7 == 1)//是否包含vendorID
                        {
                            ivendor_Result = 4;
                        }//对应if ((chunckAVPFlag & 0x80) >> 7 == 1)

                        uint resultcode;
                        resultcode = (uint)(((buf[28 + i + ivendor_Result] & 0xFF) << 24)
                         | ((buf[29 + i + ivendor_Result] & 0xFF) << 16)
                         | ((buf[30 + i + ivendor_Result] & 0xFF) << 8)
                         | (buf[31 + i + ivendor_Result] & 0xFF));
                        diameterChunkHeader.ResultCode = (uint)resultcode;
                        chunckResultCode = diameterChunkHeader.ResultCode;
                        location_ResultCodeStr = 28 + i + ivendor_Result;
                        resultcodeCode = "Result-CODE AVP Code:" + chunckAVPCode + "\r\n";// +avpstrrr + "\r\n";
                        resultcodeFlag = "Result-CODE AVP Flag:" + chunckAVPFlag + "\r\n";
                        resultcodeLength = "Result-CODE AVP Length:" + chunckAVPLength + "\r\n";
                        resultcodeStr = "ResultCode:" + chunckResultCode + "\r\n";// +resultcodestrrrr + "\r\n";

                    }//if (avpcode1 == 268)

                    else if (avpcode1 == 263)
                    {
                        int ivendor_SessionID = 0;//vendorID的数据长度
                        avpLocation_SessionID = 20 + i;//SessionID avp的起始位置（相对于buf）
                        avpLength_SessionID = avplength1;//SessionID avp的长度

                        diameterChunkHeader.AVPCODE = avpcode1;
                        chunckAVPCode = diameterChunkHeader.AVPCODE;
                        diameterChunkHeader.AVPFlag = buf[24 + i];
                        chunckAVPFlag = diameterChunkHeader.AVPFlag;
                        diameterChunkHeader.AVPLength = (uint)avplength1;
                        chunckAVPLength = diameterChunkHeader.AVPLength;

                        if ((chunckAVPFlag & 0x80) >> 7 == 1)//是否包含vendorID
                        {
                            ivendor_SessionID = 4;
                        }//对应if ((chunckAVPFlag & 0x80) >> 7 == 1)

                        char[] ascii = new char[avplength1 - 8 - ivendor_SessionID];
                        int kk = 0;
                        for (int jj = 28 + i; jj < 28 + i + avplength1 - 8 - ivendor_SessionID; jj++)
                        {
                            ascii[kk++] = (char)buf[jj];
                        }
                        string sessionidstr = new string(ascii);
                        location_SessionID4 = 28 + i;
                        length_SessionID4 = avplength1 - 8 - ivendor_SessionID;
                        SessionID1 = "Session-ID AVP Code:" + chunckAVPCode + "\r\n";// +avpstrrr + "\r\n";
                        SessionID2 = "Session-ID AVPFlag:" + chunckAVPFlag + "\r\n";
                        SessionID3 = "Session-ID AVPLength:" + chunckAVPLength + "\r\n";
                        SessionID4 = "Session-ID AVPSessionid:" + sessionidstr + "\r\n";
                    }//elseif (avpcode1 == 263)

                    else if (avpcode1 == 1413)
                    {
                        avpLocation_Authentication = 20 + i;//Authentication avp的起始位置（相对于buf）
                        avpLength_Authentication = avplength1;//Authentication avp的长度
                        int num_EUTRAN = -1;//第i个E-UTRAN向量，从0开始，范围从0到4
                        int num_KASME = -1;//第i个KASME向量，从0开始，范围从0到4
                        List<byte[]> kasme1 = new List<byte[]>();
                        EUTRANVectorCode = new List<String>();
                        EUTRANVectorFlag.Clear();
                        EUTRANVectorLength.Clear();
                        EUTRANVectorVendorId.Clear();
                        KASMECode.Clear();
                        KASMEFlag.Clear();
                        KASMELength.Clear();
                        KASMEVendorID.Clear();
                        KASMEStr.Clear();
                        int length1413 = 0, length1414 = 0, length1414Sum = 0, length1450 = 0, length1450Sum = 0;
                        ivendor1 = 0;
                        diameterChunkHeader.AVPCODE = avpcode1;
                        chunckAVPCode = diameterChunkHeader.AVPCODE;
                        diameterChunkHeader.AVPFlag = buf[24 + i];
                        chunckAVPFlag = diameterChunkHeader.AVPFlag;
                        diameterChunkHeader.AVPLength = (uint)avplength1;
                        chunckAVPLength = diameterChunkHeader.AVPLength;
                        length1413 = avplength1;
                        authenticationCode = "Authentication-Info AVP Code:" + chunckAVPCode + "\r\n";// +avpstrrr + "\r\n";
                        authenticationFlag = "Authentication-Info AVPFlag:" + chunckAVPFlag + "\r\n";
                        authenticationLength = "Authentication-Info AVPLength:" + chunckAVPLength + "\r\n";
                        if ((chunckAVPFlag & 0x80) >> 7 == 1)
                        {
                            uint AVPVendorID1;
                            AVPVendorID1 = (uint)(((buf[28 + i] & 0xFF) << 24)
                              | ((buf[29 + i] & 0xFF) << 16)
                              | ((buf[30 + i] & 0xFF) << 8)
                              | (buf[31 + i] & 0xFF));
                            diameterChunkHeader.AVPVendorID = (uint)AVPVendorID1;
                            chunckAVPVendorID = diameterChunkHeader.AVPVendorID;
                            authenticationVendorId = "Authentication-Info AVPVendorID:" + chunckAVPVendorID + "\r\n";
                            ivendor1 = ivendor1 + 4;
                        }//对应if ((chunckAVPFlag & 0x80) >> 7 == 1)
                        while ((length1413 - 8 - ivendor1) - length1414Sum > 0)
                        {
                            uint avpcode2;
                            avpcode2 = (uint)(((buf[28 + i + ivendor1 + length1414Sum] & 0xFF) << 24)
                             | ((buf[29 + i + ivendor1 + length1414Sum] & 0xFF) << 16)
                             | ((buf[30 + i + ivendor1 + length1414Sum] & 0xFF) << 8)
                             | (buf[31 + i + ivendor1 + length1414Sum] & 0xFF));

                            avplength2 = (int)(((0x00) << 24)
                             | ((buf[33 + i + ivendor1 + length1414Sum] & 0xFF) << 16)
                             | ((buf[34 + i + ivendor1 + length1414Sum] & 0xFF) << 8)
                             | (buf[35 + i + ivendor1 + length1414Sum] & 0xFF));

                            if (avpcode2 == 1414)
                            {
                                num_EUTRAN = num_EUTRAN + 1;
                                avpLocation_EUTRANVector.Add(28 + i + ivendor1 + length1414Sum);
                                avpLength_EUTRANVector.Add(avplength2);
                                ivendor2 = 0;
                                length1450Sum = 0;
                                diameterChunkHeader.AVPCODE = avpcode2;
                                chunckAVPCode = diameterChunkHeader.AVPCODE;
                                diameterChunkHeader.AVPFlag = buf[32 + i + ivendor1 + length1414Sum];
                                chunckAVPFlag = diameterChunkHeader.AVPFlag;
                                diameterChunkHeader.AVPLength = (uint)avplength2;
                                chunckAVPLength = diameterChunkHeader.AVPLength;
                                length1414 = avplength2;
                                EUTRANVectorCode.Add("E-UTRAN-Vector" + Convert.ToString(num_EUTRAN + 1) + " Code:" + chunckAVPCode + "\r\n");// + avpstrrrr + "\r\n");
                                EUTRANVectorFlag.Add("E-UTRAN-Vector" + Convert.ToString(num_EUTRAN + 1) + " Flag:" + chunckAVPFlag + "\r\n");
                                EUTRANVectorLength.Add("E-UTRAN-Vector" + Convert.ToString(num_EUTRAN + 1) + " Length:" + chunckAVPLength + "\r\n");
                                if ((buf[32 + i + ivendor1 + length1414Sum] & 0x80) >> 7 == 1)
                                {
                                    uint AVPVendorID2;
                                    AVPVendorID2 = (uint)(((buf[36 + i + ivendor1 + length1414Sum] & 0xFF) << 24)
                                      | ((buf[37 + i + ivendor1 + length1414Sum] & 0xFF) << 16)
                                      | ((buf[38 + i + ivendor1 + length1414Sum] & 0xFF) << 8)
                                      | (buf[39 + i + ivendor1 + length1414Sum] & 0xFF));
                                    diameterChunkHeader.AVPVendorID = (uint)AVPVendorID2;
                                    chunckAVPVendorID = diameterChunkHeader.AVPVendorID;
                                    EUTRANVectorVendorId.Add("E-UTRAN-Vector" + Convert.ToString(num_EUTRAN + 1) + " VendorID:" + chunckAVPVendorID + "\r\n");
                                    ivendor2 = ivendor2 + 4;
                                }//if (chunckAVPFlag & 0x80) >> 7 == 1)
                                while ((length1414 - 8 - ivendor2) - length1450Sum > 0)
                                {
                                    uint avpcode3;
                                    avpcode3 = (uint)(((buf[36 + i + ivendor1 + length1414Sum + ivendor2 + length1450Sum] & 0xFF) << 24)
                                 | ((buf[37 + i + ivendor1 + length1414Sum + ivendor2 + length1450Sum] & 0xFF) << 16)
                                 | ((buf[38 + i + ivendor1 + length1414Sum + ivendor2 + length1450Sum] & 0xFF) << 8)
                                 | (buf[39 + i + ivendor1 + length1414Sum + ivendor2 + length1450Sum] & 0xFF));

                                    avplength3 = (int)(((0x00) << 24)
                                 | ((buf[41 + i + ivendor1 + length1414Sum + ivendor2 + length1450Sum] & 0xFF) << 16)
                                 | ((buf[42 + i + ivendor1 + length1414Sum + ivendor2 + length1450Sum] & 0xFF) << 8)
                                 | (buf[43 + i + ivendor1 + length1414Sum + ivendor2 + length1450Sum] & 0xFF));

                                    if (avpcode3 == 1450)
                                    {
                                        num_KASME = num_KASME + 1;
                                        avpLocation_KASME.Add(36 + i + ivendor1 + length1414Sum + ivendor2 + length1450Sum);
                                        avpLength_KASME.Add(avplength3);
                                        ivendor3 = 0;
                                        diameterChunkHeader.AVPCODE = avpcode3;
                                        chunckAVPCode = diameterChunkHeader.AVPCODE;
                                        diameterChunkHeader.AVPFlag = buf[40 + i + ivendor1 + length1414Sum + ivendor2 + length1450Sum];
                                        chunckAVPFlag = diameterChunkHeader.AVPFlag;
                                        diameterChunkHeader.AVPLength = (uint)avplength3;
                                        chunckAVPLength = diameterChunkHeader.AVPLength;
                                        KASMECode.Add("KASME" + Convert.ToString(num_KASME + 1) + " Code:" + chunckAVPCode + "\r\n");//+ avpstrrr + "\r\n");
                                        KASMEFlag.Add("KASME" + Convert.ToString(num_KASME + 1) + " Flag:" + chunckAVPFlag + "\r\n");
                                        KASMELength.Add("KASME" + Convert.ToString(num_KASME + 1) + " Length:" + chunckAVPLength + "\r\n");
                                        if ((chunckAVPFlag & 0x80) >> 7 == 1)
                                        {
                                            uint AVPVendorID3;
                                            AVPVendorID3 = (uint)(((buf[44 + i + ivendor1 + length1414Sum + ivendor2 + length1450Sum] & 0xFF) << 24)
                                              | ((buf[45 + i + ivendor1 + length1414Sum + ivendor2 + length1450Sum] & 0xFF) << 16)
                                              | ((buf[46 + i + ivendor1 + length1414Sum + ivendor2 + length1450Sum] & 0xFF) << 8)
                                              | (buf[47 + i + ivendor1 + length1414Sum + ivendor2 + length1450Sum] & 0xFF));
                                            diameterChunkHeader.AVPVendorID = (uint)AVPVendorID3;
                                            chunckAVPVendorID = diameterChunkHeader.AVPVendorID;
                                            KASMEVendorID.Add("KASME" + Convert.ToString(num_KASME + 1) + " VendorID:" + chunckAVPVendorID + "\r\n");
                                            ivendor3 = ivendor3 + 4;
                                        }//if ((chunckAVPFlag & 0x80) >> 7 == 1)
                                        byte[] KASME = new byte[32];
                                        System.Array.Copy(buf, (44 + i + ivendor1 + length1414Sum + ivendor2 + length1450Sum + ivendor3), KASME, 0, 32);
                                        location_KASMEStr.Add(44 + i + ivendor1 + length1414Sum + ivendor2 + length1450Sum + ivendor3);
                                        kasme = "";
                                        foreach (byte ab in KASME)
                                        {
                                            kasme += ab.ToString("X2");
                                        }
                                        kasme1.Add(KASME);
                                        KASMEValue = kasme1;
                                        KASMEStr.Add("KASME" + Convert.ToString(num_KASME + 1) + ":" + kasme);
                                    }//if(avpcode3==1450)
                                    if ((avplength3 % 4) != 0)
                                    {
                                        padding = 4 - avplength3 % 4;
                                        avplength3 = avplength3 + padding;
                                    }//if((avplength1%4)!=0)
                                    length1450Sum = length1450Sum + avplength3;
                                }//while(length1414-length1450Sum>0)

                            }//if(avplength2==1414)
                            if ((avplength2 % 4) != 0)
                            {
                                padding = 4 - avplength2 % 4;
                                avplength2 = avplength2 + padding;
                            }//if((avplength1%4)!=0)
                            length1414Sum = length1414Sum + avplength2;
                        }//while(length1413-length1414Sum>0)//不用break
                    }//elseif (avpcode1 == 1413)
                    if ((avplength1 % 4) != 0)
                    {
                        padding = 4 - avplength1 % 4;
                        avplength1 = avplength1 + padding;
                    }//if((avplength1%4)!=0)
                    i = i + avplength1;
                }//对应while (length - 20 - i > 0),因为还要继续循环，不能用break

                diameterContent = command + "(" + commandcode + ")" + " Flag:" + chunckFlag + " " + appl + " h2h:0X" + chunckHopbyHopIdentifier + " e2e:0X" + chunckEndtoEndIdentifier;
                version = "Version:" + chunckVersion;
                lengthStr = "Length:" + chunckLength;
                flag = "Flag:" + chunckFlag;
                commandCODE = "CommandCODE:" + commandstr + "(" + chunckCommandCODE + ")";
                applicationID = applicationIDString;
                H2HIdentifier = "Hop-by-HopIdentifier:" + "0X" + chunckHopbyHopIdentifier;
                E2EIdentifier = "End-to-EndIdentifier:" + "0X" + chunckEndtoEndIdentifier;
            }
            return diameterContent;
        }
    }

}
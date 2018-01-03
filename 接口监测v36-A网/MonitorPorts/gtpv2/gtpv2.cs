using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTP_V2Decoder
{
     class gtpv2
    {
        public int version,pflag,tflag,spare;
        public int mesType;
        public string ver, pinfo, tinfo;
        public string mt, teidinfo, seqinfo, leninfo, spareinfo;
        public string IEvalue;
        public Int64 tnumber, length, snumber;
        public string protocol = string.Empty;
        public string sourIP = string.Empty;
        public string sourPort = string.Empty;
        public string destIP = string.Empty;
        public string destPort = string.Empty;
        public string allLength = string.Empty;
        public string messageBodyLen = string.Empty;
        public string messageBodyHex = string.Empty;
        public string FTEID = "";
        public string IMSI = "";
        public string GUTI = "";

        #region gtp包解析
        public void decoder(byte[] Chunk)
        {

            Int64 IElengthForAll = 0;
            #region GTP版本判断
            version = Chunk[0] >> 5;  //右移
            switch (version)
            {
                case 2: ver = "010. .... = Version:2"; break;
                default: ver = "error！"; break;
            }
            #endregion

            #region P标号判断
            pflag = Chunk[0] / 16 % 2;
            switch (pflag)
            {
                case 1: pinfo = "...1 .... = Piggybacking flag(P):1"; break;
                case 0: pinfo = "...0 .... = Piggybacking flag(P):0"; break;
                default: pinfo = "error！"; break;
            }
            #endregion

            #region GTP消息长度(不包括前4个字节)
            length = Convert.ToInt64(Chunk[2] * System.Math.Pow(256, 1) + Chunk[3]);
            leninfo = "Message Length:" + Convert.ToString(length, 10);
            #endregion

            #region 消息类型判断
            //消息类型判断
            mesType = Chunk[1];
            switch (mesType)
            {
                case 0: mt = "Reserved"; break;
                case 1: mt = "Echo Request"; break;
                case 2: mt = "Echo Response"; break;
                case 3: mt = "Version Not Supported Indication"; break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                case 24: mt = "Reserved for S101 interface"; break;
                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                case 30:
                case 31: mt = "Reserved for Sv interface"; break;
                case 32: mt = "Create Session Request"; break;
                case 33: mt = "Create Session Response"; break;
                case 34: mt = "Modify Bearer Request"; break;
                case 35: mt = "Modify Bearer Response"; break;
                case 36: mt = "Delete Session Request"; break;
                case 37: mt = "Delete Session Response"; break;
                case 38: mt = "Change Notification Request"; break;
                case 39: mt = "Change Notification Response"; break;

                case 164: mt = "Resume Notification"; break;
                case 165: mt = "Resume Acknowledge"; break;
                case 64: mt = "Modify Bearer Command"; break;
                case 65: mt = "Modify Bearer Failure Indication"; break;
                case 66: mt = "Delete Bearer Command"; break;
                case 67: mt = "Delete Bearer Failure Indication"; break;
                case 68: mt = "Bearer Resource Command "; break;
                case 69: mt = "Bearer Resource Failure Indication "; break;
                case 70: mt = "Downlink Data Notification Failure Indication "; break;
                case 71: mt = "Trace Session Activation "; break;
                case 72: mt = "Trace Session Deactivation "; break;
                case 73: mt = "Stop Paging Indication "; break;

                case 95: mt = "Create Bearer Request "; break;
                case 96: mt = "Create Bearer Response "; break;
                case 97: mt = "Update Bearer Request "; break;
                case 98: mt = "Update Bearer Response "; break;
                case 99: mt = "Delete Bearer Request "; break;
                case 100: mt = "Delete Bearer Response "; break;
                case 101: mt = "Delete PDN Connection Set Request "; break;
                case 102: mt = "Delete PDN Connection Set Response "; break;

                // case 103: mt = "Delete PGW Downlink Triggering Notification "; break;
                //case 104: mt = "PGW Downlink Triggering Acknowledge "; break;

                case 128: mt = "Identification Request "; break;
                case 129: mt = "Identification Response "; break;
                case 130: mt = "Context Request"; break;
                case 131: mt = "Context Response"; break;
                case 132: mt = "Context Acknowledge"; break;
                case 133: mt = "Forward Relocation Request"; break;
                case 134: mt = "Forward Relocation Response"; break;
                case 135: mt = "Forward Relocation Complete Notification"; break;
                case 136: mt = "Forward Relocation Complete Acknowledge"; break;
                case 137: mt = "Forward Access Context Notification"; break;
                case 138: mt = "Forward Access Context Acknowledge"; break;
                case 139: mt = "Relocation Cancel Request"; break;
                case 140: mt = "Relocation Cancel Response"; break;
                case 141: mt = "Configuration Transfer Tunnel"; break;
                case 152: mt = "RAN Information Relay"; break;

                case 149: mt = "Detach Notification"; break;
                case 150: mt = "Detach Acknowledge"; break;
                case 151: mt = "CS Paging Indication"; break;
                case 153: mt = "Alert MME Notification"; break;
                case 154: mt = "Alert MME Acknowledge"; break;
                case 155: mt = "UE Activity Notification"; break;
                case 156: mt = "UE Activity Acknowledge"; break;

                //case 157: mt = "ISR Status Indication"; break;
                // case 158: mt = "UE Registration Query Request"; break;
                //  case 159: mt = "UE Registration Query Response"; break;

                case 162: mt = "Suspend Notification"; break;
                case 163: mt = "Suspend Acknowledge"; break;

                case 160: mt = "Create Forwarding Tunnel Request"; break;
                case 161: mt = "Create Forwarding Tunnel Response"; break;
                case 166: mt = "Create Indirect Data Forwarding Tunnel Request"; break;
                case 167: mt = "Create Indirect Data Forwarding Tunnel Response"; break;
                case 168: mt = "Delete Indirect Data Forwarding Tunnel Request"; break;
                case 169: mt = "Delete Indirect Data Forwarding Tunnel Response"; break;
                case 170: mt = "Release Access Bearers Request"; break;
                case 171: mt = "Release Access Bearers Response"; break;
                case 176: mt = "Downlink Data Notification"; break;
                case 177: mt = "Downlink Data Notification Acknowledge"; break;
                case 178: mt = "Reserved. Allocated in earlier version of the specification"; break;
                // case 179: mt = "PGW Restart Notification"; break;
                // case 180: mt = "PGW Restart Notification Acknowledge"; break;
                case 200: mt = "Update PDN Connection Set Request"; break;
                case 201: mt = "Update PDN Connection Set Response"; break;
                //case 211: mt = "Modify Access Bearers Request"; break;
                //case 212: mt = "Modify Access Bearers Response"; break;
                case 231: mt = "MBMS Session Start Request"; break;
                case 232: mt = "MBMS Session Start Response"; break;
                case 233: mt = "MBMS Session Update Request"; break;
                case 234: mt = "MBMS Session Update Response"; break;
                case 235: mt = "MBMS Session Stop Requestt"; break;
                case 236: mt = "MBMS Session Stop Response"; break;
                default: mt = "error！"; break;
            }
            #endregion

            #region IE信元解析
            tflag = Chunk[0] / 8 % 2;
            switch (tflag)
            {
                //T=1,消息头有12个字节，解析IE从13个字节开始
                case 1: tinfo = ".... 1... = TEID flag(T):1";
                        tnumber =Convert.ToInt64(Chunk[4] * System.Math.Pow(256, 3) + Chunk[5] * System.Math.Pow(256, 2) + Chunk[6] * System.Math.Pow(256, 1) + Chunk[7]) ;       
                        teidinfo = "Tunnel Endpoint Identifier :" + Convert.ToString(tnumber,16)+" ("+tnumber+")";
                        snumber = Convert.ToInt64(Chunk[8] * System.Math.Pow(256, 2) + Chunk[9] * System.Math.Pow(256, 1) + Chunk[10]);
                        seqinfo = "Sequence Number:" + Convert.ToString(snumber, 16) + " (" + snumber + ")";
                        spare = Chunk[11];
                        switch(spare)
                        {
                            case 0: spareinfo = "Spare:0"; break;
                            default: spareinfo = "error!"; break;
                        } 
                    //找出cause IE解析
                        if (mesType == 32 )
                        {
                            while (length - 8 - IElengthForAll > 0)
                            {

                                if (Chunk[12 + IElengthForAll] == 2)//2代表cause,即信元类型
                                {
                                    #region  casue Value
                                    switch (Chunk[12 + IElengthForAll + 4])
                                    {
                                        case 0: IEvalue = "Reserved. Shall not be sent and if received the Cause shall be treated as an invalid IE"; break;
                                        case 1: IEvalue = "Reserved"; break;
                                        case 2: IEvalue = "Local Detach"; break;
                                        case 3: IEvalue = "complete Detach"; break;
                                        case 4: IEvalue = "RAT changed from 3GPP to Non-3GPP"; break;
                                        case 5: IEvalue = "ISR deactivation"; break;
                                        case 6: IEvalue = "Error Indication received from RAN/eNodeB/S4-SGSN"; break;
                                        case 7: IEvalue = "IMSI Detach Only"; break;
                                        case 8:
                                        case 9:
                                        case 10:
                                        case 11:
                                        case 12:
                                        case 13:
                                        case 14:
                                        case 15: IEvalue = "Spare"; break;
                                        case 16: IEvalue = "Request accepted"; break;
                                        case 17: IEvalue = "Request accepted partially"; break;
                                        case 18: IEvalue = "New PDN type due to network preference"; break;
                                        case 19: IEvalue = "New PDN type due to single address beare only"; break;
                                        case 20:
                                        case 21:
                                        case 22:
                                        case 23:
                                        case 24:
                                        case 25:
                                        case 26:
                                        case 27:
                                        case 28:
                                        case 29:
                                        case 30:
                                        case 31:
                                        case 32:
                                        case 33:
                                        case 34:
                                        case 35:
                                        case 36:
                                        case 37:
                                        case 38:
                                        case 39:
                                        case 40:
                                        case 41:
                                        case 42:
                                        case 43:
                                        case 44:
                                        case 45:
                                        case 46:
                                        case 47:
                                        case 48:
                                        case 49:
                                        case 50:
                                        case 51:
                                        case 52:
                                        case 53:
                                        case 54:
                                        case 55:
                                        case 56:
                                        case 57:
                                        case 58:
                                        case 59:
                                        case 60:
                                        case 61:
                                        case 62:
                                        case 63: IEvalue = "Spare"; break;
                                        case 64: IEvalue = "Context Not Found"; break;
                                        case 65: IEvalue = "Invalid Message Format"; break;
                                        case 66: IEvalue = "Version Not supported by next peer"; break;
                                        case 67: IEvalue = "Invalid Message Format"; break;
                                        case 68: IEvalue = "Service not supported"; break;
                                        case 69: IEvalue = "Mandatory IE incorrect"; break;
                                        case 70: IEvalue = "Mandatory IE missing"; break;
                                        case 71: IEvalue = "Shall not be used"; break;
                                        case 72: IEvalue = "System failure"; break;
                                        case 73: IEvalue = "No resource available"; break;
                                        case 74: IEvalue = "Semantic error in the TFT operation"; break;
                                        case 75: IEvalue = "Syntatic error in the TFT operation"; break;
                                        case 76: IEvalue = "Semantic errors in packet filter(s)"; break;
                                        case 77: IEvalue = "Syntactic errors in packet filter(s)"; break;
                                        case 78: IEvalue = "Missing or unknow APN"; break;
                                        case 79: IEvalue = "Shall not be used"; break;
                                        case 80: IEvalue = "GRE key not found"; break;
                                        case 81: IEvalue = "Relocation failure"; break;
                                        case 82: IEvalue = "Denied in RAT"; break;
                                        case 83: IEvalue = "Preferred PDN type not supported"; break;
                                        case 84: IEvalue = "All dynamic addresses are occupied"; break;
                                        case 85: IEvalue = "UE context without TFT already activated"; break;
                                        case 86: IEvalue = "Protocol type not supported"; break;
                                        case 87: IEvalue = "UE not responding"; break;
                                        case 88: IEvalue = "UE refuses"; break;
                                        case 89: IEvalue = "Service denied"; break;
                                        case 90: IEvalue = "Unable to page UE"; break;
                                        case 91: IEvalue = "No memory available"; break;
                                        case 92: IEvalue = "User authentication"; break;
                                        case 93: IEvalue = "APN access denied - no subscription"; break;
                                        case 94: IEvalue = "Request rejection(reason not specified)"; break;
                                        case 95: IEvalue = "P-TMSI Signature mismatch"; break;
                                        case 96: IEvalue = "IMSI not known"; break;
                                        case 97: IEvalue = "Semantic error in the TAD operation"; break;
                                        case 98: IEvalue = "Syntactic error in the TAD operation"; break;
                                        case 99: IEvalue = "Reserved Message Value Received"; break;
                                        case 100: IEvalue = "Remote peer not responding"; break;
                                        case 101: IEvalue = "Collision with network initiated request"; break;
                                        case 102: IEvalue = "Unable to page UE due to Suspension"; break;
                                        case 103: IEvalue = "Conditional IE missing"; break;
                                        case 104: IEvalue = "APN Restriction type Incompatible with currently active PDN connection"; break;
                                        case 105: IEvalue = "Invalid overall length of the triggered response message and a piggybacked initial message"; break;
                                        case 106: IEvalue = "Data forwarding not supported"; break;
                                        case 107: IEvalue = "Invalid reply from remote peer"; break;
                                        case 108: IEvalue = "Fallback to GTPv1"; break;
                                        case 109: IEvalue = "Invalid peer"; break;
                                        case 110: IEvalue = "Temporarily rejected due to handover procedure in progress"; break;
                                        case 111: IEvalue = ""; break;
                                        case 112: IEvalue = "Request rejected for a PMIPv6 reason(see 3GPP TS 29.275[26])"; break;
                                        case 113: IEvalue = ""; break;
                                        case 114: IEvalue = ""; break;
                                        case 115: IEvalue = ""; break;
                                        case 116: IEvalue = "Multiple PDN connection for a given APN not allowed"; break;
                                        case 117:
                                        case 118:
                                        case 119:
                                        case 120:
                                        case 121:
                                        case 122:
                                        case 123:
                                        case 124:
                                        case 125:
                                        case 126:
                                        case 127:
                                        case 128:
                                        case 129:
                                        case 130:
                                        case 131:
                                        case 132:
                                        case 133:
                                        case 134:
                                        case 135:
                                        case 136:
                                        case 137:
                                        case 138:
                                        case 139:
                                        case 140:
                                        case 141:
                                        case 142:
                                        case 143:
                                        case 144:
                                        case 145:
                                        case 146:
                                        case 147:
                                        case 148:
                                        case 149:
                                        case 150:
                                        case 151:
                                        case 152:
                                        case 153:
                                        case 154:
                                        case 155:
                                        case 156:
                                        case 157:
                                        case 158:
                                        case 159:
                                        case 160:
                                        case 171:
                                        case 172:
                                        case 173:
                                        case 174:
                                        case 175:
                                        case 176:
                                        case 177:
                                        case 178:
                                        case 179:
                                        case 180:
                                        case 181:
                                        case 182:
                                        case 183:
                                        case 184:
                                        case 185:
                                        case 186:
                                        case 187:
                                        case 188:
                                        case 189:
                                        case 190:
                                        case 191:
                                        case 192:
                                        case 193:
                                        case 194:
                                        case 195:
                                        case 196:
                                        case 197:
                                        case 198:
                                        case 199:
                                        case 200:
                                        case 201:
                                        case 202:
                                        case 203:
                                        case 204:
                                        case 205:
                                        case 206:
                                        case 207:
                                        case 208:
                                        case 209:
                                        case 210:
                                        case 211:
                                        case 212:
                                        case 213:
                                        case 214:
                                        case 215:
                                        case 216:
                                        case 217:
                                        case 218:
                                        case 219:
                                        case 220:
                                        case 221:
                                        case 222:
                                        case 223:
                                        case 224:
                                        case 225:
                                        case 226:
                                        case 227:
                                        case 228:
                                        case 229:
                                        case 230:
                                        case 231:
                                        case 232:
                                        case 233:
                                        case 234:
                                        case 235:
                                        case 236:
                                        case 237:
                                        case 238:
                                        case 239: IEvalue = "Spare"; break;
                                        case 240:
                                        case 241:
                                        case 242:
                                        case 243:
                                        case 244:
                                        case 245:
                                        case 246:
                                        case 247:
                                        case 248:
                                        case 249:
                                        case 250:
                                        case 251:
                                        case 252:
                                        case 253:
                                        case 254:
                                        case 255: IEvalue = "Spare"; break;
                                        default: IEvalue = "Error!"; break;
                                    }
                                    #endregion
                                    IElengthForAll = Convert.ToInt64(Chunk[12 + IElengthForAll + 1] * System.Math.Pow(256, 1) + Chunk[12 + IElengthForAll + 2] + 4 + IElengthForAll);
                                }
                                else if (Chunk[12 + IElengthForAll] == 1)
                                {
                                    int[] imsi = new int[16];
                                    int count = 0;
                                    for (int i = 4; i < 12; i++)
                                    {
                                        imsi[count]=(Chunk[12 + IElengthForAll + i]) % 16;
                                        IMSI = IMSI + imsi[count].ToString();
                                        count++;
                                        if(i<11)
                                        {
                                             imsi[count]=(Chunk[12 + IElengthForAll + i]) >> 4;
                                             IMSI = IMSI + imsi[count].ToString();
                                             count++;
                                        }
                                       
                                    }
                                    IElengthForAll = Convert.ToInt64(Chunk[12 + IElengthForAll + 1] * System.Math.Pow(256, 1) + Chunk[12 + IElengthForAll + 2] + 4 + IElengthForAll);
                                }
                                // else if (Chunk[12 + IElengthForAll] == 87)
                                // {
                                //     FTEID = "LIULIULU";
                                //}
                                else
                                {
                                    IEvalue = "null";
                                    IElengthForAll = Convert.ToInt64(Chunk[12 + IElengthForAll + 1] * System.Math.Pow(256, 1) + Chunk[12 + IElengthForAll + 2] + 4 + IElengthForAll);
                                }
                            }
                        }
                        else
                        {
                            while (length - 8 - IElengthForAll > 0)
                            {

                                if (Chunk[12 + IElengthForAll] == 2)//2代表cause,即信元类型
                                {
                                    #region  casue Value
                                    switch (Chunk[12 + IElengthForAll + 4])
                                    {
                                        case 0: IEvalue = "Reserved. Shall not be sent and if received the Cause shall be treated as an invalid IE"; break;
                                        case 1: IEvalue = "Reserved"; break;
                                        case 2: IEvalue = "Local Detach"; break;
                                        case 3: IEvalue = "complete Detach"; break;
                                        case 4: IEvalue = "RAT changed from 3GPP to Non-3GPP"; break;
                                        case 5: IEvalue = "ISR deactivation"; break;
                                        case 6: IEvalue = "Error Indication received from RAN/eNodeB/S4-SGSN"; break;
                                        case 7: IEvalue = "IMSI Detach Only"; break;
                                        case 8:
                                        case 9:
                                        case 10:
                                        case 11:
                                        case 12:
                                        case 13:
                                        case 14:
                                        case 15: IEvalue = "Spare"; break;
                                        case 16: IEvalue = "Request accepted"; break;
                                        case 17: IEvalue = "Request accepted partially"; break;
                                        case 18: IEvalue = "New PDN type due to network preference"; break;
                                        case 19: IEvalue = "New PDN type due to single address beare only"; break;
                                        case 20:
                                        case 21:
                                        case 22:
                                        case 23:
                                        case 24:
                                        case 25:
                                        case 26:
                                        case 27:
                                        case 28:
                                        case 29:
                                        case 30:
                                        case 31:
                                        case 32:
                                        case 33:
                                        case 34:
                                        case 35:
                                        case 36:
                                        case 37:
                                        case 38:
                                        case 39:
                                        case 40:
                                        case 41:
                                        case 42:
                                        case 43:
                                        case 44:
                                        case 45:
                                        case 46:
                                        case 47:
                                        case 48:
                                        case 49:
                                        case 50:
                                        case 51:
                                        case 52:
                                        case 53:
                                        case 54:
                                        case 55:
                                        case 56:
                                        case 57:
                                        case 58:
                                        case 59:
                                        case 60:
                                        case 61:
                                        case 62:
                                        case 63: IEvalue = "Spare"; break;
                                        case 64: IEvalue = "Context Not Found"; break;
                                        case 65: IEvalue = "Invalid Message Format"; break;
                                        case 66: IEvalue = "Version Not supported by next peer"; break;
                                        case 67: IEvalue = "Invalid Message Format"; break;
                                        case 68: IEvalue = "Service not supported"; break;
                                        case 69: IEvalue = "Mandatory IE incorrect"; break;
                                        case 70: IEvalue = "Mandatory IE missing"; break;
                                        case 71: IEvalue = "Shall not be used"; break;
                                        case 72: IEvalue = "System failure"; break;
                                        case 73: IEvalue = "No resource available"; break;
                                        case 74: IEvalue = "Semantic error in the TFT operation"; break;
                                        case 75: IEvalue = "Syntatic error in the TFT operation"; break;
                                        case 76: IEvalue = "Semantic errors in packet filter(s)"; break;
                                        case 77: IEvalue = "Syntactic errors in packet filter(s)"; break;
                                        case 78: IEvalue = "Missing or unknow APN"; break;
                                        case 79: IEvalue = "Shall not be used"; break;
                                        case 80: IEvalue = "GRE key not found"; break;
                                        case 81: IEvalue = "Relocation failure"; break;
                                        case 82: IEvalue = "Denied in RAT"; break;
                                        case 83: IEvalue = "Preferred PDN type not supported"; break;
                                        case 84: IEvalue = "All dynamic addresses are occupied"; break;
                                        case 85: IEvalue = "UE context without TFT already activated"; break;
                                        case 86: IEvalue = "Protocol type not supported"; break;
                                        case 87: IEvalue = "UE not responding"; break;
                                        case 88: IEvalue = "UE refuses"; break;
                                        case 89: IEvalue = "Service denied"; break;
                                        case 90: IEvalue = "Unable to page UE"; break;
                                        case 91: IEvalue = "No memory available"; break;
                                        case 92: IEvalue = "User authentication"; break;
                                        case 93: IEvalue = "APN access denied - no subscription"; break;
                                        case 94: IEvalue = "Request rejection(reason not specified)"; break;
                                        case 95: IEvalue = "P-TMSI Signature mismatch"; break;
                                        case 96: IEvalue = "IMSI not known"; break;
                                        case 97: IEvalue = "Semantic error in the TAD operation"; break;
                                        case 98: IEvalue = "Syntactic error in the TAD operation"; break;
                                        case 99: IEvalue = "Reserved Message Value Received"; break;
                                        case 100: IEvalue = "Remote peer not responding"; break;
                                        case 101: IEvalue = "Collision with network initiated request"; break;
                                        case 102: IEvalue = "Unable to page UE due to Suspension"; break;
                                        case 103: IEvalue = "Conditional IE missing"; break;
                                        case 104: IEvalue = "APN Restriction type Incompatible with currently active PDN connection"; break;
                                        case 105: IEvalue = "Invalid overall length of the triggered response message and a piggybacked initial message"; break;
                                        case 106: IEvalue = "Data forwarding not supported"; break;
                                        case 107: IEvalue = "Invalid reply from remote peer"; break;
                                        case 108: IEvalue = "Fallback to GTPv1"; break;
                                        case 109: IEvalue = "Invalid peer"; break;
                                        case 110: IEvalue = "Temporarily rejected due to handover procedure in progress"; break;
                                        case 111: IEvalue = ""; break;
                                        case 112: IEvalue = "Request rejected for a PMIPv6 reason(see 3GPP TS 29.275[26])"; break;
                                        case 113: IEvalue = ""; break;
                                        case 114: IEvalue = ""; break;
                                        case 115: IEvalue = ""; break;
                                        case 116: IEvalue = "Multiple PDN connection for a given APN not allowed"; break;
                                        case 117:
                                        case 118:
                                        case 119:
                                        case 120:
                                        case 121:
                                        case 122:
                                        case 123:
                                        case 124:
                                        case 125:
                                        case 126:
                                        case 127:
                                        case 128:
                                        case 129:
                                        case 130:
                                        case 131:
                                        case 132:
                                        case 133:
                                        case 134:
                                        case 135:
                                        case 136:
                                        case 137:
                                        case 138:
                                        case 139:
                                        case 140:
                                        case 141:
                                        case 142:
                                        case 143:
                                        case 144:
                                        case 145:
                                        case 146:
                                        case 147:
                                        case 148:
                                        case 149:
                                        case 150:
                                        case 151:
                                        case 152:
                                        case 153:
                                        case 154:
                                        case 155:
                                        case 156:
                                        case 157:
                                        case 158:
                                        case 159:
                                        case 160:
                                        case 171:
                                        case 172:
                                        case 173:
                                        case 174:
                                        case 175:
                                        case 176:
                                        case 177:
                                        case 178:
                                        case 179:
                                        case 180:
                                        case 181:
                                        case 182:
                                        case 183:
                                        case 184:
                                        case 185:
                                        case 186:
                                        case 187:
                                        case 188:
                                        case 189:
                                        case 190:
                                        case 191:
                                        case 192:
                                        case 193:
                                        case 194:
                                        case 195:
                                        case 196:
                                        case 197:
                                        case 198:
                                        case 199:
                                        case 200:
                                        case 201:
                                        case 202:
                                        case 203:
                                        case 204:
                                        case 205:
                                        case 206:
                                        case 207:
                                        case 208:
                                        case 209:
                                        case 210:
                                        case 211:
                                        case 212:
                                        case 213:
                                        case 214:
                                        case 215:
                                        case 216:
                                        case 217:
                                        case 218:
                                        case 219:
                                        case 220:
                                        case 221:
                                        case 222:
                                        case 223:
                                        case 224:
                                        case 225:
                                        case 226:
                                        case 227:
                                        case 228:
                                        case 229:
                                        case 230:
                                        case 231:
                                        case 232:
                                        case 233:
                                        case 234:
                                        case 235:
                                        case 236:
                                        case 237:
                                        case 238:
                                        case 239: IEvalue = "Spare"; break;
                                        case 240:
                                        case 241:
                                        case 242:
                                        case 243:
                                        case 244:
                                        case 245:
                                        case 246:
                                        case 247:
                                        case 248:
                                        case 249:
                                        case 250:
                                        case 251:
                                        case 252:
                                        case 253:
                                        case 254:
                                        case 255: IEvalue = "Spare"; break;
                                        default: IEvalue = "Error!"; break;
                                    }
                                    #endregion
                                    IElengthForAll = Convert.ToInt64(Chunk[12 + IElengthForAll + 1] * System.Math.Pow(256, 1) + Chunk[12 + IElengthForAll + 2] + 4 + IElengthForAll);
                                    break;
                                }
                                else
                                {
                                    IEvalue = "null";
                                    IElengthForAll = Convert.ToInt64(Chunk[12 + IElengthForAll + 1] * System.Math.Pow(256, 1) + Chunk[12 + IElengthForAll + 2] + 4 + IElengthForAll);
                                }
                            }
                        }

                        break;

                //T=0,消息头有8个字节，解析IE从第9个字节开始
                case 0: tinfo = ".... 0... = TEID flag(T):0"; 
                        snumber = Convert.ToInt64(Chunk[4] * System.Math.Pow(256, 2) + Chunk[5] * System.Math.Pow(256, 1) + Chunk[6]);
                        seqinfo="Sequence Number:"+Convert.ToString(snumber,10);
                        spare = Chunk[7];
                        switch (spare)
                        {
                            case 0: spareinfo = "Spare:0"; break;
                            default: spareinfo = "error!"; break;
                        } 
                    //找出cause IE解析
                        while (length-4 - IElengthForAll > 0)
                        {

                            if(Chunk[8+IElengthForAll]==2)
                            {
                            #region  casue Value
                            switch (Chunk[12 + IElengthForAll + 4])
                            {
                                case 0: IEvalue = "Reserved. Shall not be sent and if received the Cause shall be treated as an invalid IE"; break;
                                case 1: IEvalue = "Reserved"; break;
                                case 2: IEvalue = "Local Detach"; break;
                                case 3: IEvalue = "complete Detach"; break;
                                case 4: IEvalue = "RAT changed from 3GPP to Non-3GPP"; break;
                                case 5: IEvalue = "ISR deactivation"; break;
                                case 6: IEvalue = "Error Indication received from RAN/eNodeB/S4-SGSN"; break;
                                case 7: IEvalue = "IMSI Detach Only"; break;
                                case 8:
                                case 9:
                                case 10:
                                case 11:
                                case 12:
                                case 13:
                                case 14:
                                case 15: IEvalue = "Spare"; break;
                                case 16: IEvalue = "Request accepted"; break;
                                case 17: IEvalue = "Request accepted partially"; break;
                                case 18: IEvalue = "New PDN type due to network preference"; break;
                                case 19: IEvalue = "New PDN type due to single address beare only"; break;
                                case 20:
                                case 21:
                                case 22:
                                case 23:
                                case 24:
                                case 25:
                                case 26:
                                case 27:
                                case 28:
                                case 29:
                                case 30:
                                case 31:
                                case 32:
                                case 33:
                                case 34:
                                case 35:
                                case 36:
                                case 37:
                                case 38:
                                case 39:
                                case 40:
                                case 41:
                                case 42:
                                case 43:
                                case 44:
                                case 45:
                                case 46:
                                case 47:
                                case 48:
                                case 49:
                                case 50:
                                case 51:
                                case 52:
                                case 53:
                                case 54:
                                case 55:
                                case 56:
                                case 57:
                                case 58:
                                case 59:
                                case 60:
                                case 61:
                                case 62:
                                case 63: IEvalue = "Spare"; break;
                                case 64: IEvalue = "Context Not Found"; break;
                                case 65: IEvalue = "Invalid Message Format"; break;
                                case 66: IEvalue = "Version Not supported by next peer"; break;
                                case 67: IEvalue = "Invalid Message Format"; break;
                                case 68: IEvalue = "Service not supported"; break;
                                case 69: IEvalue = "Mandatory IE incorrect"; break;
                                case 70: IEvalue = "Mandatory IE missing"; break;
                                case 71: IEvalue = "Shall not be used"; break;
                                case 72: IEvalue = "System failure"; break;
                                case 73: IEvalue = "No resource available"; break;
                                case 74: IEvalue = "Semantic error in the TFT operation"; break;
                                case 75: IEvalue = "Syntatic error in the TFT operation"; break;
                                case 76: IEvalue = "Semantic errors in packet filter(s)"; break;
                                case 77: IEvalue = "Syntactic errors in packet filter(s)"; break;
                                case 78: IEvalue = "Missing or unknow APN"; break;
                                case 79: IEvalue = "Shall not be used"; break;
                                case 80: IEvalue = "GRE key not found"; break;
                                case 81: IEvalue = "Relocation failure"; break;
                                case 82: IEvalue = "Denied in RAT"; break;
                                case 83: IEvalue = "Preferred PDN type not supported"; break;
                                case 84: IEvalue = "All dynamic addresses are occupied"; break;
                                case 85: IEvalue = "UE context without TFT already activated"; break;
                                case 86: IEvalue = "Protocol type not supported"; break;
                                case 87: IEvalue = "UE not responding"; break;
                                case 88: IEvalue = "UE refuses"; break;
                                case 89: IEvalue = "Service denied"; break;
                                case 90: IEvalue = "Unable to page UE"; break;
                                case 91: IEvalue = "No memory available"; break;
                                case 92: IEvalue = "User authentication"; break;
                                case 93: IEvalue = "APN access denied - no subscription"; break;
                                case 94: IEvalue = "Request rejection(reason not specified)"; break;
                                case 95: IEvalue = "P-TMSI Signature mismatch"; break;
                                case 96: IEvalue = "IMSI not known"; break;
                                case 97: IEvalue = "Semantic error in the TAD operation"; break;
                                case 98: IEvalue = "Syntactic error in the TAD operation"; break;
                                case 99: IEvalue = "Reserved Message Value Received"; break;
                                case 100: IEvalue = "Remote peer not responding"; break;
                                case 101: IEvalue = "Collision with network initiated request"; break;
                                case 102: IEvalue = "Unable to page UE due to Suspension"; break;
                                case 103: IEvalue = "Conditional IE missing"; break;
                                case 104: IEvalue = "APN Restriction type Incompatible with currently active PDN connection"; break;
                                case 105: IEvalue = "Invalid overall length of the triggered response message and a piggybacked initial message"; break;
                                case 106: IEvalue = "Data forwarding not supported"; break;
                                case 107: IEvalue = "Invalid reply from remote peer"; break;
                                case 108: IEvalue = "Fallback to GTPv1"; break;
                                case 109: IEvalue = "Invalid peer"; break;
                                case 110: IEvalue = "Temporarily rejected due to handover procedure in progress"; break;
                                case 111: IEvalue = ""; break;
                                case 112: IEvalue = "Request rejected for a PMIPv6 reason(see 3GPP TS 29.275[26])"; break;
                                case 113: IEvalue = ""; break;
                                case 114: IEvalue = ""; break;
                                case 115: IEvalue = ""; break;
                                case 116: IEvalue = "Multiple PDN connection for a given APN not allowed"; break;
                                case 117:
                                case 118:
                                case 119:
                                case 120:
                                case 121:
                                case 122:
                                case 123:
                                case 124:
                                case 125:
                                case 126:
                                case 127:
                                case 128:
                                case 129:
                                case 130:
                                case 131:
                                case 132:
                                case 133:
                                case 134:
                                case 135:
                                case 136:
                                case 137:
                                case 138:
                                case 139:
                                case 140:
                                case 141:
                                case 142:
                                case 143:
                                case 144:
                                case 145:
                                case 146:
                                case 147:
                                case 148:
                                case 149:
                                case 150:
                                case 151:
                                case 152:
                                case 153:
                                case 154:
                                case 155:
                                case 156:
                                case 157:
                                case 158:
                                case 159:
                                case 160:
                                case 171:
                                case 172:
                                case 173:
                                case 174:
                                case 175:
                                case 176:
                                case 177:
                                case 178:
                                case 179:
                                case 180:
                                case 181:
                                case 182:
                                case 183:
                                case 184:
                                case 185:
                                case 186:
                                case 187:
                                case 188:
                                case 189:
                                case 190:
                                case 191:
                                case 192:
                                case 193:
                                case 194:
                                case 195:
                                case 196:
                                case 197:
                                case 198:
                                case 199:
                                case 200:
                                case 201:
                                case 202:
                                case 203:
                                case 204:
                                case 205:
                                case 206:
                                case 207:
                                case 208:
                                case 209:
                                case 210:
                                case 211:
                                case 212:
                                case 213:
                                case 214:
                                case 215:
                                case 216:
                                case 217:
                                case 218:
                                case 219:
                                case 220:
                                case 221:
                                case 222:
                                case 223:
                                case 224:
                                case 225:
                                case 226:
                                case 227:
                                case 228:
                                case 229:
                                case 230:
                                case 231:
                                case 232:
                                case 233:
                                case 234:
                                case 235:
                                case 236:
                                case 237:
                                case 238:
                                case 239: IEvalue = "Spare"; break;
                                case 240:
                                case 241:
                                case 242:
                                case 243:
                                case 244:
                                case 245:
                                case 246:
                                case 247:
                                case 248:
                                case 249:
                                case 250:
                                case 251:
                                case 252:
                                case 253:
                                case 254:
                                case 255: IEvalue = "Spare"; break;
                                default: IEvalue = "Error!"; break;
                            }
                            #endregion

                            IElengthForAll = Convert.ToInt64(Chunk[8 + IElengthForAll + 1] * System.Math.Pow(256, 1) + Chunk[8 + IElengthForAll + 2] + 4 + IElengthForAll);
                                break;
                            }
                            else
                            {
                                IEvalue = "null";
                                IElengthForAll = Convert.ToInt64(Chunk[8 + IElengthForAll + 1] * System.Math.Pow(256, 1) + Chunk[8 + IElengthForAll + 2] + 4 + IElengthForAll);//转化为64位整数
                            }
                        }
                        break;
                default: tinfo = "error！"; break;
            }
            #endregion

        }
        #endregion

        #region  整个包的解析
        public void decoder_All(byte[] Chunk)
        {    
            // 包含数据链路层
            //sourIP=Convert.ToString(Chunk[28])+"."+Convert.ToString(Chunk[29])+"."+Convert.ToString(Chunk[30])+"."+Convert.ToString(Chunk[31]);
            //destIP = Convert.ToString(Chunk[32]) + "." + Convert.ToString(Chunk[33]) + "." + Convert.ToString(Chunk[34]) + "." + Convert.ToString(Chunk[35]);
            //sourPort = Convert.ToString(Chunk[36] * 256 + Chunk[37]);
            //destPort = Convert.ToString(Chunk[38] * 256 + Chunk[39]);
            //allLength = Convert.ToString(Chunk.Length);

            //不包含数据链路层 -16
            sourIP = Convert.ToString(Chunk[12]) + "." + Convert.ToString(Chunk[13]) + "." + Convert.ToString(Chunk[14]) + "." + Convert.ToString(Chunk[15]);
            destIP = Convert.ToString(Chunk[16]) + "." + Convert.ToString(Chunk[17]) + "." + Convert.ToString(Chunk[18]) + "." + Convert.ToString(Chunk[19]);
            sourPort = Convert.ToString(Chunk[20] * 256 + Chunk[21]);
            destPort = Convert.ToString(Chunk[22] * 256 + Chunk[23]);
            allLength = Convert.ToString(Chunk.Length);

            protocol = "GTPv2";
        }
        #endregion

    }
}

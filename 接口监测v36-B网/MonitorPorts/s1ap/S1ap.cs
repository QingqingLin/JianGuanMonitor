using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
#region
//namespace s1ap
//{
//    class s1apdecode
//    {
//        public byte type = 0;
//        public byte procedureCode = 0;
//        public byte direction = 0;
//        public void decode(byte[] a)
//        {

//            type = a[0];
//            procedureCode = a[1];
//            if (type == 0x00)//此条消息为初始化消息
//                switch (procedureCode)
//                {
//                    case 0:
//                        {
//                            direction = 0;
//                            Console.WriteLine("HandoverPreparation\n");
//                            Console.WriteLine("HandoverRequired\n");
//                        }
//                        break;
//                    case 1:
//                        {
//                            direction = 1;
//                            Console.WriteLine("HandoverResourceAllocation\n");
//                            Console.WriteLine("HandoverRequest\n");
//                        }
//                        break;

//                    case 2:
//                        {
//                            direction = 0;
//                            Console.WriteLine("HandoverNotification\n");
//                            Console.WriteLine("HandoverNotify\n");
//                        }
//                        break;
//                    case 3:
//                        {
//                            direction = 0;
//                            Console.WriteLine("PathSwitchRequest\n");
//                            Console.WriteLine("PathSwitchRequest\n");
//                        }
//                        break;
//                    case 4:
//                        {
//                            direction = 0;
//                            Console.WriteLine("HandoverCancellation\n");
//                            Console.WriteLine("HandoverCancel\n");
//                        }
//                        break;
//                    case 5:
//                        {
//                            direction = 1;
//                            Console.WriteLine("E_RABSetup\n");
//                            Console.WriteLine("E_RABSetupRequest\n");//nas-pdu

//                        }
//                        break;
//                    case 6:
//                        {
//                            direction = 1;
//                            Console.WriteLine("E_RABModify\n");
//                            Console.WriteLine("E_RABModifyRequest\n");//nas-pdu			
//                        }
//                        break;
//                    case 7:
//                        {
//                            direction = 1;
//                            Console.WriteLine("E_RABRelease\n");
//                            Console.WriteLine("E_RABReleaseCommand\n");//nas-pdu			
//                        }
//                        break;
//                    case 8:
//                        {
//                            direction = 0;
//                            Console.WriteLine("E_RABReleaseIndication\n");
//                            Console.WriteLine("E_RABReleaseIndication\n");
//                        }
//                        break;
//                    case 9:
//                        {
//                            direction = 1;
//                            Console.WriteLine("InitialContextSetup\n");
//                            Console.WriteLine("InitialContextSetupRequest\n");  //nas-pdu		
//                        }
//                        break;
//                    case 10:
//                        {
//                            direction = 1;
//                            Console.WriteLine("Paging\n");
//                            Console.WriteLine("Paging\n");
//                        }
//                        break;
//                    case 11:
//                        {
//                            direction = 1;
//                            Console.WriteLine("downlinkNASTransport\n");
//                            Console.WriteLine("downlinkNASTransport\n");//nas-pdu			
//                        }
//                        break;
//                    case 12:
//                        {
//                            direction = 0;
//                            Console.WriteLine("initialUEMessage\n");
//                            Console.WriteLine("initialUEMessage\n");//nas-pdu			
//                        }
//                        break;
//                    case 13:
//                        {
//                            direction = 0;
//                            Console.WriteLine("uplinkNASTransport\n");
//                            Console.WriteLine("uplinkNASTransport\n");//nas-pdu			
//                        }
//                        break;
//                    case 14:
//                        {
//                            direction = 2;
//                            Console.WriteLine("Reset\n");
//                            Console.WriteLine("Reset\n");
//                        }
//                        break;
//                    case 15:
//                        {

//                            Console.WriteLine("ErrorIndication\n");//由于ErrorIndication信令，在mme与enodeB处都可以发起，所以direction不确定，但是该信令不含有NAS-pdu,故不需要设置direction
//                            Console.WriteLine("ErrorIndication\n");
//                        }
//                        break;
//                    case 16:
//                        {
//                            direction = 0;
//                            Console.WriteLine("NASNonDeliveryIndication\n");
//                            Console.WriteLine("NASNonDeliveryIndication\n");//nas-pdu			
//                        }
//                        break;
//                    case 17:
//                        {
//                            direction = 0;
//                            Console.WriteLine("S1Setup\n");
//                            Console.WriteLine("S1SetupRequest\n");
//                        }
//                        break;
//                    case 18:
//                        {
//                            direction = 0;
//                            Console.WriteLine("UEContextReleaseRequest\n");
//                            Console.WriteLine("UEContextReleaseRequest\n");
//                        }
//                        break;
//                    case 19:
//                        {
//                            direction = 1;
//                            Console.WriteLine("DownlinkS1cdma2000tunneling\n");
//                            Console.WriteLine("DownlinkS1cdma2000tunneling\n");
//                        }
//                        break;
//                    case 20:
//                        {
//                            direction = 0;
//                            Console.WriteLine("UplinkS1cdma2000tunneling\n");
//                            Console.WriteLine("UplinkS1cdma2000tunneling\n");
//                        }
//                        break;
//                    case 21:
//                        {
//                            direction = 1;
//                            Console.WriteLine("UEContextModification\n");
//                            Console.WriteLine("UEContextModificationRequest\n");
//                        }
//                        break;
//                    case 22:
//                        {
//                            direction = 0;
//                            Console.WriteLine("UECapabilityInfoIndication\n");
//                            Console.WriteLine("UECapabilityInfoIndication\n");
//                        }
//                        break;
//                    case 23:
//                        {
//                            direction = 1;
//                            Console.WriteLine("UEContextRelease\n");
//                            Console.WriteLine("UEContextReleaseCommand\n");
//                        }
//                        break;
//                    case 24:
//                        {
//                            direction = 0;
//                            Console.WriteLine("eNBStatusTransfer\n");
//                            Console.WriteLine("eNBStatusTransfer\n");
//                        }
//                        break;
//                    case 25:
//                        {
//                            direction = 1;
//                            Console.WriteLine("MMEStatusTransfer\n");
//                            Console.WriteLine("MMEStatusTransfer\n");
//                        }
//                        break;
//                    case 26:
//                        {
//                            direction = 1;
//                            Console.WriteLine("DeactivateTrace\n");
//                            Console.WriteLine("DeactivateTrace\n");
//                        }
//                        break;
//                    case 27:
//                        {
//                            direction = 1;
//                            Console.WriteLine("TraceStart\n");
//                            Console.WriteLine("TraceStart\n");
//                        }
//                        break;
//                    case 28:
//                        {
//                            direction = 0;
//                            Console.WriteLine("TraceFailureIndication\n");
//                            Console.WriteLine("TraceFailureIndication\n");
//                        }
//                        break;
//                    case 29:
//                        {
//                            direction = 0;
//                            Console.WriteLine("ENBConfigurationUpdate\n");
//                            Console.WriteLine("ENBConfigurationUpdate\n");
//                        }
//                        break;
//                    case 30:
//                        {
//                            direction = 1;
//                            Console.WriteLine("MMEConfigurationUpdate\n");
//                            Console.WriteLine("MMEConfigurationUpdate\n");
//                        }
//                        break;
//                    case 31:
//                        {
//                            direction = 1;
//                            Console.WriteLine("LocationReportingControl\n");
//                            Console.WriteLine("LocationReportingControl\n");
//                        }
//                        break;
//                    case 32:
//                        {
//                            direction = 0;
//                            Console.WriteLine("LocationReportingFailureIndication\n");
//                            Console.WriteLine("LocationReportingFailureIndication\n");
//                        }
//                        break;
//                    case 33:
//                        {
//                            direction = 0;
//                            Console.WriteLine("LocationReport\n");
//                            Console.WriteLine("LocationReport\n");
//                        }
//                        break;
//                    case 34:
//                        {
//                            direction = 1;
//                            Console.WriteLine("OverloadStart\n");
//                            Console.WriteLine("OverloadStart\n");
//                        }
//                        break;
//                    case 35:
//                        {
//                            direction = 1;
//                            Console.WriteLine("OverloadStop\n");
//                            Console.WriteLine("OverloadStop\n");
//                        }
//                        break;
//                    case 36:
//                        {
//                            direction = 1;
//                            Console.WriteLine("WriteReplaceWarning\n");
//                            Console.WriteLine("WriteReplaceWarningRequest\n");
//                        }
//                        break;
//                    case 37:
//                        {
//                            direction = 0;
//                            Console.WriteLine("eNBDirectInformationTransfer\n");
//                            Console.WriteLine("eNBDirectInformationTransfer\n");
//                        }
//                        break;
//                    case 38:
//                        {
//                            direction = 1;
//                            Console.WriteLine("MMEDirectInformationTransfer\n");
//                            Console.WriteLine("MMEDirectInformationTransfer\n");
//                        }
//                        break;
//                    case 39:
//                        {
//                            Console.WriteLine("PrivateMessage\n");//?
//                            Console.WriteLine("PrivateMessage\n");  //不清楚该过程属于class1还是class2，故先把他作为class1，有初始化，成功，失败消息		
//                        }
//                        break;
//                    case 40:
//                        {
//                            direction = 0;
//                            Console.WriteLine("eNBConfigurationTransfer\n");
//                            Console.WriteLine("eNBConfigurationTransfer\n");
//                        }
//                        break;
//                    case 41:
//                        {
//                            direction = 1;
//                            Console.WriteLine("MMEConfigurationTransfer\n");
//                            Console.WriteLine("MMEConfigurationTransfer\n");
//                        }
//                        break;
//                    case 42:
//                        {
//                            direction = 0;
//                            Console.WriteLine("CellTrafficTrace\n");
//                            Console.WriteLine("CellTrafficTrace\n");
//                        }
//                        break;
//                    case 43:
//                        {
//                            direction = 1;
//                            Console.WriteLine("Kill\n");
//                            Console.WriteLine("KillRequest\n");
//                        }
//                        break;
//                }


//            if (type == 0x20)//此条消息为成功响应消息
//                switch (procedureCode)
//                {
//                    case 0:
//                        {
//                            direction = 1;
//                            Console.WriteLine("HandoverPreparation\n");
//                            Console.WriteLine("HandoverCommand\n");
//                        }
//                        break;
//                    case 1:
//                        {
//                            direction = 0;
//                            Console.WriteLine("HandoverResourceAllocation\n");
//                            Console.WriteLine("HandoverRequestAcknowledge\n");
//                        }
//                        break;
//                    case 3:
//                        {
//                            direction = 1;
//                            Console.WriteLine("PathSwitchRequest\n");
//                            Console.WriteLine("PathSwitchRequestAcknowledge\n");
//                        }
//                        break;
//                    case 4:
//                        {
//                            direction = 1;
//                            Console.WriteLine("HandoverCancellation\n");
//                            Console.WriteLine("HandoverCancelAcknowledge\n");
//                        }
//                        break;
//                    case 5:
//                        {
//                            direction = 0;
//                            Console.WriteLine("E_RABSetup\n");
//                            Console.WriteLine("E_RABSetupResponse\n");
//                        }
//                        break;
//                    case 6:
//                        {
//                            direction = 0;
//                            Console.WriteLine("E_RABModify\n");
//                            Console.WriteLine("E_RABModifyResponse\n");
//                        }
//                        break;
//                    case 7:
//                        {
//                            direction = 0;
//                            Console.WriteLine("E_RABRelease\n");
//                            Console.WriteLine("E_RABReleaseResponse\n");
//                        }
//                        break;
//                    case 9:
//                        {
//                            direction = 0;
//                            Console.WriteLine("InitialContextSetup\n");
//                            Console.WriteLine("InitialContextSetupResponse\n");
//                        }
//                        break;
//                    case 14:
//                        {
//                            direction = 0;
//                            Console.WriteLine("Reset\n");
//                            Console.WriteLine("ResetAcknowledge\n");
//                        }
//                        break;
//                    case 17:
//                        {
//                            direction = 1;
//                            Console.WriteLine("S1Setup\n");
//                            Console.WriteLine("S1SetupResponse\n");
//                        }
//                        break;
//                    case 21:
//                        {
//                            direction = 0;
//                            Console.WriteLine("UEContextModification\n");
//                            Console.WriteLine("UEContextModificationResponse\n");
//                        }
//                        break;
//                    case 23:
//                        {
//                            direction = 0;
//                            Console.WriteLine("UEContextRelease\n");
//                            Console.WriteLine("UEContextReleaseComplete\n");
//                        }
//                        break;
//                    case 29:
//                        {
//                            direction = 1;
//                            Console.WriteLine("ENBConfigurationUpdate\n");
//                            Console.WriteLine("ENBConfigurationUpdateAcknowledge\n");
//                        }
//                        break;
//                    case 30:
//                        {
//                            direction = 0;
//                            Console.WriteLine("MMEConfigurationUpdate\n");
//                            Console.WriteLine("MMEConfigurationUpdateAcknowledge\n");
//                        }
//                        break;
//                    case 36:
//                        {
//                            direction = 0;
//                            Console.WriteLine("WriteReplaceWarning\n");
//                            Console.WriteLine("WriteReplaceWarningResponse\n");
//                        }
//                        break;
//                    case 43:
//                        {
//                            direction = 0;
//                            Console.WriteLine("Kill\n");
//                            Console.WriteLine("KillResponse\n");
//                        }
//                        break;
//                }

//            if (type == 0x40)//此条消息为失败响应消息
//                switch (procedureCode)
//                {
//                    case 0:
//                        {
//                            direction = 1;
//                            Console.WriteLine("HandoverPreparation\n");
//                            Console.WriteLine("HandoverPreparationFailure\n");
//                        }
//                        break;
//                    case 1:
//                        {
//                            direction = 0;
//                            Console.WriteLine("HandoverResourceAllocation\n");
//                            Console.WriteLine("HandoverFailure\n");
//                        }
//                        break;
//                    case 3:
//                        {
//                            direction = 1;
//                            Console.WriteLine("PathSwitchRequest\n");
//                            Console.WriteLine("PathSwitchRequestFailure\n");
//                        }
//                        break;
//                    case 9:
//                        {
//                            direction = 0;
//                            Console.WriteLine("InitialContextSetup\n");
//                            Console.WriteLine("InitialContextSetupFailure\n");
//                        }
//                        break;
//                    case 17:
//                        {
//                            direction = 1;
//                            Console.WriteLine("S1Setup\n");
//                            Console.WriteLine("S1SetupFailure\n");
//                        }
//                        break;
//                    case 21:
//                        {
//                            direction = 0;
//                            Console.WriteLine("UEContextModification\n");
//                            Console.WriteLine("UEContextModificationFailure\n");
//                        }
//                        break;
//                    case 29:
//                        {
//                            direction = 1;
//                            Console.WriteLine("ENBConfigurationUpdate\n");
//                            Console.WriteLine("ENBConfigurationUpdateFailure\n");
//                        }
//                        break;
//                    case 30:
//                        {
//                            direction = 0;
//                            Console.WriteLine("MMEConfigurationUpdate\n");
//                            Console.WriteLine("MMEConfigurationUpdateFailure\n");
//                        }
//                        break;
//                }
//        }
//    }
//}
#endregion
namespace MonitorPorts
{
    class s1ap_name
    {
        #region  //方向hashTable
        static Hashtable GetHashtable()
        {
            Hashtable direHash = new Hashtable();//存放信令方向
            //0为上行，1为下行
            direHash.Add(0x0000, 0);
            direHash.Add(0x0001, 1);
            direHash.Add(0x0002, 0);
            direHash.Add(0x0003, 0);
            direHash.Add(0x0004, 0);
            direHash.Add(0x0005, 1);
            direHash.Add(0x0006, 1);
            direHash.Add(0x0007, 1);
            direHash.Add(0x0008, 0);
            direHash.Add(0x0009, 1);
            direHash.Add(0x000a, 1);
            direHash.Add(0x000b, 1);
            direHash.Add(0x000c, 0);
            direHash.Add(0x000d, 0);
            direHash.Add(0x000e, 2);//该条信令两方的网元都可发送，故方向不定
            direHash.Add(0x000f, 2);
            direHash.Add(0x0010, 0);
            direHash.Add(0x0011, 0);
            direHash.Add(0x0012, 0);
            direHash.Add(0x0013, 1);
            direHash.Add(0x0014, 0);
            direHash.Add(0x0015, 1);
            direHash.Add(0x0016, 0);
            direHash.Add(0x0017, 1);
            direHash.Add(0x0018, 0);
            direHash.Add(0x0019, 1);
            direHash.Add(0x001a, 1);
            direHash.Add(0x001b, 1);
            direHash.Add(0x001c, 0);
            direHash.Add(0x001d, 0);
            direHash.Add(0x001e, 1);
            direHash.Add(0x001f, 1);
            direHash.Add(0x0020, 0);
            direHash.Add(0x0021, 0);
            direHash.Add(0x0022, 1);
            direHash.Add(0x0023, 1);
            direHash.Add(0x0024, 1);
            direHash.Add(0x0025, 0);
            direHash.Add(0x0026, 1);
            direHash.Add(0x0027, 2);//该条信令两方的网元都可发送，故方向不定
            direHash.Add(0x0028, 0);
            direHash.Add(0x0029, 1);
            direHash.Add(0x002a, 0);
            direHash.Add(0x002b, 1);
            direHash.Add(0x2000, 1);
            direHash.Add(0x2001, 0);
            direHash.Add(0x2003, 1);
            direHash.Add(0x2004, 1);
            direHash.Add(0x2005, 0);
            direHash.Add(0x2006, 0);
            direHash.Add(0x2007, 0);
            direHash.Add(0x2009, 0);
            direHash.Add(0x200e, 0);
            direHash.Add(0x2011, 1);
            direHash.Add(0x2015, 0);
            direHash.Add(0x2017, 0);
            direHash.Add(0x201d, 1);
            direHash.Add(0x201e, 0);
            direHash.Add(0x2024, 0);
            direHash.Add(0x202b, 0);
            direHash.Add(0x4000, 1);
            direHash.Add(0x4001, 0);
            direHash.Add(0x4003, 1);
            direHash.Add(0x4009, 0);
            direHash.Add(0x4011, 1);
            direHash.Add(0x4015, 0);
            direHash.Add(0x401d, 1);
            direHash.Add(0x401e, 0);
            return direHash;
        }
        #endregion
        #region   //信令hashTable
        static Hashtable GetHashtable_1()
        {
            Hashtable sigHash = new Hashtable();//存放信令名称
            sigHash.Add(0x0000, "HandoverRequired");
            sigHash.Add(0x0001, "HandoverRequest");
            sigHash.Add(0x0002, "HandoverNotify");
            sigHash.Add(0x0003, "PathSwitchRequest");
            sigHash.Add(0x0004, "HandoverCancel");
            sigHash.Add(0x0005, "E_RABSetupRequest");
            sigHash.Add(0x0006, "E_RABModifyRequest");
            sigHash.Add(0x0007, "E_RABReleaseCommand");
            sigHash.Add(0x0008, "E_RABReleaseIndication");
            sigHash.Add(0x0009, "InitialContextSetupRequest");
            sigHash.Add(0x000a, "Paging");
            sigHash.Add(0x000b, "downlinkNASTransport");
            sigHash.Add(0x000c, "initialUEMessage");
            sigHash.Add(0x000d, "uplinkNASTransport");
            sigHash.Add(0x000e, "Reset");
            sigHash.Add(0x000f, "ErrorIndication");
            sigHash.Add(0x0010, "NASNonDeliveryIndication");
            sigHash.Add(0x0011, "S1SetupRequest");
            sigHash.Add(0x0012, "UEContextReleaseRequest");
            sigHash.Add(0x0013, "DownlinkS1cdma2000tunneling");
            sigHash.Add(0x0014, "UplinkS1cdma2000tunneling");
            sigHash.Add(0x0015, "UEContextModificationRequest");
            sigHash.Add(0x0016, "UECapabilityInfoIndication");
            sigHash.Add(0x0017, "UEContextReleaseCommand");
            sigHash.Add(0x0018, "eNBStatusTransfer");
            sigHash.Add(0x0019, "MMEStatusTransfer");
            sigHash.Add(0x001a, "DeactivateTrace");
            sigHash.Add(0x001b, "TraceStart");
            sigHash.Add(0x001c, "TraceFailureIndication");
            sigHash.Add(0x001d, "ENBConfigurationUpdate");
            sigHash.Add(0x001e, "MMEConfigurationUpdate");
            sigHash.Add(0x001f, "LocationReportingControl");
            sigHash.Add(0x0020, "LocationReportingFailureIndication");
            sigHash.Add(0x0021, "LocationReport");
            sigHash.Add(0x0022, "OverloadStart");
            sigHash.Add(0x0023, "OverloadStop");
            sigHash.Add(0x0024, "WriteReplaceWarningRequest");
            sigHash.Add(0x0025, "eNBDirectInformationTransfer");
            sigHash.Add(0x0026, "MMEDirectInformationTransfer");
            sigHash.Add(0x0027, "PrivateMessage");
            sigHash.Add(0x0028, "eNBConfigurationTransfer");
            sigHash.Add(0x0029, "MMEConfigurationTransfer");
            sigHash.Add(0x002a, "CellTrafficTrace");
            sigHash.Add(0x002b, "KillRequest");
            sigHash.Add(0x2000, "HandoverCommand");
            sigHash.Add(0x2001, "HandoverRequestAcknowledge");
            sigHash.Add(0x2003, "PathSwitchRequestAcknowledge");
            sigHash.Add(0x2004, "HandoverCancelAcknowledge");
            sigHash.Add(0x2005, "E_RABSetupResponse");
            sigHash.Add(0x2006, "E_RABModifyResponse");
            sigHash.Add(0x2007, "E_RABReleaseResponse");
            sigHash.Add(0x2009, "InitialContextSetupResponse");
            sigHash.Add(0x200e, "ResetAcknowledge");
            sigHash.Add(0x2011, "S1SetupResponse");
            sigHash.Add(0x2015, "UEContextModificationResponse");
            sigHash.Add(0x2017, "UEContextReleaseComplete");
            sigHash.Add(0x201d, "ENBConfigurationUpdateAcknowledge");
            sigHash.Add(0x201e, "MMEConfigurationUpdateAcknowledge");
            sigHash.Add(0x2024, "WriteReplaceWarningResponse");
            sigHash.Add(0x202b, "KillResponse");
            sigHash.Add(0x4000, "HandoverPreparationFailure");
            sigHash.Add(0x4001, "HandoverFailure");
            sigHash.Add(0x4003, "PathSwitchRequestFailure");
            sigHash.Add(0x4009, "InitialContextSetupFailure");
            sigHash.Add(0x4011, "S1SetupFailure");
            sigHash.Add(0x4015, "UEContextModificationFailure");
            sigHash.Add(0x401d, "ENBConfigurationUpdateFailure");
            sigHash.Add(0x401e, "MMEConfigurationUpdateFailure");
            return sigHash;
        }
        #endregion
        public byte type = 0;
        public byte procedure = 0;
        public string str1 = "";
        public int direction = 0;
        /// <summary>
        /// 该方法用于解析s1ap信令的名称及方向
        /// </summary>
        /// <param name="a">s1ap信令编码列表</param>
        /// <returns>信令名称</returns>
        public string s1ap_decode(List<byte[]> a)
        {
            Hashtable direHash = GetHashtable();
            Hashtable sigHash = GetHashtable_1();
            str1 = "";
            int key = 0;
            for (int i = 0; i < a.Count; i++)
            {
                type = a[i][0];
                procedure = a[i][1];
                key = (int)(procedure + type * 256);//哈希表的哈希函数
                str1 = str1 + (string)sigHash[key] + " ";
                direction = (int)direHash[key];
            }
            return str1;
        }
        public string s1ap_decode1(byte[] a)
        {
            Hashtable direHash = GetHashtable();
            Hashtable sigHash = GetHashtable_1();
            int key = 0;
            type = a[0];
            procedure = a[1];
            key = (int)(procedure + type * 256);//哈希表的哈希函数
            str1 = (string)sigHash[key];
            direction = (int)direHash[key];
            return str1;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
namespace MonitorPorts
{
    class causeOnline
    {
        public int Length = 0;
        public int Index = 0;
        /// <summary>
        /// 该类用于解析s1ap信令中的信元cause
        /// </summary>
        /// <returns></returns>
        #region   //判定信令是否含有cause
        static Hashtable GetHashtable()
        {
            Hashtable signalHash = new Hashtable();
            signalHash.Add(0x0000, true);
            signalHash.Add(0x0001, true);
            signalHash.Add(0x0004, true);
            signalHash.Add(0x000e, true);
            signalHash.Add(0x000f, true);
            signalHash.Add(0x0010, true);
            signalHash.Add(0x0012, true);
            signalHash.Add(0x0017, true);
            signalHash.Add(0x001c, true);
            signalHash.Add(0x4000, true);
            signalHash.Add(0x4001, true);
            signalHash.Add(0x4003, true);
            signalHash.Add(0x4009, true);
            signalHash.Add(0x4011, true);
            signalHash.Add(0x4015, true);
            signalHash.Add(0x401d, true);
            signalHash.Add(0x401e, true);
            return signalHash;
        }
        #endregion
        static Hashtable GetHashtable1()
        {
            Hashtable causeHash = new Hashtable();
            causeHash.Add(0, "unspecified");
            causeHash.Add(1, "tx2relocoverall-expiry");
            causeHash.Add(2, "successful-handover");
            causeHash.Add(3, "release-due-to-eutran-generated-reason");
            causeHash.Add(4, "handover-cancelled");
            causeHash.Add(5, "partial-handover");
            causeHash.Add(6, "ho-failure-in-target-EPC-eNB-or-target-system");
            causeHash.Add(7, "ho-target-not-allowed");
            causeHash.Add(8, "tS1relocoverall-expiry");
            causeHash.Add(9, "tS1relocprep-expiry");
            causeHash.Add(10, "cell-not-available");
            causeHash.Add(11, "unknown-targetID");
            causeHash.Add(12, "no-radio-resources-available-in-target-cell");
            causeHash.Add(13, "unknown-mme-ue-s1ap-id");
            causeHash.Add(14, "unknown-enb-ue-s1ap-id");
            causeHash.Add(15, "unknown-pair-ue-s1ap-id");
            causeHash.Add(16, "handover-desirable-for-radio-reason");
            causeHash.Add(17, "time-critical-handover");
            causeHash.Add(18, "resource-optimisation-handover");
            causeHash.Add(19, "reduce-load-in-serving-cell");
            causeHash.Add(20, "user-inactivity");
            causeHash.Add(21, "radio-connection-with-ue-lost");
            causeHash.Add(22, "load-balancing-tau-required");
            causeHash.Add(23, "cs-fallback-triggered");
            causeHash.Add(24, "ue-not-available-for-ps-service");
            causeHash.Add(25, "radio-resources-not-available");
            causeHash.Add(26, "failure-in-radio-interface-procedure");
            causeHash.Add(27, "invalid-qos-combination");
            causeHash.Add(28, "interrat-redirection");
            causeHash.Add(29, "interaction-with-other-procedure");
            causeHash.Add(30, "unknown-E-RAB-ID");
            causeHash.Add(31, "multiple-E-RAB-ID-instances");
            causeHash.Add(32, "encryption-and-or-integrity-protection-algorithms-not-supported");
            causeHash.Add(33, "s1-intra-system-handover-triggered");
            causeHash.Add(34, "s1-inter-system-handover-triggered");
            causeHash.Add(35, "x2-handover-triggered");
            return causeHash;
        }
        public String str1 = "";//str1是cause类型
        public String str2 = "";//str2是具体cause名称
        public void decodeCause(byte[] a)//a[]为s1ap编码流
        {
            Hashtable signalHash = GetHashtable();
            Hashtable causeHash = GetHashtable1();
            str1 = "";
            str2 = "";
            byte type1 = a[0];//信令类型
            byte procedure = a[1];//信令所属过程
            int signal = type1 * 256 + procedure;//该变量指示具体的信令

            if (signalHash[signal] == null)//表示该信令没有含有cause
                str2 = "";
            else if ((bool)signalHash[signal] == true)////表示该信令含有cause
            {
                int value = 0, type = 0;//value为编码中表示cause类型的字节，type表示cause5大类别中的一类
                ushort kind = 0;//kind表示某一大类中的具体cause
                ushort id = 0, length2 = 0;//id为信元编号,length2为一个信元的编码长度
                uint length1 = 0, i = 0;//length1为所有信元的编码长度
                byte sum = 0;//sum为信元数量
                length1 = a[3];
                if (length1 < 128)//当s1ap信令编码长度小于128
                {

                    sum = a[6];
                    for (i = 7; i < a.Length && sum != 0; sum--)//i运行到最后可能小于length，因为编码中可能含有补0的情况
                    {
                        id = (ushort)(a[i] * 256 + a[i + 1]);
                        length2 = a[i + 3];
                        if (id == 2)
                        {
                            #region
                            Index = (int)i + 4;
                            Length = a[i + 3];                            
                            if (length2 == 2)//该cause是Radio Network cause
                            {
                                str1 = ("CauseRadioNetwork\n");
                                value = (ushort)(a[i + 4] * 256 + a[i + 5]);
                                value = (ushort)(value >> 5);//因为radio network cause表示编码长度有6bit，跨越2字节
                                str2 = (string)causeHash[value];
                                break;
                                #region  //该处的功能由hash表实现了
                                //switch (value)
                                //{
                                //    case 0:
                                //        str2 = ("unspecified");
                                //        break;
                                //    case 1:
                                //        str2 = ("tx2relocoverall-expiry");
                                //        break;
                                //    case 2:
                                //        str2 = ("successful-handover");
                                //        break;
                                //    case 3:
                                //        str2 = ("release-due-to-eutran-generated-reason");
                                //        break;
                                //    case 4:
                                //        str2 = ("handover-cancelled");
                                //        break;
                                //    case 5:
                                //        str2 = ("partial-handover");
                                //        break;
                                //    case 6:
                                //        str2 = ("ho-failure-in-target-EPC-eNB-or-target-system");
                                //        break;
                                //    case 7:
                                //        str2 = ("ho-target-not-allowed");
                                //        break;
                                //    case 8:
                                //        str2 = ("tS1relocoverall-expiry");
                                //        break;
                                //    case 9:
                                //        str2 = ("tS1relocprep-expiry");
                                //        break;
                                //    case 10:
                                //        str2 = ("cell-not-available");
                                //        break;
                                //    case 11:
                                //        str2 = ("unknown-targetID");
                                //        break;
                                //    case 12:
                                //        str2 = ("no-radio-resources-available-in-target-cell");
                                //        break;
                                //    case 13:
                                //        str2 = ("unknown-mme-ue-s1ap-id");
                                //        break;
                                //    case 14:
                                //        str2 = ("unknown-enb-ue-s1ap-id");
                                //        break;
                                //    case 15:
                                //        str2 = ("unknown-pair-ue-s1ap-id");
                                //        break;
                                //    case 16:
                                //        str2 = ("handover-desirable-for-radio-reason");
                                //        break;
                                //    case 17:
                                //        str2 = ("time-critical-handover");
                                //        break;
                                //    case 18:
                                //        str2 = ("resource-optimisation-handover");
                                //        break;
                                //    case 19:
                                //        str2 = ("reduce-load-in-serving-cell");
                                //        break;
                                //    case 20:
                                //        str2 = ("user-inactivity");
                                //        break;
                                //    case 21:
                                //        str2 = ("radio-connection-with-ue-lost");
                                //        break;
                                //    case 22:
                                //        str2 = ("load-balancing-tau-required");
                                //        break;
                                //    case 23:
                                //        str2 = ("cs-fallback-triggered");
                                //        break;
                                //    case 24:
                                //        str2 = ("ue-not-available-for-ps-service");
                                //        break;
                                //    case 25:
                                //        str2 = ("radio-resources-not-available");
                                //        break;
                                //    case 26:
                                //        str2 = ("failure-in-radio-interface-procedure");
                                //        break;
                                //    case 27:
                                //        str2 = ("invalid-qos-combination");
                                //        break;
                                //    case 28:
                                //        str2 = ("interrat-redirection");
                                //        break;
                                //    case 29:
                                //        str2 = ("interaction-with-other-procedure");
                                //        break;
                                //    case 30:
                                //        str2 = ("unknown-E-RAB-ID");
                                //        break;
                                //    case 31:
                                //        str2 = ("multiple-E-RAB-ID-instances");
                                //        break;
                                //    case 32:
                                //        str2 = ("encryption-and-or-integrity-protection-algorithms-not-supported");
                                //        break;
                                //    case 33:
                                //        str2 = ("s1-intra-system-handover-triggered");
                                //        break;
                                //    case 34:
                                //        str2 = ("s1-inter-system-handover-triggered");
                                //        break;
                                //    case 35:
                                //        str2 = ("x2-handover-triggered");
                                //        break;
                                //    default:
                                //        str2 = ("error");
                                //        break;
                                //}
                                #endregion
                            }

                            if (length2 == 1)//该cause不是Radio Network cause
                            {
                                value = a[i + 4];
                                type = (ushort)(value >> 4);
                                if (type == 1)//transport cause
                                {
                                    #region
                                    str1 = ("CauseTransport\n");
                                    kind = (ushort)(value & 4);
                                    kind = (ushort)(kind >> 2);//kind表示transport cause的具体cause
                                    switch (kind)
                                    {
                                        case 0:
                                            str2 = ("transport-resource-unavailable");
                                            break;
                                        case 1:
                                            str2 = ("unspecified");
                                            break;
                                        default:
                                            str2 = ("error");
                                            break;
                                    }
                                    #endregion
                                }
                                if (type == 2)//nas cause
                                {
                                    #region
                                    str1 = ("CauseNas\n");
                                    kind = (ushort)(value & 6);
                                    kind = (ushort)(kind >> 1);//kind表示nas cause的具体cause
                                    switch (kind)
                                    {
                                        case 0:
                                            str2 = ("normal-release");
                                            break;
                                        case 1:
                                            str2 = ("authentication-failure");
                                            break;
                                        case 2:
                                            str2 = ("detach");
                                            break;
                                        case 3:
                                            str2 = ("unspecified");
                                            break;
                                        default:
                                            str2 = ("error");
                                            break;
                                    }
                                    #endregion
                                }

                                if (type == 3)//protocol cause
                                {
                                    #region
                                    str1 = ("CauseProtocol\n");
                                    kind = (ushort)(value & 7);//kind表示protocol cause的具体cause
                                    switch (kind)
                                    {
                                        case 0:
                                            str2 = ("transfer-syntax-error");
                                            break;
                                        case 1:
                                            str2 = ("abstract-syntax-error-reject");
                                            break;
                                        case 2:
                                            str2 = ("abstract-syntax-error-ignore-and-notify");
                                            break;
                                        case 3:
                                            str2 = ("message-not-compatible-with-receiver-state");
                                            break;
                                        case 4:
                                            str2 = ("semantic-error");
                                            break;
                                        case 5:
                                            str2 = ("abstract-syntax-error-falsely-constructed-message");
                                            break;
                                        case 6:
                                            str2 = ("unspecified");
                                            break;
                                        default:
                                            str2 = ("error");
                                            break;
                                    }
                                    #endregion
                                }
                                if (type == 4)//Misc cause
                                {
                                    #region
                                    str1 = ("CauseMisc\n");
                                    kind = (ushort)(value & 7);//kind表示Misc cause的具体cause
                                    switch (kind)
                                    {
                                        case 0:
                                            str2 = ("control-processing-overload");
                                            break;
                                        case 1:
                                            str2 = ("not-enough-user-plane-processing-resources");
                                            break;
                                        case 2:
                                            str2 = ("hardware-failure");
                                            break;
                                        case 3:
                                            str2 = ("om-intervention");
                                            break;
                                        case 4:
                                            str2 = ("unspecified");
                                            break;
                                        case 5:
                                            str2 = ("unknown-PLMN");
                                            break;
                                        default:
                                            str2 = ("error");
                                            break;
                                    }
                                    #endregion
                                }
                            }
                            #endregion
                        }
                        else
                         i = i + length2 + 4;// length=a[i+3]不变,信元的编码长度不包含type（2字节），criticality（1字节）及本身所占字节(1字节)，需要加4
                    }                  
                   
                }

                if (length1 > 128)//当s1ap信令编码长度大于128
                {
                    #region
                    sum = a[7];
                    for (i = 8; i < a.Length && sum != 0; sum--)//i运行到最后可能小于length，因为编码中可能含有补0的情况
                    {
                        id = (ushort)(a[i] * 256 + a[i + 1]);

                        if (id == 2)
                        {
                            #region
                            length2 = a[i + 3];
                            if (length2 == 2)//该cause是Radio Network cause
                            {
                                str1 = ("CauseRadioNetwork\n");
                                value = (ushort)(a[i + 4] * 256 + a[i + 5]);
                                value = (ushort)(value >> 5);//因为radio network cause表示编码长度有6bit，跨越2字节
                                switch (value)
                                {
                                    case 0:
                                        str2 = ("unspecified");
                                        break;
                                    case 1:
                                        str2 = ("tx2relocoverall-expiry");
                                        break;
                                    case 2:
                                        str2 = ("successful-handover");
                                        break;
                                    case 3:
                                        str2 = ("release-due-to-eutran-generated-reason");
                                        break;
                                    case 4:
                                        str2 = ("handover-cancelled");
                                        break;
                                    case 5:
                                        str2 = ("partial-handover");
                                        break;
                                    case 6:
                                        str2 = ("ho-failure-in-target-EPC-eNB-or-target-system");
                                        break;
                                    case 7:
                                        str2 = ("ho-target-not-allowed");
                                        break;
                                    case 8:
                                        str2 = ("tS1relocoverall-expiry");
                                        break;
                                    case 9:
                                        str2 = ("tS1relocprep-expiry");
                                        break;
                                    case 10:
                                        str2 = ("cell-not-available");
                                        break;
                                    case 11:
                                        str2 = ("unknown-targetID");
                                        break;
                                    case 12:
                                        str2 = ("no-radio-resources-available-in-target-cell");
                                        break;
                                    case 13:
                                        str2 = ("unknown-mme-ue-s1ap-id");
                                        break;
                                    case 14:
                                        str2 = ("unknown-enb-ue-s1ap-id");
                                        break;
                                    case 15:
                                        str2 = ("unknown-pair-ue-s1ap-id");
                                        break;
                                    case 16:
                                        str2 = ("handover-desirable-for-radio-reason");
                                        break;
                                    case 17:
                                        str2 = ("time-critical-handover");
                                        break;
                                    case 18:
                                        str2 = ("resource-optimisation-handover");
                                        break;
                                    case 19:
                                        str2 = ("reduce-load-in-serving-cell");
                                        break;
                                    case 20:
                                        str2 = ("user-inactivity");
                                        break;
                                    case 21:
                                        str2 = ("radio-connection-with-ue-lost");
                                        break;
                                    case 22:
                                        str2 = ("load-balancing-tau-required");
                                        break;
                                    case 23:
                                        str2 = ("cs-fallback-triggered");
                                        break;
                                    case 24:
                                        str2 = ("ue-not-available-for-ps-service");
                                        break;
                                    case 25:
                                        str2 = ("radio-resources-not-available");
                                        break;
                                    case 26:
                                        str2 = ("failure-in-radio-interface-procedure");
                                        break;
                                    case 27:
                                        str2 = ("invalid-qos-combination");
                                        break;
                                    case 28:
                                        str2 = ("interrat-redirection");
                                        break;
                                    case 29:
                                        str2 = ("interaction-with-other-procedure");
                                        break;
                                    case 30:
                                        str2 = ("unknown-E-RAB-ID");
                                        break;
                                    case 31:
                                        str2 = ("multiple-E-RAB-ID-instances");
                                        break;
                                    case 32:
                                        str2 = ("encryption-and-or-integrity-protection-algorithms-not-supported");
                                        break;
                                    case 33:
                                        str2 = ("s1-intra-system-handover-triggered");
                                        break;
                                    case 34:
                                        str2 = ("s1-inter-system-handover-triggered");
                                        break;
                                    case 35:
                                        str2 = ("x2-handover-triggered");
                                        break;
                                    default:
                                        str2 = ("error");
                                        break;
                                }
                            }

                            if (length2 == 1)//该cause不是Radio Network cause
                            {
                                value = a[i + 4];
                                type = (ushort)(value >> 4);
                                if (type == 1)//transport cause
                                {
                                    str1 = ("CauseTransport\n");
                                    kind = (ushort)(value & 4);
                                    kind = (ushort)(kind >> 2);//kind表示transport cause的具体cause
                                    switch (kind)
                                    {
                                        case 0:
                                            str2 = ("transport-resource-unavailable");
                                            break;
                                        case 1:
                                            str2 = ("unspecified");
                                            break;
                                        default:
                                            str2 = ("error");
                                            break;
                                    }
                                }
                                if (type == 2)//nas cause
                                {
                                    str1 = ("CauseNas\n");
                                    kind = (ushort)(value & 6);
                                    kind = (ushort)(kind >> 1);//kind表示nas cause的具体cause
                                    switch (kind)
                                    {
                                        case 0:
                                            str2 = ("normal-release");
                                            break;
                                        case 1:
                                            str2 = ("authentication-failure");
                                            break;
                                        case 2:
                                            str2 = ("detach");
                                            break;
                                        case 3:
                                            str2 = ("unspecified");
                                            break;
                                        default:
                                            str2 = ("error");
                                            break;
                                    }
                                }

                                if (type == 3)//protocol cause
                                {
                                    str1 = ("CauseProtocol\n");
                                    kind = (ushort)(value & 7);//kind表示protocol cause的具体cause
                                    switch (kind)
                                    {
                                        case 0:
                                            str2 = ("transfer-syntax-error");
                                            break;
                                        case 1:
                                            str2 = ("abstract-syntax-error-reject");
                                            break;
                                        case 2:
                                            str2 = ("abstract-syntax-error-ignore-and-notify");
                                            break;
                                        case 3:
                                            str2 = ("message-not-compatible-with-receiver-state");
                                            break;
                                        case 4:
                                            str2 = ("semantic-error");
                                            break;
                                        case 5:
                                            str2 = ("abstract-syntax-error-falsely-constructed-message");
                                            break;
                                        case 6:
                                            str2 = ("unspecified");
                                            break;
                                        default:
                                            str2 = ("error");
                                            break;
                                    }
                                }
                                if (type == 4)//Misc cause
                                {
                                    str1 = ("CauseMisc\n");
                                    kind = (ushort)(value & 7);//kind表示Misc cause的具体cause
                                    switch (kind)
                                    {
                                        case 0:
                                            str2 = ("control-processing-overload");
                                            break;
                                        case 1:
                                            str2 = ("not-enough-user-plane-processing-resources");
                                            break;
                                        case 2:
                                            str2 = ("hardware-failure");
                                            break;
                                        case 3:
                                            str2 = ("om-intervention");
                                            break;
                                        case 4:
                                            str2 = ("unspecified");
                                            break;
                                        case 5:
                                            str2 = ("unknown-PLMN");
                                            break;
                                        default:
                                            str2 = ("error");
                                            break;
                                    }
                                }

                            }
                            #endregion
                        }
                        else 
                        {

                            length2 = a[i + 3];//不管长度域是1字节还是2字节
                            if (length2 < 128)//即长度域只有一个字节
                                i = i + length2 + 4;// length=a[i+3]不变,信元的编码长度不包含type（2字节），criticality（1字节）及本身所占字节(1字节)，需要加4
                            else//表示长度域的有两个字节
                            {
                                length2 = (ushort)((a[i + 3] & 0x0f) * 256 + a[i + 4]);//假设一个信元的编码长度不超过255，则后一个字节表示编码长度
                                i = i + length2 + 5;//由于信元的编码长度不包含type，criticality及本身所占字节（2字节），故需要加5
                            }
                        }
                    }
                    #endregion
                }
            }
        }
    }
}







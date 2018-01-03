using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
namespace MonitorPorts
{
    class CauseType
    {
        /// <summary>
        /// GetHashtable是EMM cause的hash表
        /// </summary>
        /// <returns></returns>
        static Hashtable GetHashtable()
        {
            #region
            Hashtable EmmCauseHash = new Hashtable();
            EmmCauseHash.Add(0x02, "EMM cause: IMSI unknown in HSS");
            EmmCauseHash.Add(0x03, "EMM cause: Illegal UE");
            EmmCauseHash.Add(0x05, "EMM cause: IMEI not accepted");
            EmmCauseHash.Add(0x06, "EMM cause: Illegal ME");
            EmmCauseHash.Add(0x07, "EMM cause: EPS services not NasMessageowed");
            EmmCauseHash.Add(0x08, "EMM cause: EPS services and non-EPS services not NasMessageowed");
            EmmCauseHash.Add(0x09, "EMM cause: UE identity cannot be derived by the network");
            EmmCauseHash.Add(0x0a, "EMM cause: Implicitly detached");
            EmmCauseHash.Add(0x0b, "EMM cause: PLMN not NasMessageowed");
            EmmCauseHash.Add(0x0c, "EMM cause: Tracking Area not NasMessageowed");
            EmmCauseHash.Add(0x0d, "EMM cause: Roaming not NasMessageowed in this tracking area");
            EmmCauseHash.Add(0x0e, "EMM cause: EPS services not NasMessageowed in this PLMN");
            EmmCauseHash.Add(0x0f, "EMM cause: No Suitable Cells In tracking area");
            EmmCauseHash.Add(0x10, "EMM cause: MSC temporarily not reachable");
            EmmCauseHash.Add(0x11, "EMM cause: Network failure");
            EmmCauseHash.Add(0x12, "EMM cause: CS domain not available");
            EmmCauseHash.Add(0x13, "EMM cause: ESM failure");
            EmmCauseHash.Add(0x14, "EMM cause: MAC failure");
            EmmCauseHash.Add(0x15, "EMM cause: Synch failure");
            EmmCauseHash.Add(0x16, "EMM cause: Congestion");
            EmmCauseHash.Add(0x17, "EMM cause: UE security capabilities mismatch");
            EmmCauseHash.Add(0x18, "EMM cause: Security mode rejected, unspecified");
            EmmCauseHash.Add(0x19, "EMM cause: Not authorized for this CSG");
            EmmCauseHash.Add(0x1a, "EMM cause: Non-EPS authentication unacceptable");
            EmmCauseHash.Add(0x27, "EMM cause: CS domain temporarily not available");
            EmmCauseHash.Add(0x28, "EMM cause: No EPS bearer context activated");
            EmmCauseHash.Add(0x5f, "EMM cause: SemanticNasMessagey incorrect message");
            EmmCauseHash.Add(0x60, "EMM cause: Invalid mandatory information");
            EmmCauseHash.Add(0x61, "EMM cause: Message type non-existent or not implemented");
            EmmCauseHash.Add(0x62, "EMM cause: Message type not compatible with the protocol state");
            EmmCauseHash.Add(0x63, "EMM cause: Information element non-existent or not implemented");
            EmmCauseHash.Add(0x64, "EMM cause: Conditional IE error");
            EmmCauseHash.Add(0x65, "EMM cause: Message not compatible with the protocol state");
            EmmCauseHash.Add(0x6f, "EMM cause: Protocol error, unspecified");
            return EmmCauseHash;
            #endregion
        }
        /// <summary>
        /// GetHashtable_1是ESM cause的hash表
        /// </summary>
        /// <returns></returns>
        static Hashtable GetHashtable_1()
        {
            {
                #region
                Hashtable EsmCauseHash = new Hashtable();
                EsmCauseHash.Add(0x08, "ESM cause: Operator Determined Barring");
                EsmCauseHash.Add(0x1a, "ESM cause: Insufficient resources");
                EsmCauseHash.Add(0x1b, "ESM cause: Unknown or missing APN");
                EsmCauseHash.Add(0x1c, "ESM cause: Unknown PDN type");
                EsmCauseHash.Add(0x1d, "ESM cause: User authentication failed");
                EsmCauseHash.Add(0x1e, "ESM cause: Request rejected by Serving GW or PDN GW");
                EsmCauseHash.Add(0x1f, "ESM cause: Request rejected, unspecified");
                EsmCauseHash.Add(0x20, "ESM cause: Service option not supported");
                EsmCauseHash.Add(0x21, "ESM cause: Requested service option not subscribed");
                EsmCauseHash.Add(0x22, "ESM cause: Service option temporarily out of order");
                EsmCauseHash.Add(0x23, "ESM cause: PTI already in use");
                EsmCauseHash.Add(0x24, "ESM cause: Regular deactivation");
                EsmCauseHash.Add(0x25, "ESM cause: EPS QoS not accepted");
                EsmCauseHash.Add(0x26, "ESM cause: Network failure");
                EsmCauseHash.Add(0x27, "ESM cause: Reactivation requested");
                EsmCauseHash.Add(0x29, "ESM cause: Semantic error in the TFT operation");
                EsmCauseHash.Add(0x2a, "ESM cause: Syntactical error in the TFT operation");
                EsmCauseHash.Add(0x2b, "ESM cause: Invalid EPS bearer identity");
                EsmCauseHash.Add(0x2c, "ESM cause: Semantic errors in packet filter(s)");
                EsmCauseHash.Add(0x2d, "ESM cause: Syntactical errors in packet filter(s)");
                EsmCauseHash.Add(0x2e, "ESM cause: EPS bearer context without TFT already activated");
                EsmCauseHash.Add(0x2f, "ESM cause: PTI mismatch");
                EsmCauseHash.Add(0x31, "ESM cause: Last PDN disconnection not NasMessageowed");
                EsmCauseHash.Add(0x32, "ESM cause: PDN type IPv4 only NasMessageowed");
                EsmCauseHash.Add(0x33, "ESM cause: PDN type IPv6 only NasMessageowed");
                EsmCauseHash.Add(0x34, "ESM cause: Single address bearers only NasMessageowed");
                EsmCauseHash.Add(0x35, "ESM cause: ESM information not received");
                EsmCauseHash.Add(0x36, "ESM cause: PDN connection does not exist");
                EsmCauseHash.Add(0x37, "ESM cause: Multiple PDN connections for a given APN not NasMessageowed");
                EsmCauseHash.Add(0x38, "ESM cause: Collision with network initiated request");
                EsmCauseHash.Add(0x3b, "ESM cause: Unsupported QCI value");
                EsmCauseHash.Add(0x51, "ESM cause: Invalid PTI value");
                EsmCauseHash.Add(0x5f, "ESM cause: SemanticNasMessagey incorrect message");
                EsmCauseHash.Add(0x60, "ESM cause: Invalid mandatory information");
                EsmCauseHash.Add(0x61, "ESM cause: Message type non-existent or not implemented");
                EsmCauseHash.Add(0x62, "ESM cause: Message type not compatible with the protocol state");
                EsmCauseHash.Add(0x63, "ESM cause: Information element non-existent or not implemented");
                EsmCauseHash.Add(0x64, "ESM cause: Conditional IE error");
                EsmCauseHash.Add(0x65, "ESM cause: Message not compatible with the protocol state");
                EsmCauseHash.Add(0x6f, "ESM cause: Protocol error, unspecified");
                EsmCauseHash.Add(0x70, "ESM cause: APN restriction value incompatible with active EPS bearer context");
                return EsmCauseHash;
                #endregion
            }
        }
        /// <summary>
        /// 解析Nas中的EMM cause
        /// </summary>
        /// <param name="a">特定的标识字节</param>
        /// <returns>EMM cause</returns>
        public string name(byte a)
        {
            string str = "";//EMM cause
            int b = (int)a;
            Hashtable EmmCauseHash = GetHashtable();
            str = (string)EmmCauseHash[b];//注意hash表的索引值必须为int
            return str;
        }
        /// <summary>
        /// 解析NAS中的ESM cause
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public string name1(byte a)
        {
            string str = "";//ESM cause
            int b = (int)a;
            Hashtable EsmCauseHash = GetHashtable_1();
            str = (string)EsmCauseHash[b];//注意hash表的索引值必须为int
            return str;
        }
    }
}

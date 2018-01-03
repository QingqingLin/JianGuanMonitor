using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
namespace MonitorPorts
{
    class hash
    {
       
        static Hashtable GetHashtable()
        {
            Hashtable nasHash = new Hashtable();
            nasHash.Add(0x41, "Attach request");
            nasHash.Add(0x42, "Attach accept");
            nasHash.Add(0x43, "Attach complete");
            nasHash.Add(0x44, "Attach reject");
            nasHash.Add(0x45, "Detach request");
            nasHash.Add(0x46, "Detach accept");
            nasHash.Add(0x48, "Tracking area update request");
            nasHash.Add(0x49, "Tracking area update accept");
            nasHash.Add(0x4a, "Tracking area update complete");
            nasHash.Add(0x4b, "Tracking area update reject");
            nasHash.Add(0x4c, "Extended service request");
            nasHash.Add(0x4e, "Service reject");
            nasHash.Add(0x50, "GUTI reallocation command");
            nasHash.Add(0x51, "GUTI reallocation complete");
            nasHash.Add(0x52, "Authentication request");
            nasHash.Add(0x53, "Authentication response");
            nasHash.Add(0x54, "Authentication reject");
            nasHash.Add(0x5c, "Authentication failure");
            nasHash.Add(0x55, "Identity request");
            nasHash.Add(0x56, "Identity response");
            nasHash.Add(0x5d, "Security mode command");
            nasHash.Add(0x5e, "Security mode complete");
            nasHash.Add(0x5f, "Security mode reject");
            nasHash.Add(0x60, "EMM status");
            nasHash.Add(0x61, "EMM information");
            nasHash.Add(0x62, "Downlink NAS transport");
            nasHash.Add(0x63, "Uplink NAS transport");
            nasHash.Add(0x64, "CS Service notification");
            nasHash.Add(0x68, "Downlink generic NAS transport");
            nasHash.Add(0x69, "Uplink generic NAS transport");
            nasHash.Add(0xc1, "Activate default EPS bearer context request");
            nasHash.Add(0xc2, "Activate default EPS bearer context accept");
            nasHash.Add(0xc3, "Activate default EPS bearer context reject");
            nasHash.Add(0xc5, "Activate dedicated EPS bearer context request");
            nasHash.Add(0xc6, "Activate dedicated EPS bearer context accept");
            nasHash.Add(0xc7, "Activate dedicated EPS bearer context reject");
            nasHash.Add(0xc9, "Modify EPS bearer context request");
            nasHash.Add(0xca, "Modify EPS bearer context accept");
            nasHash.Add(0xcb, "Modify EPS bearer context reject");
            nasHash.Add(0xcd, "Deactivate EPS bearer context request");
            nasHash.Add(0xce, "Deactivate EPS bearer context accept");
            nasHash.Add(0xd0, "PDN connectivity request");
            nasHash.Add(0xd1, "PDN connectivity reject");
            nasHash.Add(0xd2, "PDN disconnect request");
            nasHash.Add(0xd3, "PDN disconnect reject");
            nasHash.Add(0xd4, "Bearer resource allocation request");
            nasHash.Add(0xd5, "Bearer resource allocation reject");
            nasHash.Add(0xd6, "Bearer resource modification request");
            nasHash.Add(0xd7, "Bearer resource modification reject");
            nasHash.Add(0xd9, "ESM information request");
            nasHash.Add(0xda, "ESM information response");
            nasHash.Add(0xdb, "Notification");
            nasHash.Add(0xe8, "ESM status");
            return nasHash;
        }
        public string str = " ";
        /// <summary>
        /// 该方法根据传入的特征字节返回具体的nas消息
        /// </summary>
        /// <param name="a">传入的特征字节</param>
        /// <returns>nas信令</returns>
        public string name(byte a)
        {
            int b = (int)a;
            Hashtable nasHash = GetHashtable();
            try
            { str = (string)nasHash[b]; }//注意hash表的索引值必须为int}          
            catch (NullReferenceException)
            { str = ""; }           
             return str;        
        }
    }
}

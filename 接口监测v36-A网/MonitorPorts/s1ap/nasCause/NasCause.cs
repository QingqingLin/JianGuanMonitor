using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorPorts
{
    class decodeNasCause
    {
        public int Index = 0;
        public int Length = 0;
        public string causeStr = "";
        /// <summary>
        /// 该方法判定输入的数据是EMM message还是ESM message，分别调用其对应的方法，解析nas cause
        /// </summary>
        /// <param name="NasMessage">nas编码，不包含完整性保护的安全头及加密的安全头</param>
        public void Nas_Cause(byte[] NasMessage)
        {
            if ((byte)(NasMessage[0] & 0x0f) == 0x02)
            {
                Sorts(NasMessage,NasMessage[2]);
            }
            if ((byte)(NasMessage[0] & 0x0f) == 0x07)
            {
                Sorts(NasMessage,NasMessage[1]);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="NasMessage">不包括完整性保护的安全头及加密的安全头的nas编码</param>
        /// <param name="sort">指示信令名称的特定字节在编码中的位置</param>
        public void Sorts(byte[] NasMessage, byte sort)
        {
            EmmMessage type1 = new EmmMessage();
            EsmMessage type2 = new EsmMessage();
            switch (sort)
            {
                case 0x42: type1.ATTACH_ACCEPT(NasMessage);
                    Length = type1.Length;
                    Index = type1.Index;
                    causeStr = type1.cause;
                    break;
                case 0x44: type1.ATTACH_REJECT(NasMessage);
                     Length = type1.Length;
                    Index = type1.Index;
                    causeStr = type1.cause;
                    break;
                case 0x45: type1.DETACH_REQUEST(NasMessage);
                    Length = type1.Length;
                    Index = type1.Index;
                    causeStr = type1.cause;
                    break;
                case 0x5c: type1.AUTHENTICATION_FAILURE(NasMessage);
                     Length = type1.Length;
                    Index = type1.Index;
                    causeStr = type1.cause;
                    break;
                case 0x60: type1.EMM_STATUS(NasMessage);
                     Length = type1.Length;
                    Index = type1.Index;
                    causeStr = type1.cause;
                    break;
                case 0x5f: type1.SECURITY_MODE_REJECT(NasMessage);
                     Length = type1.Length;
                    Index = type1.Index;
                    causeStr = type1.cause;
                    break;
                case 0x4e: type1.SERVICE_REJECT(NasMessage);
                     Length = type1.Length;
                    Index = type1.Index;
                    causeStr = type1.cause;
                    break;
                case 0x49: type1.TRACKING_AREA_UPDATE_ACCEPT(NasMessage);
                     Length = type1.Length;
                    Index = type1.Index;
                    causeStr = type1.cause;
                    break;
                case 0x4b: type1.TRACKING_AREA_UPDATE_REJECT(NasMessage);
                     Length = type1.Length;
                    Index = type1.Index;
                    causeStr = type1.cause;
                    break;
                case 0xc7: type2.Activate_dedicated_EPS_bearer_context_reject(NasMessage);
                     Length = type2.Length;
                    Index = type2.Index;
                    causeStr = type2.cause1;
                    break;
                case 0xc3: type2.Activate_default_EPS_bearer_context_reject(NasMessage);
                     Length = type2.Length;
                    Index = type2.Index;
                    causeStr = type2.cause1;
                    break;
                case 0xc1: type2.Activate_default_EPS_bearer_context_request(NasMessage);
                     Length = type2.Length;
                    Index = type2.Index;
                    causeStr = type2.cause1;
                    break;
                case 0xd5: type2.Bearer_resource_NasMessageocation_reject(NasMessage);
                     Length = type2.Length;
                    Index = type2.Index;
                    causeStr = type2.cause1;
                    break;
                case 0xd7: type2.Bearer_resource_modification_reject(NasMessage);
                     Length = type2.Length;
                    Index = type2.Index;
                    causeStr = type2.cause1;
                    break;
                case 0xd6: type2.Bearer_resource_mondification_request(NasMessage);
                     Length = type2.Length;
                    Index = type2.Index;
                    causeStr = type2.cause1;
                    break;
                case 0xcd: type2.Deactivate_EPS_bearer_context_request(NasMessage);
                     Length = type2.Length;
                    Index = type2.Index;
                    causeStr = type2.cause1;
                    break;
                case 0xe8: type2.ESM_status(NasMessage);
                     Length = type2.Length;
                    Index = type2.Index;
                    causeStr = type2.cause1;
                    break;
                case 0xcb: type2.Modify_EPS_bearer_context_reject(NasMessage);
                     Length = type2.Length;
                    Index = type2.Index;
                    causeStr = type2.cause1;
                    break;
                case 0xd1: type2.PDN_connectivity_reject(NasMessage);
                     Length = type2.Length;
                    Index = type2.Index;
                    causeStr = type2.cause1;
                    break;
                case 0xd3: type2.PDN_disconnect_reject(NasMessage);
                     Length = type2.Length;
                    Index = type2.Index;
                    causeStr = type2.cause1;
                    break;
            }
        }
    }



}

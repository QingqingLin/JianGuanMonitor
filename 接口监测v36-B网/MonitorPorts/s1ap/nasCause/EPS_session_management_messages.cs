using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorPorts
{
    /// <summary>
    /// 该方法用于解析ESM信令中的nas cause,具体做法见TS24.301
    /// </summary>
    class EsmMessage
    {
        public int Index = 0;
        public int Length = 0;
        public string cause1 = "";
        #region EPS_session_management_messages的具体的每一种type，解出ESM cause
        CauseType c = new CauseType();
        public void Activate_dedicated_EPS_bearer_context_reject(byte[] nas)
        {
            byte o = nas[3];
            Index = 3;
            Length = 1;
            cause1=c.name1(o);
        }
        public void Activate_default_EPS_bearer_context_reject(byte[] nas)
        {
            byte o = nas[3];
            Index = 3;
            Length = 1;
            cause1 = c.name1(o);
        }
        public void Activate_default_EPS_bearer_context_request(byte[] nas)
        {
            int length = nas.Count();
            nas = nas.Skip(3).ToArray();
            length = length - 3;
            byte Qos = nas[0];
            nas = nas.Skip(Qos + 1).ToArray();
            length = length - Convert.ToInt16(Qos) - 1;
            byte Access = nas[0];
            nas = nas.Skip(Access + 1).ToArray();
            length = length - Convert.ToInt16(Access) - 1;
            byte PDN = nas[0];
            nas = nas.Skip(PDN + 1).ToArray();
            length = length - Convert.ToInt16(PDN) - 1;
            for (int i = 0; i <length; )
            {
                byte type = nas[i];
                if (type == 0x5d)
                {
                    byte len = nas[i + 1];
                    i = i + Convert.ToInt16(len) + 2;
                }
                else if (type == 0x30)
                {
                    byte len = nas[i + 1];
                    i = i + Convert.ToInt16(len) + 2;
                }
                else if (type == 0x32)
                {
                    i = i + 2;
                }
                else if ((byte)(nas[i] & 0xf0) == 0x80)
                {
                    i = i + 1;
                }
                else if (type == 0x34)
                {
                    i = i + 3;
                }
                else if (type == 0x5e)
                {
                    byte len = nas[i + 1];
                    i = i + Convert.ToInt16(len) + 2;
                }
                else if (type == 0x58)
                {
                    Index = i+1+3+Qos+1+Access +1+PDN +1;
                    Length = 1;
                    byte o = nas[i + 1];
                    cause1 = c.name1(o);
                    break;
                }
                else if (type == 0x27)
                {
                    byte len = nas[i + 1];
                    i = i + Convert.ToInt16(len) + 2;
                }
                else
                    break;
            }
        }
        public void Bearer_resource_NasMessageocation_reject(byte[] nas)
        {
            byte o = nas[3];
            Index = 3;
            Length = 1;
            cause1 = c.name1(o);
        }
        public void Bearer_resource_modification_reject(byte[] nas)
        {
            byte o = nas[3];
            Index = 3;
            Length = 1;
            cause1 = c.name1(o);
        }
        public void Bearer_resource_mondification_request(byte[] nas)
        {
            int length = nas.Count();
            nas = nas.Skip(4).ToArray();
            length = length - 4;
            byte traffic = nas[0];
            nas = nas.Skip(traffic + 1).ToArray();
            length = length - Convert.ToInt16(traffic) - 1;
            for (int i = 0; i < length; )
            {
                byte type = nas[i];
                if (type == 0x5b)
                {
                    byte len = nas[i + 1];
                    i = i + Convert.ToInt16(len) + 2;
                }
                else if (type == 0x58)
                {
                    Index = i+1+4+traffic+1;
                    Length = 1;
                    byte o = nas[i + 1];
                    cause1 = c.name1(o);
                    break;
                }
                else if (type == 0x27)
                {
                    byte len = nas[i + 1];
                    i = i + Convert.ToInt16(len) + 2;
                }
                else
                    break;
            }
        }
        public void Deactivate_EPS_bearer_context_request(byte[] nas)
        {
            byte o = nas[3];
            Index = 3;
            Length = 1;
            cause1 = c.name1(o);
        }
        public void ESM_status(byte[] nas)
        {
            byte o = nas[3];
            Index = 3;
            Length = 1;
            cause1 = c.name1(o);
        }
        public void Modify_EPS_bearer_context_reject(byte[] nas)
        {
            byte o = nas[3];
            Index = 3;
            Length = 1;
            cause1 = c.name1(o);
        }
        public void PDN_connectivity_reject(byte[] nas)
        {
            byte o = nas[3];
            Index = 3;
            Length = 1;
            cause1 = c.name1(o);
        }
        public void PDN_disconnect_reject(byte[] nas)
        {
            byte o = nas[3];
            Index = 3;
            Length = 1;
            cause1 = c.name1(o);
        }
        #endregion

    }
}

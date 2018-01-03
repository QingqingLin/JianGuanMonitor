using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorPorts
{
    /// <summary>
    /// 该类中的方法用于解析特定EMM信令中的cause,具体的解法见TS24.301
    /// </summary>
    class EmmMessage
    {
        public int Index = 0;
        public int Length = 0;

        public string cause = "";
        #region EmmMessage的具体的每一种type，解出EMM cause
        CauseType c = new CauseType();
        public void ATTACH_ACCEPT(byte[] nas)
        {
            int length = nas.Count();
            nas = nas.Skip(4).ToArray();
            length = length - 4;
            byte TAI = nas[0];
            nas = nas.Skip(TAI + 1).ToArray();
            length = length - Convert.ToInt16(TAI) - 1;
            int container = nas[0] * 256 + nas[1];
            nas = nas.Skip(container + 2).ToArray();
            length = length - Convert.ToInt16(container) - 2;
            for (int i = 0; i < length; )
            {
                byte type = nas[i];
                if (type == 0x50)
                {
                    i = i + 13;
                }
                else if (type == 0x13)
                {
                    i = i + 6;
                }
                else if (type == 0x23)
                {
                    byte len = nas[i + 1];
                    i = i + Convert.ToInt16(len) + 2;
                }
                else if (type == 0x53)
                {
                    //Console.WriteLine("Element ID: 0x58");
                    byte o = nas[i + 1];
                    Index = i + 1+7+TAI+container ;
                    Length = 1;
                    cause = c.name(o);
                    break;
                }
                else if (type == 0x17)
                {
                    i = i + 2;
                }
                else if (type == 0x59)
                {
                    i = i + 2;
                }
                else if (type == 0x4a)
                {
                    byte len = nas[i + 1];
                    i = i + Convert.ToInt16(len) + 2;
                }
                else if (type == 0x34)
                {
                    byte len = nas[i + 1];
                    i = i + Convert.ToInt16(len) + 2;
                }
                else if (type == 0x64)
                {
                    i = i + 3;
                }
                else if ((byte)(nas[i] & 0xf0) == 0xf0)
                {
                    i = i + 1;
                }
                else
                    break;
            }
        }
        public void ATTACH_REJECT(byte[] nas)
        {
            byte o = nas[2];
            Index = 2;
            Length = 1;
            cause = c.name(o);
        }
        public void AUTHENTICATION_FAILURE(byte[] nas)
        {
            byte o = nas[2];
            Index = 2;
            Length = 1;
            cause = c.name(o);
        }
        public void EMM_STATUS(byte[] nas)
        {
            byte o = nas[2];
            Index = 2;
            Length = 1;
            cause = c.name(o);
        }
        public void SECURITY_MODE_REJECT(byte[] nas)
        {
            byte o = nas[2];
            Index = 2;
            Length = 1;
            cause = c.name(o);
        }
        public void SERVICE_REJECT(byte[] nas)
        {
            byte o = nas[2];
            Index = 2;
            Length = 1;
            cause = c.name(o);
        }
        public void TRACKING_AREA_UPDATE_ACCEPT(byte[] nas)
        {
            int length = nas.Count();
            nas = nas.Skip(3).ToArray();
            length = length - 3;
            for (int i = 0; i < length; )
            {
                byte type = nas[i];
                if (type == 0x5a)
                {
                    i = i + 2;
                }
                else if (type == 0x50)
                {
                    i = i + 13;
                }
                else if (type == 0x54)
                {
                    byte len = nas[i + 1];
                    i = i + Convert.ToInt16(len) + 2;
                }
                else if (type == 0x57)
                {
                    i = i + 13;
                }
                else if (type == 0x13)
                {
                    i = i + 6;
                }
                else if (type == 0x23)
                {
                    byte len =nas[i + 1];
                    i = i + Convert.ToInt16(len) + 2;
                }
                else if (type == 0x53)
                {                    
                    byte o = nas[i + 1];
                    Index = i + 1+3;
                    Length = 1;
                    cause = c.name(o);
                    break;
                }
                else if (type == 0x17)
                {
                    i = i + 6;
                }
                else if (type == 0x59)
                {
                    i = i + 2;
                }
                else if (type == 0x4a)
                {
                    byte len = nas[i + 1];
                    i = i + Convert.ToInt16(len) + 2;
                }
                else if (type == 0x34)
                {
                    byte len = nas[i + 1];
                    i = i + Convert.ToInt16(len) + 2;
                }
                else if (type == 0x64)
                {
                    i = i + 3;
                }
                else if ((byte)(nas[i] & 0xf0) == 0xf0)
                {
                    i = i + 1;
                }
                else
                    break;
            }
        }
        public void TRACKING_AREA_UPDATE_REJECT(byte[] nas)
        {
            byte o = nas[2];
            Index = 2;
            Length = 1;
            cause = c.name(o);
        }
        public void DETACH_REQUEST(byte[] nas)
        {
            if (nas.Length < 4)
                cause = "";
            else
            {
                byte type = nas[3];
                if (type == 0x53)
                {
                    byte o = nas[4];
                    cause = c.name(o);
                    Index = 4;
                    Length = 1;
                }
                else
                    cause = "";
            }
        }
        #endregion
    }
}

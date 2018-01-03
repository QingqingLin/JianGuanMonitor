using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTP_V2Decoder
{
    public struct gtpStruct
    {
        public int ID;
        public double time;                 //信令时间
        public byte[] gtpFrame;             //信令数据帧，从ip层开始
        public string messageType;         //消息类型
        public string faceType;            //接口类型
        public string timeAll;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTP_V2Decoder
{
    class S10Message
    {
        public static string[] s10 ={"Identification Request","Identification Response","Context Request","Context Response",
                                    "Context Acknowledge","Forward Relocation Request","Forward Relocation Response","Forward Relocation Complete Notification",
                                    "Forward Relocation Complete Acknowledge","Forward Access Context Notification","Forward Access Context Acknowledge","Relocation Cancel Request",
                                    "Relocation Cancel Response","Configuration Transfer Tunnel","RAN Information Relay"};
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonitorPorts
{
    class ShowThread
    {
        public static List<PacketProperties> ShowStorate;
        public delegate void PacketArrivedEventHandler(PacketProperties Properties);
        public static event PacketArrivedEventHandler PacketArrival;

        public static List<PacketProperties> GUIStorage1 = new List<PacketProperties>();
        public static List<PacketProperties> GUIStorage2 = new List<PacketProperties>();

        public static void Show(object o)
        {
            if (ShowStorate == GUIStorage1)
            {
                ShowStorate = GUIStorage2;
                OutToForm(GUIStorage1);
            }
            else if (ShowStorate == GUIStorage2)
            {
                ShowStorate = GUIStorage1;
                OutToForm(GUIStorage2);
            }
        }

        private static void OutToForm(List<PacketProperties> gUIStorage1)
        {
            try
            {
                foreach (var Properties in gUIStorage1)
                {
                    if (PacketArrival != null)
                    {
                        PacketArrival(Properties);
                    }
                }
            }
            catch (Exception)
            {
            }
            gUIStorage1.Clear();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace MonitorPorts
{
    class SnifferService
    {
        private RawSocket[] Sniffers;

        public SnifferService(MainForm Main)
        {
            string[] IPList = GetLocalIPList();
            Sniffers = new RawSocket[1];
            for (int i = 0; i < 1; i++)
            {
                try
                {
                    Sniffers[i] = new RawSocket(Main);
                    try
                    {

                        Sniffers[i].CreateAndBindSocket(IPList[0]);
                    }
                    catch { }

                    Sniffers[i].PacketArrival += new RawSocket.PacketArrivedEventHandler(SnifferServer_PacketArrival);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("适配器" + IPList[i] + "上的监听启动失败:" + ex.Message);
                }
            }

        }

        #region   Events
        public delegate void PacketArrivedEventHandler(Object sender, RawSocket.PacketArrivedEventArgs args);
        public event PacketArrivedEventHandler PacketArrival;
        #endregion


        void SnifferServer_PacketArrival(object sender, RawSocket.PacketArrivedEventArgs args)
        {
            if (PacketArrival != null)
            {
                PacketArrival(this, args);
            }
        }

        #region 外部接口
        public void Start()
        {
            foreach (RawSocket Sniffer in Sniffers)
            {
                Sniffer.Start();
            }
        }

        public void Stop()
        {
            foreach (RawSocket Sniffer in Sniffers)
            {
                Sniffer.Stop();
            }
        }
        #endregion 

        private string[] GetLocalIPList()
        {
            string HostName = Dns.GetHostName();
            IPHostEntry IPEntry = Dns.GetHostEntry(HostName);
            IPAddress[] IPList = IPEntry.AddressList;//获取或设置与主机关联的IP地址列表

            System.Collections.ArrayList LocalIPList = new System.Collections.ArrayList();
            for (int i = 0; i < IPList.Length; i++)
            {
                if (IPList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    LocalIPList.Add(IPList[i].ToString());
                }
            }
            return (string[])LocalIPList.ToArray(typeof(string));
        }
    }
}

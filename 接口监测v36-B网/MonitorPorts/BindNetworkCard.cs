using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace MonitorPorts
{
    class BindNetworkCard
    {
        public Socket socket;
        public BindNetworkCard()
        {
            string[] LocalNetwork = GetLocalIPList();
            if (LocalNetwork.Length == 1)
            {
                Bind(LocalNetwork[0]);
            }
            else
            {
                Warning Warn = new Warning(LocalNetwork);
                if (Warn.ShowDialog() == DialogResult.OK)
                {
                    Bind(Warning.SelectedNetCard);
                }
            }
        }

        private void Bind(string localNetwork)
        {
            try
            {
                CreateAndBindSocket(localNetwork);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("适配器" + localNetwork + "上的监听启动失败:" + ex.Message);
            }
        }

        public void CreateAndBindSocket(string ip)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
            socket.Blocking = false;
            socket.Bind(new IPEndPoint(IPAddress.Parse(ip), 0));
            SetSocketOption();
        }

        private void SetSocketOption()
        {
            try
            {
                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, 1);
                byte[] inValue = new byte[4] { 1, 0, 0, 0 };
                byte[] outValue = new byte[4];
                int ioControlCode = unchecked((int)0x98000001);
                int returnCode = socket.IOControl(ioControlCode, inValue, outValue);
                returnCode = outValue[0] + outValue[1] + outValue[2] + outValue[3];
                if (returnCode != 0)
                {
                    throw new SnifferSocketException("command excute error!");
                }
            }
            catch (SocketException ex)
            {
                throw new SnifferSocketException("socket error!", ex);
            }
        }

        private string[] GetLocalIPList()
        {
            string HostName = Dns.GetHostName();
            IPHostEntry IPEntry = Dns.GetHostEntry(HostName);
            IPAddress[] IPList = IPEntry.AddressList;
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

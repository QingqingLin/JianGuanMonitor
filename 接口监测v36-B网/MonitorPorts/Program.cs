using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MonitorPorts
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            HardwareInfo hardwareInfo = new HardwareInfo();
            string cpuID = hardwareInfo.GetCpuID();
            string HardId = hardwareInfo.GetHardDiskID();
            if (cpuID == "BFEBFBFF000406F1" && HardId == "Z6T946DAS")
            {
                Application.Run(new MainForm());
            }
            else
            {
                MessageBox.Show("非本机，不能使用！");
                Application.Exit();
            }
}
    }
}

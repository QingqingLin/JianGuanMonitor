using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace MonitorWatcher
{
    class Communication
    {
        public delegate void IconEventHandler();
        public event IconEventHandler Icon;


        private UdpClient udpReceiver;
        private UdpClient udpSender;


        //定义接收和发送端口号
        private int remotePort = 12570;
        private int localPort = 12580;

        private byte[] sendBuffer = new byte[1024];
        private byte[] receiveBuffer = new byte[2048];
        private string strSend;

        //用于判定程序是否接收到退出消息
        private char bufferOne = '1';
        private char bufferTwo = '1';
        private char[] bufferChar;

        //读取设置文件中的内容并保存
        private int plusSeconds;

        //定义各个IPEndPoint
        private IPEndPoint localEndPoint;
        private IPEndPoint remoteEndPoint;
        private EndPoint senderRemote;

        private bool _bRestart;    //定义标志位，是否重新启动
        private bool _bConnect;     //定义标志位，检测是否收到对方消息    
        private bool _bIsRunning;   //定义标志位，监控被监测程序是否正在运行状态     
        private DateTime _receiveTime;

        #region//属性
        public UdpClient UdpSender
        {
            get { return udpSender; }
            set { udpSender = value; }
        }
        public bool BIsRunning
        {
            get { return _bIsRunning; }
            set { _bIsRunning = value; }
        }
        public bool BConnect
        {
            get { return _bConnect; }
            set { _bConnect = value; }
        }
        public bool BRestart
        {
            get { return _bRestart; }
            set { _bRestart = value; }
        }

        public UdpClient SocketProtecter
        {
            get { return udpReceiver; }
            set { udpReceiver = value; }
        }

        public string StrSend
        {
            get { return strSend; }
            set { strSend = value; }
        }
        public DateTime ReceiveTime
        {
            get { return _receiveTime; }
            set { _receiveTime = value; }
        }
        #endregion


        public Communication()
        {
            this.strSend = "轨道交通控制与安全国家重点实验室";        //发送任意字符串
            this.plusSeconds = 10;

            ReceiveTime = DateTime.Now;
            BRestart = false;                                //重启标志位置否
            this.BConnect = true;
            this.BIsRunning = false;
            this.BindPort();


        }
        public void BindPort()
        {
            IPHostEntry localIpEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress localIpAddr = null;
            foreach (IPAddress ip in localIpEntry.AddressList)              //得到本地IP地址
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    localIpAddr = ip;
                    break;
                }
            }
            localEndPoint = new IPEndPoint(localIpAddr, localPort);     //接收端口号
            remoteEndPoint = new IPEndPoint(localIpAddr, remotePort);   //发送给原程序端口号
            senderRemote = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));
            udpReceiver = new UdpClient(localEndPoint);
            udpSender = new UdpClient();
           
        }

        public void Send()
        {
            while (true)
            {
                Thread.Sleep(4000);             //每隔4秒发送一次          
                char[] bufferC = strSend.ToCharArray();
                byte[] buffer = Encoding.Default.GetBytes(bufferC);
                Array.Copy(buffer, sendBuffer, buffer.Length);      //将发送的字符串转为byte数组，并拷贝到sendBuffer中，为了确保每次发送长度为1024位
                try
                {
                    udpSender.Send(sendBuffer,sendBuffer.Length,remoteEndPoint);       //发送数据 sendBuffer
                   
                }
                catch
                {
                    Thread.Sleep(1000);
                }
            }
        }

       

        public void Receive()
        {
            while (true)
            {
                try
                {
                    receiveBuffer = udpReceiver.Receive(ref localEndPoint);
                    if (receiveBuffer != null)
                    {
                        ReceiveTime = DateTime.Now;
                        BIsRunning = true;
                        this.BConnect = true;

                        bufferChar = Encoding.Default.GetChars(receiveBuffer);
                        bufferOne = bufferTwo;
                        bufferTwo = bufferChar[0];
                        if (bufferOne == bufferTwo && bufferTwo == '0')
                        {
                            System.Environment.Exit(0);              //连续收到两个0字符则退出程序

                        }
                        if (BRestart)
                        {
                            this.OnIconed();
                            this.BRestart = false;
                        }
                    }
                }
                catch
                {
                    Thread.Sleep(1000);             //如果没有收到消息，就休眠一秒，之后再次接受                   
                }
                Array.Clear(receiveBuffer, 0, receiveBuffer.Length);
            }
        }
        /// <summary>
        /// 记录重启次数和重启时间
        /// </summary>
        public void Record()
        {
            string strPath = "RestartRecord.txt";
            string strWrite = "软件重启：" + DateTime.Now.ToString() + "\r\n";
            byte[] bufferWrite = System.Text.Encoding.UTF8.GetBytes(strWrite);
            FileStream fs = new FileStream(strPath, FileMode.Append, FileAccess.Write);
            fs.Write(bufferWrite, 0, bufferWrite.Length);
            fs.Flush();
            fs.Close();
            fs.Dispose();
        }


        public void Check()
        {
            while (true)
            {
                Thread.Sleep(2000);     //每隔两秒判定一次
                if (BIsRunning)
                {
                    if (this.ReceiveTime.AddSeconds(this.plusSeconds) < DateTime.Now)
                    {
                        this.BIsRunning = false;
                        this.BConnect = false;
                        Restart();
                        BRestart = true;
                    }
                }
            }
        }


        /// <summary>
        /// 重启程序
        /// </summary>
        private void Restart()
        {
            this.OnIconed();
           

            Process[] myProcess = Process.GetProcesses();
            for (int i = 0; i < myProcess.Length; i++)
            {
                if (myProcess[i].ProcessName.StartsWith("MonitorPorts"))
                {
                    myProcess[i].Kill();
                    myProcess[i].Close();
                    myProcess[i].Dispose();
                }

            }
            Thread.Sleep(5000);
            Process myNewProcess = new Process();
            myNewProcess.StartInfo.FileName = "MonitorPortsBackUp.exe";
            myNewProcess.StartInfo.WorkingDirectory = Application.StartupPath;
            myNewProcess.Start();

            this.ReceiveTime = DateTime.Now;        //更新接收时间
            this.Record();                          //记录当前时间
        }

        protected virtual void OnIconed()
        {
            if (Icon != null)
            {
                this.Icon();
            }
        }

    }
}

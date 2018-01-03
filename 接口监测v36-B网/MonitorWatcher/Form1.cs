using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace MonitorWatcher
{
    public partial class Form1 : Form
    {
        Thread thReceive;        //定义三个线程和一个Communication类
        Thread thCheck;
        Thread thSend;

        Communication comm;
        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Minimized;
            comm = new Communication();       //实例化Communication类
            comm.Icon += this.ChangeIcon;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            thReceive = new Thread(comm.Receive);     //开线程接收数据
            thReceive.Start();
            thReceive.IsBackground = true;

            thSend = new Thread(comm.Send);     //开线程开始发送数据
            thSend.Start();
            thSend.IsBackground = true;

            thCheck = new Thread(comm.Check);        //开线程判断接收数据是否超时
            thCheck.Start();
            thCheck.IsBackground = true;

           
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            thReceive.Abort();       //终止接收线程、发送线程和判定线程
            thSend.Abort();
            thCheck.Abort();
            
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            
            this.Dispose();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.notifyIcon1.Visible = true;
                
            }
        }

       

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            this.notifyIcon1.BalloonTipText = "MonitorWatcher";
            this.notifyIcon1.ShowBalloonTip(1000);
        }

        private void ChangeIcon()
        {
            if(comm.BConnect==false)
            {
                this.notifyIcon1.Icon = Properties.Resources.stop;
                this.notifyIcon1.BalloonTipText = "软件重启中...";
                this.notifyIcon1.ShowBalloonTip(3000);
            }
            else
            {
                this.notifyIcon1.Icon = Properties.Resources.start;
                this.notifyIcon1.BalloonTipText = "重启完成";
                this.notifyIcon1.ShowBalloonTip(1000);
            }
        }
    }
}

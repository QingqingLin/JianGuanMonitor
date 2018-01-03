using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MonitorPorts
{
    public partial class FilterForm : Form
    {
        public static string pathDll=Application.StartupPath;
        #region 全局变量
        //Edit by Kuang
        public bool CheckBoxStatues_CBTC = false;
        public bool CheckBoxStatues_Signal = false;  //标记选择的是那一界面，即选择的是信令监测、信号监测

        //传递参数
        public bool CheckBoxStatues_CBTC1 = false;
        public bool CheckBoxStatues_Signal1 = false;
        public bool FilterState = false; //表示进行过该窗口的选择

        //辅助参数，避免同时增加多个page
        public bool StatuesFlag_CBTC = false;
        public bool StatuesFlag_Signal = false;

        // 三种协议
        public bool CheckBoxStatues_GTPv2 = false;
        public bool CheckBoxStatues_DIAMETER = false;
        public bool CheckBoxStatues_S1AP = false;

        // 传递参数
        public bool CheckBoxStatues_GTPv21 = false;
        public bool CheckBoxStatues_DIAMETER1 = false;
        public bool CheckBoxStatues_S1AP1 = false;
        //选项
        public int MainFormCount = 0;
        public int Slb = 0, Dlb = 0;// Alb = 0;



        public string Protocol;
        public string SourceIP_Port;
        public string SourceIP;
        public string SourcePort;
        public string DestIP_Port;
        public string DestIP;
        public string DestPort;
        //public string maxIntervalVtoZ;
        //public string maxIntervalZtoV;

        //public List<string> listSourIP;
        //public List<string> listSourPort;
        //public List<string> listDestIP;
        //public List<string> listDestPort;
        /// <summary>
        /// dic<IP 名称，IP-端口号>
        /// </summary>
        public Dictionary<string, string> dic;
        public Dictionary<string, string> dicIP = new Dictionary<string, string>();
        //variate_signal
        public static double PcapLengthNum;
        public static double PcapLengthLte;//信令报文保存的最大值
        public double MaxVOBCToZC;
        public double MaxVOBCToCI;
        public double MaxVOBCToATS;
        public double MaxATSToVOBC;
        public double MaxCIToVOBC;
        public double MaxZCToVOBC;
        private List<string> BackUps = new List<string>();
        #endregion

        public FilterForm()
        {
            InitializeComponent();
            SourceListBox1.CheckOnClick = true;
            DestListBox2.CheckOnClick = true;
            //CBTC
            Protocol = "GTPv2,DIAMETER,S1AP,";
            SourceIP = "";
            SourceIP_Port = "";
            SourcePort = "";

            DestIP = "";
            DestIP_Port = "";
            DestPort = "";

            MaxVOBCToATS = MaxVOBCToZC = MaxVOBCToCI = MaxATSToVOBC = MaxCIToVOBC = MaxZCToVOBC = 2.4;
            IPList ReadIP = new IPList();
            LoadIpConfiguration();
            LoadHistoryConfig();
        }

        private void LoadHistoryConfig()
        {
            try                                               //lishuai
            {
                StreamReader sr = new StreamReader("BackUps.txt", Encoding.Default);
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    BackUps.Add(line);
                }
                sr.Close();
                if (BackUps.Count != 0)
                {
                    LoadHistory(BackUps);
                }
            }
            catch (Exception e)
            {

                throw new Exception("打开文件BackUps.txt出错！", e);
            }
        }

        private void LoadHistory(List<string> backUps)
        {
            if (backUps[0] != "" && backUps[0] != null)
            {
                this.txtPcapLength.Text = backUps[0];
            }
            this.txtVOBCToZC.Text = backUps[4];
            this.txtZCToVOBC.Text = backUps[5];
            this.txtVOBCToCI.Text = backUps[6];
            this.txtCIToVOBC.Text = backUps[7];
            this.txtVOBCToATS.Text = backUps[8];
            this.txtATSToVOBC.Text = backUps[9];
        }

        public void LoadIpConfiguration()
        {
            for (int i = 1; i < IPList.ConfigProperties.Count; i++)
            {
                SourceListBox1.Items.Add(IPList.ConfigProperties[i].Name);
                DestListBox2.Items.Add(IPList.ConfigProperties[i].Name);
            }
        }

        #region 跨窗体,解决panel显示的问题
        // Edit by Kuang
        public FilterForm(MainForm f1):this()
       {
        this.f1=f1;
       }
        MainForm f1;

        // 跨窗体，解决向MainForm传送按钮的状态
        public bool Form2Value_CBTC
        {
            get
            {
                return this.CheckBoxStatues_CBTC1;
            }
            set
            {
                this.CheckBoxStatues_CBTC1 = value;
            }
        }

        public bool Form2Value_Signal
        {
            get
            {
                return this.CheckBoxStatues_Signal1;
          
            }
            set
            {
                this.CheckBoxStatues_Signal1 = value;
            
            }
        }


        public bool Form2Value_GTPv2
        {
            get
            {
                return this.CheckBoxStatues_GTPv21;

            }
            set
            {
                this.CheckBoxStatues_GTPv21 = value;

            }
        }

        public bool Form2Value_Diameter
        {
            get
            {
                return this.CheckBoxStatues_DIAMETER1;

            }
            set
            {
                this.CheckBoxStatues_DIAMETER1 = value;

            }
        }

        public bool Form2Value_S1AP
        {
            get
            {
                return this.CheckBoxStatues_S1AP1;

            }
            set
            {
                this.CheckBoxStatues_S1AP1 = value;

            }
        }

        public bool Form2Value_FilterState
        {
            get
            {
                return this.FilterState;

            }
            set
            {
                this.FilterState = value;

            }
        }

        public event EventHandler accept;

        #endregion
        #region 窗体控件 
        #region CBTC
        ////信号监测
        //private void button_CBTC_OK_Click(object sender, EventArgs e)
        //{
        //    SaveSettings();
        //    this.Hide();         
        //    checkBox__CBTC.CheckState = CheckState.Unchecked;
        //    splitContainer1.Panel2.Enabled = true; //恢复   

        //    if (listSourIP.Count != 0)
        //    {
        //        for (int iS = 0; iS <= listSourIP.Count() - 1; iS++)
        //        {
        //            SourceIP_Port += listSourIP[iS] + "-" + listSourPort[iS] + ";";
        //        }
        //    }

        //    //保存并显示目的IP和端口
        //    if (listDestIP.Count != 0)
        //    {
        //        for (int iD = 0; iD <= listDestIP.Count() - 1; iD++)
        //        {
        //            DestIP_Port += listDestIP[iD] + "-" + listDestPort[iD] + ";";

        //        }
        //    }

        //    maxIntervalVtoZ = textBox1.Text;
        //    maxIntervalZtoV = textBox2.Text;

        //    FileStream fs = new FileStream("a.txt", FileMode.Create, FileAccess.Write);
        //    StreamWriter sw = new StreamWriter(fs);
        //    //开始写入
        //    sw.WriteLine(Protocol);
        //    sw.WriteLine(SourceIP_Port);
        //    sw.WriteLine(DestIP_Port);
        //    sw.WriteLine(maxIntervalVtoZ);
        //    sw.WriteLine(maxIntervalZtoV);
        //    //清空缓冲区
        //    sw.Flush();
        //    //关闭流
        //    sw.Close();
        //    fs.Close();
        //}

        //private void button_CBTC_Cancel_Click(object sender, EventArgs e)
        //{
        //   // LoadSettings();
        //    this.Hide();
        //    checkBox__CBTC.CheckState = CheckState.Unchecked;
        //    splitContainer1.Panel2.Enabled = true; //恢复  
        //}
        #endregion

       


      #region  信令监测
        private void button_Signal_OK_Click(object sender, EventArgs e)
        {
            CheckBoxStatues_CBTC1 = checkBox__CBTC.Checked;
            CheckBoxStatues_Signal1 = checkBox_Signal.Checked;

            //协议
            CheckBoxStatues_GTPv21 = CheckBoxStatues_GTPv2;
            CheckBoxStatues_DIAMETER1 = CheckBoxStatues_DIAMETER;
            CheckBoxStatues_S1AP1 = CheckBoxStatues_S1AP;

            #region  //Edit By K


            for (int i = 0; i < SourceListBox1.Items.Count; i++)
            {
                if (SourceListBox1.GetItemChecked(i))
                {
                    Slb++;  

                }
            }

            for (int j = 0; j < DestListBox2.Items.Count; j++)
            {
                if (DestListBox2.GetItemChecked(j))
                {
                    Dlb++;
                }
            }



            #endregion


            if ((CheckBoxStatues_CBTC1 && (Slb >=1) && (Dlb >=1)) || (!CheckBoxStatues_CBTC1 && CheckBoxStatues_Signal1))
            {
                FilterState = true;
            }
            


            if (accept != null)
            {
                accept(this, EventArgs.Empty); //当窗体触发事件，传递自身引用 
            }

            //signal
            GetAlarmInterval();
            SetPcapLength();
            SetPcapLengthLte();
            SaveSetting();
            SaveToTxt();

            this.Hide();
            SaveConfigToTxt();

        //    CheckBoxStatues_CBTC = false;
           // CheckBoxStatues_Signal = false;

            //协议
            CheckBoxStatues_GTPv21 = false;
            CheckBoxStatues_DIAMETER1 = false;
            CheckBoxStatues_S1AP1 = false;
            Slb = 0;
            Dlb = 0;
            //Alb = 0;
        }

        private void SaveConfigToTxt()
        {
            FileStream fs = new FileStream("HistoryConfig.txt", FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);

            if (checkBox__CBTC.Checked)
            {
                sw.WriteLine("true");
            }
            else
            {
                sw.WriteLine("false");
            }


            sw.WriteLine(this.txtPcapLength.Text);
            sw.WriteLine("MB");

            string Source = "";
            string Dest = "";
            for (int i = 0; i < SourceListBox1.Items.Count; i++)
            {
                if (SourceListBox1.GetItemChecked(i))
                {
                    Source += "#" + SourceListBox1.Items[i].ToString();
                }
            }
            for (int i = 0; i < DestListBox2.Items.Count; i++)
            {
                if (DestListBox2.GetItemChecked(i))
                {
                    Dest += "#" + DestListBox2.Items[i].ToString();
                }
            }

            sw.WriteLine(Source);
            sw.WriteLine(Dest);

            sw.WriteLine(MaxVOBCToZC * 1000);
            sw.WriteLine(MaxZCToVOBC * 1000);

            sw.WriteLine(MaxVOBCToCI * 1000);
            sw.WriteLine(MaxCIToVOBC * 1000);

            sw.WriteLine(MaxVOBCToATS * 1000);
            sw.WriteLine(MaxATSToVOBC * 1000);

            if (checkBox_Signal.Checked)
            {
                sw.WriteLine("true");
            }
            else
            {
                sw.WriteLine("false");
            }

            sw.WriteLine(this.Lte_Length.Text);
            sw.WriteLine("MB");

            sw.WriteLine(CheckBox_GTPv2.Checked? "true":"false");
            sw.WriteLine(CheckBox_DIAMETER.Checked ? "true" : "false");
            sw.WriteLine(CheckBox_S1AP.Checked ? "true" : "false");

            sw.Flush();
            sw.Close();
            fs.Close();
        }

        private void SaveToTxt()
        {
            FileStream fs = new FileStream("BackUps.txt", FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);

            sw.WriteLine(this.txtPcapLength.Text);
            sw.WriteLine("MB");

            string Source = "";
            string Dest = "";
            for (int i = 0; i < SourceListBox1.Items.Count; i++)
            {
                if (SourceListBox1.GetItemChecked(i))
                {
                    Source += "#" + SourceListBox1.Items[i].ToString();
                }
            }
            for (int i = 0; i < DestListBox2.Items.Count; i++)
            {
                if (DestListBox2.GetItemChecked(i))
                {
                    Dest += "#" + DestListBox2.Items[i].ToString();
                }
            }
            sw.WriteLine(Source);
            sw.WriteLine(Dest);

            sw.WriteLine(MaxVOBCToZC * 1000);
            sw.WriteLine(MaxZCToVOBC * 1000);

            sw.WriteLine(MaxVOBCToCI * 1000);
            sw.WriteLine(MaxCIToVOBC * 1000);

            sw.WriteLine(MaxVOBCToATS * 1000);
            sw.WriteLine(MaxATSToVOBC * 1000);

            sw.Flush();
            sw.Close();
            fs.Close();
        }

        private void SaveSetting()
        {
            for (int i = 0; i < SourceListBox1.Items.Count; i++)
            {
                foreach (var item in IPList.ConfigProperties)
                {
                    if (item.Name == SourceListBox1.Items[i].ToString())
                    {
                        if (SourceListBox1.GetItemChecked(i))
                        {
                            item.IsSourceChoose = true;
                        }
                        else
                        {
                            item.IsSourceChoose = false;
                        }
                    }
                }
            }
            for (int i = 0; i < DestListBox2.Items.Count; i++)
            {
                foreach (var item in IPList.ConfigProperties)
                {
                    if (item.Name == DestListBox2.Items[i].ToString())
                    {
                        if (DestListBox2.GetItemChecked(i))
                        {
                            item.IsDestChoose = true;
                        }
                        else
                        {
                            item.IsDestChoose = false;
                        }
                    }
                }
            }
        }

        private void SetPcapLength()
        {
            double PcapLength = double.Parse(this.txtPcapLength.Text);
            string PcapLengthUnit;

                PcapLengthUnit = "MB";
            GetPcapLengthNum(PcapLength, PcapLengthUnit);
        }

        private void SetPcapLengthLte()
        {
            double PcapLength = 0;
            if (this.Lte_Length.Text == "")
                MessageBox.Show("请设置保存文件的大小");
            else
                PcapLength = double.Parse(this.Lte_Length.Text);
            string LteLengthUnit="";//用户所选择的存储单位
            LteLengthUnit = "MB";
            GetPcapLengthNum1(PcapLength, LteLengthUnit);
        }

        private void GetPcapLengthNum1(double PcapLength, string PcapLengthUnit)
        {
            PcapLengthLte = PcapLength * 1024 * 1024;
        }

        private void GetPcapLengthNum(double PcapLength, string PcapLengthUnit)
        {
            PcapLengthNum = PcapLength * 1024 * 1024;               
        }

        private void GetAlarmInterval()
        {
            if (txtVOBCToZC.Text != "" && txtVOBCToZC.Text != null)
            {
                MaxVOBCToZC = Convert.ToDouble(this.txtVOBCToZC.Text) / 1000;
            }
            if (txtVOBCToCI.Text != "" && txtVOBCToCI.Text != null)
            {
                MaxVOBCToCI = Convert.ToDouble(this.txtVOBCToCI.Text) / 1000;
            }
            if (txtVOBCToATS.Text != "" && txtVOBCToATS.Text != null)
            {
                MaxVOBCToATS = Convert.ToDouble(this.txtVOBCToATS.Text) / 1000;
            }
            if (txtZCToVOBC.Text != "" && txtZCToVOBC.Text != null)
            {
                MaxZCToVOBC = Convert.ToDouble(this.txtZCToVOBC.Text) / 1000;
            }
            if (txtCIToVOBC.Text != "" && txtCIToVOBC.Text != null)
            {
                MaxCIToVOBC = Convert.ToDouble(this.txtCIToVOBC.Text) / 1000;
            }
            if (txtATSToVOBC.Text != "" && txtATSToVOBC.Text != null)
            {
                MaxATSToVOBC = Convert.ToDouble(this.txtATSToVOBC.Text) / 1000;
            }
        }

        private void button_Signal_Cancel_Click(object sender, EventArgs e)
        {
            this.Hide();
            checkBox__CBTC.CheckState = CheckState.Unchecked;
            checkBox_Signal.CheckState = CheckState.Unchecked;
          
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void checkBox__CBTC_CheckedChanged(object sender, EventArgs e)
        {
         
            //splitContainer1.Panel2.Enabled = false;
            CheckBoxStatues_CBTC = checkBox__CBTC.Checked;
            //bStatues_Signal = false;
          // f1.SetPanelVisible_CBTC(false);//设置显示CBTC界面

            
        }

        private void checkBox_Signal_CheckedChanged(object sender, EventArgs e)
        {
                //splitContainer1.Panel1.Enabled = false;
                //bStatues_CBTC = false;
                
                //f1.SetPanelVisible_Signal(true); //设置显示信令界面

            CheckBoxStatues_Signal = checkBox_Signal.Checked;
            
          
            
        }
    #endregion



       

        private void FilterForm_Load(object sender, EventArgs e)
        {
            this.SavePath_CBTC.Text = Application.StartupPath + "\\CBTC";
            this.SavePath_LTE.Text = Application.StartupPath + "\\LTE-M";
            ProtocolSelectAll.Checked = true;

            if (MainFormCount == 0)
            {
                checkBox__CBTC.Checked = true;
                checkBox_Signal.Checked = true;
            }

            MainFormCount++;
        }

        #endregion

        #region 定义方法
        /// <summary>
        /// 保存本页设置信息，存为txt文件
        /// </summary>
        //private void SaveSettings()
        //{
            


        //}
        /// <summary>
        /// 加载IPlist等配置信息
        /// </summary>
        private void LoadSettings()
        {
            //Indexof==-1 在字符串中从前向后定位字符和字符串；所有的返回值都是指定在字符串的绝对位置，如为空，则为-1
            if (Protocol.IndexOf("GTPv2") != -1) CheckBox_GTPv2.Checked = true; else CheckBox_GTPv2.Checked = false;
            if (Protocol.IndexOf("DIAMETER") != -1) CheckBox_DIAMETER.Checked = true; else CheckBox_DIAMETER.Checked = false;
            if (Protocol.IndexOf("S1AP") != -1) CheckBox_S1AP.Checked = true; else CheckBox_S1AP.Checked = false;

            
        }
        /// <summary>
        /// 将IPlist信息显示在本界面的两个ListBox中
        /// </summary>
        /// Dictionary<IP对应名称，IP-端口>
        /// <param name="dic"></param>
        //public void ShowInForm(Dictionary<string, string> dic)
        //{


        //    foreach (string str in dic.Keys)
        //    {
        //        SourceListBox1.Items.Add(str);
        //        DestListBox2.Items.Add(str);
        //    }
        //}
        /// <summary>
        /// 加载IPlist等基本配置信息，读取为字典表格式
        /// </summary>
        /// <returns></returns>
        //private Dictionary<string, string> ReadIPlist()
        //{
        //    Dictionary<string, string> tmpdic = new Dictionary<string, string>();
        //    List<string> list = new List<string>();
        //    string path = "IPlist.csv";
        //    StreamReader sr = new StreamReader(path, Encoding.Default);
        //    String line;
        //    while ((line = sr.ReadLine()) != null)
        //    {
        //        list.Add(line);
        //    }
        //    for (int i = 1; i < list.Count; i++)
        //    {
        //        string[] strs = list[i].Split(new char[] { ',' });
        //        string IP_Port = strs[1] + "-" + strs[2];
        //        tmpdic.Add(strs[0], IP_Port);
        //    }
        //    return tmpdic;
        //}

   #endregion

        private void SourceSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (this.SourceSelectAll.Checked)
            {
                SelectAll(SourceListBox1, CheckState.Checked);
            }
            else
            {
                SelectAll(SourceListBox1, CheckState.Unchecked);
            }
        }

        private void SelectAll(CheckedListBox ListBox, CheckState state)
        {
            for (int i = 0; i < ListBox.Items.Count; i++)
            {
                ListBox.SetItemCheckState(i, state);
            }
        }

        private void DestSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (this.DestSelectAll.Checked)
            {
                SelectAll(DestListBox2, CheckState.Checked);
            }
            else
            {
                SelectAll(DestListBox2, CheckState.Unchecked);
            }
        }

        private void ProtocolSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ProtocolSelectAll.Checked)
            {
                CheckBox_GTPv2.Checked = true;
                CheckBox_DIAMETER.Checked = true;
                CheckBox_S1AP.Checked = true;
            }
            else
            {
                CheckBox_GTPv2.Checked = false;
                CheckBox_DIAMETER.Checked = false;
                CheckBox_S1AP.Checked = false;                
            }
        }

        private void CheckBox_GTPv2_CheckedChanged(object sender, EventArgs e)
        {        
            if (CheckBox_GTPv2.Checked)
                CheckBoxStatues_GTPv2 = true;
        }

        private void CheckBox_DIAMETER_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox_DIAMETER.Checked)
                CheckBoxStatues_DIAMETER = true;
        }

        private void CheckBox_S1AP_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox_S1AP.Checked)
                CheckBoxStatues_GTPv2 = true;
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {
            if (CheckBox_S1AP.Checked)
                CheckBoxStatues_S1AP = true;
        }



        private void FilterForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
        }

        private void txtPcapLength_TextChanged(object sender, EventArgs e)
        {
            string pattern = @"(^[1-9]\d{0,2}$)|(^(1000)$)";
            string param1 = null;
            if (this.txtPcapLength.Text.Trim() == "")
            {
                txtPcapLength.Text = "";
            }
            else
            {
                Match m = Regex.Match(this.txtPcapLength.Text, pattern);   // 匹配正则表达式

                if (!m.Success)   // 输入的不是数字
                {
                    this.txtPcapLength.Text = param1;   // textBox内容不变

                    // 将光标定位到文本框的最后
                    this.txtPcapLength.SelectionStart = this.txtPcapLength.Text.Length;
                }
                else   // 输入的是数字
                {
                    param1 = this.txtPcapLength.Text;   // 将现在textBox的值保存下来
                }
            }
        }

        private void SavePath_CBTC_TextChanged(object sender, EventArgs e)
        {

        }

        private void Lte_Length_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(((e.KeyChar >= '0') && (e.KeyChar <= '9')) || e.KeyChar <= 31))
            {
                if (e.KeyChar == '.')
                {
                    if (((TextBox)sender).Text.Trim().IndexOf('.') > -1)
                        e.Handled = true;
                }
                else
                    e.Handled = true;
            }
            else
            {
                if (e.KeyChar <= 31)
                {
                    e.Handled = false;
                }
                else if (((TextBox)sender).Text.Trim().IndexOf('.') > -1)
                {
                    if (((TextBox)sender).Text.Trim().Substring(((TextBox)sender).Text.Trim().IndexOf('.') + 1).Length >= 4)
                        e.Handled = true;
                }
            }
        }

        private void txtPcapLength_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(((e.KeyChar >= '0') && (e.KeyChar <= '9')) || e.KeyChar <= 31))
            {
                if (e.KeyChar == '.')
                {
                    if (((TextBox)sender).Text.Trim().IndexOf('.') > -1)
                        e.Handled = true;
                }
                else
                    e.Handled = true;
            }
            else
            {
                if (e.KeyChar <= 31)
                {
                    e.Handled = false;
                }
                else if (((TextBox)sender).Text.Trim().IndexOf('.') > -1)
                {
                    if (((TextBox)sender).Text.Trim().Substring(((TextBox)sender).Text.Trim().IndexOf('.') + 1).Length >= 4)
                        e.Handled = true;
                }
            }
        }

        private void SourceListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Slb != SourceListBox1.Items.Count)
            {
                SourceSelectAll.CheckedChanged -= new System.EventHandler(this.SourceSelectAll_CheckedChanged);
                SourceSelectAll.Checked = false;
                SourceSelectAll.CheckedChanged += new System.EventHandler(this.SourceSelectAll_CheckedChanged);
            }
        }

        private void DestListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Dlb != DestListBox2.Items.Count)
            {
                DestSelectAll.CheckedChanged -= new System.EventHandler(this.DestSelectAll_CheckedChanged);
                DestSelectAll.Checked = false;
                DestSelectAll.CheckedChanged += new System.EventHandler(this.DestSelectAll_CheckedChanged);
            }
        }
    }
}

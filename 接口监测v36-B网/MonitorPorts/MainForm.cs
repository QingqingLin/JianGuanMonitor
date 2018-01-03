using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using System.Linq;
using GTP_V2Decoder;
using MonitorPorts.Packet;
using MonitorPorts.nas;
using read;
using sctp;
using System.Text;

namespace MonitorPorts
{
    public partial class MainForm : Form
    {
        #region 全局变量

        #region 主界面
        CatchSocket Catch;
        BindNetworkCard Bind;
        private FilterForm FilterOptionForm;
        private AboutSoftware AboutSoftFrom;
        DateTime startTime;//起始时间
        bool bStatus = false;//标记按钮当前状态,false表示按钮目前显示的是扫描，true显示的是暂停
        const int ICMP_DATA_OFFSET = 4;//ICMP包头长度
        const int IGMP_DATA_OFFSET = 4;//IGMP包头长度
        const int TCP_DATA_OFFSET = 20;//TCP包头长度
        const int UDP_DATA_OFFSET = 8;//UDP包头长度
        const int SCTP_DATA_OFFSET = 12;//SCTP包头长度(通用数据头)
        private int itemID;//筛选到的数据灯
        private bool DeleteFlag = true;//每隔固定时间删除文件所用标志位
        private double captureTimeOnline;//在线抓包时数据包的捕获时间
        private int filterCount;//收到的数据包个数
        List<byte[]> listPackets = new List<byte[]>();//用于存储离线解析文件中的S1AP、DIAMETER、GTP三种协议数据流
        private readonly string _path = Application.StartupPath;// 程序所在路径
        public delegate void UpdateForm(PacketProperties Properties);
        delegate void listViewDelegate(string ID, string Capturetime, string Protocol, string SourceIP, string SourcePort, string DestIP, string DestPort,
                                        string AllLength, string MessageBodyHex);
        delegate void refresh(DateTime Time, string Protocol, string SourceIP, string SourcePort, string DestIP, string DestPort, uint IPHeaderLength, Byte[] IPHeaderBuffer, uint MessageLength,
                                        Byte[] MessageBuffer, uint PacketLength, Byte[] PacketBuffer);
        #endregion

        #region 界面变量
        //Edit by kuang
        public bool bStatues_CBTC = false;
        public bool bStatues_Signal = false;  //标记选择的是那一界面，即选择的是信令监测、信号监测
        public bool procol_GTPv2 = false;
        public bool procol_DIAMETER = false;
        public bool procol_S1AP = false;
        public bool OffLine = false;
        public bool FilterState = false;
        // 界面逻辑用
        public bool tabpage_GTPv2 = false;
        public bool tabpage_DIAMETER = false;
        public bool tabpage_S1AP = false;
        public bool tabpage_CDR = false;
        public bool tabpage_OffLine = false;

        public System.Windows.Forms.Panel ActivePanel = new Panel();
        OpenFileDialog file = new OpenFileDialog(); //打开文件的位置，定义新的文件打开位置控件
        #endregion

        #region GTP相关变量
        gtpv2 A = new gtpv2();

        public List<byte[]> allClone = new List<byte[]>();//所有gtp消息的集合，该消息包含底层头部
        public List<gtpStruct> gtpChunkCopy1 = new List<gtpStruct>();
        List<gtpStruct> gtpChunkCopy2 = new List<gtpStruct>();
        List<List<gtpStruct>> sortedIP = new List<List<gtpStruct>>();
        List<gtpStruct> TEIDExist = new List<gtpStruct>();
        List<gtpStruct> TEIDExistClone = new List<gtpStruct>();
        List<gtpStruct> nullTEID = new List<gtpStruct>();
        List<List<gtpStruct>> sortedTEID = new List<List<gtpStruct>>();
        List<List<gtpStruct>> subCDR = new List<List<gtpStruct>>();
        List<List<gtpStruct>> subCDR_Copy = new List<List<gtpStruct>>();
        List<List<gtpStruct>> GTPsubCDR = new List<List<gtpStruct>>();
        List<gtpCDRstruct> GTPCDR = new List<gtpCDRstruct>();      //多段关联用
        public List<gtpStruct> gtpChunk1 = new List<gtpStruct>();
        #endregion

        #region DIAMETER相关变量
        SCTPDiameter SCTP = new SCTPDiameter();
        TCPDiameter TCPDiameter = new TCPDiameter();

        static int IMSILocation = -1;//用于标记KASME中特定IMSI所在的位置
        string[] protocol = new string[25];//每条CDR对应的协议名称存储列表
        List<string> IMSIList = new List<string>();//该list列表用于存储与diameter流程各条CDR对应的imsi标识号
        List<List<List<string>>> diameterFlow = new List<List<List<string>>>();//list中的每个list用于存储一个终端下的每个信令流程，同时list中的一个string数组用于存储一条信令
        List<int> SCTPDiameterID = new List<int>();//采用SCTP承载diameter消息流对应在离线解析展示界面的ID号
        List<int> TCPDiameterID = new List<int>();//S6a_diameter消息对应的数据流
        public List<string> diameterTime = new List<string>();//所有diameter数据流对应的时间列表
        public List<string> diameterTimeCDR = new List<string>();//所有diameter数据流对应的时间列表-相对时间
        public List<byte[]> diameterClone_TCP = new List<byte[]>();//TCP承载diameter数据包的数据流（包含IP头部的数据流）
        public List<byte[]> TCPDiameterClone = new List<byte[]>();//TCP承载diameter数据包的数据流（不包含IP头部、包含TCP头部的数据流）
        public List<string> diameterTime_TCP = new List<string>();//TCP承载diameter数据包的数据流所对应的时间列表
        public List<string> diameterTime_SCTP = new List<string>();//SCTP承载diameter数据包的数据流所对应的时间列表
        public List<string> diameterTimeCDR_TCP = new List<string>();//TCP承载diameter数据包的数据流所对应的时间列表-相对时间
        public List<string> diameterTimeCDR_SCTP = new List<string>();//SCTP承载diameter数据包的数据流所对应的时间列表-相对时间
        public List<byte[]> diameterClone_SCTP = new List<byte[]>();//SCTP承载diameter数据包的数据流（包含IP头部的数据流）
        public List<byte[]> SCTPDiameterClone = new List<byte[]>();//SCTP承载diameter数据包的数据流（不包含IP头部、包含SCTP头部的数据流）
        int[] num1 = new int[25] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };//diameter CDR中含有的信令条数
        delegate void listView1Delegate(string ID1, string STime, string Endtime, string SessionID, string Interface, string imsi);
        DiameterKASME KAS = new DiameterKASME();//存储不同IMSI所携带的KASME信息（包含时间以及不同时间对应的KASME列表）
        List<DiameterKASME> KASME = new List<DiameterKASME>();//KASME的所有信息列表
        /// <summary>
        /// 存储不同IMSI所携带的KASME信息（包含时间以及不同时间对应的KASME列表）
        /// </summary>
        public struct DiameterKASME
        {
            public string imsi;
            public List<diameterKASME> Kasme;
        }
        /// <summary>
        /// 包含时间以及不同时间对应的KASME列表
        /// </summary>
        public struct diameterKASME
        {
            public string time;
            public List<byte[]> kasme;
        }
        //各项消息的位置和长度记录_离线
        int diameterLocationOffline, diameterLengthOffline;
        public int avpLocation_UserNameOffline, avpLocation_ResultOffline, avpLocation_AuthenticationOffline, avpLocation_SessionIDOffline;//记录各avp的起始位置信息
        public int avpLength_UserNameOffline, avpLength_ResultOffline, avpLength_AuthenticationOffline, avpLength_SessionIDOffline;//记录各avp的长度信息
        public int location_UserNameStrOffline, location_ResultCodeStrOffline, location_SessionID4Offline, length_UserNameStrOffline, length_SessionID4Offline;//记录各项avp中详细信息的位置，长度参数
        public List<int> avpLocation_EUTRANVectorOffline = new List<int>();//记录各EUTRANVector avp的起始位置信息
        public List<int> avpLength_EUTRANVectorOffline = new List<int>();//记录各EUTRANVector avp的长度信息
        public List<int> avpLocation_KASMEOffline = new List<int>();//记录各KASME avp的起始位置信息
        public List<int> avpLength_KASMEOffline = new List<int>();//记录各KASME avp的长度信息
        public List<int> location_KASMEStrOffline = new List<int>();//记录各KASMEStr的起始位置信息
        //各项消息的位置和长度记录_在线
        int diameterLocationOnline, diameterLengthOnline;
        public int avpLocation_UserNameOnline, avpLocation_ResultOnline, avpLocation_AuthenticationOnline, avpLocation_SessionIDOnline;//记录各avp的起始位置信息
        public int avpLength_UserNameOnline, avpLength_ResultOnline, avpLength_AuthenticationOnline, avpLength_SessionIDOnline;//记录各avp的长度信息
        public int location_UserNameStrOnline, location_ResultCodeStrOnline, location_SessionID4Online, length_UserNameStrOnline, length_SessionID4Online;//记录各项avp中详细信息的位置，长度参数
        public List<int> avpLocation_EUTRANVectorOnline = new List<int>();//记录各EUTRANVector avp的起始位置信息
        public List<int> avpLength_EUTRANVectorOnline = new List<int>();//记录各EUTRANVector avp的长度信息
        public List<int> avpLocation_KASMEOnline = new List<int>();//记录各KASME avp的起始位置信息
        public List<int> avpLength_KASMEOnline = new List<int>();//记录各KASME avp的长度信息
        public List<int> location_KASMEStrOnline = new List<int>();//记录各KASMEStr的起始位置信息
        //int clickFlag_Offline = -1;//listView点击后置为1，用于点击树状结构时除去多余繁杂流程_离线
        //int clickFlag_Online = -1;//listView点击后置为1，用于点击树状结构时除去多余繁杂流程_在线
        #endregion

        #region S1AP
        //string protocol1 = "";//无用变量（黄罡2017.5.27）
        SctpDecode SctpToS1ap = new SctpDecode();
        Id1 Id1 = new Id1();
        Id2 Id2 = new Id2();
        s1apDec s1apDec1 = new s1apDec();
        process Process = new process();
        process.key6 Key6 = new process.key6();
        process.S1apContent S1apContent = new process.S1apContent();
        causeOnline decCause = new causeOnline();
        IMSI_TMSI Id_paging = new IMSI_TMSI();
        Imsi_Tmsi Id_initial = new Imsi_Tmsi();
        s1ap_nas1 s1ap_nas2 = new s1ap_nas1();
        NAS_DEC nas_dec = new NAS_DEC();//用于多段关联之后的nas解析
        s1ap_name decodeS1ap = new s1ap_name();
        List<List<nas_class>> Nasclass1 = new List<List<nas_class>>();
        //public List<byte[]> ScS1Clone = new List<byte[]>();//包含sctp头部的sctp消息
        //public List<byte[]> IPS1Clone = new List<byte[]>();//包含ip头部的sctp消息                
        int IDSelected;//s1ap的CDR序号
        public class nas_class
        {
            public string identity;//UE的标识IMSI或TMSI
            public string cause_str;//nas cause
            public string nas_name;//nas 名称
            public string ue_ip;//终端IP
        }
        public class CdrIdentity//用于标识一个CDR
        {
            public string identity1;
            public string identity2;
            public string mmeUeS1apId;
            public string enbUeS1apId;
        }
        List<process.key6> S1apKey6All = new List<process.key6>();//所有信令的6元组关键字(注意List里边的元素是process.key6而不是key6,因为调用函数只能识别自己程序里的结构体而不能识别主函数中的结构体),且不能是Process.key,因为Process是实例化的对象，表示类中的数据类型是只能用类名不能是对象名。！！！
        List<process.S1apContent> S1apContentAll = new List<process.S1apContent>();//所有信令的2元组关键字
        List<string> tmsi = new List<string>();//存储一个流程中的old tmsi与new tmsi，从而在CDR中采用最新tmsi
        public static List<List<process.S1apContent>> CdrList = new List<List<process.S1apContent>>();//存放一个离线文件中的所有CDR单元
        List<process.IdPair> CdrIdpair = new List<process.IdPair>();//cdrIdpair的单元是对应于CdrList中CDR的IDpair
        public List<List<List<process.S1apContent>>> UeS1apProcessList = new List<List<List<process.S1apContent>>>();//List<process.S1apContent>代表一个cdr,List<List<process.S1apContent>表示ue的一次使用imsi的通信流程，而现在新建的变量是存储所有终端的通信流程
        public List<string> UeImsiList = new List<string>();//用于对应UeS1apProcessList中的不同ue通信流程
        public uint MmeUeS1apId = 0;
        public int EnbUeS1apId = 0;
        public string S1apNameOnline = "";
        public string S1apCauseKindOnline = "";
        public string S1apCauseNameOnline = "";
        public string IdentityOnline = "";
        /// <summary>
        /// nas结构体存放的是一个nas信令的解析内容
        /// </summary>
        struct nas
        {
            public uint MmeUeS1apId;
            public int EnbUeS1apId;
            public string cause_kind;
            public string cause_name;
            public string s1ap_identity;
            public string nas_name;
            public string nas_cause;
            public string nas_identity;
            public string ue_ip;
        }
        List<Dictionary<int, nas>> NasList = new List<Dictionary<int, nas>>();
        #endregion

        #region 多段关联内容
        List<Dictionary<int, LTEmessage>> AllUeSignal = new List<Dictionary<int, LTEmessage>>();
        #endregion

        #region 信令监测
        Dictionary<string, Color> DicColor = new Dictionary<string, Color>();
        DateTime countInterval;
        List<string> xData = new List<string> { "0-500", "500-1000", "1000-2400", "≥2400" };
        List<int> yData = new List<int> { 0, 0, 0, 0 };
        List<string> VOBCToZC = new List<string>();
        List<string> VOBCToCI = new List<string>();
        List<string> VOBCToATS = new List<string>();
        List<string> ZCToVOBC = new List<string>();
        List<string> CIToVOBC = new List<string>();
        List<string> ATSToVOBC = new List<string>();
        #endregion

      
        public static bool IsFirstClick;
        #endregion

        public static System.Threading.Timer ShowSignalingTimer;
        public static System.Threading.Timer OutputLTEDataTimer;
        public static System.Threading.Timer OutputCBTCDataTimer;
        public static List<PacketProperties> CBTCTemporaryStorage1 = new List<PacketProperties>();
        public static List<PacketProperties> CBTCTemporaryStorage2 = new List<PacketProperties>();
        public static List<PacketProperties> LTETemporaryStorage1 = new List<PacketProperties>();
        public static List<PacketProperties> LTETemporaryStorage2 = new List<PacketProperties>();

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public MainForm()
        {
            IsFirstClick = true;
            itemID = 0;
            filterCount = 0;
            //FilterOptionForm = new FilterForm(this);
            //FilterOptionForm.accept += new EventHandler(f2_accept); //绑定过滤窗体
            Catch = new CatchSocket(this);
            Bind = new BindNetworkCard();
            Thread innerDelFileThread = new Thread(DelFileThread);
            innerDelFileThread.IsBackground = true;
            innerDelFileThread.Start();
            InitializeComponent();
            ActivePanel = panel6;
            //界面显示用1
            this.tabControl1.Visible = true;
            //edit by kuang
            panel2.BackColor = Color.FromArgb(55, 159, 251);
            // edit by Kuang

            ShowSignalingTimer = new System.Threading.Timer(new TimerCallback(ShowThread.Show), null, -1, 0);
            OutputCBTCDataTimer = new System.Threading.Timer(new TimerCallback(Catch.CBTCOutToPcap), null, -1, 0);
            OutputLTEDataTimer = new System.Threading.Timer(new TimerCallback(Catch.LTEOutToPcap), null, -1, 0);

            //守护进程使用，实例化Communication类

        }
        private void DelFileThread()
        {
            string fileName;//存储pcap文件夹的名称
            string fileName_signaling;//存储pcap文件夹的名称

            string pathConfigurationInfo = Application.StartupPath + @"\DeleteDateInterval.ini";
            string interval = IniRead.ReadIniData("LTE信令", "间隔", "", pathConfigurationInfo);     //读取ini文件
            ushort DeleteDataInterval = Convert.ToUInt16(interval);
            DeleteData Delete = new DeleteData(DeleteDataInterval);
            fileName = "\\CBTC";
            string pathConfigurationInfo_signaling = Application.StartupPath + @"\DeleteDateInterval.ini";

            string interval_signaling = IniRead.ReadIniData("CBTC数据", "间隔", "", pathConfigurationInfo);
            ushort DeleteDataInterval_signaling = Convert.ToUInt16(interval_signaling);
            DeleteData Delete_signaling = new DeleteData(DeleteDataInterval_signaling);
            fileName_signaling = "\\LTE-M";
            while (DeleteFlag)
            {
                Delete_signaling.DeleteOverdueData(fileName);
                Delete.DeleteOverdueData(fileName_signaling);
                Thread.Sleep(60 * 60 * 1000);
            }
        }

        #region 窗体控件

        //关闭界面
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult.OK == MessageBox.Show("你确定要关闭应用程序吗？", "关闭提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1))
            {
                this.FormClosing -= new FormClosingEventHandler(this.Form1_FormClosing);//为保证Application.Exit();时不再弹出提示，所以将FormClosing事件取消
                Application.Exit();//退出整个应用程序
                if (p != null && !p.HasExited)
                {
                    p.Kill();
                }
                //System.Diagnostics.Process.Start(Application.);

                //守护进程使用
            }
            else
            {
                e.Cancel = true;  //取消关闭事件
            }
        }

        /// <summary>
        /// 将捕获的数据添加到ListView中
        /// </summary>
        private void AddItem(string captureTimeOnline, string protocol, string sourceIP, string sourcePort, string destIP, string destPort, string allLength, string messageBodyHex, string sourceData)
        {
            listView_Data.Items.Add(new ListViewItem(new string[] { captureTimeOnline, protocol, sourceIP, sourcePort, destIP, destPort, allLength, messageBodyHex, sourceData }));
            //UpdateStatus();
        }

        /// <summary>
        /// 将二进制数据转换成16进制
        /// </summary>
        /// <returns></returns>
        private string GetDataHex(Byte[] data, int index, int count)
        {
            string dataHex = "";
            for (int i = index; i < index + count; i++)
            {
                if (i > index && (i - index) % 16 == 0)
                {
                    dataHex += "\r\n";
                }
                if (data[i].ToString("X").Length != 1)
                {
                    dataHex += data[i].ToString("X") + " ";
                }
                else
                {
                    dataHex += "0" + data[i].ToString("X") + " ";
                }
            }
            return dataHex;
        }

        private int GetMessageHeaderLen(string protocol)
        {
            switch (protocol)
            {
                case "ICMP": return ICMP_DATA_OFFSET;
                case "IGMP": return IGMP_DATA_OFFSET;
                case "TCP": return TCP_DATA_OFFSET;
                case "UDP": return UDP_DATA_OFFSET;
                case "SCTP": return SCTP_DATA_OFFSET;
                case "UNKNOW": return 0;
                default: return 0;
            }
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        private void Clear()
        {
            itemID = 0;
            filterCount = 0;
            listView_Data.Items.Clear();
            //UpdateStatus();

        }

        /// <summary>
        /// 清除信号筛选解析后的展示列表
        /// </summary>
        /// <param name="charts"></param>
        private void ClearChart(Chart[] charts)
        {
            foreach (var item in charts)
            {
                for (int i = 0; i < item.Series.Count(); i++)
                {
                    item.Series[i].Points.Clear();
                }
            }
        }
        //edit by shuya
        #endregion

        //窗体
        private void PanelReSize(object sender, EventArgs e)
        {
            for (int i = 0; i < ActivePanel.Controls.Count; i++)
            {
                ActivePanel.Controls[i].Left = (ActivePanel.Width - ActivePanel.Controls[i].Width) / 2;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.tabControl1.TabPages.Remove(tabPage1);
            //this.tabControl1.TabPages.Remove(tabPage2);
            this.tabControl1.TabPages.Remove(tabPage3);
            this.tabControl1.TabPages.Remove(tabPage4);
            this.tabControl1.TabPages.Remove(tabPage5);
            this.tabControl1.TabPages.Remove(tabPage6);
            this.tabControl1.TabPages.Remove(tabPage7);
            this.tabControl1.TabPages.Remove(tabPage8);

            this.toolStripButton_Start.Enabled = false;
            this.toolStripButton_Pause.Enabled = false;
            this.toolStripButton_Stop.Enabled = false;
            ShowThread.PacketArrival += ShowSignalingInForm;
            FilterOptionForm = new FilterForm(this);


            button_OffLine.Enabled = false;
            button_GTPv2.Enabled = false;
            button_DIAMETER.Enabled = false;
            button_S1AP.Enabled = false;
            button_Process.Enabled = false;
        }

        private void button_FindFile_Click(object sender, EventArgs e) //打开文件
        {

        }

        private void button_MessStatistic_Click(object sender, EventArgs e)
        {
            //消息统计
        }

        private void button_Protocolflow_Click(object sender, EventArgs e)
        {
            //协议流量统计
        }

        private void button_Portflow_Click(object sender, EventArgs e)
        {
            //协议端口流量
        }

        //edit by kuang




        List<DiameterCDRStruct> DIAMETERCDR = new List<DiameterCDRStruct>();//用于存储diameter的CDR列表
        List<DiameterStruct> diameterSignaling;//一条CDR中包含的信令的列表
        DiameterCDRStruct diameterCDR;//用于存储diameter的一条CDR
        /// <summary>
        /// diameter的CDR合成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_DIAMETER_Click(object sender, EventArgs e)
        {
            if (changePcap_Diameter)//黄罡
            {
                if (!tabpage_DIAMETER)
                {
                    this.tabControl1.TabPages.Add(tabPage4);
                    this.tabControl1.SelectedTab = this.tabPage4;
                }

                listView_DIAMETER.BackColor = Color.SkyBlue;

                DiameterStruct signaling_Diameter;//用于存储一条信令的各项信息
                listView_DIAMETER.Items.Clear();//清除上次报文在listview_diameter上的显示
                //因为后续还要对新打开的pcaP文件进行CDR合成，故不能将按钮Enable状态设为false
                //button_DIAMETER.Enabled = false;
                #region Diameter
                string ID1 = "0";
                string beginTime = "";
                string endTime = "";
                string beginTimeCDR = "";
                string endTimeCDR = "";
                string sessionID = string.Empty;
                string Interface = string.Empty;
                string sourceIP = "";
                string sourcePort = "";
                string destIP = "";
                string destPort = "";
                int i, j, num = 0;
                string applicationid = "";
                string imsi = "";

            F2:
                for (i = num; i < SCTPDiameterClone.Count; i++)
                {

                    while (SCTPDiameterClone[i] != null)
                    {
                        SCTP.DecodeSctpChunk(SCTPDiameterClone[i]);
                        SCTP.decoder_All(diameterClone_SCTP[i]);

                        if (SCTP.applicationID == @"3GPP S6a/S6d(16777251)")
                        {
                            diameterSignaling = new List<DiameterStruct>();
                            ID1 = Convert.ToString(Convert.ToInt16(ID1) + 1);
                            sessionID = SCTP.avp_SessionID4;
                            imsi = SCTP.userNameStr;
                            diameterCDR.CDRIMSI = imsi;
                            Interface = "S6a";
                            beginTime = diameterTime[i];
                            endTime = diameterTime[i];
                            beginTimeCDR = diameterTimeCDR[i];
                            endTimeCDR = diameterTimeCDR[i];
                            applicationid = SCTP.applicationID;
                            sourceIP = SCTP.sourIP;
                            sourcePort = SCTP.sourPort;
                            destIP = SCTP.destIP;
                            destPort = SCTP.destPort;
                            signaling_Diameter.ID_diamter = SCTPDiameterID[i];
                            signaling_Diameter.time = diameterTime[i];
                            signaling_Diameter.re = SCTP.diameterDirection;
                            signaling_Diameter.command = SCTP.command;
                            signaling_Diameter.diameterFrame = diameterClone_SCTP[i];
                            signaling_Diameter.protocol = "diameter-sctp";
                            diameterSignaling.Add(signaling_Diameter);
                            num++;
                            goto F1;
                        }
                        break;
                    }
                    num++;
                }
            F1:
                for (j = num; j < SCTPDiameterClone.Count; j++)
                {

                    while (j != SCTPDiameterClone.Count)
                    {
                        SCTP.DecodeSctpChunk(SCTPDiameterClone[j]);
                        SCTP.decoder_All(diameterClone_SCTP[j]);

                        if (SCTP.applicationID == @"3GPP S6a/S6d(16777251)")
                        {

                            if ((sourceIP == SCTP.sourIP && sourcePort == SCTP.sourPort && destIP == SCTP.destIP && destPort == SCTP.destPort) && (Convert.ToDouble(diameterTimeCDR[j]) - Convert.ToDouble(beginTimeCDR) < 30 && Convert.ToDouble(diameterTimeCDR[j]) - Convert.ToDouble(beginTimeCDR) >= 0))
                            {
                                if (SCTP.userNameStr == imsi)
                                {
                                    endTime = diameterTime[j];
                                    endTimeCDR = diameterTimeCDR[j];
                                    signaling_Diameter.ID_diamter = SCTPDiameterID[j];
                                    signaling_Diameter.time = diameterTime[j];
                                    signaling_Diameter.re = SCTP.diameterDirection;
                                    signaling_Diameter.command = SCTP.command;
                                    signaling_Diameter.diameterFrame = diameterClone_SCTP[j];
                                    signaling_Diameter.protocol = "diameter-sctp";
                                    diameterSignaling.Add(signaling_Diameter);
                                    num1[Convert.ToInt16(ID1)]++;
                                    num++;
                                    while ((j + 1) == diameterClone_SCTP.Count)
                                    {
                                        diameterCDR.CDRbuffer = diameterSignaling;
                                        DIAMETERCDR.Add(diameterCDR);

                                        AddItem_DiameterCDR(ID1, beginTime, endTime, sessionID, Interface, imsi);
                                        listView1Delegate listDelegate = new listView1Delegate(AddItem_DiameterCDR);
                                        protocol[Convert.ToInt16(ID1)] = "sctp";
                                        break;
                                    }
                                    goto F1;
                                }
                                else
                                {
                                    diameterCDR.CDRbuffer = diameterSignaling;
                                    DIAMETERCDR.Add(diameterCDR);
                                    AddItem_DiameterCDR(ID1, beginTime, endTime, sessionID, Interface, imsi);
                                    listView1Delegate listDelegate = new listView1Delegate(AddItem_DiameterCDR);
                                    protocol[Convert.ToInt16(ID1)] = "sctp";
                                    goto F2;
                                }

                            }
                            else if ((sourceIP == SCTP.destIP && sourcePort == SCTP.destPort && destIP == SCTP.sourIP && destPort == SCTP.sourPort) && (Convert.ToDouble(diameterTimeCDR[j]) - Convert.ToDouble(beginTimeCDR) < 30 && Convert.ToDouble(diameterTimeCDR[j]) - Convert.ToDouble(beginTimeCDR) >= 0))
                            {
                                if (SCTP.userNameStr == imsi)
                                {
                                    endTime = diameterTime[j];
                                    endTimeCDR = diameterTime[j];
                                    signaling_Diameter.ID_diamter = SCTPDiameterID[j];
                                    signaling_Diameter.time = diameterTime[j];
                                    signaling_Diameter.re = SCTP.diameterDirection;
                                    signaling_Diameter.command = SCTP.command;
                                    signaling_Diameter.diameterFrame = diameterClone_SCTP[j];
                                    signaling_Diameter.protocol = "diameter-sctp";
                                    diameterSignaling.Add(signaling_Diameter);
                                    num1[Convert.ToInt16(ID1)]++;
                                    num++;
                                    while ((j + 1) == diameterClone_SCTP.Count)
                                    {
                                        diameterCDR.CDRbuffer = diameterSignaling;
                                        DIAMETERCDR.Add(diameterCDR);
                                        AddItem_DiameterCDR(ID1, beginTime, endTime, sessionID, Interface, imsi);
                                        listView1Delegate listDelegate = new listView1Delegate(AddItem_DiameterCDR);
                                        protocol[Convert.ToInt16(ID1)] = "sctp";
                                        break;
                                    }
                                    goto F1;
                                }
                                else
                                {
                                    diameterCDR.CDRbuffer = diameterSignaling;
                                    DIAMETERCDR.Add(diameterCDR);
                                    AddItem_DiameterCDR(ID1, beginTime, endTime, sessionID, Interface, imsi);
                                    listView1Delegate listDelegate = new listView1Delegate(AddItem_DiameterCDR);
                                    protocol[Convert.ToInt16(ID1)] = "sctp";
                                    goto F2;
                                }
                            }
                            else
                            {
                                diameterCDR.CDRbuffer = diameterSignaling;
                                DIAMETERCDR.Add(diameterCDR);
                                AddItem_DiameterCDR(ID1, beginTime, endTime, sessionID, Interface, imsi);
                                listView1Delegate listDelegate = new listView1Delegate(AddItem_DiameterCDR);
                                protocol[Convert.ToInt16(ID1)] = "sctp";
                                goto F2;
                            }
                        }
                        else if (applicationid == @"3GPP S6a/S6d(16777251)")
                        {
                            diameterCDR.CDRbuffer = diameterSignaling;
                            DIAMETERCDR.Add(diameterCDR);
                            AddItem_DiameterCDR(ID1, beginTime, endTime, sessionID, Interface, imsi);
                            listView1Delegate listDelegate = new listView1Delegate(AddItem_DiameterCDR);
                            protocol[Convert.ToInt16(ID1)] = "sctp";
                            num++;
                            goto F2;
                        }
                        applicationid = SCTP.applicationID;
                        break;
                    }
                }

                if (TCPDiameterClone.Count != 0)
                {
                F4:
                    for (i = num; i < TCPDiameterClone.Count + SCTPDiameterClone.Count; i++)
                    {

                        while (TCPDiameterClone[i - SCTPDiameterClone.Count] != null)
                        {
                            TCPDiameter.DecodeTcpChunk(TCPDiameterClone[i - SCTPDiameterClone.Count]);
                            TCPDiameter.decoder_All(diameterClone_TCP[i - SCTPDiameterClone.Count]);
                            if (TCPDiameter.ApplicationID == @"3GPP S6a/S6d(16777251)")
                            {
                                diameterSignaling = new List<DiameterStruct>();
                                ID1 = Convert.ToString(Convert.ToInt16(ID1) + 1);
                                sessionID = TCPDiameter.avp_SessionID4;
                                imsi = TCPDiameter.userNameStr;
                                diameterCDR.CDRIMSI = imsi;
                                Interface = "S6a";
                                beginTime = diameterTime[i - SCTPDiameterClone.Count];
                                endTime = diameterTime[i - SCTPDiameterClone.Count];
                                beginTimeCDR = diameterTimeCDR[i - SCTPDiameterClone.Count];
                                endTimeCDR = diameterTimeCDR[i - SCTPDiameterClone.Count];
                                applicationid = TCPDiameter.ApplicationID;
                                sourceIP = TCPDiameter.sourIP;
                                sourcePort = TCPDiameter.sourPort;
                                destIP = TCPDiameter.destIP;
                                destPort = TCPDiameter.destPort;
                                signaling_Diameter.ID_diamter = TCPDiameterID[i - SCTPDiameterClone.Count];
                                signaling_Diameter.time = diameterTime[i - SCTPDiameterClone.Count];
                                signaling_Diameter.re = TCPDiameter.diameterDirection;
                                signaling_Diameter.command = TCPDiameter.command;
                                signaling_Diameter.diameterFrame = diameterClone_TCP[i - SCTPDiameterClone.Count];
                                signaling_Diameter.protocol = "diameter-tcp";
                                diameterSignaling.Add(signaling_Diameter);
                                num++;
                                goto F3;
                            }
                            break;
                        }
                        num++;
                    }
                F3:
                    for (j = num; j <= TCPDiameterClone.Count + SCTPDiameterClone.Count; j++)
                    {
                        while (j != TCPDiameterClone.Count + SCTPDiameterClone.Count)
                        {
                            TCPDiameter.DecodeTcpChunk(TCPDiameterClone[j - SCTPDiameterClone.Count]);
                            TCPDiameter.decoder_All(diameterClone_TCP[j - SCTPDiameterClone.Count]);
                            if (TCPDiameter.ApplicationID == @"3GPP S6a/S6d(16777251)")
                            {

                                if (sourceIP == TCPDiameter.sourIP && sourcePort == TCPDiameter.sourPort && destIP == TCPDiameter.destIP && destPort == TCPDiameter.destPort)
                                {
                                    if (Convert.ToDouble(diameterTimeCDR[j]) - Convert.ToDouble(beginTimeCDR) < 30 && Convert.ToDouble(diameterTimeCDR[j]) - Convert.ToDouble(beginTimeCDR) >= 0)
                                    {
                                        if (TCPDiameter.userNameStr == imsi)
                                        {
                                            endTime = diameterTime[j - SCTPDiameterClone.Count];
                                            endTimeCDR = diameterTimeCDR[j - SCTPDiameterClone.Count];
                                            signaling_Diameter.ID_diamter = TCPDiameterID[j - SCTPDiameterClone.Count];
                                            signaling_Diameter.time = diameterTime[j - SCTPDiameterClone.Count];
                                            signaling_Diameter.re = TCPDiameter.diameterDirection;
                                            signaling_Diameter.command = TCPDiameter.command;
                                            signaling_Diameter.diameterFrame = diameterClone_TCP[j - SCTPDiameterClone.Count];
                                            signaling_Diameter.protocol = "diameter-tcp";
                                            diameterSignaling.Add(signaling_Diameter);
                                            num1[Convert.ToInt16(ID1)]++;
                                            num++;
                                            while (j == TCPDiameterClone.Count + SCTPDiameterClone.Count)
                                            {
                                                diameterCDR.CDRbuffer = diameterSignaling;
                                                DIAMETERCDR.Add(diameterCDR);
                                                AddItem_DiameterCDR(ID1, beginTime, endTime, sessionID, Interface, imsi);
                                                listView1Delegate listDelegate = new listView1Delegate(AddItem_DiameterCDR);
                                                protocol[Convert.ToInt16(ID1)] = "tcp";
                                                break;
                                            }
                                            goto F3;
                                        }
                                        else
                                        {
                                            diameterCDR.CDRbuffer = diameterSignaling;
                                            DIAMETERCDR.Add(diameterCDR);
                                            AddItem_DiameterCDR(ID1, beginTime, endTime, sessionID, Interface, imsi);
                                            listView1Delegate listDelegate = new listView1Delegate(AddItem_DiameterCDR);
                                            protocol[Convert.ToInt16(ID1)] = "tcp";
                                            goto F4;
                                        }
                                    }
                                }
                                else if (sourceIP == TCPDiameter.destIP && sourcePort == TCPDiameter.destPort && destIP == TCPDiameter.sourIP && destPort == TCPDiameter.sourPort)
                                {
                                    if (Convert.ToDouble(diameterTimeCDR[j]) - Convert.ToDouble(beginTimeCDR) < 30 && Convert.ToDouble(diameterTimeCDR[j]) - Convert.ToDouble(beginTimeCDR) >= 0)
                                    {
                                        if (TCPDiameter.userNameStr == imsi)
                                        {
                                            endTime = diameterTime[j - SCTPDiameterClone.Count];
                                            endTimeCDR = diameterTimeCDR[j - SCTPDiameterClone.Count];

                                            signaling_Diameter.ID_diamter = TCPDiameterID[j - SCTPDiameterClone.Count];
                                            signaling_Diameter.time = diameterTime[j - SCTPDiameterClone.Count];
                                            signaling_Diameter.re = TCPDiameter.diameterDirection;
                                            signaling_Diameter.command = TCPDiameter.command;
                                            signaling_Diameter.diameterFrame = diameterClone_TCP[j - SCTPDiameterClone.Count];
                                            signaling_Diameter.protocol = "diameter-tcp";
                                            diameterSignaling.Add(signaling_Diameter);
                                            num1[Convert.ToInt16(ID1)]++;
                                            num++;
                                            while (j == TCPDiameterClone.Count + SCTPDiameterClone.Count)
                                            {
                                                diameterCDR.CDRbuffer = diameterSignaling;
                                                DIAMETERCDR.Add(diameterCDR);
                                                AddItem_DiameterCDR(ID1, beginTime, endTime, sessionID, Interface, imsi);
                                                listView1Delegate listDelegate = new listView1Delegate(AddItem_DiameterCDR);
                                                protocol[Convert.ToInt16(ID1)] = "tcp";
                                                break;
                                            }
                                            goto F3;
                                        }
                                        else
                                        {
                                            diameterCDR.CDRbuffer = diameterSignaling;
                                            DIAMETERCDR.Add(diameterCDR);
                                            AddItem_DiameterCDR(ID1, beginTime, endTime, sessionID, Interface, imsi);
                                            listView1Delegate listDelegate = new listView1Delegate(AddItem_DiameterCDR);
                                            protocol[Convert.ToInt16(ID1)] = "tcp";
                                            goto F4;
                                        }
                                    }
                                }
                                else
                                {
                                    diameterCDR.CDRbuffer = diameterSignaling;
                                    DIAMETERCDR.Add(diameterCDR);
                                    AddItem_DiameterCDR(ID1, beginTime, endTime, sessionID, Interface, imsi);
                                    listView1Delegate listDelegate = new listView1Delegate(AddItem_DiameterCDR);
                                    protocol[Convert.ToInt16(ID1)] = "tcp";
                                    //num++;
                                    goto F4;
                                }
                            }
                            else if (applicationid == @"3GPP S6a/S6d(16777251)")
                            {
                                diameterCDR.CDRbuffer = diameterSignaling;
                                DIAMETERCDR.Add(diameterCDR);
                                AddItem_DiameterCDR(ID1, beginTime, endTime, sessionID, Interface, imsi);
                                listView1Delegate listDelegate = new listView1Delegate(AddItem_DiameterCDR);
                                protocol[Convert.ToInt16(ID1)] = "tcp";
                                num++;
                                goto F4;
                            }
                            applicationid = TCPDiameter.ApplicationID;
                            break;
                        }
                    }
                    num++;
                }
                #endregion
            }
            changePcap_Diameter = false;//黄罡
            //DIAMETER CDR合成

            //按钮不能再次按下
            //button_DIAMETER.Enabled = false;
            if (tabControl1.TabPages.Count >= 0)
            {
                bool isExist = false;
                for (int i = 0; i < tabControl1.TabPages.Count; i++)
                {
                    if (tabControl1.TabPages[i] == tabPage4)
                    {
                        tabControl1.SelectedTab = tabPage4;
                        isExist = true;
                        break;
                    }
                }
                if (!isExist)
                {
                    tabControl1.TabPages.Add(tabPage4);
                    tabControl1.SelectedTab = tabPage4;
                }
            }
        }

        private void AddItem_DiameterCDR(string ID, string beginTime, string endtime, string sessionID, string Interface, string imsi)
        {
            listView_DIAMETER.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            listView_DIAMETER.Items.Add(new ListViewItem(new string[] { ID, beginTime, endtime, sessionID, Interface, imsi }));
            //UpdateStatus();
        }


        List<DiameterCDRStruct> DIAMETERCDR_Process = new List<DiameterCDRStruct>();//用于存储diameter的CDR列表
        List<DiameterStruct> diameterSignaling_Process;//一条CDR中包含的信令的列表
        DiameterCDRStruct diameterCDR_Process;//用于存储diameter的一条CDR
        static public int kasmeCount = -1;//在解析nas时，当有新的Kasme出现时，置为-1

        private void button_Process_Click_1(object sender, EventArgs e)//多段关联功能
        {
            if (changePcap_combine)
            {
                if (!tabpage_CDR)
                {
                    this.tabControl1.TabPages.Add(tabPage6);
                    this.tabControl1.SelectedTab = this.tabPage6;
                }

                listView_ProcessIMSI.Items.Clear();
                listView_Process.Items.Clear();
                //S1AP 多段关联数据
                #region
                string IMSI = string.Empty;//                
                List<CdrIdentity> AllCdrIdentity = new List<CdrIdentity>();//用于存储所有cdr的特征值（idpair，imsi，tmsi）
                List<List<process.S1apContent>> CdrListCombine = new List<List<process.S1apContent>>();//存放所有的CDR单元
                List<process.IdPair> CdrIDPairCombine = new List<process.IdPair>();//存放所有CDR对应的IDpair
                //CDR合成
                Process.CdrCreate(S1apKey6All, S1apContentAll);
                CdrListCombine = Process.CdrList;
                CdrIDPairCombine = Process.CdrId;
                for (int j = 0; j < CdrListCombine.Count && CdrListCombine.Count > 0; j++)
                {
                    #region
                    CdrIdentity SingleCdrIdentity = new CdrIdentity();//用于存储当前cdr的特征值（idpair，imsi，tmsi）
                    string[] iden = new string[3] { "", "", "" };//将cdr中的用户标识传给CDR（key4），且在一个cdr中最多有两个标识                    
                    IMSI = "";
                    SingleCdrIdentity.mmeUeS1apId = CdrIDPairCombine[j].MmeUeS1apId.ToString();
                    SingleCdrIdentity.enbUeS1apId = CdrIDPairCombine[j].EnbUeS1apId.ToString();
                    for (int k = 0, p = 0; k < CdrListCombine[j].Count && CdrListCombine[j].Count > 0; k++)//搜集当前CDR中的用户标识
                    {
                        #region
                        if (CdrListCombine[j][k].s1ap_identity != "")
                        {
                            iden[p] = CdrListCombine[j][k].s1ap_identity;
                            p++;
                        }

                        if (CdrListCombine[j][k].nas_identity != "")
                        {
                            iden[p] = CdrListCombine[j][k].nas_identity;
                            p++;
                        }
                        #endregion
                    }
                    if (iden[0] != "")
                        SingleCdrIdentity.identity1 = iden[0];
                    else
                        SingleCdrIdentity.identity1 = "";
                    if (iden[1] != "")
                        SingleCdrIdentity.identity2 = iden[1];//将cdr中的用户标识提取到CDR（key4）中
                    else
                        SingleCdrIdentity.identity2 = "";
                    AllCdrIdentity.Add(SingleCdrIdentity);
                    #endregion
                }
                //该部分程序作用：将零散的CDR划分到所属的UE中，并以UE为单位储存                               
                //下面的循环完成了将flow1中零散的cdr按照UE为单元放置在UeS1apProcessList中，UeS1apProcessList的单元是特定UE以ISMI一次发起的所有通信流程                         
                for (int u = 0; CdrListCombine.Count > 0 && u < CdrListCombine.Count; u = 0)
                {
                    #region
                    List<List<process.S1apContent>> CdrRest = new List<List<process.S1apContent>>();//经过一次合成，与当前CDR不是同一个通信流程的CDR暂时存放在cdrRest中
                    List<CdrIdentity> CdrRestIdentity = new List<CdrIdentity>();//存放的是与CdrRest对应的特征值
                    List<List<process.S1apContent>> UeS1apProcess = new List<List<process.S1apContent>>();//存放的单元是CDR合成后的s1ap通信流程
                    string CdrTmsi = "";//用于传递cdr中的TMSI                
                    string CdrId1 = "";//用于记录当前的cdr的特征值中的mme_ue_s1ap_id
                    DiameterKASME kasme0 = new DiameterKASME();//特定终端所使用的密钥组  

                    if (AllCdrIdentity[u].identity1.Length > 8)//一个通信流程最开始的CDR，一定是有IMSI的cdr
                    {
                        #region
                        NAS_DEC Nasdecrypt1 = new NAS_DEC();
                        //获取首个带有IMSI的CDR的密文用户标识
                        #region
                        CdrTmsi = AllCdrIdentity[u].identity1;
                        CdrId1 = AllCdrIdentity[u].mmeUeS1apId;
                        kasmeCount = -1;//初始化使用的密钥序号
                        //获取当前终端的IMSI
                        UeImsiList.Add(AllCdrIdentity[u].identity1);
                        //当前终端的CDR链中添加CDR
                        UeS1apProcess.Add(CdrListCombine[u]);

                        CdrId1 = AllCdrIdentity[u].mmeUeS1apId;
                        //获得该IMSI对应的密钥组
                        for (int p = 0; p < KASME.Count; p++)
                        {
                            #region
                            if (KASME[p].imsi == AllCdrIdentity[u].identity1)
                            {
                                kasme0 = KASME[p];
                                break;
                            }
                            #endregion
                        }
                        //该部分是为了找到一个CDR中的密文用户标识
                        string CurrentTMSI = "";
                        List<byte[]> kasmeOld = new List<byte[]>();//存储上一条信令使用的一组kasme
                        List<byte[]> kasme_current = new List<byte[]>();
                        int count = -1;//该终端请求分配密钥的次数
                        if (kasme0.Kasme != null)
                            count = kasme0.Kasme.Count;//一个终端在其流程中分配的KASME次数（不是个数）
                        else
                            count = 0;
                        for (int i = 0; i < CdrListCombine[u].Count; i++)//获得第一个带有IMSI CDR块的密文tmsi
                        {
                            #region
                            if (CdrListCombine[u][i].NasChunk.Length > 1)
                            {
                                #region
                                int direction = CdrListCombine[u][i].direction;//当前信令的方向 
                                if (((CdrListCombine[u][i].NasChunk[0] & 0xf0) >> 4) == 4 || ((CdrListCombine[u][i].NasChunk[0] & 0xf0) >> 4) == 2)//只选择加密的nas
                                {
                                    #region
                                    for (int k = 0; k < count && count > 0; k++)
                                    {
                                        #region
                                        if (count > k + 1)
                                        {
                                            #region
                                            //当前信令的时间要晚于当前nas加密所采用的根密钥的分配时间，且当前信令时间要早于下一次密钥分配时间
                                            if (double.Parse(CdrListCombine[u][i].timeNas) > (double.Parse(kasme0.Kasme[k].time)) && double.Parse(CdrListCombine[u][i].timeNas) < (double.Parse(kasme0.Kasme[k + 1].time)))
                                            {
                                                kasme_current = kasme0.Kasme[k].kasme;
                                                break;
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                            #endregion
                                        }
                                        else if (count == k + 1 && double.Parse(CdrListCombine[u][i].timeNas) >
                                                   (double.Parse(kasme0.Kasme[count - 1].time)))//当前信令之后再没有分配新的密钥
                                        {
                                            kasme_current = kasme0.Kasme[count - 1].kasme;
                                            break;
                                        }
                                        else
                                            kasme_current.Clear();
                                        #endregion
                                    }
                                    bool bool1 = equalList(kasmeOld, kasme_current);//比较上次与本次的Kasme是否属于同一次分配的，若不是，则使用的Kasme序数要从头开始
                                    //获得合适的解密密钥(加密nas的解析)
                                    if (bool1 == true)//前后使用的Kasme是同一组，则,kasme_conut不用置为-1
                                    {
                                        nas_dec.nas_decode1(CdrListCombine[u][i].chunk, kasme_current, direction);
                                        kasmeOld = kasme_current;
                                    }
                                    else
                                    {
                                        kasmeCount = -1;
                                        nas_dec.nas_decode1(CdrListCombine[u][i].chunk, kasme_current, direction);
                                        kasmeOld = kasme_current;
                                    }
                                    #endregion
                                }                      
                                else//未加密的nas解析
                                {
                                    List<byte[]> KeyNull = new List<byte[]>();//不加密的NAS不需要密钥组
                                    nas_dec.nas_decode1(CdrListCombine[u][i].chunk, KeyNull, direction);
                                }
                                //获得第一个CDR块中的加密NAS中的GUTI
                                CurrentTMSI = nas_dec.identity;
                                //判断当前信令中是否有用户标识，如果有,且不是明文用户标识则说明已经解析出密文tmsi，不再解析其他信令
                                if (CurrentTMSI != "" && CurrentTMSI != AllCdrIdentity[u].identity1)
                                    break;
                                else
                                    ;
                                #endregion
                            }
                            else//该s1ap信令不含NAS
                                ;
                            #endregion
                        }
                        CdrTmsi = CurrentTMSI;
                        #endregion
                        //添加属于该终端的后续CDR，制作CDR链
                        for (u++; CdrListCombine.Count > 0 && u < CdrListCombine.Count; u++)
                        {
                            if ((CdrTmsi == AllCdrIdentity[u].identity1) || (AllCdrIdentity[u].identity1 == "" && (CdrId1 == AllCdrIdentity[u].mmeUeS1apId)))//当前CDR链中最后一个CDR的密文tmsi与该CDR的明文TMSI相同或者mmeues1apid相同
                            {
                                #region
                                UeS1apProcess.Add(CdrListCombine[u]);
                                CdrId1 = AllCdrIdentity[u].mmeUeS1apId;
                                for (int i = 0; i < CdrListCombine[u].Count; i++)
                                {
                                    //获得当前CDR的密文用户标识
                                    #region
                                    if (CdrListCombine[u][i].NasChunk.Length > 1)
                                    {
                                        int direction = CdrListCombine[u][i].direction;//当前信令的方向                                               
                                        for (int k = 0; k < count && count > 0; k++)
                                        {
                                            #region
                                            if (count > k + 1)
                                            {
                                                //当前信令的时间要晚于当前nas加密所采用的根密钥的分配时间，且当前信令时间要早于下一次密钥分配时间
                                                #region
                                                if (double.Parse(CdrListCombine[u][i].timeNas) > (double.Parse(kasme0.Kasme[k].time)) && double.Parse(CdrListCombine[u][i].timeNas) < (double.Parse(kasme0.Kasme[k + 1].time)))
                                                {
                                                    kasme_current = kasme0.Kasme[k].kasme;
                                                    break;
                                                }
                                                else
                                                {
                                                    continue;
                                                }
                                                #endregion
                                            }
                                            else if (count == k + 1 && double.Parse(CdrListCombine[u][i].timeNas) >
                                                (double.Parse(kasme0.Kasme[count - 1].time)))//当前信令之后再没有分配新的密钥
                                            {
                                                kasme_current = kasme0.Kasme[count - 1].kasme;
                                                break;
                                            }
                                            else//没有合适的密钥
                                                kasme_current.Clear();//没有
                                            #endregion
                                        }
                                        bool bool1 = equalList(kasmeOld, kasme_current);//比较上次与本次的Kasme是否属于同一次分配的，若不是，则使用的Kasme序数要从头开始
                                        //获得该CDR中的密文用户标识
                                        #region
                                        if (bool1 == true)//前后使用的Kasme是同一组，则,kasme_conut不用置为-1
                                        {
                                            Nasdecrypt1.nas_decode1(CdrListCombine[u][i].chunk, kasme_current, direction);
                                            kasmeOld = kasme_current;
                                        }
                                        else
                                        {
                                            kasmeCount = -1;
                                            Nasdecrypt1.nas_decode1(CdrListCombine[u][i].chunk, kasme_current, direction);
                                            kasmeOld = kasme_current;
                                        }
                                        CurrentTMSI = Nasdecrypt1.identity;
                                        //判断当前信令中是否有用户标识，如果有,且不是明文用户标识则说明已经解析出密文tmsi，不再解析其他信令
                                        if (CurrentTMSI != "" && CurrentTMSI != AllCdrIdentity[u].identity1)
                                            break;
                                        else
                                            ;
                                        #endregion
                                    }
                                    #endregion
                                }
                                CdrTmsi = CurrentTMSI;
                                #endregion
                            }
                            else//当前的CDR不属于当前终端
                            {
                                CdrRest.Add(CdrListCombine[u]);
                                CdrRestIdentity.Add(AllCdrIdentity[u]);
                            }
                        }
                        #endregion
                    }
                    else//此处需要区分当用户标识不是IMSI时，该CDR是否有安全模式流程（count重置为0）
                    {
                        #region                                      //变量
                        NAS_DEC Nasdecrypt2 = new NAS_DEC();
                        string CurrentImsi = "";
                        string CurrentTmsi = "";
                        kasmeCount = -1;
                        List<byte[]> KeyNew = new List<byte[]>();
                        List<byte[]> KeyOld = new List<byte[]>();
                        #endregion
                        //当没有IMSI时，挑选合适制作CDR链的首个CDR块
                        #region
                        bool SMC = false;//判断当前CDR中是否有新的安全上下文（COUNT重置为0）
                        string CurrentTime = "";//记录收条加密信令的时间
                        string KeyTime = "";//记录首条加密信令使用的密钥的分配时间
                        for (int no = 0; no < CdrListCombine[u].Count; no++)
                        {
                            if (CdrListCombine[u][no].NasChunk.Length > 1 && CdrListCombine[u][no].NasChunk[0] == 0x47)
                            {
                                SMC = true;
                                CurrentTime = CdrListCombine[u][no].timeNas;
                                break;
                            }
                            else
                                SMC = false;
                        }
                        #endregion
                        if (SMC == true)//有新的安全上下文，可以使用穷举法
                        {
                            #region
                            #region
                            CdrTmsi = AllCdrIdentity[u].identity1;
                            CdrId1 = AllCdrIdentity[u].mmeUeS1apId;
                            //kasmeCount = -1;//初始化使用的密钥序号   ????是否需要                                              
                            //准备穷举法所需要的密钥
                            List<DiameterKASME> KeyUnkonwn = new List<DiameterKASME>();//存储早于首条加密nas的密钥    
                            #endregion
                            for (int NumImsi = 0; NumImsi < KASME.Count; NumImsi++)
                            {
                                #region
                                DiameterKASME Key = new DiameterKASME();//用于存储当前IMSI所属的符合条件的密钥组                                
                                Key.Kasme = new List<diameterKASME>();
                                Key.imsi = KASME[NumImsi].imsi;
                                for (int NumTime = 0; NumTime < KASME[NumImsi].Kasme.Count; NumTime++)
                                {
                                    if (double.Parse(CurrentTime) > double.Parse(KASME[NumImsi].Kasme[NumTime].time))
                                    {
                                        Key.Kasme.Add(KASME[NumImsi].Kasme[NumTime]);
                                    }
                                }
                                if (Key.Kasme.Count > 0)
                                    KeyUnkonwn.Add(Key);
                                #endregion
                            }
                            //获得连续两条加密NAS
                            #region
                            List<process.S1apContent> CiperNas = new List<process.S1apContent>();
                            bool header04 = false;
                            //在穷举法之前需要先获得加密算法
                            for (int no1 = 0; no1 < CdrListCombine[u].Count; no1++)
                            {
                                byte alg = 4;//非正常加密算法
                                if (CdrListCombine[u][no1].NasChunk.Length > 1 && CdrListCombine[u][no1].nas_name == "Security mode command")
                                {
                                    int direction = CdrListCombine[u][no1].direction;
                                    List<byte[]> KeyNull = new List<byte[]>();
                                    Nasdecrypt2.nas_decode1(CdrListCombine[u][no1].NasChunk, KeyNull, direction);
                                    alg = Nasdecrypt2.algorithm;
                                }
                                if (alg > 0 && alg < 4)
                                    break;
                            }
                            for (int no1 = 0; no1 < CdrListCombine[u].Count; no1++)
                            {
                                if (CdrListCombine[u][no1].NasChunk.Length > 1 && CdrListCombine[u][no1].NasChunk[0] == 0x47)
                                {
                                    CiperNas.Add(CdrListCombine[u][no1]);
                                    header04 = true;
                                }
                                if (header04 == true && CiperNas.Count < 2 && CdrListCombine[u][no1].NasChunk.Length > 1 && ((CdrListCombine[u][no1].NasChunk[0] & 0xf0) >> 4) == 2)//保证该NAS是使用安全上下文之后的nas
                                    CiperNas.Add(CdrListCombine[u][no1]);
                                if (CiperNas.Count == 2)
                                    break;
                            }
                            #endregion
                            //获得合适的密钥
                            for (int NoImsi = 0; NoImsi < KeyUnkonwn.Count; NoImsi++)//循环IMSI
                            {
                                #region
                                for (int NoTime = 0; NoTime < KeyUnkonwn[NoImsi].Kasme.Count; NoTime++)//假设终端使用的密钥一定是分配的密钥组中的第一个密钥
                                {
                                    int direction1 = CiperNas[0].direction;
                                    Nasdecrypt2.nas_decode1(CiperNas[0].chunk, KeyUnkonwn[NoImsi].Kasme[NoTime].kasme, direction1);//此处的密钥一定是个密钥组，不能指定是哪一个密钥，密钥的序号由kasmeCount决定
                                    string NasName1 = Nasdecrypt2.nas_name;
                                    if (NasName1 != " ")
                                    {
                                        int direction2 = CiperNas[1].direction;
                                        Nasdecrypt2.nas_decode1(CiperNas[1].chunk, KeyUnkonwn[NoImsi].Kasme[NoTime].kasme, direction2);
                                        string NasName2 = Nasdecrypt2.nas_name;
                                        if (NasName1 != " ")//当前密钥组是正确的密钥组
                                        {
                                            CurrentImsi = KeyUnkonwn[NoImsi].imsi;
                                            KeyTime = KeyUnkonwn[NoImsi].Kasme[NoTime].time;
                                            break;
                                        }
                                        else//当前的密钥不是真正的密钥
                                            ;
                                    }
                                    else //当前的密钥不是真正的密钥
                                        ;
                                }
                                if (CurrentImsi != "")//此时已经找到了合适的密钥，以及对应的IMSI
                                    break;
                                #endregion
                            }
                            UeImsiList.Add(CurrentImsi);//向IMSI列表中添加当前CDR对应的IMSI
                            UeS1apProcess.Add(CdrListCombine[u]);//向CDR链中添加当前的CDR
                            //获得当前IMSI对应的时间合适的所有密钥
                            List<diameterKASME> KeyKonwn = new List<diameterKASME>();//存储当前IMSI对应的时间合适的密钥组（时间大于等于KeyTime的密钥组）
                            for (int NumImsi = 0; NumImsi < KASME.Count; NumImsi++)
                            {
                                #region
                                if (KASME[NumImsi].imsi == CurrentImsi)
                                {
                                    for (int NumTime = 0; NumTime < KASME[NumImsi].Kasme.Count; NumTime++)
                                    {
                                        if (double.Parse(KASME[NumImsi].Kasme[NumTime].time) >= double.Parse(KeyTime))
                                            KeyKonwn.Add(KASME[NumImsi].Kasme[NumTime]);
                                    }
                                }
                                #endregion
                            }
                            //获取当前CDR中的密文用户标识（由于在穷举法中，kasmeCount已经改变，所以在此需要重置为-1）
                            kasmeCount = -1;
                            for (int NoNAS = 0; NoNAS < CdrListCombine[u].Count; NoNAS++)//此处不是所有的NAS都需要判断使用的是那组密钥，只有加密的信令才需要判断#####
                            {
                                #region
                                if (CdrListCombine[u][NoNAS].NasChunk.Length > 1)
                                {
                                    int direction = CdrListCombine[u][NoNAS].direction;//当前信令的方向
                                    if (((CdrListCombine[u][NoNAS].NasChunk[0] & 0xf0) >> 4) == 4 || ((CdrListCombine[u][NoNAS].NasChunk[0] & 0xf0) >> 4) == 2)
                                    {
                                        for (int NoTime = 0; KeyKonwn.Count > 0 && NoTime < KeyKonwn.Count; NoTime++)
                                        {
                                            #region
                                            if (KeyKonwn.Count > NoTime + 1)
                                            {
                                                #region
                                                if (double.Parse(CdrListCombine[u][NoTime].timeNas) > (double.Parse(KeyKonwn[NoTime].time)) && double.Parse(CdrListCombine[u][NoTime].timeNas) < (double.Parse(kasme0.Kasme[NoTime + 1].time)))
                                                {
                                                    KeyNew = KeyKonwn[NoTime].kasme;
                                                    break;
                                                }
                                                else
                                                {
                                                    continue;
                                                }
                                                #endregion
                                            }
                                            else if (KeyKonwn.Count == NoTime + 1 && double.Parse(CdrListCombine[u][NoNAS].timeNas) >
                                                    (double.Parse(KeyKonwn[KeyKonwn.Count - 1].time)))//当前信令之后再没有分配新的密钥
                                            {
                                                KeyNew = KeyKonwn[KeyKonwn.Count - 1].kasme;
                                                break;
                                            }
                                            else
                                                KeyNew.Clear();
                                            #endregion
                                        }
                                        bool bool1 = equalList(KeyOld, KeyNew);
                                        //解码当前的CDR中的信令内容，获得密文用户标识
                                        #region
                                        if (bool1 == true)//前后使用的Kasme是同一组，则,kasme_conut不用置为-1
                                        {
                                            Nasdecrypt2.nas_decode1(CdrListCombine[u][NoNAS].chunk, KeyNew, direction);
                                            KeyOld = KeyNew;
                                        }
                                        else
                                        {
                                            kasmeCount = -1;
                                            Nasdecrypt2.nas_decode1(CdrListCombine[u][NoNAS].chunk, KeyNew, direction);
                                            KeyOld = KeyNew;
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        List<byte[]> KeyNull = new List<byte[]>();//不加密的NAS不需要密钥组
                                        Nasdecrypt2.nas_decode1(CdrListCombine[u][NoNAS].chunk, KeyNull, direction);
                                    }
                                    #region

                                    CurrentTmsi = Nasdecrypt2.identity;
                                    //判断当前信令中是否有用户标识，如果有,且不是明文用户标识则说明已经解析出密文tmsi，不再解析其他信令
                                    if (CurrentTmsi != "" && CurrentTmsi != AllCdrIdentity[u].identity1)
                                        break;
                                    else
                                        ;
                                    #endregion
                                }
                                else ;//无nas消息
                                #endregion
                            }
                            CdrTmsi = CurrentTmsi;//CdrTmsi代表CDR链中最后一个CDR的密文用户标识
                            //添加属于该终端的后续CDR，制作CDR链
                            for (u++; CdrListCombine.Count > 0 && u < CdrListCombine.Count; u++)
                            {
                                #region
                                if ((CdrTmsi == AllCdrIdentity[u].identity1) || (AllCdrIdentity[u].identity1 == "" && (CdrId1 == AllCdrIdentity[u].mmeUeS1apId)))//当前CDR链中最后一个CDR的密文tmsi与该CDR的明文TMSI相同或者mmeues1apid相同
                                {
                                    #region
                                    UeS1apProcess.Add(CdrListCombine[u]);
                                    CdrId1 = AllCdrIdentity[u].mmeUeS1apId;
                                    for (int i = 0; i < CdrListCombine[u].Count; i++)
                                    {
                                        //获得当前CDR的密文用户标识                                  
                                        if (CdrListCombine[u][i].NasChunk.Length > 1)
                                        {
                                            #region
                                            int direction = CdrListCombine[u][i].direction;//当前信令的方向 
                                            for (int NoTime = 0; KeyKonwn.Count > 0 && NoTime < KeyKonwn.Count; NoTime++)
                                            {
                                                #region
                                                if (KeyKonwn.Count > NoTime + 1)
                                                {
                                                    if (double.Parse(CdrListCombine[u][NoTime].timeNas) > (double.Parse(KeyKonwn[NoTime].time)) && double.Parse(CdrListCombine[u][NoTime].timeNas) < (double.Parse(kasme0.Kasme[NoTime + 1].time)))
                                                    {
                                                        KeyNew = KeyKonwn[NoTime].kasme;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        continue;
                                                    }
                                                }
                                                else if (KeyKonwn.Count == NoTime + 1 && double.Parse(CdrListCombine[u][i].timeNas) >
                                                (double.Parse(KeyKonwn[KeyKonwn.Count - 1].time)))//当前信令之后再没有分配新的密钥
                                                {
                                                    KeyNew = KeyKonwn[KeyKonwn.Count - 1].kasme;
                                                    break;
                                                }
                                                else
                                                    KeyNew.Clear();
                                                #endregion
                                            }
                                            bool bool1 = equalList(KeyOld, KeyNew);
                                            //解码当前的CDR中的信令内容，获得密文用户标识
                                            if (bool1 == true)//前后使用的Kasme是同一组，则,kasme_conut不用置为-1
                                            {
                                                Nasdecrypt2.nas_decode1(CdrListCombine[u][i].chunk, KeyNew, direction);
                                                KeyOld = KeyNew;
                                            }
                                            else
                                            {
                                                kasmeCount = -1;
                                                Nasdecrypt2.nas_decode1(CdrListCombine[u][i].chunk, KeyNew, direction);
                                                KeyOld = KeyNew;
                                            }
                                            CurrentTmsi = Nasdecrypt2.identity;
                                            //判断当前信令中是否有用户标识，如果有,且不是明文用户标识则说明已经解析出密文tmsi，不再解析其他信令
                                            if (CurrentTmsi != "" && CurrentTmsi != AllCdrIdentity[u].identity1)
                                                break;
                                            else
                                                ;
                                            #endregion
                                        }
                                        else ;//无nas消息                                       
                                    }
                                    CdrTmsi = CurrentTmsi;//CdrTmsi代表CDR链中最后一个CDR的密文用户标识
                                    #endregion
                                }
                                else
                                {
                                    CdrRest.Add(CdrListCombine[u]);
                                    CdrRestIdentity.Add(AllCdrIdentity[u]);
                                }
                                #endregion
                            }
                            #endregion
                        }
                        else //该CDR中既没有IMSI，且没有使用新的安全上下文，故不能作为一个UE CDR链的首个CDR，需将其抛弃掉，不能放在CdrRest中
                        {
                            ;
                        }
                    }
                    #endregion
                    CdrListCombine = CdrRest;
                    AllCdrIdentity = CdrRestIdentity;
                    UeS1apProcessList.Add(UeS1apProcess);//存放所有UE的CDR链
                }
                #endregion
                //diameter多段关联数据
                DiameterStruct diameterFlow;
                #region Diameter
                string ID1 = "0";
                string beginTimeCDR = "";
                string endTimeCDR = "";
                string beginTime = "";
                string endTime = "";
                string sessionID = string.Empty;
                string Interface = string.Empty;
                string sourceIP = "";
                string sourcePort = "";
                string destIP = "";
                string destPort = "";
                int num = 0;
                string applicationid = "";
                string imsi = "";

            F2:
                for (int i = num; i < SCTPDiameterClone.Count; i++)
                {

                    while (SCTPDiameterClone[i] != null)
                    {
                        SCTP.DecodeSctpChunk(SCTPDiameterClone[i]);
                        SCTP.decoder_All(diameterClone_SCTP[i]);
                        if (SCTP.applicationID == @"3GPP S6a/S6d(16777251)")
                        {
                            diameterSignaling_Process = new List<DiameterStruct>();
                            ID1 = Convert.ToString(Convert.ToInt16(ID1) + 1);
                            sessionID = SCTP.avp_SessionID4;
                            imsi = SCTP.userNameStr;
                            diameterCDR_Process.CDRIMSI = imsi;
                            Interface = "S6a";
                            beginTime = diameterTime[i];
                            beginTimeCDR = diameterTimeCDR[i];
                            endTime = diameterTime[i];
                            endTimeCDR = diameterTimeCDR[i];
                            applicationid = SCTP.applicationID;
                            sourceIP = SCTP.sourIP;
                            sourcePort = SCTP.sourPort;
                            destIP = SCTP.destIP;
                            destPort = SCTP.destPort;
                            diameterFlow.ID_diamter = SCTPDiameterID[i];
                            diameterFlow.time = diameterTime[i];
                            diameterFlow.re = SCTP.diameterDirection;
                            diameterFlow.command = SCTP.command;
                            diameterFlow.diameterFrame = diameterClone_SCTP[i];
                            diameterFlow.protocol = "diameter-sctp";
                            diameterSignaling_Process.Add(diameterFlow);
                            num++;
                            goto F1;
                        }
                        break;
                    }
                    num++;
                }
            F1:
                for (int j = num; j < SCTPDiameterClone.Count; j++)
                {
                    while (j != SCTPDiameterClone.Count)
                    {
                        SCTP.DecodeSctpChunk(SCTPDiameterClone[j]);
                        SCTP.decoder_All(diameterClone_SCTP[j]);

                        if (SCTP.applicationID == @"3GPP S6a/S6d(16777251)")
                        {
                            if ((sourceIP == SCTP.sourIP && sourcePort == SCTP.sourPort && destIP == SCTP.destIP && destPort == SCTP.destPort) && (Convert.ToDouble(diameterTimeCDR[j]) - Convert.ToDouble(beginTimeCDR) < 30 && Convert.ToDouble(diameterTimeCDR[j]) - Convert.ToDouble(beginTimeCDR) >= 0))
                            {
                                if (SCTP.userNameStr == imsi)
                                {
                                    endTime = diameterTime[j];
                                    endTimeCDR = diameterTimeCDR[j];
                                    diameterFlow.ID_diamter = SCTPDiameterID[j];
                                    diameterFlow.time = diameterTime[j];
                                    diameterFlow.re = SCTP.diameterDirection;
                                    diameterFlow.command = SCTP.command;
                                    diameterFlow.diameterFrame = diameterClone_SCTP[j];
                                    diameterFlow.protocol = "diameter-sctp";
                                    diameterSignaling_Process.Add(diameterFlow);
                                    num++;
                                    while ((j + 1) == diameterClone_SCTP.Count)
                                    {
                                        diameterCDR_Process.CDRbuffer = diameterSignaling_Process;
                                        DIAMETERCDR_Process.Add(diameterCDR_Process);
                                        protocol[Convert.ToInt16(ID1)] = "sctp";
                                        break;
                                    }
                                    goto F1;
                                }
                                else
                                {
                                    diameterCDR_Process.CDRbuffer = diameterSignaling_Process;
                                    DIAMETERCDR_Process.Add(diameterCDR_Process);
                                    protocol[Convert.ToInt16(ID1)] = "sctp";
                                    goto F2;
                                }
                            }
                            else if ((sourceIP == SCTP.destIP && sourcePort == SCTP.destPort && destIP == SCTP.sourIP && destPort == SCTP.sourPort) && (Convert.ToDouble(diameterTimeCDR[j]) - Convert.ToDouble(beginTimeCDR) < 30 && Convert.ToDouble(diameterTimeCDR[j]) - Convert.ToDouble(beginTimeCDR) >= 0))
                            {
                                if (SCTP.userNameStr == imsi)
                                {
                                    endTime = diameterTime[j];
                                    endTimeCDR = diameterTimeCDR[j];

                                    diameterFlow.ID_diamter = SCTPDiameterID[j];
                                    diameterFlow.time = diameterTime[j];
                                    diameterFlow.re = SCTP.diameterDirection;
                                    diameterFlow.command = SCTP.command;
                                    diameterFlow.diameterFrame = diameterClone_SCTP[j];
                                    diameterFlow.protocol = "diameter-sctp";
                                    diameterSignaling_Process.Add(diameterFlow);
                                    num++;
                                    while ((j + 1) == diameterClone_SCTP.Count)
                                    {
                                        diameterCDR_Process.CDRbuffer = diameterSignaling_Process;
                                        DIAMETERCDR_Process.Add(diameterCDR_Process);
                                        protocol[Convert.ToInt16(ID1)] = "sctp";
                                        break;
                                    }
                                    goto F1;
                                }
                                else
                                {
                                    diameterCDR_Process.CDRbuffer = diameterSignaling_Process;
                                    DIAMETERCDR_Process.Add(diameterCDR_Process);
                                    protocol[Convert.ToInt16(ID1)] = "sctp";
                                    goto F2;
                                }
                            }
                            else
                            {
                                diameterCDR_Process.CDRbuffer = diameterSignaling_Process;
                                DIAMETERCDR_Process.Add(diameterCDR_Process);

                                protocol[Convert.ToInt16(ID1)] = "sctp";
                                goto F2;
                            }


                        }

                        else if (applicationid == @"3GPP S6a/S6d(16777251)")
                        {
                            diameterCDR_Process.CDRbuffer = diameterSignaling_Process;
                            DIAMETERCDR_Process.Add(diameterCDR_Process);

                            protocol[Convert.ToInt16(ID1)] = "sctp";
                            num++;
                            goto F2;
                        }
                        applicationid = SCTP.applicationID;
                        break;
                    }


                }
                if (TCPDiameterClone.Count != 0)
                {
                F4:
                    for (int i = num; i < TCPDiameterClone.Count + SCTPDiameterClone.Count; i++)
                    {

                        while (TCPDiameterClone[i - SCTPDiameterClone.Count] != null)
                        {
                            TCPDiameter.DecodeTcpChunk(TCPDiameterClone[i - SCTPDiameterClone.Count]);
                            TCPDiameter.decoder_All(diameterClone_TCP[i - SCTPDiameterClone.Count]);
                            if (TCPDiameter.ApplicationID == @"3GPP S6a/S6d(16777251)")
                            {
                                diameterSignaling_Process = new List<DiameterStruct>();
                                ID1 = Convert.ToString(Convert.ToInt16(ID1) + 1);
                                sessionID = TCPDiameter.avp_SessionID4;
                                imsi = TCPDiameter.userNameStr;
                                diameterCDR_Process.CDRIMSI = imsi;
                                Interface = "S6a";
                                beginTime = diameterTime[i - SCTPDiameterClone.Count];
                                endTime = diameterTime[i - SCTPDiameterClone.Count];
                                beginTimeCDR = diameterTimeCDR[i - SCTPDiameterClone.Count];
                                endTimeCDR = diameterTimeCDR[i - SCTPDiameterClone.Count];
                                applicationid = TCPDiameter.ApplicationID;
                                sourceIP = TCPDiameter.sourIP;
                                sourcePort = TCPDiameter.sourPort;
                                destIP = TCPDiameter.destIP;
                                destPort = TCPDiameter.destPort;
                                diameterFlow.ID_diamter = TCPDiameterID[i - SCTPDiameterClone.Count];
                                diameterFlow.time = diameterTime[i - SCTPDiameterClone.Count];
                                diameterFlow.re = TCPDiameter.diameterDirection;
                                diameterFlow.command = TCPDiameter.command;
                                diameterFlow.diameterFrame = diameterClone_TCP[i - SCTPDiameterClone.Count];
                                diameterFlow.protocol = "diameter-tcp";
                                diameterSignaling_Process.Add(diameterFlow);
                                num++;
                                goto F3;
                            }
                            break;
                        }
                        num++;
                    }
                F3:
                    for (int j = num; j <= TCPDiameterClone.Count + SCTPDiameterClone.Count; j++)
                    {
                        while (j != TCPDiameterClone.Count + SCTPDiameterClone.Count)
                        {
                            TCPDiameter.DecodeTcpChunk(TCPDiameterClone[j - SCTPDiameterClone.Count]);
                            TCPDiameter.decoder_All(diameterClone_TCP[j - SCTPDiameterClone.Count]);
                            if (TCPDiameter.ApplicationID == @"3GPP S6a/S6d(16777251)")
                            {
                                if (sourceIP == TCPDiameter.sourIP && sourcePort == TCPDiameter.sourPort && destIP == TCPDiameter.destIP && destPort == TCPDiameter.destPort)
                                {
                                    if (Convert.ToDouble(diameterTimeCDR[j]) - Convert.ToDouble(beginTimeCDR) < 30 && Convert.ToDouble(diameterTimeCDR[j]) - Convert.ToDouble(beginTimeCDR) >= 0)
                                    {
                                        if (TCPDiameter.userNameStr == imsi)
                                        {
                                            endTime = diameterTime[j - SCTPDiameterClone.Count];
                                            endTimeCDR = diameterTimeCDR[j - SCTPDiameterClone.Count];
                                            diameterFlow.ID_diamter = TCPDiameterID[j - SCTPDiameterClone.Count];
                                            diameterFlow.time = diameterTime[j - SCTPDiameterClone.Count];
                                            diameterFlow.re = TCPDiameter.diameterDirection;
                                            diameterFlow.command = TCPDiameter.command;
                                            diameterFlow.diameterFrame = diameterClone_TCP[j - SCTPDiameterClone.Count];
                                            diameterFlow.protocol = "diameter-tcp";
                                            diameterSignaling_Process.Add(diameterFlow);
                                            num++;
                                            while (j == TCPDiameterClone.Count + SCTPDiameterClone.Count)
                                            {
                                                diameterCDR_Process.CDRbuffer = diameterSignaling_Process;
                                                DIAMETERCDR_Process.Add(diameterCDR_Process);

                                                protocol[Convert.ToInt16(ID1)] = "tcp";
                                                break;
                                            }
                                            goto F3;
                                        }
                                        else
                                        {
                                            diameterCDR_Process.CDRbuffer = diameterSignaling_Process;
                                            DIAMETERCDR_Process.Add(diameterCDR_Process);
                                            protocol[Convert.ToInt16(ID1)] = "tcp";
                                            goto F4;
                                        }
                                    }
                                }
                                else if (sourceIP == TCPDiameter.destIP && sourcePort == TCPDiameter.destPort && destIP == TCPDiameter.sourIP && destPort == TCPDiameter.sourPort)
                                {
                                    if (Convert.ToDouble(diameterTimeCDR[j]) - Convert.ToDouble(beginTimeCDR) < 30 && Convert.ToDouble(diameterTimeCDR[j]) - Convert.ToDouble(beginTimeCDR) >= 0)
                                    {
                                        if (TCPDiameter.userNameStr == imsi)
                                        {
                                            endTime = diameterTime[j - SCTPDiameterClone.Count];
                                            endTimeCDR = diameterTimeCDR[j - SCTPDiameterClone.Count];

                                            diameterFlow.ID_diamter = TCPDiameterID[j - SCTPDiameterClone.Count];
                                            diameterFlow.time = diameterTime[j - SCTPDiameterClone.Count];
                                            diameterFlow.re = TCPDiameter.diameterDirection;
                                            diameterFlow.command = TCPDiameter.command;
                                            diameterFlow.diameterFrame = diameterClone_TCP[j - SCTPDiameterClone.Count];
                                            diameterFlow.protocol = "diameter-tcp";
                                            diameterSignaling_Process.Add(diameterFlow);
                                            num++;
                                            while (j == TCPDiameterClone.Count + SCTPDiameterClone.Count)
                                            {
                                                diameterCDR_Process.CDRbuffer = diameterSignaling_Process;
                                                DIAMETERCDR_Process.Add(diameterCDR_Process);

                                                protocol[Convert.ToInt16(ID1)] = "tcp";
                                                break;
                                            }
                                            goto F3;
                                        }
                                        else
                                        {
                                            diameterCDR_Process.CDRbuffer = diameterSignaling_Process;
                                            DIAMETERCDR_Process.Add(diameterCDR_Process);
                                            protocol[Convert.ToInt16(ID1)] = "tcp";
                                            goto F4;
                                        }
                                    }
                                }
                                else
                                {
                                    diameterCDR_Process.CDRbuffer = diameterSignaling_Process;
                                    DIAMETERCDR_Process.Add(diameterCDR_Process);
                                    protocol[Convert.ToInt16(ID1)] = "tcp";
                                    goto F4;
                                }


                            }

                            else if (applicationid == @"3GPP S6a/S6d(16777251)")
                            {
                                diameterCDR_Process.CDRbuffer = diameterSignaling_Process;
                                DIAMETERCDR_Process.Add(diameterCDR_Process);

                                protocol[Convert.ToInt16(ID1)] = "tcp";
                                num++;
                                goto F4;
                            }
                            applicationid = TCPDiameter.ApplicationID;
                            break;
                        }

                    }
                    num++;
                }
                #endregion
                //gtpv2多段关联数据
                #region //gtpv2多段关联
                #region  GTPCDR合成
                #region 集合清空
                GTPCDR.Clear();
                GTPsubCDR.Clear();
                subCDR_Copy.Clear();
                subCDR.Clear();
                sortedTEID.Clear();
                nullTEID.Clear();
                TEIDExistClone.Clear();
                TEIDExist.Clear();
                sortedIP.Clear();
                gtpChunkCopy1.Clear();
                gtpChunkCopy2.Clear();
                #endregion
                List<gtpStruct> TeidZore = new List<gtpStruct>();
                subCDR_Copy.Clear();
                foreach (gtpStruct a in gtpChunk1)
                {
                    gtpChunkCopy1.Add(a);
                }
                foreach (gtpStruct a in gtpChunk1)
                {
                    gtpChunkCopy2.Add(a);
                }

                Sort_Ip.FindIpPair(gtpChunkCopy2, gtpChunkCopy1, sortedIP);
                for (int Ipcount = 0; Ipcount < sortedIP.Count; Ipcount++)
                {
                    TEIDExist.Clear();
                    nullTEID.Clear();
                    TEIDExist.Clear();
                    TEIDExistClone.Clear();
                    sortedTEID.Clear();
                    Sort_TEID.Exist_sort(sortedIP[Ipcount], TEIDExist, nullTEID);
                    foreach (gtpStruct l in TEIDExist)
                    {
                        TEIDExistClone.Add(l);
                    }
                    Sort_TEID.Sort(TEIDExist, TEIDExistClone, sortedTEID);
                    foreach (List<gtpStruct> a in sortedTEID)
                    {
                        int TEID;
                        TEID = a[0].gtpFrame[32] + a[0].gtpFrame[33] + a[0].gtpFrame[34] + a[0].gtpFrame[35];    //第一位是时间，从33开始
                        if (TEID == 0)
                        {
                            sortedTEID.Remove(a);
                            foreach (gtpStruct b in a)
                            {
                                TeidZore.Add(b);
                            }
                            break;
                        }
                    }
                    List<List<gtpStruct>> sortedTEID_Copy = new List<List<gtpStruct>>();
                    foreach (List<gtpStruct> a in sortedTEID)
                    {
                        sortedTEID_Copy.Add(a);
                    }
                    subCDR = SubCDR.CDRFunction(sortedTEID_Copy);
                    for (int k = 0; k < TeidZore.Count; k++)
                    {
                        String sn1 = Convert.ToString(TeidZore[k].gtpFrame[36]) + Convert.ToString(TeidZore[k].gtpFrame[37]) + Convert.ToString(TeidZore[k].gtpFrame[38]);  //第一位是时间，从36开始
                        foreach (List<gtpStruct> a in subCDR)
                        {
                            int flag = 0;
                            foreach (gtpStruct c in a)
                            {
                                String sn2 = Convert.ToString(c.gtpFrame[36]) + Convert.ToString(c.gtpFrame[37]) + Convert.ToString(c.gtpFrame[38]);  //第一位是时间，从36开始
                                if (sn1 == sn2)
                                {
                                    a.Add(TeidZore[k]);
                                    flag = 1;
                                    break;
                                }
                            }
                            if (flag == 1)
                            {
                                break;
                            }

                        }
                    }
                    foreach (List<gtpStruct> a in subCDR)
                    {
                        subCDR_Copy.Add(a);
                    }
                    List<gtpStruct> nullTEID_Copy = new List<gtpStruct>();
                    foreach (gtpStruct a in nullTEID)
                    {
                        nullTEID_Copy.Add(a);
                    }
                    if (nullTEID_Copy.Count > 0)
                    {
                        subCDR_Copy.Add(nullTEID_Copy);

                    }
                }
                //按照时间排序

                for (int subCount = 0; subCount < subCDR_Copy.Count; subCount++)
                {
                    Dictionary<double, gtpStruct> dic = new Dictionary<double, gtpStruct>();
                    double[] timeList = new double[subCDR_Copy[subCount].Count];
                    for (int k = 0; k < subCDR_Copy[subCount].Count; k++)
                    {
                        //dic.Add(subCDR_Copy[subCount][k].time, subCDR_Copy[subCount][k]);
                        dic.Add(subCDR_Copy[subCount][k].ID, subCDR_Copy[subCount][k]);
                        //timeList[k] = subCDR_Copy[subCount][k].time;
                        timeList[k] = subCDR_Copy[subCount][k].ID;
                    }
                    Array.Sort(timeList);  //从小到大
                    List<gtpStruct> temp = new List<gtpStruct>();
                    for (int n = 0; n < timeList.Length; n++)
                    {
                        temp.Add(dic[timeList[n]]);
                    }
                    GTPsubCDR.Add(temp);
                }
                for (int j = 0; j < GTPsubCDR.Count; j++)
                {
                    string SorIp, DesIp, IMSI_GTP = "";
                    gtpCDRstruct GTP;
                    GTP.CDRbuffer = GTPsubCDR[j];
                    GTP.CDRIMSI = "";
                    DesIp = Convert.ToString(GTPsubCDR[j][0].gtpFrame[16]) + "." + Convert.ToString(GTPsubCDR[j][0].gtpFrame[17]) + "." + Convert.ToString(GTPsubCDR[j][0].gtpFrame[18]) + "." + Convert.ToString(GTPsubCDR[j][0].gtpFrame[19]);
                    SorIp = Convert.ToString(GTPsubCDR[j][0].gtpFrame[12]) + "." + Convert.ToString(GTPsubCDR[j][0].gtpFrame[13]) + "." + Convert.ToString(GTPsubCDR[j][0].gtpFrame[14]) + "." + Convert.ToString(GTPsubCDR[j][0].gtpFrame[15]);  //第一位是时间，从13开始
                    gtpv2 liu = new gtpv2();
                    for (int k = 0; k < GTPsubCDR[j].Count; k++)            //第一位是时间，减29
                    {
                        byte[] GTPv2 = new byte[GTPsubCDR[j][k].gtpFrame.Length - 28];
                        for (int count = 0; count < GTPsubCDR[j][k].gtpFrame.Length - 28; count++)
                        {
                            GTPv2[count] = GTPsubCDR[j][k].gtpFrame[28 + count];
                        }
                        liu.decoder(GTPv2);
                        if (liu.IMSI != "")
                        {
                            IMSI_GTP = liu.IMSI;
                            GTP.CDRIMSI = IMSI_GTP;
                            //break;
                        }
                        //结构体是值类型，只能这样处理？
                        gtpStruct g = GTPsubCDR[j][k];
                        g.messageType = liu.mt;
                        bool isS10 = false;
                        for (int S10Count = 0; S10Count < S10Message.s10.Length; S10Count++)
                        {
                            if (S10Message.s10[S10Count] == g.messageType)
                            {
                                isS10 = true;
                                break;
                            }
                        }
                        if (isS10)
                        {
                            g.faceType = "S10";
                        }
                        else
                        {
                            if (liu.mesType > 4)
                            {
                                g.faceType = "S5";
                            }
                        }
                        GTPsubCDR[j][k] = g;
                    }
                    GTPCDR.Add(GTP);                           ///准备CDR的GTP输出
                }
                #endregion
                #endregion
                //多段关联过程
                Combine(UeS1apProcessList, UeImsiList, GTPCDR, DIAMETERCDR_Process);
                s1ap_content(AllUeSignal, KASME, UeImsiList);//解析出所有nas的内容存放在list<字典>中（ NasList）
            }
            changePcap_combine = false;

            //button_Process.Enabled = false;
            if (tabControl1.TabPages.Count >= 0)
            {
                bool isExist = false;
                for (int i = 0; i < tabControl1.TabPages.Count; i++)
                {
                    if (tabControl1.TabPages[i] == tabPage6)
                    {
                        tabControl1.SelectedTab = tabPage6;
                        isExist = true;
                        break;
                    }
                }
                if (!isExist)
                {
                    tabControl1.TabPages.Add(tabPage6);
                    tabControl1.SelectedTab = tabPage6;
                }
            }
        }
        /// <summary>
        /// 多段关联
        /// </summary>
        /// <param name="UeS1apProcessList">所有终端完整的s1ap通信流程</param>
        /// <param name="UeImsiList">所有终端的imsi，其顺序与UeS1apProcessList单元相同</param>
        /// <param name="GTPCDR"></param>
        /// <param name="DIAMETERCDR"></param>
        public void Combine(List<List<List<process.S1apContent>>> UeS1apProcessList, List<string> UeImsiList, List<gtpCDRstruct> GTPCDR, List<DiameterCDRStruct> DIAMETERCDR)
        {
            AllUeSignal.Clear();
            for (int i = 0; i < UeImsiList.Count && UeImsiList.Count > 0; i++)//将每个终端的信令内容放在字典中，所有终端的信令构成一个LIST<字典>
            {
                #region
                Dictionary<int, LTEmessage> SingleUeSingnal = new Dictionary<int, LTEmessage>();//用于存放一个终端所有接口的信令（s1ap,diameter,gtpv2）
                for (int u = 0; u < UeS1apProcessList[i].Count && UeS1apProcessList[i].Count > 0; u++)//将一个UE所有的信令添加到新建的字典中
                {
                    #region
                    ///将当前终端的信令以LTEmessage的结构放入词典中
                    for (int p = 0; p < UeS1apProcessList[i][u].Count && UeS1apProcessList[i][u].Count > 0; p++)
                    {
                        LTEmessage msg_s1ap = new LTEmessage();
                        msg_s1ap.name = UeS1apProcessList[i][u][p].s1ap_name;
                        msg_s1ap.direction = UeS1apProcessList[i][u][p].direction;
                        msg_s1ap.protocol = "S1AP";
                        msg_s1ap.frame = UeS1apProcessList[i][u][p].chunk;
                        msg_s1ap.src_ip = UeS1apProcessList[i][u][p].src_ip;
                        msg_s1ap.des_ip = UeS1apProcessList[i][u][p].des_ip;
                        msg_s1ap.face = "s1ap";
                        msg_s1ap.time = Double.Parse(UeS1apProcessList[i][u][p].timeNas);
                        msg_s1ap.absTime = UeS1apProcessList[i][u][p].time1;
                        SingleUeSingnal.Add(UeS1apProcessList[i][u][p].id, msg_s1ap);
                    }
                    #endregion
                }
                for (int j = 0; j < GTPCDR.Count; j++)
                {
                    #region
                    if (GTPCDR[j].CDRIMSI == UeImsiList[i])
                    {
                        for (int u = 0; u < GTPCDR[j].CDRbuffer.Count && GTPCDR[j].CDRbuffer.Count > 0; u++)
                        {
                            LTEmessage msg_GTPv2 = new LTEmessage();
                            msg_GTPv2.des_ip = "";
                            msg_GTPv2.src_ip = "";
                            msg_GTPv2.name = GTPCDR[j].CDRbuffer[u].messageType;
                            msg_GTPv2.direction = 0;
                            msg_GTPv2.frame = GTPCDR[j].CDRbuffer[u].gtpFrame;
                            msg_GTPv2.protocol = "GTPv2";
                            msg_GTPv2.face = GTPCDR[j].CDRbuffer[u].faceType;
                            msg_GTPv2.absTime = GTPCDR[j].CDRbuffer[u].timeAll;
                            SingleUeSingnal.Add(GTPCDR[j].CDRbuffer[u].ID, msg_GTPv2);
                        }
                    }
                    #endregion
                }
                for (int k = 0; k < DIAMETERCDR.Count; k++)
                {
                    #region
                    if (DIAMETERCDR[k].CDRIMSI == UeImsiList[i])
                    {
                        for (int u = 0; u < DIAMETERCDR[k].CDRbuffer.Count && DIAMETERCDR[k].CDRbuffer.Count > 0; u++)
                        {
                            LTEmessage msg_DIAMETER = new LTEmessage();
                            msg_DIAMETER.src_ip = "";
                            msg_DIAMETER.des_ip = "";
                            msg_DIAMETER.name = DIAMETERCDR[k].CDRbuffer[u].command;
                            msg_DIAMETER.direction = DIAMETERCDR[k].CDRbuffer[u].re;
                            msg_DIAMETER.frame = DIAMETERCDR[k].CDRbuffer[u].diameterFrame;
                            msg_DIAMETER.protocol = DIAMETERCDR[k].CDRbuffer[u].protocol;
                            msg_DIAMETER.face = "s6a";
                            // msg_DIAMETER.time = double.Parse(DIAMETERCDR[k].CDRbuffer[u].time);
                            msg_DIAMETER.absTime = DIAMETERCDR[k].CDRbuffer[u].time;
                            SingleUeSingnal.Add(DIAMETERCDR[k].CDRbuffer[u].ID_diamter, msg_DIAMETER);
                        }
                    }
                    #endregion
                }
                SingleUeSingnal = (from entry in SingleUeSingnal
                                   orderby entry.Key ascending
                                   select entry).ToDictionary(pair => pair.Key, pair => pair.Value); //将字典里的信令先后排序
                AllUeSignal.Add(SingleUeSingnal);//字典的LIST
                #endregion
            }
            List<string> TimeKey = new List<string>();//存放一个终端的所有信令时间
            //在控件listview中展示所有的终端信令流程的起止时间，用户标识
            for (int count = 0; count < AllUeSignal.Count && AllUeSignal.Count > 0; count++)
            {
                string ID = "";
                string startTime = "";
                string endTime = "";
                string IMSI = UeImsiList[count];
                TimeKey.Clear();
                foreach (int key in AllUeSignal[count].Keys)
                {
                    TimeKey.Add(AllUeSignal[count][key].absTime);
                }
                ID = count.ToString();
                startTime = TimeKey[0].ToString();
                endTime = TimeKey[TimeKey.Count - 1].ToString();
                AddItem_Dictionary(ID, startTime, endTime, IMSI);
                listView_ProcessIMSI.Items[count].BackColor = System.Drawing.Color.LightGoldenrodYellow;
            }
            listView_ProcessIMSI.BackColor = Color.SkyBlue;
        }
        /// <summary>
        /// 将终端的多段关联添加到列表中
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="IMSI"></param>
        private void AddItem_Dictionary(string ID, string startTime, string endTime, string IMSI)
        {
            listView_ProcessIMSI.Items.Add(new ListViewItem(new string[] { ID, startTime, endTime, IMSI }));
        }

        public bool equalList(List<byte[]> list1, List<byte[]> list2)//比较两个list是否相同
        {
            return list1.Equals(list2);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="AllUeSignal">所有终端的通信信令</param>
        /// <param name="KASME">所有的根密钥Kasme</param>
        /// <param name="UeImsiList">所有终端的IMSI</param>
        public void s1ap_content(List<Dictionary<int, LTEmessage>> AllUeSignal, List<DiameterKASME> KASME, List<string> UeImsiList)
        {
            for (int j = 0; j < AllUeSignal.Count; j++)
            {
                #region
                Dictionary<int, nas> NAS = new Dictionary<int, nas>();//存储一个UE对应的信令中所有NAS消息内容
                DiameterKASME kasme0 = new DiameterKASME();//存储一个UE所分配的所有Kasme及分配时间
                for (int p = 0; p < KASME.Count; p++)
                {
                    if (KASME[p].imsi == UeImsiList[j])//找到当前字典所对应的Kasme列表
                    {
                        kasme0 = KASME[p];
                        break;
                    }
                    else
                        ;
                }
                List<byte[]> kasmeOld = new List<byte[]>();//用于存放上一条信令使用的密钥，并将其与当前密钥对比，判定解密密钥是否改变
                foreach (int key in AllUeSignal[j].Keys)
                {
                    int id_s1ap = key;//当前信令在字典中的序号
                    int direction = AllUeSignal[j][key].direction;//当前信令的方向
                   
                    //解析S1ap消息内容                                              
                    if (AllUeSignal[j][key].protocol == "S1AP")
                    {
                        #region
                        #region
                        nas nasMessage = new nas();
                        List<byte[]> kasme_current = new List<byte[]>();
                      
                        int count;
                        if (kasme0.Kasme != null)
                            count = kasme0.Kasme.Count;//一个终端在其流程中分配的KASME次数（不是个数）
                        else
                            count = 0;
                        //以下程序是为了获得当前信令的加密密钥
                        for (int u = 0; u < count && count > 0; u++)
                        {
                            if (count > u + 1)
                            {
                                #region
                                //当前信令的时间要晚于当前nas加密所采用的根密钥的分配时间，且当前信令时间要早于下一次密钥分配时间
                                if ((AllUeSignal[j][key].time) > (double.Parse(kasme0.Kasme[u].time)) && (AllUeSignal[j][key].time) < (double.Parse(kasme0.Kasme[u + 1].time)))
                                {
                                    kasme_current = kasme0.Kasme[u].kasme;
                                    break;
                                }
                                else
                                {
                                    continue;
                                }
                                #endregion
                            }
                            else if (count == u + 1 && AllUeSignal[j][key].time >
                                (double.Parse(kasme0.Kasme[count - 1].time)))//当前信令之后再没有分配新的密钥
                            {
                                kasme_current = kasme0.Kasme[count - 1].kasme;
                                break;
                            }
                            else
                                ;
                        }
                        #endregion
                        bool bool1 = equalList(kasmeOld, kasme_current);//比较上次与本次的Kasme是否属于同一次分配的，若不是，则使用的Kasme序数要从头开始
                        if (bool1 == true)//前后使用的Kasme是同一组，则,kasme_conut不用置为-1
                        {
                            nas_dec.nas_decode1(AllUeSignal[j][key].frame, kasme_current, direction);
                            //kasmeOld = kasme_current;//密钥未发生改变就不用重新赋值了
                        }
                        else
                        {
                            kasmeCount = -1;
                            nas_dec.nas_decode1(AllUeSignal[j][key].frame, kasme_current, direction);
                            kasmeOld = kasme_current;
                        }
                        nasMessage.MmeUeS1apId = Id1.s1ap_id(AllUeSignal[j][key].frame);
                        nasMessage.EnbUeS1apId = Id2.s1ap_id(AllUeSignal[j][key].frame);
                        decCause.decodeCause(AllUeSignal[j][key].frame);
                        nasMessage.cause_kind = decCause.str1;//cause类别                       
                        nasMessage.cause_name = decCause.str2;//具体的cause名称 
                        //解析s1ap中的用户标识
                        if (AllUeSignal[j][key].name == "Paging")
                            nasMessage.s1ap_identity = Id_paging.identity(AllUeSignal[j][key].frame);
                        else if (AllUeSignal[j][key].name == "initialUEMessage")
                            nasMessage.s1ap_identity = Id_initial.identity(AllUeSignal[j][key].frame);
                        else//其他s1ap信令无ismi或tmsi
                        {
                            nasMessage.s1ap_identity = "";
                        }
                        nasMessage.nas_name = nas_dec.nas_name;
                        nasMessage.nas_cause = nas_dec.cause_str;
                        nasMessage.nas_identity = nas_dec.identity;
                        nasMessage.ue_ip = nas_dec.ue_ip;
                        NAS.Add(id_s1ap, nasMessage);
                        #endregion
                    }
                    else//非S1Ap消息(不包含nas消息)
                        ;
                }
                NasList.Add(NAS);
                #endregion
            }
        }





        #endregion
        static byte[] famer;
        static bool equal(byte[] a, byte[] b)
        {
            bool res = true;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] == b[i])
                    res = true;
                else
                {
                    res = false;
                    break;
                }
            }
            return res;
        }
        public void ClearGTPvalueAll()
        {
            allClone.Clear();
            gtpChunkCopy1.Clear();
            gtpChunkCopy2.Clear();
            sortedIP.Clear();
            TEIDExist.Clear();
            TEIDExistClone.Clear();
            nullTEID.Clear();
            sortedTEID.Clear();
            subCDR.Clear();
            subCDR_Copy.Clear();
            GTPsubCDR.Clear();
            GTPCDR.Clear();
            gtpChunk1.Clear();
        }               //GTP变量清空  by 刘黎
        public void ClearS1APvalueAll()                  //S1AP全局变量清除（黄罡）                   
        {
            S1apKey6All.Clear();
            S1apContentAll.Clear();
            CdrList.Clear();
            CdrIdpair.Clear();
            UeS1apProcessList.Clear();
            UeImsiList.Clear();
            KASME.Clear();
            Nasclass1.Clear();
            AllUeSignal.Clear();
            NasList.Clear();
            listPackets.Clear();
            kasmeCount = -1;
            //kasmeOld.Clear();
            No = -1;
            bool1 = false;
        }
        public void ClearDIAMETERvalueAll()   //diameter全局变量清除（黄罡）
        {
            IMSIList.Clear();
            TCPDiameterClone.Clear();
            diameterClone_TCP.Clear();
            diameterTime_TCP.Clear();
            diameterTime.Clear();
            TCPDiameterID.Clear();
            SCTPDiameterClone.Clear();
            diameterClone_SCTP.Clear();
            SCTPDiameterID.Clear();
            diameterTime_SCTP.Clear();
            diameterTime.Clear();
            diameterFlow.Clear();
            if (diameterSignaling != null)
                diameterSignaling.Clear();
            DIAMETERCDR.Clear();
            DIAMETERCDR_Process.Clear();
            if (diameterSignaling_Process != null)
                diameterSignaling_Process.Clear();
            num1 = null;
            num1 = new int[25] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        }
        public void ClearClass()                       //清除类
        {
            SCTP = null;
            TCPDiameter = null;
            A = null;
            SctpToS1ap = null;
            Id1 = null;
            Id2 = null;
            s1apDec1 = null;
            Process = null;
            decCause = null;
            Id_paging = null;
            Id_initial = null;
            s1ap_nas2 = null;
            nas_dec = null;//用于多段关联之后的nas解析  

            SCTP = new SCTPDiameter();
            TCPDiameter = new TCPDiameter();
            A = new gtpv2();
            SctpToS1ap = new SctpDecode();
            Id1 = new Id1();
            Id2 = new Id2();
            s1apDec1 = new s1apDec();
            Process = new process();
            decCause = new causeOnline();
            Id_paging = new IMSI_TMSI();
            Id_initial = new Imsi_Tmsi();
            s1ap_nas2 = new s1ap_nas1();
            nas_dec = new NAS_DEC();//用于多段关联之后的nas解析
        }
        int DataID = 0;//离线解析数据流界面的编号
        public bool changePcap_s1ap = false;//辅助CDR点击按钮判断是否打开了新的pcap文件
        public bool changePcap_Diameter = false;//辅助CDR点击按钮判断是否打开了新的pcap文件
        public bool changePcap_gtpv2 = false;//辅助CDR点击按钮判断是否打开了新的pcap文件
        public bool changePcap_combine = false;//辅助CDR点击按钮判断是否打开了新的pcap文件
        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            famer = new byte[1];//该数组用来存放报文中的前一条s1ap信令，用于比较和当s1ap报文是否相同（判定当前报文是否是重发的）
            DataID = 0;
            ClearGTPvalueAll();                     //GTP变量清空  by 刘黎
            ClearS1APvalueAll();        //S1AP全局变量清空（黄罡）
            ClearClass();        //清除上一次打开文件时所创建的类
            ClearDIAMETERvalueAll();//DIAMETER全局变量清空
            changePcap_s1ap = true;//当该变量为true时，点击s1ap按钮才能进行s1ap的cdr合成，该变量的设置是防止，对同一个报文进行多次点击s1ap按钮（cdr合成）的操作
            changePcap_Diameter = true;//当该变量为true时，点击diameter按钮才能进行diameter的cdr合成，该变量的设置是防止，对同一个报文进行多次点击diameter按钮（cdr合成）的操作
            changePcap_gtpv2 = true;//当该变量为true时，点击GTPv2按钮才能进行GTPv2的cdr合成，该变量的设置是防止，对同一个报文进行多次点击GTPv2按钮（cdr合成）的操作
            changePcap_combine = true;//当该变量为true时，点击GTPv2按钮才能进行GTPv2的cdr合成，该变量的设置是防止，对同一个报文进行多次点击GTPv2按钮（cdr合成）的操作
            listView_Off.Items.Clear();//当该变量为true时，点击"消息流程图"按钮才能进行多段关联，该变量的设置是防止，对同一个报文进行多次点击“消息流程图”按钮（多段关联）的操作
            treeView_OffLine.Nodes.Clear();//清空treeview_offline记录的上一个文件的内容（黄刚2017.6.9）
            textBox_OffLine.Clear();//清空textBox_OffLine记录的上一个文件的内容（黄刚2017.6.9）
            IMSILocation = -1;
            DataID = 0;
            List<string[]> ItemBuffer = new List<string[]>();//存储所有信令要展示到离线消息
            #region
            string PcapPath = "";//离线文件所在路径
            string[] files;//存放pcap文件的文件夹
            bool isStart = true;
            decimal startTime = 0;
            FolderBrowserDialog fd = new FolderBrowserDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                PcapPath = fd.SelectedPath;
                this.Text = "LTE-M接口监测软件  打开文件夹路径：" + PcapPath;
                //Edit By Kuang 
                this.tabControl1.TabPages.Remove(tabPage8);
                this.tabControl1.TabPages.Add(tabPage8);
                //this.tabControl1.TabPages.Remove(tabPage2);
                this.tabControl1.TabPages.Remove(tabPage3);
                this.tabControl1.TabPages.Remove(tabPage4);
                this.tabControl1.TabPages.Remove(tabPage5);
                this.tabControl1.TabPages.Remove(tabPage6);
                this.tabControl1.SelectedTab = this.tabPage8;

                OffLine = true;

                // 按钮可以按下
                button_OffLine.Enabled = true;
                button_GTPv2.Enabled = true;
                button_S1AP.Enabled = true;
                button_DIAMETER.Enabled = true;
                button_Process.Enabled = true;

                // button1.Enabled = true;
            }

            if (PcapPath != "")
                files = Directory.GetFiles(PcapPath, "*.pcap");
            else
                files = null;
            if (files != null)
            {
                foreach (string txt in files)
                {
                    try
                    {
                        PcapDecoder_Signaling pcdSignaling = new PcapDecoder_Signaling(txt);       //实例化解析类
                        FileStream fs = new FileStream(txt, FileMode.Open, FileAccess.Read, FileShare.Read);       //创建流文件读取
                        ProtocolID pType = new ProtocolID();
                        pcdSignaling.GetFileHead(fs);        //解析文件头

                        while (pcdSignaling.isFinish != 0)      // 当前文件读取
                        {
                            #region
                            int Protocolid;
                            pcdSignaling.GetDataHead(fs);           //解析当前文件数据帧头部
                            if (pcdSignaling.isFinish == 0)
                            {
                                break;
                            }
                            pcdSignaling.ReadPcapFile(fs);     //解析数据帧
                            Protocolid = pType.protocolID(pcdSignaling.PcapFile.UpToIPData);
                            byte[] frame = pcdSignaling.PcapFile.UpToIPData;                               //    pcap数据 从ip层开始 
                            if (isStart)
                            {
                                startTime = pcdSignaling.PcapFile.DataHead_Signaling.Time;
                                isStart = false;
                            }

                            string time = (pcdSignaling.PcapFile.DataHead_Signaling.Time - startTime).ToString();
                            string timedate = pcdSignaling.PcapFile.DataHead_Signaling.TimeAll.ToString();         //刘黎改

                            string ID = string.Empty;//信令序号
                            string Capturetime = string.Empty;//捕获时间
                            string Protocol = string.Empty;//信令协议
                            string SourceIP = string.Empty;//源IP地址
                            string SourcePort = string.Empty; //源端口
                            string DestIP = string.Empty;//目的ip地址
                            string DestPort = string.Empty;//目的端口
                            string AllLength = string.Empty;//报文长度
                            string MessageBodyLen = string.Empty;//信令编码长度
                            string MessageBodyHex = string.Empty;//信令名称
                            if (Protocolid == 2)//TCP-diameter
                            {
                                #region
                                nas_class nas_class1 = new nas_class();
                                List<nas_class> Nasclass = new List<nas_class>();//用于存放所有解析出来的nas信息
                                nas_class1.cause_str = " ";
                                nas_class1.identity = " ";
                                nas_class1.nas_name = " ";
                                nas_class1.ue_ip = "";
                                Nasclass.Add(nas_class1);
                                Nasclass1.Add(Nasclass);
                                byte[] Tc = new byte[frame.Length - 20];//底层若为20
                                Array.Copy(frame, 20, Tc, 0, frame.Length - 20);
                                TCPDiameterClone.Add(Tc);
                                byte[] Tc1 = new byte[frame.Length];
                                Array.Copy(frame, 0, Tc1, 0, frame.Length);
                                diameterClone_TCP.Add(Tc1);
                                TCPDiameter.DecodeTcpChunk(Tc);
                                Protocol = TCPDiameter.protocol;
                                if (TCPDiameter.chunckCommandCODE == 318)//判定该消息为认证
                                {
                                    if (TCPDiameter.diameterDirection == 1) //若是request消息
                                    {
                                        if (KASME.Count == 0)
                                        {
                                            DiameterKASME key = new DiameterKASME();
                                            key.imsi = TCPDiameter.userNameStr;
                                            key.Kasme = new List<diameterKASME>();
                                            KASME.Add(key);
                                            IMSILocation = 0;
                                        }
                                        else
                                        {
                                            for (int i = 0; i < KASME.Count; i++)//遍历所有的IMSI
                                            {
                                                if (TCPDiameter.userNameStr == KASME[i].imsi)
                                                {
                                                    IMSILocation = i;
                                                    break;
                                                }
                                                else
                                                {
                                                    IMSILocation = -1;
                                                }
                                            }
                                            if (IMSILocation == -1)//遍历之后没有发现与之相同的单元，则新建一个添加到KASME中
                                            {
                                                DiameterKASME key = new DiameterKASME();
                                                key.imsi = TCPDiameter.userNameStr;
                                                key.Kasme = new List<diameterKASME>();
                                                KASME.Add(key);
                                                IMSILocation = KASME.Count - 1;
                                            }
                                            else//发现前面有与本次的IMSI相同的单元
                                            { ;}
                                        }

                                    }
                                    else//若是answer消息
                                    {
                                        diameterKASME kas1 = new diameterKASME();
                                        kas1.time = time;                //刘黎改
                                        kas1.kasme = TCPDiameter.KASMEValue;
                                        KASME[IMSILocation].Kasme.Add(kas1);
                                    }
                                }
                                TCPDiameter.decoder_All(frame);   //刘黎改
                                Capturetime = timedate;   //刘黎改
                                diameterTime_TCP.Add(Capturetime);
                                diameterTime.Add(Capturetime);//diameter-tcp的时间列表
                                diameterTimeCDR_TCP.Add(time);
                                diameterTimeCDR.Add(time);
                                if (Protocol == "TCP")
                                {
                                    MessageBodyHex = "DATA";
                                }
                                else
                                {
                                    MessageBodyHex = TCPDiameter.diameterContent;
                                }
                                SourceIP = TCPDiameter.sourIP;
                                SourcePort = TCPDiameter.sourPort;
                                DestIP = TCPDiameter.destIP;
                                DestPort = TCPDiameter.destPort;
                                AllLength = Convert.ToString(frame.Length);
                                ID = DataID.ToString();
                                TCPDiameterID.Add(DataID);
                                if (MessageBodyHex != "DATA")//黄罡
                                {
                                    string[] a = new string[9] { ID, Capturetime, Protocol, SourceIP, SourcePort, DestIP, DestPort, AllLength, MessageBodyHex };
                                    ItemBuffer.Add(a);
                                    DataID = DataID + 1;
                                    listPackets.Add(frame);
                                }
                                #endregion
                            }
                            if (Protocolid == 3)//该消息为gtpv2,故不含有nas-pdu（某些gtpv2可能也含有nas-pdu,此处后面可能需要添加nas解析程序，待商议后再处理，暂且不考虑）
                            {
                                #region
                                nas_class nas_class1 = new nas_class();
                                List<nas_class> Nasclass = new List<nas_class>();//用于存放所有解析出来的nas信息
                                nas_class1.cause_str = " ";
                                nas_class1.identity = " ";
                                nas_class1.nas_name = " ";
                                nas_class1.ue_ip = "";
                                Nasclass.Add(nas_class1);
                                Nasclass1.Add(Nasclass);
                                listPackets.Add(frame);//黄罡
                                byte[] gtpList = new byte[frame.Length - 28];
                                Array.Copy(frame, 28, gtpList, 0, frame.Length - 28);     //allMeaasgeList 从IP层开始                     
                                allClone.Add(frame);                                          //  从IP层开始，第一位不是时间；第一位要改回时间啊！ 不然后面怎么找时间？                      
                                gtpStruct g;                                               //gtp数据结构
                                g.time = Convert.ToDouble(time);                  //时间                                         
                                g.gtpFrame = frame;                         //数据
                                g.messageType = "";
                                g.faceType = "";
                                g.ID = DataID;
                                g.timeAll = timedate;
                                //gtpChunk.Add(gtpchunk);                            //gtp数据集，第一位时间
                                gtpChunk1.Add(g);                                  //gtp数据集
                                A.decoder(gtpList);
                                A.decoder_All(frame);
                                Protocol = A.protocol;
                                SourceIP = A.sourIP;
                                SourcePort = A.sourPort;
                                DestIP = A.destIP;
                                DestPort = A.destPort;
                                AllLength = Convert.ToString(frame.Length);
                                MessageBodyHex = A.mt;
                                ID = (DataID).ToString();
                                Capturetime = timedate;
                                //gtpShijian.Add(Capturetime);//gtpShijian该变量没有被引用（黄罡）
                                // AddItem(ID, Capturetime, Protocol, SourceIP, SourcePort, DestIP, DestPort, AllLength, MessageBodyHex);
                                string[] a = new string[9] { ID, Capturetime, Protocol, SourceIP, SourcePort, DestIP, DestPort, AllLength, MessageBodyHex };
                                ItemBuffer.Add(a);
                                DataID = DataID + 1;
                                #endregion
                            }
                            if (Protocolid == 4)  //对S1ap消息进行解析，整理出其中的协议，原IP，目的IP，原端口，目的端口，长度，消息的简要解释等信息并将其显示
                            {
                                #region
                                byte[] S1ap = new byte[frame.Length - 20];  //存储运输层头部及应用层数据                          
                                Array.Copy(frame, 20, S1ap, 0, frame.Length - 20); //去掉IP头部数据    
                                byte[] S1ap1 = new byte[frame.Length]; //存储IP层头部，运输层头部和应用层数据
                                int PPID = 0;//指示应用层的协议类型
                                Array.Copy(frame, 0, S1ap1, 0, frame.Length);
                                MessageBodyHex = "";
                                SctpToS1ap.DeScChunk(S1ap);//解析带有运输层头部的报文
                                PPID = SctpToS1ap.paylaodProtocolIdentifier;
                                if (PPID == 18)//应用层是s1ap协议
                                {
                                    #region
                                    if (equal(famer, frame))//判断当前信令是否与上一条信令相同
                                    {
                                        famer = frame;
                                    }
                                    else
                                    {
                                        uint mmeUeS1apId = 0;
                                        int enbUeS1apId = 0;
                                        String S1apName = "";//s1ap的信令名称
                                        String CauseKindS1ap = "";//s1ap 信令中的cause种类
                                        String CauseNameS1ap = "";//s1ap 信令中的cause名称
                                        string S1apIdentity = "";//用于获得s1ap的用户标识     
                                        string identity;//UE的标识IMSI或TMSI
                                        string CauseNAS;//nas cause
                                        string nas_name;//nas 名称
                                        string ue_ip = "";//终端IP地址

                                        //ScS1Clone.Add(S1ap);
                                        //IPS1Clone.Add(S1ap1);//包含ip头部的sctp消息

                                        //获取端口号，IP，Protocol id，信令编码长度，抓包时间
                                        SctpToS1ap.decoder_All(frame);                //刘黎改动
                                        Protocol = SctpToS1ap.protocol;
                                        SourceIP = SctpToS1ap.sourIP;
                                        S1apContent.id = DataID;
                                        Key6.sourceIP = SctpToS1ap.sourIP;
                                        S1apContent.src_ip = SctpToS1ap.sourIP;
                                        SourcePort = SctpToS1ap.sourPort;
                                        Key6.sourceport = Convert.ToInt32(SourcePort);
                                        DestIP = SctpToS1ap.destIP;
                                        Key6.desIP = SctpToS1ap.destIP;
                                        S1apContent.des_ip = SctpToS1ap.destIP;
                                        DestPort = SctpToS1ap.destPort;
                                        Key6.desport = Convert.ToInt32(DestPort);
                                        //AllLength = Convert.ToString(frame.Length);             //刘黎改动
                                        int length = 0;
                                        for (int u = 0; u < SctpToS1ap.s1ap.Count; u++)
                                            length += SctpToS1ap.s1ap[u].Length;
                                        AllLength = length.ToString();//ALLLength是纯粹的s1ap编码长度
                                        ID = (DataID).ToString();
                                        Capturetime = timedate;                            //刘黎改动                                   
                                        List<nas_class> Nasclass = new List<nas_class>();//若一条数据流中有多条s1ap消息时，就有多个list<nas_class>存储信息
                                        for (int k = 0; k < SctpToS1ap.s1ap.Count; k++)//循环针对于一个报文中有两条s1ap信令的状况（若只有一条s1ap信令也适用）
                                        {
                                            //解析s1ap信令中的ID pair，抓包时间，信令名称，信令方向，s1ap cause，用户标识（imsi或tmsi）
                                            nas_class nas_class1 = new nas_class();
                                            S1apContent.chunk = SctpToS1ap.s1ap[k];//
                                            mmeUeS1apId = Id1.s1ap_id(SctpToS1ap.s1ap[k]);//sctp.s1ap[j]中存放的是s1ap数据流
                                            Key6.mmeUeS1apId = mmeUeS1apId;
                                            S1apContent.MmeUeS1apId = mmeUeS1apId;
                                            enbUeS1apId = Id2.s1ap_id(SctpToS1ap.s1ap[k]);
                                            Key6.enbUeS1apId = enbUeS1apId;
                                            S1apContent.EnbUeS1apId = enbUeS1apId;
                                            S1apContent.time1 = Capturetime;
                                            S1apContent.timeNas = time;
                                            S1apName = decodeS1ap.s1ap_decode1(SctpToS1ap.s1ap[k]);//S1AP消息信令
                                            S1apContent.s1ap_name = S1apName;
                                            S1apContent.direction = decodeS1ap.direction;//信令方向
                                            decCause.decodeCause(SctpToS1ap.s1ap[k]);
                                            CauseKindS1ap = decCause.str1;//cause类别
                                            S1apContent.cause_kind = decCause.str1;
                                            CauseNameS1ap = decCause.str2;//具体的cause名称 
                                            S1apContent.cause_name = decCause.str2;

                                            //解析s1ap中的用户标识
                                            if (S1apName == "Paging")
                                                S1apIdentity = Id_paging.identity(SctpToS1ap.s1ap[k]);
                                            else if (S1apName == "initialUEMessage")
                                                S1apIdentity = Id_initial.identity(SctpToS1ap.s1ap[k]);
                                            else//其他s1ap信令无ismi或tmsi
                                            {
                                                ;
                                            }
                                            S1apContent.s1ap_identity = S1apIdentity;
                                            //解析nas信令中的用户标识，cause，信令名称，终端IP
                                            s1ap_nas2.nas_decode(SctpToS1ap.s1ap[k]);
                                            S1apContent.NasChunk = s1ap_nas2.NasChunk;//获得nas编码（包括加密安全头）
                                            identity = s1ap_nas2.identity;
                                            S1apContent.nas_identity = identity;
                                            //S1apContent.NasChunk = s1ap_nas2.NasChunk;//获得nas编码（包括加密安全头）
                                            CauseNAS = s1ap_nas2.cause_str;
                                            S1apContent.nas_cause = CauseNAS;
                                            nas_name = s1ap_nas2.nas_name;
                                            S1apContent.nas_name = nas_name;
                                            ue_ip = s1ap_nas2.ue_ip;
                                            S1apContent.ue_ip = ue_ip;
                                            S1apKey6All.Add(Key6);//s1ap2中存放的对象为结构体，即所有信令的6元组关键字（s1ap2存放的是整个文件的S1ap消息的6元组）
                                            S1apContentAll.Add(S1apContent);//s1ap3中存放的对象为结构体，即所有信令的所有信息（s1ap2存放的是整个文件的S1ap消息的所有信息）
                                            nas_class1.cause_str = CauseNAS;
                                            nas_class1.identity = identity;
                                            nas_class1.nas_name = nas_name;
                                            nas_class1.ue_ip = ue_ip;
                                            Nasclass.Add(nas_class1);
                                            if (nas_class1.nas_name != " " && (nas_class1.nas_name != "ciperd message  "))
                                                MessageBodyHex = MessageBodyHex + S1apName + ' ' + nas_class1.nas_name + " ";
                                            else
                                                MessageBodyHex = MessageBodyHex + S1apName + " ";
                                        }
                                        Nasclass1.Add(Nasclass);
                                        string[] a = new string[9] { ID, Capturetime, Protocol, SourceIP, SourcePort, DestIP, DestPort, AllLength, MessageBodyHex };
                                        ItemBuffer.Add(a);
                                        DataID = DataID + 1;
                                        listPackets.Add(frame);
                                        famer = frame;
                                    }
                                    #endregion
                                }
                                else if ((PPID == 0 || PPID == 46 || PPID == 47 || PPID == 132) && ((pType.src_port == 3868) || (pType.des_port == 3868) || (pType.src_port == 60000) || (pType.des_port == 60000)))
                                {
                                    #region
                                    //该消息为diameter,故不含有nas-pdu
                                    nas_class nas_class1 = new nas_class();
                                    List<nas_class> Nasclass = new List<nas_class>();
                                    nas_class1.cause_str = " ";
                                    nas_class1.identity = " ";
                                    nas_class1.nas_name = " ";
                                    nas_class1.ue_ip = "";
                                    Nasclass.Add(nas_class1);
                                    Nasclass1.Add(Nasclass);

                                    byte[] Sc = new byte[frame.Length - 20];//sctp-diameter消息，不包含底层头部     //刘黎改动
                                    Array.Copy(frame, 20, Sc, 0, frame.Length - 20);                               //刘黎改动
                                    SCTPDiameterClone.Add(Sc);
                                    byte[] Sc1 = new byte[frame.Length];           //刘黎改动
                                    Array.Copy(frame, 0, Sc1, 0, frame.Length);    //刘黎改动
                                    diameterClone_SCTP.Add(Sc1);
                                    SCTPDiameterID.Add(DataID);
                                    SCTP.DecodeSctpChunk(Sc);

                                    SCTP.decoder_All(frame);

                                    Protocol = SCTP.protocol;
                                    SourceIP = SCTP.sourIP;
                                    SourcePort = SCTP.sourPort;
                                    DestIP = SCTP.destIP;
                                    DestPort = SCTP.destPort;
                                    AllLength = Convert.ToString(frame.Length);    //刘黎改动
                                    Capturetime = timedate;
                                    diameterTime_SCTP.Add(Capturetime);
                                    diameterTime.Add(Capturetime);//diameter-sctp的事件列表
                                    diameterTimeCDR_SCTP.Add(time);
                                    diameterTimeCDR.Add(time);//diameter-sctp的事件列表

                                    if (Protocol == "SCTP")
                                    {
                                        MessageBodyHex = "DATA";
                                    }
                                    else
                                    {
                                        MessageBodyHex = SCTP.diameterContent;
                                    }

                                    if (SCTP.chunckCommandCODE == 318)//判定该消息为认证
                                    {
                                        if (SCTP.diameterDirection == 1) //若是request消息
                                        {
                                            if (KASME.Count == 0)
                                            {
                                                DiameterKASME key = new DiameterKASME();
                                                key.imsi = SCTP.userNameStr;
                                                key.Kasme = new List<diameterKASME>();
                                                //KAS = key;
                                                KASME.Add(key);
                                                IMSILocation = 0;
                                            }
                                            else
                                            {
                                                for (int i = 0; i < KASME.Count; i++)//遍历所有的IMSI
                                                {
                                                    if (SCTP.userNameStr == KASME[i].imsi)
                                                    {
                                                        IMSILocation = i;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        IMSILocation = -1;
                                                    }
                                                }
                                                if (IMSILocation == -1)//遍历之后没有发现与之相同的单元，则新建一个添加到KASME中
                                                {
                                                    DiameterKASME key = new DiameterKASME();
                                                    key.imsi = SCTP.userNameStr;
                                                    key.Kasme = new List<diameterKASME>();
                                                    //KAS = key;
                                                    KASME.Add(key);
                                                    IMSILocation = KASME.Count - 1;
                                                }
                                                else//发现前面有与本次的IMSI相同的单元
                                                { ;}
                                            }

                                        }
                                        else//若是answer消息
                                        {
                                            diameterKASME kas1 = new diameterKASME();
                                            kas1.time = time;
                                            kas1.kasme = SCTP.KASMEValue;
                                            KASME[IMSILocation].Kasme.Add(kas1);
                                        }
                                    }
                                    ID = DataID.ToString();
                                    if (MessageBodyHex != "DATA")//黄罡
                                    {
                                        string[] a = new string[9] { ID, Capturetime, Protocol, SourceIP, SourcePort, DestIP, DestPort, AllLength, MessageBodyHex };
                                        ItemBuffer.Add(a);
                                        DataID = DataID + 1;
                                        listPackets.Add(frame);
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            #endregion
                        }
                    }
                    catch
                    {

                    }
                }
            }

            listView_Off.BeginUpdate();
            for (int count = 0; count < ItemBuffer.Count; count++)
            {
                listView_Off.Items.Add(new ListViewItem(ItemBuffer[count]));
                if (ItemBuffer[count][2] == "S1AP")
                    listView_Off.Items[count].BackColor = System.Drawing.Color.Linen;
                else if (ItemBuffer[count][2] == "DIAMETER-SCTP" || ItemBuffer[count][2] == "DIAMETER-TCP")
                    listView_Off.Items[count].BackColor = System.Drawing.Color.SkyBlue;
                else if (ItemBuffer[count][2] == "GTPv2")
                    listView_Off.Items[count].BackColor = System.Drawing.Color.LightGoldenrodYellow;
                else
                    listView_Off.Items[count].BackColor = System.Drawing.Color.LightGray;
            }
            listView_Off.EndUpdate();
            #endregion
        }

        // By Kuang 跨窗体传值
        void f2_accept(object sender, EventArgs e)
        {
            bool ControlCBTC = false;
            bool ControlSignal = false;
            FilterForm f2 = (FilterForm)sender;//事件的接收者通过一个简单的类型转换得到Form2的引用 
            this.bStatues_CBTC = f2.Form2Value_CBTC; //接收到Form2的textBox1.Text 
            this.bStatues_Signal = f2.Form2Value_Signal;

            //三种协议接口，过滤
            this.procol_GTPv2 = f2.Form2Value_GTPv2;
            this.procol_DIAMETER = f2.Form2Value_Diameter;
            this.procol_S1AP = f2.Form2Value_S1AP;
            // 判断进行过窗体选择
            // 开始、暂停、停止按钮可动
            if (f2.Form2Value_FilterState)
            {
                this.toolStripButton_Start.Enabled = true;
                this.toolStripButton_Pause.Enabled = false;
                this.toolStripButton_Stop.Enabled = false;
            }



            if (this.bStatues_CBTC)
            {
                for (int i = 0; i < tabControl1.TabCount; i++)
                {
                    if (tabControl1.TabPages[i].Text == "CBTC实时业务" && f2.Form2Value_FilterState)
                    {
                        ControlCBTC = true;
                    }
                }
                if (!ControlCBTC && f2.Form2Value_FilterState)
                {
                    this.tabControl1.TabPages.Add(tabPage7);
                    this.tabControl1.SelectedTab = this.tabPage7;
                }

            }

            else
            {
                this.tabControl1.TabPages.Remove(tabPage7);
            }



            if (this.bStatues_Signal)
            {
                for (int i = 0; i < tabControl1.TabCount; i++)
                {
                    if (tabControl1.TabPages[i].Text == "LTE-M实时信令")
                    {
                        ControlSignal = true;
                    }
                }
                if (!ControlSignal)
                {
                    this.tabControl1.TabPages.Add(tabPage1);
                    this.tabControl1.SelectedTab = this.tabPage1;
                }

            }
            else
            {
                this.tabControl1.TabPages.Remove(tabPage1);
            }

        }




        //edit by kuang
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }




        private void 重置窗口布局ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            splitContainer2.Panel2Collapsed = false;

            // 捕获消息流
            splitContainer4.Panel1Collapsed = false; //文本框占用的
            //splitContainer3.Panel1Collapsed = false; //数据流占用的
            splitContainer4.Panel2Collapsed = false; //文本框占用的
            //splitContainer3.Panel2Collapsed = false; //数据流占用的

            splitContainer7.Panel2Collapsed = false;
            splitContainer13.Panel1Collapsed = false; // S1AP文本
            //  splitContainer14.Panel1Collapsed = false;
            splitContainer13.Panel2Collapsed = false;
            //   splitContainer14.Panel2Collapsed = false;

            splitContainer5.Panel2Collapsed = false; //Diameter
            splitContainer15.Panel1Collapsed = false;
            //   splitContainer16.Panel1Collapsed = false;
            splitContainer15.Panel2Collapsed = false;
            // splitContainer16.Panel2Collapsed = false;

            splitContainer6.Panel2Collapsed = false; //GTPv2
            splitContainer17.Panel1Collapsed = false;
            //  splitContainer18.Panel1Collapsed = false;
            splitContainer17.Panel2Collapsed = false;
            //  splitContainer18.Panel2Collapsed = false;


            splitContainer8.Panel2Collapsed = false; //协议
            splitContainer9.Panel1Collapsed = false;
            //  splitContainer10.Panel1Collapsed = false;
            splitContainer9.Panel2Collapsed = false;
            //  splitContainer10.Panel2Collapsed = false;


        }

        #region 关闭TabPage
        // edit by Kuang
        private void 关闭ToolStripMenuItem_Click(object sender, EventArgs e) //关闭捕获消息
        {
            //  tabControl1.TabPages.Remove(tabPage1);
        }

        private void 关闭ToolStripMenuItem1_Click(object sender, EventArgs e) //关闭跟踪流程
        {
            //   tabControl1.TabPages.Remove(tabPage2);
        }

        private void 关闭ToolStripMenuItem2_Click(object sender, EventArgs e) //GTPv2
        {
            tabControl1.TabPages.Remove(tabPage3);

            button_GTPv2.Enabled = true;

        }

        private void 关闭ToolStripMenuItem5_Click(object sender, EventArgs e) //DIAMETER
        {
            tabControl1.TabPages.Remove(tabPage4);
            button_DIAMETER.Enabled = true;
        }

        private void 关闭ToolStripMenuItem4_Click(object sender, EventArgs e) //S1AP
        {
            tabControl1.TabPages.Remove(tabPage5);
            button_S1AP.Enabled = true;
        }

        private void 关闭ToolStripMenuItem3_Click(object sender, EventArgs e) //信令流程图
        {
            tabControl1.TabPages.Remove(tabPage6);
        }
        #endregion

        private void TrackingProcess_Opening(object sender, CancelEventArgs e)
        {

        }



        private void MainForm_Resize(object sender, EventArgs e)
        {
            PanelReSize(this, e);


        }
        /// <summary>
        /// 当点击捕获数据流的某一行时进行的操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>


        //List<diameterCDRstruct> DIAMETERCDR = new List<diameterCDRstruct>();
        //List<diameterStruct> DIAflow =new List<diameterStruct>();
        //diameterCDRstruct diameterCDR;
        /// <summary>
        /// 当点击diameter的CDR时显示其具体的信令流程交互信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        int id_diaCDR = -1;//点击diameter的具体CDR的行号
        private void listView_DIAMETER_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView_DIAMATER.Columns.Clear();
            dataGridView_DIAMATER.BackgroundColor = Color.SkyBlue;
            dataGridView_DIAMATER.GridColor = Color.Black;
            treeView_DIAMETER.BackColor = Color.SkyBlue;
            this.dataGridView_DIAMATER.Columns.Add("Column1", "No");
            this.dataGridView_DIAMATER.Columns.Add("Column2", "Time");
            this.dataGridView_DIAMATER.Columns.Add("Column5", "信令");
            this.dataGridView_DIAMATER.Columns.Add("Column3", "MME<------>HSS");
            dataGridView_DIAMATER.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView_DIAMATER.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            int number = 0;
            string id = "0";
            string stime = "";
            string imsi = "";
            int idNum;
            int imsiListNum = 0;
            List<List<string>> temp = new List<List<string>>();

            if (listView_DIAMETER.SelectedItems != null && listView_DIAMETER.SelectedItems.Count > 0)//
            {
                ListView.SelectedIndexCollection c = listView_DIAMETER.SelectedIndices;
                stime = listView_DIAMETER.Items[c[0]].SubItems[1].Text;
                id = listView_DIAMETER.Items[c[0]].SubItems[0].Text;
                id_diaCDR = Convert.ToInt32(id) - 1;
                imsi = listView_DIAMETER.Items[c[0]].SubItems[5].Text;
                if (IMSIList.Count == 0)
                {
                    imsiListNum = IMSIList.Count;
                    IMSIList.Add(imsi);
                    diameterFlow.Add(temp);
                }
                for (int i = 0; i < IMSIList.Count; i++)
                {
                    if (IMSIList[i] == imsi)
                    {
                        imsiListNum = i;
                        break;
                    }
                    if (i == (IMSIList.Count - 1))
                    {
                        imsiListNum = IMSIList.Count;
                        IMSIList.Add(imsi);
                        diameterFlow.Add(temp);
                    }
                }

                idNum = Convert.ToInt32(id) - 1;

            }

            if (protocol[Convert.ToInt16(id)] == "sctp")
            {
                this.dataGridView_DIAMATER.Rows.Add(num1[Convert.ToInt32(id)] - 1);
                //protocol1 = "sctp";         无用变量（黄罡2017.5.27）
                for (int i = 0; i < SCTPDiameterClone.Count; i++)
                {
                    while (diameterTime_SCTP[i] == stime)
                    {
                        if (Convert.ToInt32(id) == 6)
                        { }
                        for (int j = i; j < i + num1[Convert.ToInt32(id)]; j++)
                        {
                            while (SCTPDiameterClone[j] != null)
                            {
                                List<string> flow = new List<string>();
                                SCTP.DecodeSctpChunk(SCTPDiameterClone[j]);
                                this.dataGridView_DIAMATER.Rows[number].Cells[0].Value = j - i;
                                this.dataGridView_DIAMATER.Rows[number].Cells[1].Value = diameterTime_SCTP[j];
                                flow.Add(diameterTime_SCTP[j]);
                                this.dataGridView_DIAMATER.Rows[number].Cells[2].Value = SCTP.command;
                                flow.Add(SCTP.command);
                                if (SCTP.diameterDirection == 1)
                                {
                                    this.dataGridView_DIAMATER.Rows[number].Cells[3].Value = "  |-------->|";
                                }
                                else
                                {
                                    this.dataGridView_DIAMATER.Rows[number].Cells[3].Value = " |<---------|";
                                }
                                flow.Add(Convert.ToString(SCTP.diameterDirection));
                                this.dataGridView_DIAMATER.AutoResizeColumns();
                                number++;
                                if (imsi != "")
                                {
                                    diameterFlow[imsiListNum].Add(flow);
                                }

                                break;
                            }
                        }
                        if (diameterTime_SCTP[i] == diameterTime_SCTP[i + 1])
                        { i = i + 1; }
                        break;
                    }
                }
            }
            else if (protocol[Convert.ToInt16(id)] == "tcp")
            {
                this.dataGridView_DIAMATER.Rows.Add(num1[Convert.ToInt32(id)] - 1);
                //protocol1 = "tcp";无用变量（黄罡2017.5.27）
                for (int i = 0; i < TCPDiameterClone.Count; i++)
                {
                    if (diameterTime_TCP[i] == stime)
                    {
                        for (int j = i; j < i + num1[Convert.ToInt32(id)]; j++)
                        {
                            while (TCPDiameterClone[j] != null)
                            {
                                List<string> flow = new List<string>();
                                TCPDiameter.DecodeTcpChunk(TCPDiameterClone[j]);
                                this.dataGridView_DIAMATER.Rows[number].Cells[0].Value = j - i;
                                this.dataGridView_DIAMATER.Rows[number].Cells[1].Value = diameterTime_TCP[j];
                                flow.Add(diameterTime_TCP[j]);
                                this.dataGridView_DIAMATER.Rows[number].Cells[2].Value = TCPDiameter.command;
                                flow.Add(TCPDiameter.command);
                                if (TCPDiameter.diameterDirection == 1)
                                {
                                    this.dataGridView_DIAMATER.Rows[number].Cells[3].Value = " |-------->|";
                                }
                                else
                                {
                                    this.dataGridView_DIAMATER.Rows[number].Cells[3].Value = " |<--------|";
                                }
                                flow.Add(Convert.ToString(TCPDiameter.diameterDirection));
                                this.dataGridView_DIAMATER.AutoResizeColumns();
                                number++;
                                if (imsi != "")
                                {
                                    diameterFlow[imsiListNum].Add(flow);
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }


        private void textBox1_Diameter_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_Diameter_TextChanged(object sender, EventArgs e)
        {


        }

        private void textBox1_GTPv2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_GTPv2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_S1AP_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_S1AP_TextChanged(object sender, EventArgs e)
        {


        }

        private void listView_S1AP_SelectedIndexChanged(object sender, EventArgs e)
        {
            treeView_S1AP.Nodes.Clear();
            //protocol1 = "s1ap";无用变量（黄罡2017.5.27）
            //对 dataGridView_S1AP控件进行设置
            dataGridView_S1AP.Columns.Clear();
            dataGridView_S1AP.Columns.Add("Column1", "No");
            dataGridView_S1AP.Columns.Add("Column2", "Time");
            dataGridView_S1AP.Columns.Add("Column3", "信令");
            dataGridView_S1AP.Columns.Add("Column5", "eNB<-->MME");
            dataGridView_S1AP.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView_S1AP.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView_S1AP.BackgroundColor = Color.SkyBlue;
            dataGridView_S1AP.GridColor = Color.Black;
            treeView_S1AP.BackColor = Color.SkyBlue;

            if (listView_S1AP.SelectedItems != null && listView_S1AP.SelectedItems.Count > 0)//
            {
                string IdSelected;//用户所选择CDR的序号
                ListView.SelectedIndexCollection c = listView_S1AP.SelectedIndices;
                IdSelected = listView_S1AP.Items[c[0]].SubItems[0].Text;
                IDSelected = Convert.ToInt32(IdSelected);
                this.dataGridView_S1AP.Rows.Add(CdrList[IDSelected].Count);
                //在datagridview控件中展示所选择的CDR的信令
                for (int j = 0; j < CdrList[IDSelected].Count; j++)
                {
                    #region
                    this.dataGridView_S1AP.Rows[j].Cells[0].Value = j;
                    this.dataGridView_S1AP.Rows[j].Cells[1].Value = CdrList[IDSelected][j].time1;
                    if (CdrList[IDSelected][j].nas_name != " " && (CdrList[IDSelected][j].nas_name != "ciperd message  "))
                        this.dataGridView_S1AP.Rows[j].Cells[2].Value = CdrList[IDSelected][j].s1ap_name + ' ' + CdrList[IDSelected][j].nas_name;
                    else
                        this.dataGridView_S1AP.Rows[j].Cells[2].Value = CdrList[IDSelected][j].s1ap_name;
                    if (CdrList[IDSelected][j].direction == 0)
                    {
                        this.dataGridView_S1AP.Rows[j].Cells[3].Value = "---------------------->|";
                    }
                    else if (CdrList[IDSelected][j].direction == 1)
                    {
                        this.dataGridView_S1AP.Rows[j].Cells[3].Value = "|<----------------------";
                    }
                    else if (CdrList[IDSelected][j].direction == 2)
                    {
                        this.dataGridView_S1AP.Rows[j].Cells[3].Value = "|------------------------|";
                    }
                    this.dataGridView_S1AP.AutoResizeColumns();
                    //number++;
                    #endregion
                }
            }
        }

        private void dataGridView_S1AP_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            treeView_S1AP.Nodes.Clear();
            int IDS1ap;//s1ap具体CDR中信令序号
            int rowindex = e.RowIndex;//用户所选择的行号
            #region
            //用Treeview控件展示用户所选择查看的信令的解析内容
            if (rowindex > -1)//用户所选择的行不是标题行
            {
                IDS1ap = (int)dataGridView_S1AP.Rows[rowindex].Cells[0].Value;
                TreeNode sctr = new TreeNode("S1AP Protocol", 1, 3); //表示协议类型的根节点
                treeView_S1AP.Nodes.Add(sctr);
                TreeNode scmessageType = new TreeNode("procedureCode:" + CdrList[IDSelected][IDS1ap].s1ap_name, 1, 3);
                sctr.Nodes.Add(scmessageType);
                int z = 0;
                TreeNode id0 = new TreeNode("Item" + z + ":id-MME-UE-S1AP-ID" + "(" + CdrList[IDSelected][IDS1ap].MmeUeS1apId + ")", 1, 3);
                scmessageType.Nodes.Add(id0);
                z++;
                TreeNode id1 = new TreeNode("Item" + z + ":id-eNB-UE-S1AP-ID" + "(" + CdrList[IDSelected][IDS1ap].EnbUeS1apId + ")", 1, 3);
                scmessageType.Nodes.Add(id1);
                z++;
                if (CdrList[IDSelected][IDS1ap].cause_kind != "")
                {
                    TreeNode id2 = new TreeNode("Item" + z + ":id-Cause:" + CdrList[IDSelected][IDS1ap].cause_kind, 1, 3);
                    scmessageType.Nodes.Add(id2);
                    TreeNode cause = new TreeNode(CdrList[IDSelected][IDS1ap].cause_name, 1, 3);
                    scmessageType.Nodes.Add(cause);
                    z++;
                }
                if (CdrList[IDSelected][IDS1ap].s1ap_identity != "")
                {
                    if (CdrList[IDSelected][IDS1ap].s1ap_identity.Length > 8)
                    {
                        TreeNode id3 = new TreeNode("Item" + z + ":id-IMSI:" + CdrList[IDSelected][IDS1ap].s1ap_identity, 1, 3);
                        scmessageType.Nodes.Add(id3);
                    }
                    else
                    {
                        TreeNode id3 = new TreeNode("Item" + z + ":id-TMSI:" + CdrList[IDSelected][IDS1ap].s1ap_identity, 1, 3);
                        scmessageType.Nodes.Add(id3);
                    }
                    z++;
                }
                //nas解密解码
                #region
                if (CdrList[IDSelected][IDS1ap].nas_name != " " && (CdrList[IDSelected][IDS1ap].nas_name != "ciperd message  "))//(CdrList[IDSelected][IDS1ap].nas_name !="ciperd message  ")
                {
                    #region
                    TreeNode id4 = new TreeNode("Item" + z + ":id-Nas:" + CdrList[IDSelected][IDS1ap].nas_name, 1, 3);
                    scmessageType.Nodes.Add(id4);
                    z++;

                    if (CdrList[IDSelected][IDS1ap].nas_identity != "")
                    {
                        if (CdrList[IDSelected][IDS1ap].nas_identity.Length > 8)
                        {
                            TreeNode id5 = new TreeNode("Item" + z + ":id-IMSI:" + CdrList[IDSelected][IDS1ap].nas_identity, 1, 3);
                            scmessageType.Nodes.Add(id5);
                        }
                        else
                        {
                            TreeNode id5 = new TreeNode("Item" + z + ":id-TMSI:" + CdrList[IDSelected][IDS1ap].nas_identity, 1, 3);
                            scmessageType.Nodes.Add(id5);
                        }
                        z++;
                    }

                    if (CdrList[IDSelected][IDS1ap].nas_cause != "")
                    {
                        TreeNode id6 = new TreeNode("Item" + z + ":id-Nas Cause:" + CdrList[IDSelected][IDS1ap].nas_cause, 1, 3);
                        scmessageType.Nodes.Add(id6);
                        z++;
                    }
                    if (CdrList[IDSelected][IDS1ap].ue_ip != "")
                    {
                        TreeNode id7 = new TreeNode("Item" + z + ":PDN address:" + CdrList[IDSelected][IDS1ap].ue_ip, 1, 3);
                        scmessageType.Nodes.Add(id7);
                        z++;
                    }
                    #endregion
                }
                else if (CdrList[IDSelected][IDS1ap].nas_name == "ciperd message  ")
                {
                    TreeNode id4 = new TreeNode("Item" + z + ":id-Nas:" + CdrList[IDSelected][IDS1ap].nas_name, 1, 3);
                    scmessageType.Nodes.Add(id4);
                }
                #endregion
                sctr.ExpandAll();
            }
            #endregion
        }
        static int No = -1;//存储用户所选择的listView_ProcessIMSI的item的序号
        static bool bool1 = false;//用于协助判断信令画在A-s1口还是B-s1口
        private void listView_ProcessIMSI_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView_Process.Items.Clear();
            string time;
            string message;
            if (listView_ProcessIMSI.SelectedItems != null && listView_ProcessIMSI.SelectedItems.Count > 0)//
            {
                ListView.SelectedIndexCollection c = listView_ProcessIMSI.SelectedIndices;
                No = int.Parse(listView_ProcessIMSI.Items[c[0]].SubItems[0].Text);
                int count = 0;
                //在控件listView_Process上展示用户所选择的UE的通信信令
                foreach (int key in AllUeSignal[No].Keys)
                {
                    //time = AllUeSignal[No][key].time.ToString();
                    time = AllUeSignal[No][key].absTime;
                    if (AllUeSignal[No][key].protocol == "diameter-sctp" || AllUeSignal[No][key].protocol == "diameter-tcp")
                    {
                        message = AllUeSignal[No][key].name;
                        if (AllUeSignal[No][key].direction == 1)
                        {
                            listView_Process.Items.Add(new ListViewItem(new string[] { key.ToString(), time, "", "", "|------", "------", "------", "------", "------", "----->|", message, "" }));
                        }
                        else
                        {
                            listView_Process.Items.Add(new ListViewItem(new string[] { key.ToString(), time, "", "", "|<----", "------", "------", "------", "------", "------|", message, "" }));
                        }
                        listView_Process.Items[count].BackColor = System.Drawing.Color.LightGray;
                    }
                    else if (AllUeSignal[No][key].protocol == "S1AP")
                    {
                        message = AllUeSignal[No][key].name;
                        bool bool2 = source(message);
                        //当遇到切换流程时，将源侧和目的侧的s1ap信令分别放在A-s1口和B-s1口
                        if (message == "HandoverRequired")
                            bool1 = !bool1;
                        bool bool4 = !(bool1 ^ bool2);//(bool1与bool2同或操作)
                        if (bool4 == true && AllUeSignal[No][key].direction == 0)//bool4==true,信令画在A-s1口，否则在B-s1口
                            listView_Process.Items.Add(new ListViewItem(new string[] { key.ToString(), time, "|------", "------", "----->|", "", "", "", "", "", message, NasList[No][key].nas_name }));
                        else if (bool4 == true && AllUeSignal[No][key].direction == 1)
                            listView_Process.Items.Add(new ListViewItem(new string[] { key.ToString(), time, "|<------", "------", "------|", "", "", "", "", "", message, NasList[No][key].nas_name }));
                        else if (bool4 == false && AllUeSignal[No][key].direction == 0)
                            listView_Process.Items.Add(new ListViewItem(new string[] { key.ToString(), time, "", "|------", "------>|", "", "", "", "", "", message, NasList[No][key].nas_name }));
                        else
                            listView_Process.Items.Add(new ListViewItem(new string[] { key.ToString(), time, "", "|<------", "------|", "", "", "", "", "", message, NasList[No][key].nas_name }));
                        listView_Process.Items[count].BackColor = System.Drawing.Color.PowderBlue;
                    }
                    else
                    {
                        message = AllUeSignal[No][key].name;
                        string faceType = AllUeSignal[No][key].face;
                        if (faceType == "S10")
                        {
                            listView_Process.Items.Add(new ListViewItem(new string[] { key.ToString(), time, "", "", "|------", "------|", "", "", "", "", message, "" }));
                        }
                        if (faceType == "S5")
                        {
                            listView_Process.Items.Add(new ListViewItem(new string[] { key.ToString(), time, "", "", "", "", "", "|------", "------|", "", message, "" }));
                        }
                        listView_Process.Items[count].BackColor = System.Drawing.Color.LightGoldenrodYellow;
                    }
                    count++;
                }
            }


        }

        private bool source(string a)//判定一条信令是否是切换的源侧信令
        {
            string[] b = new string[6]{"HandoverRequired","HandoverRequestAcknowledge","HandoverFailure",
                "HandoverNotify","HandoverCancel","eNBStatusTransfer"};
            bool bool3 = false;
            for (int k = 0; k < b.Length; k++)
            {
                if (a == b[k])
                {
                    bool3 = true;
                    break;
                }
                else
                    bool3 = false;
            }
            return bool3;
        }

        #region  GTP相关  by刘黎
        int GtpSubCdrNum;   //记录当前选中的subCDR
        private void listView_GTPv2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            #region
            dataGridView_GTPv2.Columns.Clear();
            dataGridView_GTPv2.Rows.Clear();
            treeView_GTPv2.Nodes.Clear();
            dataGridView_GTPv2.BackgroundColor = Color.SkyBlue;
            dataGridView_GTPv2.GridColor = Color.Black; ;
            treeView_GTPv2.BackColor = Color.SkyBlue;
            //protocol1 = "gtpv2";        //无用变量  （黄罡2017.5.27）
            if (listView_GTPv2.SelectedItems != null && listView_GTPv2.SelectedItems.Count > 0)
            {
                int a = listView_GTPv2.SelectedItems[0].Index;
                GtpSubCdrNum = a;
                dataGridView_GTPv2.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView_GTPv2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                string SorIp, DesIp;
                DesIp = Convert.ToString(GTPCDR[a].CDRbuffer[0].gtpFrame[12]) + "." + Convert.ToString(GTPCDR[a].CDRbuffer[0].gtpFrame[13]) + "." + Convert.ToString(GTPCDR[a].CDRbuffer[0].gtpFrame[14]) + "." + Convert.ToString(GTPCDR[a].CDRbuffer[0].gtpFrame[15]);
                SorIp = Convert.ToString(GTPCDR[a].CDRbuffer[0].gtpFrame[16]) + "." + Convert.ToString(GTPCDR[a].CDRbuffer[0].gtpFrame[17]) + "." + Convert.ToString(GTPCDR[a].CDRbuffer[0].gtpFrame[18]) + "." + Convert.ToString(GTPCDR[a].CDRbuffer[0].gtpFrame[19]);
                dataGridView_GTPv2.Columns.Add("No", "No");
                dataGridView_GTPv2.Columns.Add("时间", "时间");
                dataGridView_GTPv2.Columns.Add("SorIp", SorIp);
                dataGridView_GTPv2.Columns.Add("DesIp", DesIp);
                dataGridView_GTPv2.Columns.Add("Message", "信令");
                dataGridView_GTPv2.Columns[0].Width = 40;
                dataGridView_GTPv2.Columns[1].Width = 95;
                dataGridView_GTPv2.Columns[2].Width = 95;
                // dataGridView_GTPv2.Rows.Add(subCDR_Copy[a].Count - 1);
                dataGridView_GTPv2.Rows.Add(subCDR_Copy[a].Count);
                for (int i = 0; i < GTPCDR[a].CDRbuffer.Count; i++)
                {
                    dataGridView_GTPv2.Rows[i].Cells[4].Value = GTPCDR[a].CDRbuffer[i].messageType;
                    dataGridView_GTPv2.Rows[i].Cells[0].Value = i + 1;
                    dataGridView_GTPv2.Rows[i].Cells[1].Value = GTPCDR[a].CDRbuffer[i].timeAll;
                    string soIP;
                    soIP = Convert.ToString(GTPCDR[a].CDRbuffer[i].gtpFrame[16]) + "." + Convert.ToString(GTPCDR[a].CDRbuffer[i].gtpFrame[17]) + "." + Convert.ToString(GTPCDR[a].CDRbuffer[i].gtpFrame[18]) + "." + Convert.ToString(GTPCDR[a].CDRbuffer[i].gtpFrame[19]);
                    if (soIP == SorIp)
                    {
                        this.dataGridView_GTPv2.Rows[i].Cells[2].Value = "|------------";
                        this.dataGridView_GTPv2.Rows[i].Cells[3].Value = "----------->|";
                    }
                    else
                    {
                        this.dataGridView_GTPv2.Rows[i].Cells[2].Value = "|<-----------";
                        this.dataGridView_GTPv2.Rows[i].Cells[3].Value = "------------|";
                    }
                }
            }
            #endregion
        }

        private void dataGridView_GTPv2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            treeView_GTPv2.Nodes.Clear();
            int rowindex = e.RowIndex;
            gtpv2 Gtp = new gtpv2();
            // if (rowindex > -1)
            if (rowindex >= 0 && rowindex < GTPCDR[GtpSubCdrNum].CDRbuffer.Count)   //选定行数有效
            {
                byte[] GTPv2 = new byte[GTPCDR[GtpSubCdrNum].CDRbuffer[rowindex].gtpFrame.Length - 28];
                for (int j = 0; j < GTPCDR[GtpSubCdrNum].CDRbuffer[rowindex].gtpFrame.Length - 28; j++)
                {
                    GTPv2[j] = GTPCDR[GtpSubCdrNum].CDRbuffer[rowindex].gtpFrame[28 + j];
                }
                Gtp.decoder(GTPv2);
                TreeNode tr = new TreeNode("GTP Protocol", 1, 3); //表示协议类型的根节点
                TreeNode messageType = new TreeNode(Gtp.mt, 1, 3);
                TreeNode htr = new TreeNode("Header", 1, 3);     //表示解析消息头的节点
                TreeNode Cause = new TreeNode("Cause", 1, 3);
                treeView_GTPv2.Nodes.Add(tr);
                tr.Nodes.Add(messageType);
                tr.Nodes.Add(htr);
                tr.Nodes.Add(Cause);
                htr.Nodes.Add("", Gtp.ver, 1, 3);
                htr.Nodes.Add("", Gtp.pinfo, 1, 3);
                htr.Nodes.Add("", Gtp.tinfo, 1, 3);
                htr.Nodes.Add("", Gtp.mt, 1, 3);
                htr.Nodes.Add("", Gtp.leninfo, 1, 3);
                Cause.Nodes.Add("", Gtp.IEvalue, 1, 3);
                if (Gtp.IMSI != "")
                {
                    TreeNode IMSI = new TreeNode("IMSI", 1, 3);
                    IMSI.Nodes.Add("", Gtp.IMSI, 1, 3);
                    tr.Nodes.Add(IMSI);
                }
                if (Gtp.tflag == 1)
                {
                    htr.Nodes.Add("", Gtp.teidinfo, 1, 3);
                }
                htr.Nodes.Add("", Gtp.seqinfo, 1, 3);
                htr.Nodes.Add("", Gtp.spareinfo, 1, 3);
                tr.ExpandAll();
            }
            else ;
        }
        #endregion
        //当用户选择一个ue的通信流程中的一条信令时，下面的事件用控件Treeview展示解码内容，且用控件textbox展示信令编码
        private void listView_Process_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idProcess;//用户所选择的信令的序号
            treeView_Process.Nodes.Clear();
            treeView_Process.BackColor = Color.SkyBlue;
            textBox5.Text = "";
            textBox5.BackColor = Color.SkyBlue;
            if (listView_Process.SelectedItems != null && listView_Process.SelectedItems.Count > 0)//
            {
                ListView.SelectedIndexCollection c = listView_Process.SelectedIndices;
                idProcess = int.Parse(listView_Process.Items[c[0]].SubItems[0].Text);
                if (AllUeSignal[No][idProcess].protocol == "diameter-sctp")
                {
                    #region
                    byte[] sctpDia = new byte[AllUeSignal[No][idProcess].frame.Length - 20];
                    Array.Copy(AllUeSignal[No][idProcess].frame, 20, sctpDia, 0, AllUeSignal[No][idProcess].frame.Length - 20);
                    SCTP.DecodeSctpChunk(sctpDia);
                    string temp = "";
                    foreach (byte ab in AllUeSignal[No][idProcess].frame)
                    {
                        temp += ab.ToString("X2") + " ";
                    }
                    textBox5.Text = temp;
                    TreeNode sctr = new TreeNode("SCTP Protocol", 1, 3); //表示协议类型的根节点
                    treeView_Process.Nodes.Add(sctr);
                    TreeNode scmessageType = new TreeNode(SCTP.protocol_Treeview, 1, 3);
                    sctr.Nodes.Add(scmessageType);
                    TreeNode version = new TreeNode(SCTP.version, 1, 3);     //表示解析消息头的节点
                    TreeNode length = new TreeNode(SCTP.length, 1, 3);
                    TreeNode flag = new TreeNode(SCTP.flag, 1, 3);
                    TreeNode commandCODE = new TreeNode(SCTP.commandCODE, 1, 3);
                    TreeNode applicationID = new TreeNode(SCTP.applicationID, 1, 3);
                    TreeNode H2HIdentifier = new TreeNode(SCTP.H2HIdentifier, 1, 3);
                    TreeNode E2EIdentifier = new TreeNode(SCTP.E2EIdentifier, 1, 3);
                    TreeNode AVP = new TreeNode("AVP", 1, 3);
                    scmessageType.Nodes.Add(version);
                    scmessageType.Nodes.Add(length);
                    scmessageType.Nodes.Add(flag);
                    scmessageType.Nodes.Add(commandCODE);
                    scmessageType.Nodes.Add(applicationID);
                    scmessageType.Nodes.Add(H2HIdentifier);
                    scmessageType.Nodes.Add(E2EIdentifier);
                    scmessageType.Nodes.Add(AVP);
                    while (SCTP.userNameCode != "")
                    {
                        TreeNode AVP1 = new TreeNode("User-Name AVP", 1, 3);
                        AVP.Nodes.Add(AVP1);
                        TreeNode avp1 = new TreeNode(SCTP.userNameCode, 1, 3);
                        TreeNode avp2 = new TreeNode(SCTP.userNameFlag, 1, 3);
                        TreeNode avp3 = new TreeNode(SCTP.userNameLength, 1, 3);
                        TreeNode avp4 = new TreeNode(SCTP.userNameStr, 1, 3);
                        AVP1.Nodes.Add(avp1);
                        AVP1.Nodes.Add(avp2);
                        AVP1.Nodes.Add(avp3);
                        AVP1.Nodes.Add(avp4);
                        break;
                    }
                    while (SCTP.avp_ResultcodeCode != "")
                    {
                        TreeNode AVP1 = new TreeNode("Result-CODE AVP", 1, 3);
                        AVP.Nodes.Add(AVP1);
                        TreeNode avp1 = new TreeNode(SCTP.avp_ResultcodeCode, 1, 3);
                        TreeNode avp2 = new TreeNode(SCTP.avp_ResultcodeFlag, 1, 3);
                        TreeNode avp3 = new TreeNode(SCTP.avp_ResultcodeLength, 1, 3);
                        TreeNode avp4 = new TreeNode(SCTP.avp_ResultcodeStr, 1, 3);
                        AVP1.Nodes.Add(avp1);
                        AVP1.Nodes.Add(avp2);
                        AVP1.Nodes.Add(avp3);
                        AVP1.Nodes.Add(avp4);
                        break;
                    }

                    while (SCTP.avp_AuthenticationCode != "")
                    {
                        TreeNode AVP2 = new TreeNode("Authentication-Info AVP", 1, 3);
                        AVP.Nodes.Add(AVP2);
                        TreeNode avp5 = new TreeNode(SCTP.avp_AuthenticationCode, 1, 3);
                        TreeNode avp6 = new TreeNode(SCTP.avp_AuthenticationFlag, 1, 3);
                        TreeNode avp7 = new TreeNode(SCTP.avp_AuthenticationLength, 1, 3);
                        TreeNode avp8 = new TreeNode(SCTP.avp_AuthenticationVendorID, 1, 3);
                        AVP2.Nodes.Add(avp5);
                        AVP2.Nodes.Add(avp6);
                        AVP2.Nodes.Add(avp7);
                        AVP2.Nodes.Add(avp8);
                        for (int iii = 0; iii < SCTP.avp_EUTRANVectorCode.Count; iii++)
                        {
                            while (SCTP.avp_EUTRANVectorCode[iii] != "")
                            {
                                TreeNode AVP3 = new TreeNode("E-UTRAN-Vector AVP", 1, 3);
                                AVP2.Nodes.Add(AVP3);
                                TreeNode avp9 = new TreeNode(SCTP.avp_EUTRANVectorCode[iii], 1, 3);
                                TreeNode avp10 = new TreeNode(SCTP.avp_EUTRANVectorFlag[iii], 1, 3);
                                TreeNode avp11 = new TreeNode(SCTP.avp_EUTRANVectorLength[iii], 1, 3);
                                AVP3.Nodes.Add(avp9);
                                AVP3.Nodes.Add(avp10);
                                AVP3.Nodes.Add(avp11);
                                if (SCTP.avp_EUTRANVectorVendorID.Count != 0)
                                {
                                    TreeNode avp12 = new TreeNode(SCTP.avp_EUTRANVectorVendorID[iii], 1, 3);
                                    AVP3.Nodes.Add(avp12);
                                }
                                TreeNode AVP4 = new TreeNode("KASME AVP", 1, 3);
                                AVP3.Nodes.Add(AVP4);
                                TreeNode avp13 = new TreeNode(SCTP.avp_KASMECode[iii], 1, 3);
                                TreeNode avp14 = new TreeNode(SCTP.avp_KASMEFlag[iii], 1, 3);
                                TreeNode avp15 = new TreeNode(SCTP.avp_KASMELength[iii], 1, 3);
                                TreeNode KASMEStr = new TreeNode(SCTP.KASMEStr[iii], 1, 3);
                                AVP4.Nodes.Add(avp13);
                                AVP4.Nodes.Add(avp14);
                                AVP4.Nodes.Add(avp15);
                                AVP4.Nodes.Add(KASMEStr);
                                break;
                            }
                        }
                        break;
                    }
                    while (SCTP.avp_SessionID1 != "")
                    {
                        TreeNode AVP5 = new TreeNode("Session-ID AVP", 1, 3);
                        AVP.Nodes.Add(AVP5);
                        TreeNode avp16 = new TreeNode(SCTP.avp_SessionID1, 1, 3);
                        TreeNode avp17 = new TreeNode(SCTP.avp_SessionID2, 1, 3);
                        TreeNode avp18 = new TreeNode(SCTP.avp_SessionID3, 1, 3);
                        TreeNode avp19 = new TreeNode(SCTP.avp_SessionID4, 1, 3);
                        AVP5.Nodes.Add(avp16);
                        AVP5.Nodes.Add(avp17);
                        AVP5.Nodes.Add(avp18);
                        AVP5.Nodes.Add(avp19);
                        break;
                    }
                    sctr.ExpandAll();
                }
                else if (AllUeSignal[No][idProcess].protocol == "diameter-tcp")
                {

                    byte[] tcpDia = new byte[AllUeSignal[No][idProcess].frame.Length - 20];
                    Array.Copy(AllUeSignal[No][idProcess].frame, 20, tcpDia, 0, AllUeSignal[No][idProcess].frame.Length - 20);
                    TCPDiameter.DecodeTcpChunk(tcpDia);

                    string temp = "";
                    foreach (byte ab in AllUeSignal[No][idProcess].frame)
                    {
                        temp += ab.ToString("X2") + " ";
                    }
                    textBox5.Text = temp;
                    TreeNode sctr = new TreeNode("TCP Protocol", 1, 3); //表示协议类型的根节点
                    treeView_Process.Nodes.Add(sctr);
                    TreeNode scmessageType = new TreeNode(TCPDiameter.protocol_Treeview, 1, 3);
                    sctr.Nodes.Add(scmessageType);
                    TreeNode version = new TreeNode(TCPDiameter.Version, 1, 3);     //表示解析消息头的节点
                    TreeNode length = new TreeNode(TCPDiameter.Length, 1, 3);
                    TreeNode flag = new TreeNode(TCPDiameter.Flag, 1, 3);
                    TreeNode commandCODE = new TreeNode(TCPDiameter.CommandCODE, 1, 3);
                    TreeNode applicationID = new TreeNode(TCPDiameter.ApplicationID, 1, 3);
                    TreeNode H2HIdentifier = new TreeNode(TCPDiameter.H2HIdentifier, 1, 3);
                    TreeNode E2EIdentifier = new TreeNode(TCPDiameter.E2EIdentifier, 1, 3);
                    TreeNode AVP = new TreeNode("AVP", 1, 3);
                    scmessageType.Nodes.Add(version);
                    scmessageType.Nodes.Add(length);
                    scmessageType.Nodes.Add(flag);
                    scmessageType.Nodes.Add(commandCODE);
                    scmessageType.Nodes.Add(applicationID);
                    scmessageType.Nodes.Add(H2HIdentifier);
                    scmessageType.Nodes.Add(E2EIdentifier);
                    scmessageType.Nodes.Add(AVP);
                    while (TCPDiameter.userNameCode != "")
                    {
                        TreeNode AVP1 = new TreeNode("User-Name AVP", 1, 3);
                        AVP.Nodes.Add(AVP1);
                        TreeNode avp1 = new TreeNode(TCPDiameter.userNameCode, 1, 3);
                        TreeNode avp2 = new TreeNode(TCPDiameter.userNameFlag, 1, 3);
                        TreeNode avp3 = new TreeNode(TCPDiameter.userNameLength, 1, 3);
                        TreeNode avp4 = new TreeNode(TCPDiameter.userNameStr, 1, 3);
                        AVP1.Nodes.Add(avp1);
                        AVP1.Nodes.Add(avp2);
                        AVP1.Nodes.Add(avp3);
                        AVP1.Nodes.Add(avp4);
                        break;
                    }
                    while (TCPDiameter.avp_ResultcodeCode != "")
                    {
                        TreeNode AVP1 = new TreeNode("Result-CODE AVP", 1, 3);
                        AVP.Nodes.Add(AVP1);
                        TreeNode avp1 = new TreeNode(TCPDiameter.avp_ResultcodeCode, 1, 3);
                        TreeNode avp2 = new TreeNode(TCPDiameter.avp_ResultcodeFlag, 1, 3);
                        TreeNode avp3 = new TreeNode(TCPDiameter.avp_ResultcodeLength, 1, 3);
                        TreeNode avp4 = new TreeNode(TCPDiameter.avp_ResultcodeStr, 1, 3);
                        AVP1.Nodes.Add(avp1);
                        AVP1.Nodes.Add(avp2);
                        AVP1.Nodes.Add(avp3);
                        AVP1.Nodes.Add(avp4);
                        break;
                    }
                    while (TCPDiameter.avp_AuthenticationCode != "")
                    {
                        TreeNode AVP2 = new TreeNode("Authentication-Info AVP", 1, 3);
                        AVP.Nodes.Add(AVP2);
                        TreeNode avp5 = new TreeNode(TCPDiameter.avp_AuthenticationCode, 1, 3);
                        TreeNode avp6 = new TreeNode(TCPDiameter.avp_AuthenticationFlag, 1, 3);
                        TreeNode avp7 = new TreeNode(TCPDiameter.avp_AuthenticationLength, 1, 3);
                        TreeNode avp8 = new TreeNode(TCPDiameter.avp_AuthenticationVendorID, 1, 3);
                        AVP2.Nodes.Add(avp5);
                        AVP2.Nodes.Add(avp6);
                        AVP2.Nodes.Add(avp7);
                        AVP2.Nodes.Add(avp8);
                        for (int iii = 0; iii < TCPDiameter.avp_EUTRANVectorCode.Count; iii++)
                        {
                            #region
                            while (TCPDiameter.avp_EUTRANVectorCode[iii] != "")
                            {
                                #region
                                TreeNode AVP3 = new TreeNode("E-UTRAN-Vector AVP", 1, 3);
                                AVP.Nodes.Add(AVP3);
                                TreeNode avp9 = new TreeNode(TCPDiameter.avp_EUTRANVectorCode[iii], 1, 3);
                                TreeNode avp10 = new TreeNode(TCPDiameter.avp_EUTRANVectorFlag[iii], 1, 3);
                                TreeNode avp11 = new TreeNode(TCPDiameter.avp_EUTRANVectorLength[iii], 1, 3);

                                AVP3.Nodes.Add(avp9);
                                AVP3.Nodes.Add(avp10);
                                AVP3.Nodes.Add(avp11);
                                if (TCPDiameter.avp_EUTRANVectorVendorID.Count != 0)
                                {
                                    TreeNode avp12 = new TreeNode(TCPDiameter.avp_EUTRANVectorVendorID[iii], 1, 3);
                                    AVP3.Nodes.Add(avp12);
                                }
                                break;
                                #endregion
                            }
                            while (TCPDiameter.avp_KASMECode[iii] != "")
                            {
                                TreeNode AVP4 = new TreeNode("KASME AVP", 1, 3);
                                AVP.Nodes.Add(AVP4);
                                TreeNode avp13 = new TreeNode(TCPDiameter.avp_KASMECode[iii], 1, 3);
                                TreeNode avp14 = new TreeNode(TCPDiameter.avp_KASMEFlag[iii], 1, 3);
                                TreeNode avp15 = new TreeNode(TCPDiameter.avp_KASMELength[iii], 1, 3);
                                TreeNode KASMEStr = new TreeNode(TCPDiameter.KASMEStr[iii], 1, 3);
                                AVP4.Nodes.Add(avp13);
                                AVP4.Nodes.Add(avp14);
                                AVP4.Nodes.Add(avp15);
                                AVP4.Nodes.Add(KASMEStr);
                                break;
                            }
                            #endregion
                        }
                        break;
                    }
                    while (TCPDiameter.avp_SessionID1 != "")
                    {
                        TreeNode AVP5 = new TreeNode("Session-ID AVP", 1, 3);
                        AVP.Nodes.Add(AVP5);
                        TreeNode avp16 = new TreeNode(TCPDiameter.avp_SessionID1, 1, 3);
                        TreeNode avp17 = new TreeNode(TCPDiameter.avp_SessionID2, 1, 3);
                        TreeNode avp18 = new TreeNode(TCPDiameter.avp_SessionID3, 1, 3);
                        TreeNode avp19 = new TreeNode(TCPDiameter.avp_SessionID4, 1, 3);
                        AVP5.Nodes.Add(avp16);
                        AVP5.Nodes.Add(avp17);
                        AVP5.Nodes.Add(avp18);
                        AVP5.Nodes.Add(avp19);
                        break;
                    }
                    sctr.ExpandAll();
                    #endregion
                }
                else if (AllUeSignal[No][idProcess].protocol == "S1AP") //s1ap对应的数据解析、树状结构以及数据流展示(注意S1ap的消息内容一部分在 AllUeSignal中，一部分在NasList中)
                {
                    #region
                    string temp = "";
                    foreach (byte ab in AllUeSignal[No][idProcess].frame)
                    {
                        temp += ab.ToString("X2") + " ";
                    }
                    textBox5.Text = temp;
                    TreeNode sctr1 = new TreeNode("Source IP address:" + AllUeSignal[No][idProcess].src_ip, 1, 3); //表示协议类型的根节点
                    treeView_Process.Nodes.Add(sctr1);
                    TreeNode sctr2 = new TreeNode("Destination IP address:" + AllUeSignal[No][idProcess].des_ip, 1, 3); //表示协议类型的根节点
                    treeView_Process.Nodes.Add(sctr2);
                    TreeNode sctr = new TreeNode("S1AP Protocol", 1, 3); //表示协议类型的根节点
                    treeView_Process.Nodes.Add(sctr);
                    TreeNode scmessageType = new TreeNode("procedureCode:" + AllUeSignal[No][idProcess].name, 1, 3);
                    sctr.Nodes.Add(scmessageType);
                    int z = 0;
                    TreeNode id0 = new TreeNode("Item" + z + ":id-MME-UE-S1AP-ID" + "(" + NasList[No][idProcess].MmeUeS1apId + ")", 1, 3);
                    scmessageType.Nodes.Add(id0);
                    z++;
                    TreeNode id1 = new TreeNode("Item" + z + ":id-eNB-UE-S1AP-ID" + "(" + NasList[No][idProcess].EnbUeS1apId + ")", 1, 3);
                    scmessageType.Nodes.Add(id1);
                    z++;
                    if (NasList[No][idProcess].cause_kind != "")
                    {
                        TreeNode id2 = new TreeNode("Item" + z + ":id-Cause:" + NasList[No][idProcess].cause_kind, 1, 3);
                        scmessageType.Nodes.Add(id2);
                        TreeNode cause = new TreeNode(NasList[No][idProcess].cause_name, 1, 3);
                        scmessageType.Nodes.Add(cause);
                        z++;
                    }
                    if (NasList[No][idProcess].s1ap_identity != "")
                    {
                        if (NasList[No][idProcess].s1ap_identity.Length > 8)
                        {
                            TreeNode id3 = new TreeNode("Item" + z + ":id-IMSI:" + NasList[No][idProcess].s1ap_identity, 1, 3);
                            scmessageType.Nodes.Add(id3);
                        }
                        else
                        {
                            TreeNode id3 = new TreeNode("Item" + z + ":id-TMSI:" + NasList[No][idProcess].s1ap_identity, 1, 3);
                            scmessageType.Nodes.Add(id3);
                        }
                        z++;
                    }
                    if (NasList[No][idProcess].nas_name != " " && (NasList[No][idProcess].nas_name != "ciperd message  "))//(CdrList[IDSelected][IDS1ap].nas_name !="ciperd message  ")
                    {
                        #region
                        TreeNode id4 = new TreeNode("Item" + z + ":id-Nas:" + NasList[No][idProcess].nas_name, 1, 3);
                        scmessageType.Nodes.Add(id4);
                        z++;

                        if (NasList[No][idProcess].nas_identity != "")
                        {
                            if (NasList[No][idProcess].nas_identity.Length > 8)
                            {
                                TreeNode id5 = new TreeNode("Item" + z + ":id-IMSI:" + NasList[No][idProcess].nas_identity, 1, 3);
                                scmessageType.Nodes.Add(id5);
                            }
                            else
                            {
                                TreeNode id5 = new TreeNode("Item" + z + ":id-TMSI:" + NasList[No][idProcess].nas_identity, 1, 3);
                                scmessageType.Nodes.Add(id5);
                            }
                            z++;
                        }
                        if (NasList[No][idProcess].nas_cause != "")
                        {
                            TreeNode id6 = new TreeNode("Item" + z + ":id-Nas Cause:" + NasList[No][idProcess].nas_cause, 1, 3);
                            scmessageType.Nodes.Add(id6);
                            z++;
                        }
                        if (NasList[No][idProcess].ue_ip != "")
                        {
                            TreeNode id7 = new TreeNode("Item" + z + ":PDN address:" + NasList[No][idProcess].ue_ip, 1, 3);
                            scmessageType.Nodes.Add(id7);
                            z++;
                        }
                        #endregion
                    }
                    else if (NasList[No][idProcess].nas_name == "ciperd message  ")
                    {
                        TreeNode id4 = new TreeNode("Item" + z + ":id-Nas:" + NasList[No][idProcess].nas_name, 1, 3);
                        scmessageType.Nodes.Add(id4);
                    }
                    sctr.ExpandAll();
                    #endregion
                }
                else//GTPv2对应的数据解析、树状结构以及数据流展示
                {
                    #region GTPv2解析
                    string temp = "";
                    foreach (byte data in AllUeSignal[No][idProcess].frame)
                    {
                        temp += data.ToString("X2") + " ";
                    }
                    textBox5.Text = temp;
                    gtpv2 Gtp = new gtpv2();
                    byte[] GTPv2 = new byte[AllUeSignal[No][idProcess].frame.Length - 28];
                    for (int j = 0; j < AllUeSignal[No][idProcess].frame.Length - 28; j++)
                    {
                        GTPv2[j] = AllUeSignal[No][idProcess].frame[28 + j];
                    }
                    Gtp.decoder(GTPv2);
                    TreeNode tr = new TreeNode("GTP Protocol", 1, 3); //表示协议类型的根节点
                    TreeNode messageType = new TreeNode(Gtp.mt, 1, 3);
                    TreeNode htr = new TreeNode("Header", 1, 3);     //表示解析消息头的节点
                    TreeNode Cause = new TreeNode("Cause", 1, 3);
                    treeView_Process.Nodes.Add(tr);
                    tr.Nodes.Add(messageType);
                    tr.Nodes.Add(htr);
                    tr.Nodes.Add(Cause);
                    htr.Nodes.Add("", Gtp.ver, 1, 3);
                    htr.Nodes.Add("", Gtp.pinfo, 1, 3);
                    htr.Nodes.Add("", Gtp.tinfo, 1, 3);
                    htr.Nodes.Add("", Gtp.mt, 1, 3);
                    htr.Nodes.Add("", Gtp.leninfo, 1, 3);
                    Cause.Nodes.Add("", Gtp.IEvalue, 1, 3);
                    if (Gtp.IMSI != "")
                    {
                        TreeNode IMSI = new TreeNode("IMSI", 1, 3);
                        IMSI.Nodes.Add("", Gtp.IMSI, 1, 3);
                        tr.Nodes.Add(IMSI);
                    }
                    if (Gtp.tflag == 1)
                    {
                        htr.Nodes.Add("", Gtp.teidinfo, 1, 3);
                    }
                    htr.Nodes.Add("", Gtp.seqinfo, 1, 3);
                    htr.Nodes.Add("", Gtp.spareinfo, 1, 3);
                    tr.ExpandAll();
                    #endregion
                }
            }
        }

        private void dataGridView_DIAMATER_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            treeView_DIAMETER.Nodes.Clear();
            int IDS1ap = -1;//s1ap具体CDR中信令序号
            int rowindex = e.RowIndex;
            if (rowindex > -1)//黄罡
            {
                #region
                IDS1ap = (int)dataGridView_DIAMATER.Rows[rowindex].Cells[0].Value;
                int lenth_dia = DIAMETERCDR[id_diaCDR].CDRbuffer[IDS1ap].diameterFrame.Length;
                byte[] ST_dia = new byte[lenth_dia - 20];
                Array.Copy(DIAMETERCDR[id_diaCDR].CDRbuffer[IDS1ap].diameterFrame, 20, ST_dia, 0, lenth_dia - 20);

                if (DIAMETERCDR[id_diaCDR].CDRbuffer[IDS1ap].protocol == "diameter-sctp")
                {
                    SCTP.DecodeSctpChunk(ST_dia);
                    TreeNode sctr = new TreeNode("SCTP Protocol", 1, 3); //表示协议类型的根节点
                    treeView_DIAMETER.Nodes.Add(sctr);
                    TreeNode scmessageType = new TreeNode(SCTP.protocol_Treeview, 1, 3);
                    sctr.Nodes.Add(scmessageType);
                    TreeNode version = new TreeNode(SCTP.version, 1, 3);     //表示解析消息头的节点
                    TreeNode length = new TreeNode(SCTP.length, 1, 3);
                    TreeNode flag = new TreeNode(SCTP.flag, 1, 3);
                    TreeNode commandCODE = new TreeNode(SCTP.commandCODE, 1, 3);
                    TreeNode applicationID = new TreeNode(SCTP.applicationID, 1, 3);
                    TreeNode H2HIdentifier = new TreeNode(SCTP.H2HIdentifier, 1, 3);
                    TreeNode E2EIdentifier = new TreeNode(SCTP.E2EIdentifier, 1, 3);
                    TreeNode AVP = new TreeNode("AVP", 1, 3);
                    scmessageType.Nodes.Add(version);
                    scmessageType.Nodes.Add(length);
                    scmessageType.Nodes.Add(flag);
                    scmessageType.Nodes.Add(commandCODE);
                    scmessageType.Nodes.Add(applicationID);
                    scmessageType.Nodes.Add(H2HIdentifier);
                    scmessageType.Nodes.Add(E2EIdentifier);
                    scmessageType.Nodes.Add(AVP);
                    while (SCTP.userNameCode != "")
                    {
                        TreeNode AVP1 = new TreeNode("User-Name AVP", 1, 3);
                        AVP.Nodes.Add(AVP1);
                        TreeNode avp1 = new TreeNode(SCTP.userNameCode, 1, 3);
                        TreeNode avp2 = new TreeNode(SCTP.userNameFlag, 1, 3);
                        TreeNode avp3 = new TreeNode(SCTP.userNameLength, 1, 3);
                        TreeNode avp4 = new TreeNode(SCTP.userNameStr, 1, 3);
                        AVP1.Nodes.Add(avp1);
                        AVP1.Nodes.Add(avp2);
                        AVP1.Nodes.Add(avp3);
                        AVP1.Nodes.Add(avp4);
                        break;
                    }
                    while (SCTP.avp_ResultcodeCode != "")
                    {
                        TreeNode AVP1 = new TreeNode("Result-CODE AVP", 1, 3);
                        AVP.Nodes.Add(AVP1);
                        TreeNode avp1 = new TreeNode(SCTP.avp_ResultcodeCode, 1, 3);
                        TreeNode avp2 = new TreeNode(SCTP.avp_ResultcodeFlag, 1, 3);
                        TreeNode avp3 = new TreeNode(SCTP.avp_ResultcodeLength, 1, 3);
                        TreeNode avp4 = new TreeNode(SCTP.avp_ResultcodeStr, 1, 3);
                        AVP1.Nodes.Add(avp1);
                        AVP1.Nodes.Add(avp2);
                        AVP1.Nodes.Add(avp3);
                        AVP1.Nodes.Add(avp4);
                        break;
                    }

                    while (SCTP.avp_AuthenticationCode != "")
                    {
                        TreeNode AVP2 = new TreeNode("Authentication-Info AVP", 1, 3);
                        AVP.Nodes.Add(AVP2);
                        TreeNode avp5 = new TreeNode(SCTP.avp_AuthenticationCode, 1, 3);
                        TreeNode avp6 = new TreeNode(SCTP.avp_AuthenticationFlag, 1, 3);
                        TreeNode avp7 = new TreeNode(SCTP.avp_AuthenticationLength, 1, 3);
                        TreeNode avp8 = new TreeNode(SCTP.avp_AuthenticationVendorID, 1, 3);
                        AVP2.Nodes.Add(avp5);
                        AVP2.Nodes.Add(avp6);
                        AVP2.Nodes.Add(avp7);
                        if (SCTP.avp_AuthenticationVendorID != "")
                        {
                            AVP2.Nodes.Add(avp8);
                        }
                        for (int iii = 0; iii < SCTP.avp_EUTRANVectorCode.Count; iii++)
                        {
                            while (SCTP.avp_EUTRANVectorCode[iii] != "")
                            {
                                TreeNode AVP3 = new TreeNode("E-UTRAN-Vector AVP", 1, 3);
                                AVP2.Nodes.Add(AVP3);
                                TreeNode avp9 = new TreeNode(SCTP.avp_EUTRANVectorCode[iii], 1, 3);
                                TreeNode avp10 = new TreeNode(SCTP.avp_EUTRANVectorFlag[iii], 1, 3);
                                TreeNode avp11 = new TreeNode(SCTP.avp_EUTRANVectorLength[iii], 1, 3);
                                AVP3.Nodes.Add(avp9);
                                AVP3.Nodes.Add(avp10);
                                AVP3.Nodes.Add(avp11);
                                if (SCTP.avp_EUTRANVectorVendorID.Count != 0)
                                {
                                    TreeNode avp12 = new TreeNode(SCTP.avp_EUTRANVectorVendorID[iii], 1, 3);
                                    AVP3.Nodes.Add(avp12);
                                }
                                while (SCTP.avp_KASMECode[iii] != "")
                                {
                                    TreeNode AVP4 = new TreeNode("KASME AVP", 1, 3);
                                    AVP3.Nodes.Add(AVP4);
                                    TreeNode avp13 = new TreeNode(SCTP.avp_KASMECode[iii], 1, 3);
                                    TreeNode avp14 = new TreeNode(SCTP.avp_KASMEFlag[iii], 1, 3);
                                    TreeNode avp15 = new TreeNode(SCTP.avp_KASMELength[iii], 1, 3);
                                    TreeNode KASMEStr = new TreeNode(SCTP.KASMEStr[iii], 1, 3);
                                    AVP4.Nodes.Add(avp13);
                                    AVP4.Nodes.Add(avp14);
                                    AVP4.Nodes.Add(avp15);
                                    AVP4.Nodes.Add(KASMEStr);
                                    break;
                                }
                                break;
                            }
                        }
                        break;
                    }
                    while (SCTP.avp_SessionID1 != "")
                    {
                        TreeNode AVP5 = new TreeNode("Session-ID AVP", 1, 3);
                        AVP.Nodes.Add(AVP5);
                        TreeNode avp16 = new TreeNode(SCTP.avp_SessionID1, 1, 3);
                        TreeNode avp17 = new TreeNode(SCTP.avp_SessionID2, 1, 3);
                        TreeNode avp18 = new TreeNode(SCTP.avp_SessionID3, 1, 3);
                        TreeNode avp19 = new TreeNode(SCTP.avp_SessionID4, 1, 3);
                        AVP5.Nodes.Add(avp16);
                        AVP5.Nodes.Add(avp17);
                        AVP5.Nodes.Add(avp18);
                        AVP5.Nodes.Add(avp19);
                        break;
                    }

                    sctr.ExpandAll();
                }
                else
                {
                    TCPDiameter.DecodeTcpChunk(ST_dia);

                    TreeNode sctr = new TreeNode("TCP Protocol", 1, 3); //表示协议类型的根节点
                    treeView_DIAMETER.Nodes.Add(sctr);
                    TreeNode scmessageType = new TreeNode(TCPDiameter.protocol_Treeview, 1, 3);
                    sctr.Nodes.Add(scmessageType);
                    TreeNode version = new TreeNode(TCPDiameter.Version, 1, 3);     //表示解析消息头的节点
                    TreeNode length = new TreeNode(TCPDiameter.Length, 1, 3);
                    TreeNode flag = new TreeNode(TCPDiameter.Flag, 1, 3);
                    TreeNode commandCODE = new TreeNode(TCPDiameter.CommandCODE, 1, 3);
                    TreeNode applicationID = new TreeNode(TCPDiameter.ApplicationID, 1, 3);
                    TreeNode H2HIdentifier = new TreeNode(TCPDiameter.H2HIdentifier, 1, 3);
                    TreeNode E2EIdentifier = new TreeNode(TCPDiameter.E2EIdentifier, 1, 3);
                    TreeNode AVP = new TreeNode("AVP", 1, 3);
                    scmessageType.Nodes.Add(version);
                    scmessageType.Nodes.Add(length);
                    scmessageType.Nodes.Add(flag);
                    scmessageType.Nodes.Add(commandCODE);
                    scmessageType.Nodes.Add(applicationID);
                    scmessageType.Nodes.Add(H2HIdentifier);
                    scmessageType.Nodes.Add(E2EIdentifier);
                    scmessageType.Nodes.Add(AVP);
                    while (TCPDiameter.userNameCode != "")
                    {
                        TreeNode AVP1 = new TreeNode("User-Name AVP", 1, 3);
                        AVP.Nodes.Add(AVP1);
                        TreeNode avp1 = new TreeNode(TCPDiameter.userNameCode, 1, 3);
                        TreeNode avp2 = new TreeNode(TCPDiameter.userNameFlag, 1, 3);
                        TreeNode avp3 = new TreeNode(TCPDiameter.userNameLength, 1, 3);
                        TreeNode avp4 = new TreeNode(TCPDiameter.userNameStr, 1, 3);
                        AVP1.Nodes.Add(avp1);
                        AVP1.Nodes.Add(avp2);
                        AVP1.Nodes.Add(avp3);
                        AVP1.Nodes.Add(avp4);
                        break;
                    }
                    while (TCPDiameter.avp_ResultcodeCode != "")
                    {
                        TreeNode AVP1 = new TreeNode("Result-CODE AVP", 1, 3);
                        AVP.Nodes.Add(AVP1);
                        TreeNode avp1 = new TreeNode(TCPDiameter.avp_ResultcodeCode, 1, 3);
                        TreeNode avp2 = new TreeNode(TCPDiameter.avp_ResultcodeFlag, 1, 3);
                        TreeNode avp3 = new TreeNode(TCPDiameter.avp_ResultcodeLength, 1, 3);
                        TreeNode avp4 = new TreeNode(TCPDiameter.avp_ResultcodeStr, 1, 3);
                        AVP1.Nodes.Add(avp1);
                        AVP1.Nodes.Add(avp2);
                        AVP1.Nodes.Add(avp3);
                        AVP1.Nodes.Add(avp4);
                        break;
                    }
                    while (TCPDiameter.avp_AuthenticationCode != "")
                    {
                        TreeNode AVP2 = new TreeNode("Authentication-Info AVP", 1, 3);
                        AVP.Nodes.Add(AVP2);
                        TreeNode avp5 = new TreeNode(TCPDiameter.avp_AuthenticationCode, 1, 3);
                        TreeNode avp6 = new TreeNode(TCPDiameter.avp_AuthenticationFlag, 1, 3);
                        TreeNode avp7 = new TreeNode(TCPDiameter.avp_AuthenticationLength, 1, 3);
                        TreeNode avp8 = new TreeNode(TCPDiameter.avp_AuthenticationVendorID, 1, 3);
                        AVP2.Nodes.Add(avp5);
                        AVP2.Nodes.Add(avp6);
                        AVP2.Nodes.Add(avp7);
                        if (TCPDiameter.avp_AuthenticationVendorID != "")
                        {
                            AVP2.Nodes.Add(avp8);
                        }
                        for (int iii = 0; iii < TCPDiameter.avp_EUTRANVectorCode.Count; iii++)
                        {
                            #region
                            while (TCPDiameter.avp_EUTRANVectorCode[iii] != "")
                            {
                                #region
                                //int k=3+iii;
                                //AVP3=AVP3
                                TreeNode AVP3 = new TreeNode("E-UTRAN-Vector AVP", 1, 3);
                                AVP2.Nodes.Add(AVP3);
                                TreeNode avp9 = new TreeNode(TCPDiameter.avp_EUTRANVectorCode[iii], 1, 3);
                                TreeNode avp10 = new TreeNode(TCPDiameter.avp_EUTRANVectorFlag[iii], 1, 3);
                                TreeNode avp11 = new TreeNode(TCPDiameter.avp_EUTRANVectorLength[iii], 1, 3);

                                AVP3.Nodes.Add(avp9);
                                AVP3.Nodes.Add(avp10);
                                AVP3.Nodes.Add(avp11);
                                if (TCPDiameter.avp_EUTRANVectorVendorID.Count != 0)
                                {
                                    TreeNode avp12 = new TreeNode(TCPDiameter.avp_EUTRANVectorVendorID[iii], 1, 3);
                                    AVP3.Nodes.Add(avp12);
                                }
                                while (TCPDiameter.avp_KASMECode[iii] != "")
                                {
                                    TreeNode AVP4 = new TreeNode("KASME AVP", 1, 3);
                                    AVP3.Nodes.Add(AVP4);
                                    TreeNode avp13 = new TreeNode(TCPDiameter.avp_KASMECode[iii], 1, 3);
                                    TreeNode avp14 = new TreeNode(TCPDiameter.avp_KASMEFlag[iii], 1, 3);
                                    TreeNode avp15 = new TreeNode(TCPDiameter.avp_KASMELength[iii], 1, 3);
                                    TreeNode KASMEStr = new TreeNode(TCPDiameter.KASMEStr[iii], 1, 3);
                                    AVP4.Nodes.Add(avp13);
                                    AVP4.Nodes.Add(avp14);
                                    AVP4.Nodes.Add(avp15);
                                    AVP4.Nodes.Add(KASMEStr);
                                    break;
                                }
                                break;
                                #endregion
                            }

                            #endregion
                        }
                        break;
                    }
                    while (TCPDiameter.avp_SessionID1 != "")
                    {
                        TreeNode AVP5 = new TreeNode("Session-ID AVP", 1, 3);
                        AVP.Nodes.Add(AVP5);
                        TreeNode avp16 = new TreeNode(TCPDiameter.avp_SessionID1, 1, 3);
                        TreeNode avp17 = new TreeNode(TCPDiameter.avp_SessionID2, 1, 3);
                        TreeNode avp18 = new TreeNode(TCPDiameter.avp_SessionID3, 1, 3);
                        TreeNode avp19 = new TreeNode(TCPDiameter.avp_SessionID4, 1, 3);
                        AVP5.Nodes.Add(avp16);
                        AVP5.Nodes.Add(avp17);
                        AVP5.Nodes.Add(avp18);
                        AVP5.Nodes.Add(avp19);
                        break;
                    }
                    sctr.ExpandAll();
                }
            }
                #endregion
        }


        public void ShowInForm(object a, PacketProperties Properties, bool showSignal, bool showSignaling)
        {
            if (this.IsHandleCreated)
            {
                if (showSignaling)//如果是信令数据的话，将信令对应界面刷新
                {
                    try                            //lishuai
                    {
                        this.BeginInvoke(new UpdateForm(Data_Receive), Properties);
                    }
                    catch (InvalidOperationException e)
                    {
                        MessageBox.Show(DateTime.Now + ":" + e.Message);
                    }
                }

            }
        }

        private void ShowSignalingInForm(PacketProperties Properties)
        {
            if (this.IsHandleCreated)
            {
                this.BeginInvoke(new UpdateForm(Show), Properties);
            }
        }

        private void Show(PacketProperties Properties)
        {
            switch (Properties.SendDir)
            {
                case SendDir.VOBCToZC:
                    UpdateChart(FilterOptionForm.MaxVOBCToZC, Properties.OriginationAddress + "-ZC", this.listViewVOBCToZC, this.chartVOBCtoZCLine, this.chartVOBCtoZCPie, VOBCToZC, Properties);
                    break;
                case SendDir.VOBCToCI:
                    UpdateChart(FilterOptionForm.MaxVOBCToCI, Properties.OriginationAddress + "-CI", this.listViewVOBCToCI, this.chartVOBCToCILine, this.chartVOBCtoCIPie, VOBCToCI, Properties);
                    break;
                case SendDir.VOBCToATS:
                    UpdateChart(FilterOptionForm.MaxVOBCToATS, Properties.OriginationAddress + "-ATS", this.listViewVOBCToATS, this.chartVOBCToATSLine, this.chartVOBCToATSPie, VOBCToATS, Properties);
                    break;
                case SendDir.ZCToVOBC:
                    UpdateChart(FilterOptionForm.MaxZCToVOBC, "ZC-" + Properties.DestinationAddress, this.listViewZCToVOBC, this.chartZCtoVOBCLine, this.chartZCtoVOBCPie, ZCToVOBC, Properties);
                    break;
                case SendDir.CIToVOBC:
                    UpdateChart(FilterOptionForm.MaxCIToVOBC, "CI-" + Properties.DestinationAddress, this.listViewCIToVOBC, this.chartCIToVOBCLine, this.chartCIToVOBCPie, CIToVOBC, Properties);
                    break;
                case SendDir.ATSToVOBC:
                    UpdateChart(FilterOptionForm.MaxATSToVOBC, "ATS-" + Properties.DestinationAddress, this.listViewATSToVOBC, this.chartATSToVOBCLine, this.chartATSToVOBCPie, ATSToVOBC, Properties);
                    break;
            }
        }

        Dictionary<string, DateTime> DicInterval = new Dictionary<string, DateTime>();
        private void UpdateChart(double AlarmInterval, string Name, ListView listView_Data, Chart chartLine, Chart chartPie, List<string> NameSeries, PacketProperties Properties)
        {
            if (DicInterval.Keys.Contains(Name))
            {
                Properties.Interval = (Properties.CaptureTime - DicInterval[Name]).TotalSeconds;
                DicInterval[Name] = Properties.CaptureTime;

                if (Properties.Interval > 2)
                {
                    int a = 1;
                }
            }
            else
            {
                Properties.Interval = 0;
                DicInterval.Add(Name, Properties.CaptureTime);
            }
            if (Convert.ToDouble(Properties.Interval) > AlarmInterval)
            {
                string MessageBodyHex = GetDataHex(Properties.MessageBuffer, 8, (int)Properties.MessageLength - 8);
                listView_Data.Items.Add(new ListViewItem(new string[] { Properties.CaptureTime.ToString("yyyy-MM-dd"), Properties.CaptureTime.ToString("HH:mm:ss:fff"), Properties.Interval.ToString(), Properties.OriginationAddress, Properties.DestinationAddress, MessageBodyHex }));
                SaveToAlarmLog(AlarmInterval, Properties, MessageBodyHex);
            }
            int indexOfSeries;
            if (NameSeries.Contains(Name))
            {
                indexOfSeries = NameSeries.IndexOf(Name);
            }
            else
            {
                NameSeries.Add(Name);
                DicColor.Add(Name, GetRandomColor());
                Series s1 = new Series();
                s1.Name = Name;
                chartLine.Series.Add(s1);
                indexOfSeries = NameSeries.IndexOf(Name);
            }
            if (chartLine.Series[indexOfSeries].Points.Count() >= 1500)
            {
                chartLine.Series[indexOfSeries].Points.Remove(chartLine.Series[indexOfSeries].Points[0]);
            }
            double Interval;
            Interval = Properties.Interval;
            if (Properties.Interval > 6)
            {
                Interval = 6;
            }
            chartLine.Series[indexOfSeries].Points.AddY(Interval);

            chartLine.Series[indexOfSeries].ChartType = SeriesChartType.Line;
            chartLine.Series[indexOfSeries].BorderWidth = 3;
            chartLine.Series[indexOfSeries].Color = DicColor[Name];

            double interval = Convert.ToDouble(Properties.Interval) * 1000;
            if (interval >= 0 && interval < 500) yData[0]++;
            else if (interval >= 500 && interval < 1000) yData[1]++;
            else if (interval >= 1000 && interval < 2400) yData[2]++;
            else yData[3]++;
            if ((DateTime.Now - countInterval).TotalMilliseconds >= 10000)
            {
                chartPie.Series[0]["PieLabelStyle"] = "Outside";
                chartPie.Series[0]["PieLineColor"] = "Black";
                chartPie.Series[0].Points.DataBindXY(xData, yData);
            }
        }

        private System.Drawing.Color GetRandomColor()
        {
            Random RandomNum_First = new Random((int)DateTime.Now.Ticks);
            System.Threading.Thread.Sleep(RandomNum_First.Next(50));
            Random RandomNum_Sencond = new Random((int)DateTime.Now.Ticks);
            int int_Red = RandomNum_First.Next(256);
            int int_Green = RandomNum_Sencond.Next(256);
            int int_Blue = (int_Red + int_Green > 400) ? 0 : 400 - int_Red - int_Green;
            int_Blue = (int_Blue > 255) ? 255 : int_Blue;
            return System.Drawing.Color.FromArgb(int_Red, int_Green, int_Blue);
        }

        private void SaveToAlarmLog(double Interval, PacketProperties Properties, string MessageBodyHex)
        {
            string path = "AlarmLog";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string Head = Properties.CaptureTime + ",";
            Head += Properties.Interval + ",";
            Head += Properties.OriginationAddress + "," + Properties.OriginationPort + "," + Properties.DestinationAddress + "," + Properties.DestinationPort + ",";
            Head += MessageBodyHex.Replace("\r\n", string.Empty);
            path += "\\" + Properties.CaptureTime.Year + "-" + Properties.CaptureTime.Month + "-" + Properties.CaptureTime.Day + ".csv";
            if (File.Exists(path))
            {
                using (StreamWriter file = new StreamWriter(path, true, Encoding.UTF8))
                {
                    file.WriteLine(Head);
                    file.Close();
                }
            }
            else
            {
                using (StreamWriter file = new StreamWriter(path, true, Encoding.UTF8))
                {
                    file.WriteLine("捕获时间,间隔,源IP,源端口,目的IP,目的端口,消息内容");
                    file.WriteLine(Head);
                    file.Close();
                }
            }
        }

        string TimeOnline = "";   //刘黎改
        private void Data_Receive(PacketProperties Properties)
        {

            //新加的变量
            int protocolid = 0;
            captureTimeOnline = (Properties.Date - startTime).TotalSeconds;
            TimeOnline = Properties.Date.ToString("yyyy-MM-dd HH:mm:ss:fff");   // 刘黎改
            string doubleIP_Port = Properties.OriginationAddress + "-" + Properties.OriginationPort + ";" + Properties.DestinationAddress + "-" + Properties.DestinationPort;
            // 数据显示
            listViewDelegate listDelegate = new listViewDelegate(AddItem);
            int IPHeaderLen = (int)Properties.IPHeaderLength;
            int messageHeaderLen = GetMessageHeaderLen(Properties.Protocol);
            int messageBodyLen = (int)Properties.MessageLength - messageHeaderLen;
            int messageALL = IPHeaderLen + messageHeaderLen + messageBodyLen;
            string messagePacketHex = Properties.MessagePacketHex;//用于展示抓获报文的名称(readData.RRead(PacketBuffer))
            //新加程序
            protocolid = Properties.protocolid;
            string SourceData = Readdata.byteToHexStr(Properties.PacketBuffer);
            if (protocolid == 1)
            { Properties.Protocol = "DIAMETER-SCTP"; }
            else if (protocolid == 2)
            { Properties.Protocol = "DIAMETER-TCP"; }
            else if (protocolid == 3)
            { Properties.Protocol = "GTPv2"; }
            else if (protocolid == 4)
            { Properties.Protocol = "S1AP"; }
            string AllLength = Convert.ToString(messageALL);
            filterCount++;
            string[] item = new string[9]{ TimeOnline, Properties.Protocol, 
            Properties.OriginationAddress, Properties.OriginationPort, Properties.DestinationAddress,
            Properties.DestinationPort, AllLength, messagePacketHex,SourceData };
            listView_Data.Items.Insert(0, new ListViewItem(item));
            for (int i = 0; i < listView_Data.Items.Count; i++)
            {
                if (listView_Data.Items[i].SubItems[1].Text == "S1AP")
                    listView_Data.Items[i].BackColor = System.Drawing.Color.Snow;
                else if ((listView_Data.Items[i].SubItems[1].Text == "DIAMETER-SCTP") || (listView_Data.Items[i].SubItems[1].Text == "DIAMETER-TCP"))
                    listView_Data.Items[i].BackColor = System.Drawing.Color.SkyBlue;
                else
                    listView_Data.Items[i].BackColor = System.Drawing.Color.LightGoldenrodYellow;
            }
            if (listView_Data.Items.Count == 100)//使捕获消息流界面展示固定数量的消息
                listView_Data.Items[99].Remove();
            itemID++;
        }

        private void Buton_Find_Click(object sender, EventArgs e)
        {
            QueryForm queryFrm = new QueryForm(this);
            queryFrm.ShowDialog();
        }

        private void GTPv2_Opening(object sender, CancelEventArgs e)
        {

        }

        private void XinlingFlow_Opening(object sender, CancelEventArgs e)
        {

        }



        private void toolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            tabControl1.TabPages.Remove(tabPage1);
        }

        private void 关闭_Click(object sender, EventArgs e)
        {
            tabControl1.TabPages.Remove(tabPage7);
        }

        private void 捕捉消息ToolStripMenuItem_Click(object sender, EventArgs e)
        {


        }




        private void 关闭ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            this.tabControl1.TabPages.Remove(tabPage8);

            //OffLine = false;
        }

        private void textBox1_OffLine_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2__OffLine_TextChanged(object sender, EventArgs e)
        {


        }



        private void Button_CBTCclear_Click(object sender, EventArgs e)
        {
            //signal
            listViewVOBCToZC.Items.Clear();
            listViewVOBCToATS.Items.Clear();
            listViewVOBCToCI.Items.Clear();
            listViewZCToVOBC.Items.Clear();
            listViewCIToVOBC.Items.Clear();
            listViewATSToVOBC.Items.Clear();
            Chart[] charts = { chartVOBCToATSLine, chartVOBCToATSPie, chartVOBCToCILine, chartVOBCtoCIPie, chartVOBCtoZCLine, chartVOBCtoZCPie,
                               chartZCtoVOBCLine, chartZCtoVOBCPie, chartCIToVOBCLine, chartCIToVOBCPie, chartATSToVOBCLine, chartATSToVOBCPie};
            ClearChart(charts);
        }

        private void contextM_OffLine_Opening(object sender, CancelEventArgs e)
        {

        }
        byte[] SelectSignal = new byte[1];
        //SignalStruct selectStruct = new SignalStruct();
        List<SignalStruct> OfflineStruct = new List<SignalStruct>();//存储离线信令信元的信息，用于显示对应编码
        //SignalStruct OnlineStruct = new SignalStruct();      
        string protocolSelected_Offline;//用户在“离线消息”界面上所选择信令的协议_yao
        List<byte[]> text_OffLine = new List<byte[]>();//点击消息对应的数据流数组_yao

        private void listView_Off_SelectedIndexChanged(object sender, EventArgs e)
        {
            treeView_OffLine.Nodes.Clear();
            OfflineStruct.Clear();
            textBox_OffLine.Text = "";
            int SelectedIndex = 0;//用户在“离线消息”界面上所选择信令的序号
            string temp = "";//用于在控件textBox中展示所选择的信令的编码
            protocolSelected_Offline = " ";
            if (listView_Off.SelectedIndices != null && listView_Off.SelectedIndices.Count > 0)
            {
                ListView.SelectedIndexCollection c = listView_Off.SelectedIndices;
                SelectedIndex = Convert.ToInt32(listView_Off.Items[c[0]].SubItems[0].Text, 10);
                protocolSelected_Offline = listView_Off.Items[c[0]].SubItems[2].Text.ToString();
            }
            foreach (byte ab in listPackets[SelectedIndex])//将byte数组转化为16进制字符串，用于在控件textBox中展示
            {
                temp += ab.ToString("X2") + " ";
            }
            text_OffLine.Add(listPackets[SelectedIndex]);//yao
            textBox_OffLine.Text = temp;   //yao
            treeView_OffLine.BackColor = Color.SkyBlue;
            textBox_OffLine.BackColor = Color.SkyBlue;
            if (protocolSelected_Offline == "DIAMETER-SCTP")
            {
                //clickFlag_Offline = 1;
                #region 采用SCTP承载diameter解析的树状结构
                byte[] SctpChunk = new byte[listPackets[SelectedIndex].Length];
                byte[] Sc = new byte[listPackets[SelectedIndex].Length - 20];
                for (int i = 0; i < listPackets[SelectedIndex].Length; i++)
                {
                    SctpChunk[i] = listPackets[SelectedIndex][i];
                }
                Array.Copy(SctpChunk, 20, Sc, 0, listPackets[SelectedIndex].Length - 20);
                SCTP.DecodeSctpChunk(Sc);
                TreeNode sctr = new TreeNode("SCTP Protocol", 1, 3); //表示协议类型的根节点
                treeView_OffLine.Nodes.Add(sctr);
                TreeNode scmessageType = new TreeNode(SCTP.protocol_Treeview, 1, 3);
                sctr.Nodes.Add(scmessageType);
                TreeNode version = new TreeNode(SCTP.version, 1, 3);     //表示解析消息头的节点
                TreeNode length = new TreeNode(SCTP.length, 1, 3);
                TreeNode flag = new TreeNode(SCTP.flag, 1, 3);
                TreeNode commandCODE = new TreeNode(SCTP.commandCODE, 1, 3);
                TreeNode applicationID = new TreeNode(SCTP.applicationID, 1, 3);
                TreeNode H2HIdentifier = new TreeNode(SCTP.H2HIdentifier, 1, 3);
                TreeNode E2EIdentifier = new TreeNode(SCTP.E2EIdentifier, 1, 3);
                TreeNode AVP = new TreeNode("AVP", 1, 3);
                scmessageType.Nodes.Add(version);
                scmessageType.Nodes.Add(length);
                scmessageType.Nodes.Add(flag);
                scmessageType.Nodes.Add(commandCODE);
                scmessageType.Nodes.Add(applicationID);
                scmessageType.Nodes.Add(H2HIdentifier);
                scmessageType.Nodes.Add(E2EIdentifier);
                scmessageType.Nodes.Add(AVP);
                while (SCTP.userNameCode != "")
                {
                    TreeNode AVP1 = new TreeNode("User-Name AVP", 1, 3);
                    AVP.Nodes.Add(AVP1);
                    TreeNode avp1 = new TreeNode(SCTP.userNameCode, 1, 3);
                    TreeNode avp2 = new TreeNode(SCTP.userNameFlag, 1, 3);
                    TreeNode avp3 = new TreeNode(SCTP.userNameLength, 1, 3);
                    TreeNode avp4 = new TreeNode(SCTP.userNameStr, 1, 3);
                    AVP1.Nodes.Add(avp1);
                    AVP1.Nodes.Add(avp2);
                    AVP1.Nodes.Add(avp3);
                    AVP1.Nodes.Add(avp4);
                    break;
                }
                while (SCTP.avp_ResultcodeCode != "")
                {
                    TreeNode AVP1 = new TreeNode("Result-CODE AVP", 1, 3);
                    AVP.Nodes.Add(AVP1);
                    TreeNode avp1 = new TreeNode(SCTP.avp_ResultcodeCode, 1, 3);
                    TreeNode avp2 = new TreeNode(SCTP.avp_ResultcodeFlag, 1, 3);
                    TreeNode avp3 = new TreeNode(SCTP.avp_ResultcodeLength, 1, 3);
                    TreeNode avp4 = new TreeNode(SCTP.avp_ResultcodeStr, 1, 3);
                    AVP1.Nodes.Add(avp1);
                    AVP1.Nodes.Add(avp2);
                    AVP1.Nodes.Add(avp3);
                    AVP1.Nodes.Add(avp4);
                    break;
                }

                while (SCTP.avp_AuthenticationCode != "")
                {
                    TreeNode AVP2 = new TreeNode("Authentication-Info AVP", 1, 3);
                    AVP.Nodes.Add(AVP2);
                    TreeNode avp5 = new TreeNode(SCTP.avp_AuthenticationCode, 1, 3);
                    TreeNode avp6 = new TreeNode(SCTP.avp_AuthenticationFlag, 1, 3);
                    TreeNode avp7 = new TreeNode(SCTP.avp_AuthenticationLength, 1, 3);
                    TreeNode avp8 = new TreeNode(SCTP.avp_AuthenticationVendorID, 1, 3);
                    AVP2.Nodes.Add(avp5);
                    AVP2.Nodes.Add(avp6);
                    AVP2.Nodes.Add(avp7);
                    AVP2.Nodes.Add(avp8);
                    for (int iii = 0; iii < SCTP.avp_EUTRANVectorCode.Count; iii++)
                    {
                        while (SCTP.avp_EUTRANVectorCode[iii] != "")
                        {
                            TreeNode AVP3 = new TreeNode("E-UTRAN-Vector" + Convert.ToString(iii + 1) + " AVP", 1, 3);
                            AVP2.Nodes.Add(AVP3);
                            TreeNode avp9 = new TreeNode(SCTP.avp_EUTRANVectorCode[iii], 1, 3);
                            TreeNode avp10 = new TreeNode(SCTP.avp_EUTRANVectorFlag[iii], 1, 3);
                            TreeNode avp11 = new TreeNode(SCTP.avp_EUTRANVectorLength[iii], 1, 3);
                            AVP3.Nodes.Add(avp9);
                            AVP3.Nodes.Add(avp10);
                            AVP3.Nodes.Add(avp11);
                            if (SCTP.avp_EUTRANVectorVendorID.Count != 0)
                            {
                                TreeNode avp12 = new TreeNode(SCTP.avp_EUTRANVectorVendorID[iii], 1, 3);
                                AVP3.Nodes.Add(avp12);
                            }
                            while (SCTP.avp_KASMECode[iii] != "")
                            {
                                TreeNode AVP4 = new TreeNode("KASME" + Convert.ToString(iii + 1) + " AVP", 1, 3);
                                AVP3.Nodes.Add(AVP4);
                                TreeNode avp13 = new TreeNode(SCTP.avp_KASMECode[iii], 1, 3);
                                TreeNode avp14 = new TreeNode(SCTP.avp_KASMEFlag[iii], 1, 3);
                                TreeNode avp15 = new TreeNode(SCTP.avp_KASMELength[iii], 1, 3);
                                TreeNode KASMEStr = new TreeNode(SCTP.KASMEStr[iii], 1, 3);
                                AVP4.Nodes.Add(avp13);
                                AVP4.Nodes.Add(avp14);
                                AVP4.Nodes.Add(avp15);
                                AVP4.Nodes.Add(KASMEStr);
                                break;
                            }
                            break;
                        }
                    }
                    break;
                }
                while (SCTP.avp_SessionID1 != "")
                {
                    TreeNode AVP5 = new TreeNode("Session-ID AVP", 1, 3);
                    AVP.Nodes.Add(AVP5);
                    TreeNode avp16 = new TreeNode(SCTP.avp_SessionID1, 1, 3);
                    TreeNode avp17 = new TreeNode(SCTP.avp_SessionID2, 1, 3);
                    TreeNode avp18 = new TreeNode(SCTP.avp_SessionID3, 1, 3);
                    TreeNode avp19 = new TreeNode(SCTP.avp_SessionID4, 1, 3);
                    AVP5.Nodes.Add(avp16);
                    AVP5.Nodes.Add(avp17);
                    AVP5.Nodes.Add(avp18);
                    AVP5.Nodes.Add(avp19);
                    break;
                }

                sctr.ExpandAll();
                #endregion

                #region 获取各项参数的位置和长度信息
                //清空各项消息的位置和长度记录
                diameterLocationOffline = 0;
                diameterLengthOffline = 0;
                avpLocation_UserNameOffline = 0; avpLocation_ResultOffline = 0; avpLocation_AuthenticationOffline = 0; avpLocation_SessionIDOffline = 0;
                avpLength_UserNameOffline = 0; avpLength_ResultOffline = 0; avpLength_AuthenticationOffline = 0; avpLength_SessionIDOffline = 0;
                location_UserNameStrOffline = 0; location_ResultCodeStrOffline = 0; location_SessionID4Offline = 0; length_UserNameStrOffline = 0; length_SessionID4Offline = 0;
                avpLocation_EUTRANVectorOffline.Clear();
                avpLength_EUTRANVectorOffline.Clear();
                avpLocation_KASMEOffline.Clear();
                avpLength_KASMEOffline.Clear();
                location_KASMEStrOffline.Clear();
                //重新获取各项消息的位置和长度记录
                diameterLengthOffline = SCTP.diameterLength;
                diameterLocationOffline = SCTP.diameterLocation;
                avpLocation_UserNameOffline = SCTP.avpLocation_UserName;
                avpLocation_ResultOffline = SCTP.avpLocation_Result;
                avpLocation_AuthenticationOffline = SCTP.avpLocation_Authentication;
                avpLocation_SessionIDOffline = SCTP.avpLocation_SessionID;
                avpLength_UserNameOffline = SCTP.avpLength_UserName;
                avpLength_ResultOffline = SCTP.avpLength_Result;
                avpLength_AuthenticationOffline = SCTP.avpLength_Authentication;
                avpLength_SessionIDOffline = SCTP.avpLength_SessionID;
                location_UserNameStrOffline = SCTP.location_UserNameStr;
                location_ResultCodeStrOffline = SCTP.location_ResultCodeStr;
                location_SessionID4Offline = SCTP.location_SessionID4;
                length_UserNameStrOffline = SCTP.length_UserNameStr;
                length_SessionID4Offline = SCTP.length_SessionID4;
                avpLocation_EUTRANVectorOffline = SCTP.avpLocation_EUTRANVector;
                avpLength_EUTRANVectorOffline = SCTP.avpLength_EUTRANVector;
                avpLocation_KASMEOffline = SCTP.avpLocation_KASME;
                avpLength_KASMEOffline = SCTP.avpLength_KASME;
                location_KASMEStrOffline = SCTP.location_KASMEStr;
                #endregion
            }
            //指定行协议为TCP-diameter时，所进行的树状结构解析
            else if (protocolSelected_Offline == "DIAMETER-TCP")
            {
                //clickFlag_Offline = 1;
                #region 采用TCP承载diameter解析的树状结构
                byte[] TcChunk = new byte[listPackets[SelectedIndex].Length];
                byte[] Tc = new byte[listPackets[SelectedIndex].Length - 20];
                for (int i = 0; i < listPackets[SelectedIndex].Length; i++)             //刘黎修改
                {
                    //TcChunk[i] = Convert.ToByte(str[i + 1], 16);
                    TcChunk[i] = listPackets[SelectedIndex][i];
                }
                Array.Copy(TcChunk, 20, Tc, 0, listPackets[SelectedIndex].Length - 20);
                TCPDiameter.DecodeTcpChunk(Tc);
                TreeNode sctr = new TreeNode("TCP Protocol", 1, 3); //表示协议类型的根节点
                treeView_OffLine.Nodes.Add(sctr);
                TreeNode scmessageType = new TreeNode(TCPDiameter.protocol_Treeview, 1, 3);
                sctr.Nodes.Add(scmessageType);
                TreeNode version = new TreeNode(TCPDiameter.Version, 1, 3);     //表示解析消息头的节点
                TreeNode length = new TreeNode(TCPDiameter.Length, 1, 3);
                TreeNode flag = new TreeNode(TCPDiameter.Flag, 1, 3);
                TreeNode commandCODE = new TreeNode(TCPDiameter.CommandCODE, 1, 3);
                TreeNode applicationID = new TreeNode(TCPDiameter.ApplicationID, 1, 3);
                TreeNode H2HIdentifier = new TreeNode(TCPDiameter.H2HIdentifier, 1, 3);
                TreeNode E2EIdentifier = new TreeNode(TCPDiameter.E2EIdentifier, 1, 3);
                TreeNode AVP = new TreeNode("AVP", 1, 3);
                scmessageType.Nodes.Add(version);
                scmessageType.Nodes.Add(length);
                scmessageType.Nodes.Add(flag);
                scmessageType.Nodes.Add(commandCODE);
                scmessageType.Nodes.Add(applicationID);
                scmessageType.Nodes.Add(H2HIdentifier);
                scmessageType.Nodes.Add(E2EIdentifier);
                scmessageType.Nodes.Add(AVP);
                while (TCPDiameter.userNameCode != "")
                {
                    TreeNode AVP1 = new TreeNode("User-Name AVP", 1, 3);
                    AVP.Nodes.Add(AVP1);
                    TreeNode avp1 = new TreeNode(TCPDiameter.userNameCode, 1, 3);
                    TreeNode avp2 = new TreeNode(TCPDiameter.userNameFlag, 1, 3);
                    TreeNode avp3 = new TreeNode(TCPDiameter.userNameLength, 1, 3);
                    TreeNode avp4 = new TreeNode(TCPDiameter.userNameStr, 1, 3);
                    AVP1.Nodes.Add(avp1);
                    AVP1.Nodes.Add(avp2);
                    AVP1.Nodes.Add(avp3);
                    AVP1.Nodes.Add(avp4);
                    break;
                }
                while (TCPDiameter.avp_ResultcodeCode != "")
                {
                    TreeNode AVP1 = new TreeNode("Result-CODE AVP", 1, 3);
                    AVP.Nodes.Add(AVP1);
                    TreeNode avp1 = new TreeNode(TCPDiameter.avp_ResultcodeCode, 1, 3);
                    TreeNode avp2 = new TreeNode(TCPDiameter.avp_ResultcodeFlag, 1, 3);
                    TreeNode avp3 = new TreeNode(TCPDiameter.avp_ResultcodeLength, 1, 3);
                    TreeNode avp4 = new TreeNode(TCPDiameter.avp_ResultcodeStr, 1, 3);
                    AVP1.Nodes.Add(avp1);
                    AVP1.Nodes.Add(avp2);
                    AVP1.Nodes.Add(avp3);
                    AVP1.Nodes.Add(avp4);
                    break;
                }
                while (TCPDiameter.avp_AuthenticationCode != "")
                {
                    TreeNode AVP2 = new TreeNode("Authentication-Info AVP", 1, 3);
                    AVP.Nodes.Add(AVP2);
                    TreeNode avp5 = new TreeNode(TCPDiameter.avp_AuthenticationCode, 1, 3);
                    TreeNode avp6 = new TreeNode(TCPDiameter.avp_AuthenticationFlag, 1, 3);
                    TreeNode avp7 = new TreeNode(TCPDiameter.avp_AuthenticationLength, 1, 3);
                    TreeNode avp8 = new TreeNode(TCPDiameter.avp_AuthenticationVendorID, 1, 3);
                    AVP2.Nodes.Add(avp5);
                    AVP2.Nodes.Add(avp6);
                    AVP2.Nodes.Add(avp7);
                    if (TCPDiameter.avp_AuthenticationVendorID != "")
                    { AVP2.Nodes.Add(avp8); }
                    for (int iii = 0; iii < TCPDiameter.avp_EUTRANVectorCode.Count; iii++)
                    {
                        #region
                        while (TCPDiameter.avp_EUTRANVectorCode[iii] != "")
                        {
                            #region
                            TreeNode AVP3 = new TreeNode("E-UTRAN-Vector" + Convert.ToString(iii + 1) + " AVP", 1, 3);
                            AVP.Nodes.Add(AVP3);
                            TreeNode avp9 = new TreeNode(TCPDiameter.avp_EUTRANVectorCode[iii], 1, 3);
                            TreeNode avp10 = new TreeNode(TCPDiameter.avp_EUTRANVectorFlag[iii], 1, 3);
                            TreeNode avp11 = new TreeNode(TCPDiameter.avp_EUTRANVectorLength[iii], 1, 3);
                            AVP3.Nodes.Add(avp9);
                            AVP3.Nodes.Add(avp10);
                            AVP3.Nodes.Add(avp11);
                            if (TCPDiameter.avp_EUTRANVectorVendorID.Count != 0)
                            {
                                TreeNode avp12 = new TreeNode(TCPDiameter.avp_EUTRANVectorVendorID[iii], 1, 3);
                                AVP3.Nodes.Add(avp12);
                            }
                            while (TCPDiameter.avp_KASMECode[iii] != "")
                            {
                                TreeNode AVP4 = new TreeNode("KASME" + Convert.ToString(iii + 1) + " AVP", 1, 3);
                                AVP.Nodes.Add(AVP4);
                                TreeNode avp13 = new TreeNode(TCPDiameter.avp_KASMECode[iii], 1, 3);
                                TreeNode avp14 = new TreeNode(TCPDiameter.avp_KASMEFlag[iii], 1, 3);
                                TreeNode avp15 = new TreeNode(TCPDiameter.avp_KASMELength[iii], 1, 3);
                                TreeNode KASMEStr = new TreeNode(TCPDiameter.KASMEStr[iii], 1, 3);
                                AVP4.Nodes.Add(avp13);
                                AVP4.Nodes.Add(avp14);
                                AVP4.Nodes.Add(avp15);
                                AVP4.Nodes.Add(KASMEStr);
                                break;
                            }
                            break;
                            #endregion
                        }
                        #endregion
                    }
                    break;
                }
                while (TCPDiameter.avp_SessionID1 != "")
                {
                    TreeNode AVP5 = new TreeNode("Session-ID AVP", 1, 3);
                    AVP.Nodes.Add(AVP5);
                    TreeNode avp16 = new TreeNode(TCPDiameter.avp_SessionID1, 1, 3);
                    TreeNode avp17 = new TreeNode(TCPDiameter.avp_SessionID2, 1, 3);
                    TreeNode avp18 = new TreeNode(TCPDiameter.avp_SessionID3, 1, 3);
                    TreeNode avp19 = new TreeNode(TCPDiameter.avp_SessionID4, 1, 3);
                    AVP5.Nodes.Add(avp16);
                    AVP5.Nodes.Add(avp17);
                    AVP5.Nodes.Add(avp18);
                    AVP5.Nodes.Add(avp19);
                    break;
                }
                sctr.ExpandAll();
                #endregion

                #region 获取各项参数的位置和长度信息
                //清空各项消息的位置和长度记录
                diameterLocationOffline = 0; diameterLengthOffline = 0;
                avpLocation_UserNameOffline = 0; avpLocation_ResultOffline = 0; avpLocation_AuthenticationOffline = 0; avpLocation_SessionIDOffline = 0;
                avpLength_UserNameOffline = 0; avpLength_ResultOffline = 0; avpLength_AuthenticationOffline = 0; avpLength_SessionIDOffline = 0;
                location_UserNameStrOffline = 0; location_ResultCodeStrOffline = 0; location_SessionID4Offline = 0; length_UserNameStrOffline = 0; length_SessionID4Offline = 0;
                avpLocation_EUTRANVectorOffline.Clear();
                avpLength_EUTRANVectorOffline.Clear();
                avpLocation_KASMEOffline.Clear();
                avpLength_KASMEOffline.Clear();
                location_KASMEStrOffline.Clear();
                //重新获取各项消息的位置和长度记录
                diameterLocationOffline = 32;
                diameterLengthOffline = TCPDiameter.diameterLength;
                avpLocation_UserNameOffline = TCPDiameter.avpLocation_UserName;
                avpLocation_ResultOffline = TCPDiameter.avpLocation_Result;
                avpLocation_AuthenticationOffline = TCPDiameter.avpLocation_Authentication;
                avpLocation_SessionIDOffline = TCPDiameter.avpLocation_SessionID;
                avpLength_UserNameOffline = TCPDiameter.avpLength_UserName;
                avpLength_ResultOffline = TCPDiameter.avpLength_Result;
                avpLength_AuthenticationOffline = TCPDiameter.avpLength_Authentication;
                avpLength_SessionIDOffline = TCPDiameter.avpLength_SessionID;
                location_UserNameStrOffline = TCPDiameter.location_UserNameStr;
                location_ResultCodeStrOffline = TCPDiameter.location_ResultCodeStr;
                location_SessionID4Offline = TCPDiameter.location_SessionID4;
                length_UserNameStrOffline = TCPDiameter.length_UserNameStr;
                length_SessionID4Offline = TCPDiameter.length_SessionID4;
                avpLocation_EUTRANVectorOffline = TCPDiameter.avpLocation_EUTRANVector;
                avpLength_EUTRANVectorOffline = TCPDiameter.avpLength_EUTRANVector;
                avpLocation_KASMEOffline = TCPDiameter.avpLocation_KASME;
                avpLength_KASMEOffline = TCPDiameter.avpLength_KASME;
                location_KASMEStrOffline = TCPDiameter.location_KASMEStr;
                #endregion

            }
            else if (protocolSelected_Offline == "GTPv2")
            {
                #region
                byte[] allMessageList = new byte[listPackets[SelectedIndex].Length];
                byte[] GTPv2 = new byte[listPackets[SelectedIndex].Length - 28];
                for (int i = 0; i < listPackets[SelectedIndex].Length; i++)
                {
                    allMessageList[i] = listPackets[SelectedIndex][i];
                }
                Array.Copy(allMessageList, 28, GTPv2, 0, listPackets[SelectedIndex].Length - 28);
                //textBox_OffLine.Text = listPackets[n];
                A.decoder(GTPv2);


                TreeNode tr = new TreeNode("GTP Protocol", 1, 3); //表示协议类型的根节点
                TreeNode messageType = new TreeNode(A.mt, 1, 3);
                TreeNode htr = new TreeNode("Header", 1, 3);     //表示解析消息头的节点
                TreeNode Cause = new TreeNode("Cause", 1, 3);
                treeView_OffLine.Nodes.Add(tr);
                tr.Nodes.Add(messageType);
                tr.Nodes.Add(htr);
                tr.Nodes.Add(Cause);
                htr.Nodes.Add("", A.ver, 1, 3);
                htr.Nodes.Add("", A.pinfo, 1, 3);
                htr.Nodes.Add("", A.tinfo, 1, 3);
                htr.Nodes.Add("", A.mt, 1, 3);
                htr.Nodes.Add("", A.leninfo, 1, 3);
                Cause.Nodes.Add("", A.IEvalue, 1, 3);
                if (A.tflag == 1)
                {
                    htr.Nodes.Add("", A.teidinfo, 1, 3);
                }
                htr.Nodes.Add("", A.seqinfo, 1, 3);
                htr.Nodes.Add("", A.spareinfo, 1, 3);
                tr.ExpandAll();
                #endregion
            }
            else if (protocolSelected_Offline == "S1AP")
            {
                #region
                byte[] S1apChunk = new byte[listPackets[SelectedIndex].Length];//带有IP头部
                byte[] S1ap = new byte[listPackets[SelectedIndex].Length - 20];//去除IP头部
                String S1apName = "";//所选择s1ap信令的名称
                String S1apCauseKind = "";//信令中的 s1ap cause种类
                String S1apCauseName = "";//信令中的s1ap cause名称
                uint mmeUeS1apId = 0;
                int enbUeS1apId = 0;
                String S1apIdentity = "";//获取s1ap中的IMSI或TMSI


                Array.Copy(listPackets[SelectedIndex], 0, S1apChunk, 0, listPackets[SelectedIndex].Length);
                Array.Copy(S1apChunk, 20, S1ap, 0, listPackets[SelectedIndex].Length - 20);
                ///解析s1ap消息的ID pair，信令名称，cause，用户标识
                SctpToS1ap.DeScChunk(S1ap);
                //textbox中展示的是s1ap的编码
                string text = "";
                for (int no = 0; no < SctpToS1ap.s1ap.Count; no++)
                {
                    foreach (byte ab in SctpToS1ap.s1ap[no])//将byte数组转化为16进制字符串，用于在控件textBox中展示
                    {
                        text += ab.ToString("X2") + " ";
                    }
                }
                textBox_OffLine.Text = text;
                SelectSignal = SctpToS1ap.s1ap[0];//假设报文中只有一条s1ap消息
                TreeNode sctr = new TreeNode("S1AP Protocol", 1, 3); //表示协议类型的根节点              
                treeView_OffLine.Nodes.Add(sctr);
                for (int k = 0; k < SctpToS1ap.s1ap.Count; k++)
                {
                    SignalStruct selectStruct = new SignalStruct();//存储当前信令的消息，用于对应显示//2017.6.19 黄刚
                    #region
                    mmeUeS1apId = Id1.s1ap_id(SctpToS1ap.s1ap[k]);//sctp.s1ap[j]中存放的是s1ap数据流
                    selectStruct.LengthMme = Id1.Length;//获取mmeUeS1apId的编码长度
                    if (k == 0)
                        selectStruct.indexMme = Id1.Index;//获取mmeUeS1apId的起始地址                  
                    else
                        selectStruct.indexMme = Id1.Index + SctpToS1ap.s1ap[0].Length;//获取mmeUeS1apId的起始地址(包含前一条s1ap信令长度)

                    selectStruct.mme = mmeUeS1apId.ToString();//获取mmeUeS1apId的编码

                    enbUeS1apId = Id2.s1ap_id(SctpToS1ap.s1ap[k]);
                    selectStruct.LengthEnb = Id2.Length;//获取enbUeS1apId的编码长度
                    if (k == 0)
                    {
                        selectStruct.IndexEnb = Id2.Index;//获取enbUeS1apId的起始地址
                    }
                    else
                    {
                        selectStruct.IndexEnb = Id2.Index + SctpToS1ap.s1ap[0].Length;//获取enbUeS1apId的起始地址(包含前一条s1ap信令长度)
                    }
                    selectStruct.enb = enbUeS1apId.ToString();//获取enbUeS1apId的编码

                    S1apName = decodeS1ap.s1ap_decode1(SctpToS1ap.s1ap[k]);//S1AP消息信令

                    selectStruct.Name = "procedureCode" + ":" + S1apName;
                    selectStruct.LengthName = 2;//获取信令名称的编码长度
                    if (k == 0)
                    {
                        selectStruct.IndexName = 0;//获取信令的起始地址                       
                    }
                    else
                    {
                        selectStruct.IndexName = 0 + SctpToS1ap.s1ap[0].Length;//获取信令的起始地址(包含前一条s1ap信令长度)                   
                    }

                    decCause.decodeCause(SctpToS1ap.s1ap[k]);
                    S1apCauseKind = decCause.str1;//cause类别
                    S1apCauseName = decCause.str2;//具体的cause名称
                    selectStruct.CauseKind = decCause.str1;
                    selectStruct.cause = decCause.str2;
                    selectStruct.LengthCause = decCause.Length;//获取cause的编码长度
                    if (k == 0)
                    {
                        selectStruct.IndexCause = decCause.Index;//获取cause的起始地址                      
                    }
                    else
                    {
                        selectStruct.IndexCause = decCause.Index + SctpToS1ap.s1ap[0].Length;//获取cause的起始地址(包含前一条s1ap信令长度)                        
                    }
                    if (S1apName == "Paging")
                    {
                        S1apIdentity = Id_paging.identity(SctpToS1ap.s1ap[k]);
                        selectStruct.Identity = S1apIdentity;
                        selectStruct.LengthIdentity = Id_paging.Length;//获取nas identity的编码长度
                        if (k == 0)
                        {
                            selectStruct.IndexIdentity = Id_paging.Index;//获取nas identity的起始地址                        
                        }
                        else
                        {
                            selectStruct.IndexIdentity = Id_paging.Index + SctpToS1ap.s1ap[0].Length;//获取nas identity的起始地址(包含前一条s1ap信令长度)                           
                        }
                    }

                    else if (S1apName == "initialUEMessage")
                    {
                        S1apIdentity = Id_initial.identity(SctpToS1ap.s1ap[k]);
                        selectStruct.Identity = S1apIdentity;
                        selectStruct.LengthIdentity = Id_initial.Length;
                        if (k == 0)
                        {
                            selectStruct.IndexIdentity = Id_initial.Index;
                        }
                        else
                        {
                            selectStruct.IndexIdentity = Id_initial.Index + SctpToS1ap.s1ap[0].Length;//获取nas identity的起始地址(包含前一条s1ap信令长度)                         
                        }

                    }
                    else//其他s1ap信令无ismi或tmsi
                    {
                        ;
                    }
                    //nas的解密解码部分
                    s1ap_nas2.nas_decode(SctpToS1ap.s1ap[k]);
                    if ((s1ap_nas2.EmmMessage1 != " ") && (s1ap_nas2.EsmMessage1 != " "))
                    {
                        selectStruct.EmmName = s1ap_nas2.nas_name;
                        if (k == 0)
                            selectStruct.IndexEmm = s1ap_nas2.IndexEmm;
                        else
                            selectStruct.IndexEmm = s1ap_nas2.IndexEmm + SctpToS1ap.s1ap[0].Length;
                        selectStruct.LengthEmm = s1ap_nas2.LengthEmm;
                    }
                    else if ((s1ap_nas2.EmmMessage1 == " ") && (s1ap_nas2.EsmMessage1 != " "))
                    {
                        selectStruct.EmmName = "  " + s1ap_nas2.EsmMessage1;
                        if (k == 0)
                            selectStruct.IndexEmm = s1ap_nas2.IndexEsm;
                        else
                            selectStruct.IndexEmm = s1ap_nas2.IndexEsm + SctpToS1ap.s1ap[0].Length;
                        selectStruct.LengthEmm = s1ap_nas2.LengthEsm;
                    }
                    else
                    {
                        selectStruct.EmmName = " ";
                        selectStruct.IndexEmm = 0;
                        selectStruct.LengthEmm = 0;
                    }
                    selectStruct.NasCause = s1ap_nas2.cause_str;
                    if (k == 0)
                        selectStruct.IndexNasCause = s1ap_nas2.IndexCause;
                    else
                        selectStruct.IndexNasCause = s1ap_nas2.IndexCause + SctpToS1ap.s1ap[0].Length;
                    selectStruct.LengthNasCause = s1ap_nas2.LengthCause;
                    selectStruct.NasIdentity = s1ap_nas2.identity;
                    if (k == 0)
                        selectStruct.IndexNasId = s1ap_nas2.IndexIdentity;
                    else
                        selectStruct.IndexNasId = s1ap_nas2.IndexIdentity + SctpToS1ap.s1ap[0].Length;
                    selectStruct.LengthNasId = s1ap_nas2.LengthIdentity;
                    selectStruct.UeIP = s1ap_nas2.ue_ip;
                    if (k == 0)
                        selectStruct.IndexUeIp = s1ap_nas2.IndexIP;
                    else
                        selectStruct.IndexUeIp = s1ap_nas2.IndexIP + SctpToS1ap.s1ap[0].Length;
                    selectStruct.LengthUeIp = s1ap_nas2.LengthIP;

                    ///用Treeview控件展示解析内容
                    TreeNode scmessageType = new TreeNode("procedureCode:" + S1apName, 1, 3);
                    sctr.Nodes.Add(scmessageType);
                    int z = 0;
                    #region
                    if (mmeUeS1apId != 0)
                    {
                        TreeNode id0 = new TreeNode("Item" + z + ":id-MME-UE-S1AP-ID" + "(" + mmeUeS1apId + ")", 1, 3);
                        scmessageType.Nodes.Add(id0);
                        selectStruct.NoMme = z;
                        z++;
                    }
                    if (enbUeS1apId != 0)
                    {
                        TreeNode id1 = new TreeNode("Item" + z + ":id-eNB-UE-S1AP-ID" + "(" + enbUeS1apId + ")", 1, 3);
                        scmessageType.Nodes.Add(id1);
                        selectStruct.NoEnb = z;
                        z++;
                    }
                    if (S1apCauseKind != "")
                    {
                        TreeNode id2 = new TreeNode("Item" + z + ":id-Cause:" + S1apCauseKind + ":" + S1apCauseName, 1, 3);
                        scmessageType.Nodes.Add(id2);
                        selectStruct.NoCause = z;
                        z++;
                    }
                    if (S1apIdentity != "")
                    {
                        if (S1apIdentity.Length > 8)//当用户标识的长度大于8个字节，说明是IMSI，反之为tmsi
                        {
                            TreeNode id3 = new TreeNode("Item" + z + ":id-IMSI:" + S1apIdentity, 1, 3);
                            scmessageType.Nodes.Add(id3);
                        }
                        else
                        {
                            TreeNode id3 = new TreeNode("Item" + z + ":id-TMSI:" + S1apIdentity, 1, 3);
                            scmessageType.Nodes.Add(id3);
                        }
                        selectStruct.NoIdentity = z;
                        z++;
                    }
                    #endregion
                    //nas解密解码
                    #region
                    //if ((Nasclass1[SelectedIndex][k].nas_name != " ") && (Nasclass1[SelectedIndex][k].nas_name != "ciperd message  "))
                    //{
                    //    #region
                    //    TreeNode id4 = new TreeNode("Item" + z + ":id-Nas:" + Nasclass1[SelectedIndex][k].nas_name, 1, 3);
                    //    scmessageType.Nodes.Add(id4);
                    //    z++;
                    //    if (Nasclass1[SelectedIndex][k].identity != "")
                    //    {
                    //        if (Nasclass1[SelectedIndex][k].identity.Length > 8)
                    //        {
                    //            TreeNode id5 = new TreeNode("Item" + z + ":id-IMSI:" + Nasclass1[SelectedIndex][k].identity, 1, 3);
                    //            scmessageType.Nodes.Add(id5);
                    //        }
                    //        else
                    //        {
                    //            TreeNode id5 = new TreeNode("Item" + z + ":id-TMSI:" + Nasclass1[SelectedIndex][k].identity, 1, 3);
                    //            scmessageType.Nodes.Add(id5);
                    //        }
                    //        z++;
                    //    }

                    //    if (Nasclass1[SelectedIndex][k].cause_str != "")
                    //    {
                    //        TreeNode id6 = new TreeNode("Item" + z + ":id-Nas Cause:" + Nasclass1[SelectedIndex][k].cause_str, 1, 3);
                    //        scmessageType.Nodes.Add(id6);
                    //        z++;
                    //    }
                    //    if (Nasclass1[SelectedIndex][k].ue_ip != "")
                    //    {
                    //        TreeNode id7 = new TreeNode("Item" + z + ":PDN address:" + Nasclass1[SelectedIndex][k].ue_ip, 1, 3);
                    //        scmessageType.Nodes.Add(id7);
                    //        z++;
                    //    }
                    //    #endregion
                    //}
                    //else if (Nasclass1[SelectedIndex][k].nas_name == "ciperd message  ")
                    //{
                    //    TreeNode id4 = new TreeNode("Item" + z + ":id-Nas:" + Nasclass1[SelectedIndex][k].nas_name, 1, 3);
                    //    scmessageType.Nodes.Add(id4);
                    //}
                    #endregion
                    if ((selectStruct.EmmName != " ") && (selectStruct.EmmName != "ciperd message  "))
                    {
                        #region
                        TreeNode id4 = new TreeNode("Item" + z + ":id-Nas:" + selectStruct.EmmName, 1, 3);
                        scmessageType.Nodes.Add(id4);
                        selectStruct.NoEmm = z;
                        z++;
                        if (selectStruct.NasIdentity != "")
                        {
                            if (selectStruct.NasIdentity.Length > 8)
                            {
                                TreeNode id5 = new TreeNode("Item" + z + ":id-IMSI:" + selectStruct.NasIdentity, 1, 3);
                                scmessageType.Nodes.Add(id5);
                            }
                            else
                            {
                                TreeNode id5 = new TreeNode("Item" + z + ":id-TMSI:" + selectStruct.NasIdentity, 1, 3);
                                scmessageType.Nodes.Add(id5);
                            }
                            selectStruct.NoNasIdentity = z;
                            z++;
                        }

                        if (selectStruct.NasCause != "")
                        {
                            TreeNode id6 = new TreeNode("Item" + z + ":id-Nas Cause:" + selectStruct.NasCause, 1, 3);
                            scmessageType.Nodes.Add(id6);
                            selectStruct.NoNasCause = z;
                            z++;
                        }
                        if (selectStruct.UeIP != "")
                        {
                            TreeNode id7 = new TreeNode("Item" + z + ":PDN address:" + selectStruct.UeIP, 1, 3);
                            scmessageType.Nodes.Add(id7);
                            selectStruct.NoUeIp = z;
                            z++;
                        }
                        #endregion
                    }
                    else if (selectStruct.EmmName == "ciperd message  ")
                    {
                        TreeNode id4 = new TreeNode("Item" + z + ":id-Nas:" + "ciperd message  ", 1, 3);
                        scmessageType.Nodes.Add(id4);
                    }
                    sctr.ExpandAll();
                    #endregion
                    OfflineStruct.Add(selectStruct);
                }
                #endregion

            }
            else
            {

            }
        }
        string protocolSelected_Online;//用户在“离线消息”界面上所选择信令的协议_yao
        List<byte[]> text_OnLine = new List<byte[]>();//点击消息对应的数据流数组_yao
        List<SignalStruct> OnlineStruct = new List<SignalStruct>();//存储在线s1ap信令信元的信息，用于显示对应编码//2017.6.19 黄刚
        private void listView_Data_Click(object sender, EventArgs e)
        {
            treeView_mess.Nodes.Clear();
            OnlineStruct.Clear();
            TextBox_Hex.Text = "";
            protocolSelected_Online = " ";//用户所选信令的协议
            string DATA = "";//用户所选信令的编码
            //查处指定行的序号以及该行所使用的协议
            if (listView_Data.SelectedIndices != null && listView_Data.SelectedIndices.Count > 0)
            {
                ListView.SelectedIndexCollection c = listView_Data.SelectedIndices;
                protocolSelected_Online = listView_Data.Items[c[0]].SubItems[1].Text.ToString();
                DATA = listView_Data.Items[c[0]].SubItems[8].Text.ToString();
            }
            byte[] DATA1 = new byte[DATA.Length];
            DATA1 = Readdata.strToToHexByte(DATA);
            TextBox_Hex.Text = Readdata.GetDataHex(DATA1, 0, DATA1.Length);
            text_OnLine.Add(DATA1);//yao
            if (protocolSelected_Online == "DIAMETER-SCTP")
            {
                //clickFlag_Online = 1;
                #region 采用SCTP承载diameter的树状解析流程
                SCTP.DecodeSctpChunk(Readdata.strToToHexByte(DATA));//
                TreeNode sctr = new TreeNode("SCTP Protocol", 1, 3); //表示协议类型的根节点
                treeView_mess.Nodes.Add(sctr);
                TreeNode scmessageType = new TreeNode(SCTP.protocol_Treeview, 1, 3);
                sctr.Nodes.Add(scmessageType);
                TreeNode version = new TreeNode(SCTP.version, 1, 3);     //表示解析消息头的节点
                TreeNode length = new TreeNode(SCTP.length, 1, 3);
                TreeNode flag = new TreeNode(SCTP.flag, 1, 3);
                TreeNode commandCODE = new TreeNode(SCTP.commandCODE, 1, 3);
                TreeNode applicationID = new TreeNode(SCTP.applicationID, 1, 3);
                TreeNode H2HIdentifier = new TreeNode(SCTP.H2HIdentifier, 1, 3);
                TreeNode E2EIdentifier = new TreeNode(SCTP.E2EIdentifier, 1, 3);
                TreeNode AVP = new TreeNode("AVP", 1, 3);
                scmessageType.Nodes.Add(version);
                scmessageType.Nodes.Add(length);
                scmessageType.Nodes.Add(flag);
                scmessageType.Nodes.Add(commandCODE);
                scmessageType.Nodes.Add(applicationID);
                scmessageType.Nodes.Add(H2HIdentifier);
                scmessageType.Nodes.Add(E2EIdentifier);
                scmessageType.Nodes.Add(AVP);
                while (SCTP.userNameCode != "")
                {
                    TreeNode AVP1 = new TreeNode("User-Name AVP", 1, 3);
                    AVP.Nodes.Add(AVP1);
                    TreeNode avp1 = new TreeNode(SCTP.userNameCode, 1, 3);
                    TreeNode avp2 = new TreeNode(SCTP.userNameFlag, 1, 3);
                    TreeNode avp3 = new TreeNode(SCTP.userNameLength, 1, 3);
                    TreeNode avp4 = new TreeNode(SCTP.userNameStr, 1, 3);
                    AVP1.Nodes.Add(avp1);
                    AVP1.Nodes.Add(avp2);
                    AVP1.Nodes.Add(avp3);
                    AVP1.Nodes.Add(avp4);
                    break;
                }
                while (SCTP.avp_ResultcodeCode != "")
                {
                    TreeNode AVP1 = new TreeNode("Result-CODE AVP", 1, 3);
                    AVP.Nodes.Add(AVP1);
                    TreeNode avp1 = new TreeNode(SCTP.avp_ResultcodeCode, 1, 3);
                    TreeNode avp2 = new TreeNode(SCTP.avp_ResultcodeFlag, 1, 3);
                    TreeNode avp3 = new TreeNode(SCTP.avp_ResultcodeLength, 1, 3);
                    TreeNode avp4 = new TreeNode(SCTP.avp_ResultcodeStr, 1, 3);
                    AVP1.Nodes.Add(avp1);
                    AVP1.Nodes.Add(avp2);
                    AVP1.Nodes.Add(avp3);
                    AVP1.Nodes.Add(avp4);
                    break;
                }
                while (SCTP.avp_AuthenticationCode != "")
                {
                    TreeNode AVP2 = new TreeNode("Authentication-Info AVP", 1, 3);
                    AVP.Nodes.Add(AVP2);
                    TreeNode avp5 = new TreeNode(SCTP.avp_AuthenticationCode, 1, 3);
                    TreeNode avp6 = new TreeNode(SCTP.avp_AuthenticationFlag, 1, 3);
                    TreeNode avp7 = new TreeNode(SCTP.avp_AuthenticationLength, 1, 3);
                    TreeNode avp8 = new TreeNode(SCTP.avp_AuthenticationVendorID, 1, 3);
                    AVP2.Nodes.Add(avp5);
                    AVP2.Nodes.Add(avp6);
                    AVP2.Nodes.Add(avp7);
                    AVP2.Nodes.Add(avp8);
                    for (int iii = 0; iii < SCTP.avp_EUTRANVectorCode.Count; iii++)
                    {
                        while (SCTP.avp_EUTRANVectorCode[iii] != "")
                        {
                            TreeNode AVP3 = new TreeNode("E-UTRAN-Vector" + Convert.ToString(iii + 1) + " AVP", 1, 3);
                            AVP2.Nodes.Add(AVP3);
                            TreeNode avp9 = new TreeNode(SCTP.avp_EUTRANVectorCode[iii], 1, 3);
                            TreeNode avp10 = new TreeNode(SCTP.avp_EUTRANVectorFlag[iii], 1, 3);
                            TreeNode avp11 = new TreeNode(SCTP.avp_EUTRANVectorLength[iii], 1, 3);
                            AVP3.Nodes.Add(avp9);
                            AVP3.Nodes.Add(avp10);
                            AVP3.Nodes.Add(avp11);
                            if (SCTP.avp_EUTRANVectorVendorID.Count != 0)
                            {
                                TreeNode avp12 = new TreeNode(SCTP.avp_EUTRANVectorVendorID[iii], 1, 3);
                                AVP3.Nodes.Add(avp12);
                            }
                            TreeNode AVP4 = new TreeNode("KASME" + Convert.ToString(iii + 1) + " AVP", 1, 3);
                            AVP3.Nodes.Add(AVP4);
                            TreeNode avp13 = new TreeNode(SCTP.avp_KASMECode[iii], 1, 3);
                            TreeNode avp14 = new TreeNode(SCTP.avp_KASMEFlag[iii], 1, 3);
                            TreeNode avp15 = new TreeNode(SCTP.avp_KASMELength[iii], 1, 3);
                            TreeNode KASMEStr = new TreeNode(SCTP.KASMEStr[iii], 1, 3);
                            AVP4.Nodes.Add(avp13);
                            AVP4.Nodes.Add(avp14);
                            AVP4.Nodes.Add(avp15);
                            AVP4.Nodes.Add(KASMEStr);
                            break;
                        }
                    }
                    break;
                }
                while (SCTP.avp_SessionID1 != "")
                {
                    TreeNode AVP5 = new TreeNode("Session-ID AVP", 1, 3);
                    AVP.Nodes.Add(AVP5);
                    TreeNode avp16 = new TreeNode(SCTP.avp_SessionID1, 1, 3);
                    TreeNode avp17 = new TreeNode(SCTP.avp_SessionID2, 1, 3);
                    TreeNode avp18 = new TreeNode(SCTP.avp_SessionID3, 1, 3);
                    TreeNode avp19 = new TreeNode(SCTP.avp_SessionID4, 1, 3);
                    AVP5.Nodes.Add(avp16);
                    AVP5.Nodes.Add(avp17);
                    AVP5.Nodes.Add(avp18);
                    AVP5.Nodes.Add(avp19);
                    break;
                }
                sctr.ExpandAll();
                #endregion

                #region 获取各项参数的位置和长度信息
                //清空各项消息的位置和长度记录
                diameterLocationOnline = 0;
                diameterLengthOnline = 0;
                avpLocation_UserNameOnline = 0; avpLocation_ResultOnline = 0; avpLocation_AuthenticationOnline = 0; avpLocation_SessionIDOnline = 0;
                avpLength_UserNameOnline = 0; avpLength_ResultOnline = 0; avpLength_AuthenticationOnline = 0; avpLength_SessionIDOnline = 0;
                location_UserNameStrOnline = 0; location_ResultCodeStrOnline = 0; location_SessionID4Online = 0; length_UserNameStrOnline = 0; length_SessionID4Online = 0;
                avpLocation_EUTRANVectorOnline.Clear();
                avpLength_EUTRANVectorOnline.Clear();
                avpLocation_KASMEOnline.Clear();
                avpLength_KASMEOnline.Clear();
                location_KASMEStrOnline.Clear();
                //重新获取各项消息的位置和长度记录
                diameterLocationOnline = SCTP.diameterLocation;
                diameterLengthOnline = SCTP.diameterLength;
                avpLocation_UserNameOnline = SCTP.avpLocation_UserName;
                avpLocation_ResultOnline = SCTP.avpLocation_Result;
                avpLocation_AuthenticationOnline = SCTP.avpLocation_Authentication;
                avpLocation_SessionIDOnline = SCTP.avpLocation_SessionID;
                avpLength_UserNameOnline = SCTP.avpLength_UserName;
                avpLength_ResultOnline = SCTP.avpLength_Result;
                avpLength_AuthenticationOnline = SCTP.avpLength_Authentication;
                avpLength_SessionIDOnline = SCTP.avpLength_SessionID;
                location_UserNameStrOnline = SCTP.location_UserNameStr;
                location_ResultCodeStrOnline = SCTP.location_ResultCodeStr;
                location_SessionID4Online = SCTP.location_SessionID4;
                length_UserNameStrOnline = SCTP.length_UserNameStr;
                length_SessionID4Online = SCTP.length_SessionID4;
                avpLocation_EUTRANVectorOnline = SCTP.avpLocation_EUTRANVector;
                avpLength_EUTRANVectorOnline = SCTP.avpLength_EUTRANVector;
                avpLocation_KASMEOnline = SCTP.avpLocation_KASME;
                avpLength_KASMEOnline = SCTP.avpLength_KASME;
                location_KASMEStrOnline = SCTP.location_KASMEStr;
                #endregion

            }
            //指定行协议为TCP-diameter时，所进行的树状结构解析
            if (protocolSelected_Online == "DIAMETER-TCP")
            {
                //clickFlag_Online = 1;
                #region 采用TCP承载diameter的树状解析流程
                TCPDiameter.DecodeTcpChunk(Readdata.strToToHexByte(DATA));

                TreeNode sctr = new TreeNode("TCP Protocol", 1, 3); //表示协议类型的根节点
                treeView_mess.Nodes.Add(sctr);
                TreeNode scmessageType = new TreeNode(TCPDiameter.protocol_Treeview, 1, 3);
                sctr.Nodes.Add(scmessageType);
                TreeNode version = new TreeNode(TCPDiameter.Version, 1, 3);     //表示解析消息头的节点
                TreeNode length = new TreeNode(TCPDiameter.Length, 1, 3);
                TreeNode flag = new TreeNode(TCPDiameter.Flag, 1, 3);
                TreeNode commandCODE = new TreeNode(TCPDiameter.CommandCODE, 1, 3);
                TreeNode applicationID = new TreeNode(TCPDiameter.ApplicationID, 1, 3);
                TreeNode H2HIdentifier = new TreeNode(TCPDiameter.H2HIdentifier, 1, 3);
                TreeNode E2EIdentifier = new TreeNode(TCPDiameter.E2EIdentifier, 1, 3);
                TreeNode AVP = new TreeNode("AVP", 1, 3);
                scmessageType.Nodes.Add(version);
                scmessageType.Nodes.Add(length);
                scmessageType.Nodes.Add(flag);
                scmessageType.Nodes.Add(commandCODE);
                scmessageType.Nodes.Add(applicationID);
                scmessageType.Nodes.Add(H2HIdentifier);
                scmessageType.Nodes.Add(E2EIdentifier);
                scmessageType.Nodes.Add(AVP);
                while (TCPDiameter.userNameCode != "")
                {
                    TreeNode AVP1 = new TreeNode("User-Name AVP", 1, 3);
                    AVP.Nodes.Add(AVP1);
                    TreeNode avp1 = new TreeNode(TCPDiameter.userNameCode, 1, 3);
                    TreeNode avp2 = new TreeNode(TCPDiameter.userNameFlag, 1, 3);
                    TreeNode avp3 = new TreeNode(TCPDiameter.userNameLength, 1, 3);
                    TreeNode avp4 = new TreeNode(TCPDiameter.userNameStr, 1, 3);
                    AVP1.Nodes.Add(avp1);
                    AVP1.Nodes.Add(avp2);
                    AVP1.Nodes.Add(avp3);
                    AVP1.Nodes.Add(avp4);
                    break;
                }
                while (TCPDiameter.avp_ResultcodeCode != "")
                {
                    TreeNode AVP1 = new TreeNode("Result-CODE AVP", 1, 3);
                    AVP.Nodes.Add(AVP1);
                    TreeNode avp1 = new TreeNode(TCPDiameter.avp_ResultcodeCode, 1, 3);
                    TreeNode avp2 = new TreeNode(TCPDiameter.avp_ResultcodeFlag, 1, 3);
                    TreeNode avp3 = new TreeNode(TCPDiameter.avp_ResultcodeLength, 1, 3);
                    TreeNode avp4 = new TreeNode(TCPDiameter.avp_ResultcodeStr, 1, 3);
                    AVP1.Nodes.Add(avp1);
                    AVP1.Nodes.Add(avp2);
                    AVP1.Nodes.Add(avp3);
                    AVP1.Nodes.Add(avp4);
                    break;
                }
                while (TCPDiameter.avp_AuthenticationCode != "")
                {
                    TreeNode AVP2 = new TreeNode("Authentication-Info AVP", 1, 3);
                    AVP.Nodes.Add(AVP2);
                    TreeNode avp5 = new TreeNode(TCPDiameter.avp_AuthenticationCode, 1, 3);
                    TreeNode avp6 = new TreeNode(TCPDiameter.avp_AuthenticationFlag, 1, 3);
                    TreeNode avp7 = new TreeNode(TCPDiameter.avp_AuthenticationLength, 1, 3);
                    TreeNode avp8 = new TreeNode(TCPDiameter.avp_AuthenticationVendorID, 1, 3);
                    AVP2.Nodes.Add(avp5);
                    AVP2.Nodes.Add(avp6);
                    AVP2.Nodes.Add(avp7);
                    AVP2.Nodes.Add(avp8);
                    for (int iii = 0; iii < TCPDiameter.avp_EUTRANVectorCode.Count; iii++)
                    {
                        while (TCPDiameter.avp_EUTRANVectorCode[iii] != "")
                        {
                            TreeNode AVP3 = new TreeNode("E-UTRAN-Vector" + Convert.ToString(iii + 1) + " AVP", 1, 3);
                            AVP2.Nodes.Add(AVP3);
                            TreeNode avp9 = new TreeNode(TCPDiameter.avp_EUTRANVectorCode[iii], 1, 3);
                            TreeNode avp10 = new TreeNode(TCPDiameter.avp_EUTRANVectorFlag[iii], 1, 3);
                            TreeNode avp11 = new TreeNode(TCPDiameter.avp_EUTRANVectorLength[iii], 1, 3);

                            AVP3.Nodes.Add(avp9);
                            AVP3.Nodes.Add(avp10);
                            AVP3.Nodes.Add(avp11);
                            if (TCPDiameter.avp_EUTRANVectorVendorID.Count != 0)
                            {
                                TreeNode avp12 = new TreeNode(TCPDiameter.avp_EUTRANVectorVendorID[iii], 1, 3);
                                AVP3.Nodes.Add(avp12);
                            }
                            break;
                        }
                        while (TCPDiameter.avp_KASMECode[iii] != "")
                        {
                            TreeNode AVP4 = new TreeNode("KASME" + Convert.ToString(iii + 1) + " AVP", 1, 3);
                            AVP.Nodes.Add(AVP4);
                            TreeNode avp13 = new TreeNode(TCPDiameter.avp_KASMECode[iii], 1, 3);
                            TreeNode avp14 = new TreeNode(TCPDiameter.avp_KASMEFlag[iii], 1, 3);
                            TreeNode avp15 = new TreeNode(TCPDiameter.avp_KASMELength[iii], 1, 3);
                            TreeNode KASMEStr = new TreeNode(TCPDiameter.KASMEStr[iii], 1, 3);
                            AVP4.Nodes.Add(avp13);
                            AVP4.Nodes.Add(avp14);
                            AVP4.Nodes.Add(avp15);
                            AVP4.Nodes.Add(KASMEStr);
                            break;
                        }
                    }
                    break;
                }
                while (TCPDiameter.avp_SessionID1 != "")
                {
                    TreeNode AVP5 = new TreeNode("Session-ID AVP", 1, 3);
                    AVP.Nodes.Add(AVP5);
                    TreeNode avp16 = new TreeNode(TCPDiameter.avp_SessionID1, 1, 3);
                    TreeNode avp17 = new TreeNode(TCPDiameter.avp_SessionID2, 1, 3);
                    TreeNode avp18 = new TreeNode(TCPDiameter.avp_SessionID3, 1, 3);
                    TreeNode avp19 = new TreeNode(TCPDiameter.avp_SessionID4, 1, 3);
                    AVP5.Nodes.Add(avp16);
                    AVP5.Nodes.Add(avp17);
                    AVP5.Nodes.Add(avp18);
                    AVP5.Nodes.Add(avp19);
                    break;
                }
                sctr.ExpandAll();
                #endregion

                #region 获取各项参数的位置和长度信息
                //清空各项消息的位置和长度记录
                diameterLocationOnline = 0; diameterLengthOnline = 0;
                avpLocation_UserNameOnline = 0; avpLocation_ResultOnline = 0; avpLocation_AuthenticationOnline = 0; avpLocation_SessionIDOnline = 0;
                avpLength_UserNameOnline = 0; avpLength_ResultOnline = 0; avpLength_AuthenticationOnline = 0; avpLength_SessionIDOnline = 0;
                location_UserNameStrOnline = 0; location_ResultCodeStrOnline = 0; location_SessionID4Online = 0; length_UserNameStrOnline = 0; length_SessionID4Online = 0;
                avpLocation_EUTRANVectorOnline.Clear();
                avpLength_EUTRANVectorOnline.Clear();
                avpLocation_KASMEOnline.Clear();
                avpLength_KASMEOnline.Clear();
                location_KASMEStrOnline.Clear();
                //重新获取各项消息的位置和长度记录
                diameterLocationOnline = 32;
                diameterLengthOnline = TCPDiameter.diameterLength;
                avpLocation_UserNameOnline = TCPDiameter.avpLocation_UserName;
                avpLocation_ResultOnline = TCPDiameter.avpLocation_Result;
                avpLocation_AuthenticationOnline = TCPDiameter.avpLocation_Authentication;
                avpLocation_SessionIDOnline = TCPDiameter.avpLocation_SessionID;
                avpLength_UserNameOnline = TCPDiameter.avpLength_UserName;
                avpLength_ResultOnline = TCPDiameter.avpLength_Result;
                avpLength_AuthenticationOnline = TCPDiameter.avpLength_Authentication;
                avpLength_SessionIDOnline = TCPDiameter.avpLength_SessionID;
                location_UserNameStrOnline = TCPDiameter.location_UserNameStr;
                location_ResultCodeStrOnline = TCPDiameter.location_ResultCodeStr;
                location_SessionID4Online = TCPDiameter.location_SessionID4;
                length_UserNameStrOnline = TCPDiameter.length_UserNameStr;
                length_SessionID4Online = TCPDiameter.length_SessionID4;
                avpLocation_EUTRANVectorOnline = TCPDiameter.avpLocation_EUTRANVector;
                avpLength_EUTRANVectorOnline = TCPDiameter.avpLength_EUTRANVector;
                avpLocation_KASMEOnline = TCPDiameter.avpLocation_KASME;
                avpLength_KASMEOnline = TCPDiameter.avpLength_KASME;
                location_KASMEStrOnline = TCPDiameter.location_KASMEStr;
                #endregion


            }
            if (protocolSelected_Online == "GTPv2")
            {
                #region
                A.decoder(Readdata.strToToHexByte(DATA));
                TreeNode tr = new TreeNode("GTP Protocol", 1, 3); //表示协议类型的根节点
                TreeNode messageType = new TreeNode(A.mt, 1, 3);
                TreeNode htr = new TreeNode("Header", 1, 3);     //表示解析消息头的节点
                TreeNode Cause = new TreeNode("Cause", 1, 3);
                treeView_mess.Nodes.Add(tr);
                tr.Nodes.Add(messageType);
                tr.Nodes.Add(htr);
                tr.Nodes.Add(Cause);
                htr.Nodes.Add("", A.ver, 1, 3);
                htr.Nodes.Add("", A.pinfo, 1, 3);
                htr.Nodes.Add("", A.tinfo, 1, 3);
                htr.Nodes.Add("", A.mt, 1, 3);
                htr.Nodes.Add("", A.leninfo, 1, 3);
                Cause.Nodes.Add("", A.IEvalue, 1, 3);
                if (A.tflag == 1)
                {
                    htr.Nodes.Add("", A.teidinfo, 1, 3);
                }
                htr.Nodes.Add("", A.seqinfo, 1, 3);
                htr.Nodes.Add("", A.spareinfo, 1, 3);
                tr.ExpandAll();
                #endregion
            }
            if (protocolSelected_Online == "S1AP")
            {
                #region
                SctpDecode sctp1 = new SctpDecode();
                TreeNode sctr = new TreeNode("S1AP Protocol", 1, 3); //表示协议类型的根节点
                treeView_mess.Nodes.Add(sctr);
                List<byte[]> s1ap2 = new List<byte[]>();//存储一条报文中可能包含的多条s1ap消息
                s1ap2 = sctp1.DeScChunk(Readdata.strToToHexByte(DATA));
                string text = "";
                for (int no = 0; no < s1ap2.Count; no++)
                {
                    foreach (byte ab in s1ap2[no])//将byte数组转化为16进制字符串，用于在控件textBox中展示
                    {
                        text += ab.ToString("X2") + " ";
                    }
                }
                TextBox_Hex.Text = text;

                //解析所选择的s1ap信令的id pair，信令名称，cause，用户标识，并用Treeview控件展示
                for (int k = 0; (k < s1ap2.Count) && (s1ap2[k].Length > 1); k++)
                {
                    SignalStruct SelectStruct = new SignalStruct();
                    #region
                    s1apDec1.s1apIeDec(s1ap2[k]);
                    MmeUeS1apId = s1apDec1.MmeUeS1apId;//sctp.s1ap[j]中存放的是s1ap数据流
                    SelectStruct.mme = s1apDec1.MmeUeS1apId.ToString();
                    if (k == 0)
                        SelectStruct.indexMme = s1apDec1.indexMme;
                    else
                        SelectStruct.indexMme = s1apDec1.indexMme + s1ap2[0].Length;
                    SelectStruct.LengthMme = s1apDec1.LengthMme;
                    EnbUeS1apId = s1apDec1.EnbUeS1apId;
                    SelectStruct.enb = s1apDec1.EnbUeS1apId.ToString();
                    SelectStruct.LengthEnb = s1apDec1.LengthEnb;
                    if (k == 0)
                        SelectStruct.IndexEnb = s1apDec1.IndexEnb;
                    else
                        SelectStruct.IndexEnb = s1apDec1.IndexEnb + s1ap2[0].Length;
                    S1apNameOnline = s1apDec1.str1;//S1AP消息信令        
                    SelectStruct.Name = s1apDec1.str1;
                    if (k == 0)
                        SelectStruct.IndexName = 0;
                    else
                        SelectStruct.IndexName = s1ap2[0].Length;
                    SelectStruct.LengthName = 2;
                    S1apCauseKindOnline = s1apDec1.str2;//cause类别
                    S1apCauseNameOnline = s1apDec1.str3;//具体的cause名称
                    SelectStruct.cause = s1apDec1.str2;
                    SelectStruct.CauseKind = s1apDec1.str3;
                    if (k == 0)
                        SelectStruct.IndexCause = s1apDec1.IndexCause;
                    else
                        SelectStruct.IndexCause = s1apDec1.IndexCause + s1ap2[0].Length;
                    SelectStruct.LengthCause = s1apDec1.LengthCause;

                    if (S1apNameOnline == "Paging")
                    {
                        IdentityOnline = Id_paging.identity(s1ap2[k]);
                        SelectStruct.Identity = IdentityOnline;
                        SelectStruct.LengthIdentity = Id_paging.Length;//获取nas identity的编码长度
                        if (k == 0)
                        {
                            SelectStruct.IndexIdentity = Id_paging.Index;//获取nas identity的起始地址                        
                        }
                        else
                        {
                            SelectStruct.IndexIdentity = Id_paging.Index + s1ap2[0].Length;//获取nas identity的起始地址(包含前一条s1ap信令长度)                           
                        }
                    }

                    else if (S1apNameOnline == "initialUEMessage")
                    {
                        IdentityOnline = Id_initial.identity(s1ap2[k]);
                        SelectStruct.Identity = IdentityOnline;
                        SelectStruct.LengthIdentity = Id_initial.Length;
                        if (k == 0)
                        {
                            SelectStruct.IndexIdentity = Id_initial.Index;
                        }
                        else
                        {
                            SelectStruct.IndexIdentity = Id_initial.Index + s1ap2[0].Length;//获取nas identity的起始地址(包含前一条s1ap信令长度)                         
                        }

                    }
                    TreeNode scmessageType = new TreeNode("procedureCode:" + S1apNameOnline, 1, 3);
                    sctr.Nodes.Add(scmessageType);
                    int z = 0;

                    if (MmeUeS1apId != 0)
                    {
                        TreeNode id0 = new TreeNode("Item" + z + ":id-MME-UE-S1AP-ID" + "(" + MmeUeS1apId + ")", 1, 3);
                        scmessageType.Nodes.Add(id0);
                        SelectStruct.NoMme = z;
                        z++;
                    }
                    if (EnbUeS1apId != 0)
                    {
                        TreeNode id1 = new TreeNode("Item" + z + ":id-eNB-UE-S1AP-ID" + "(" + EnbUeS1apId + ")", 1, 3);
                        scmessageType.Nodes.Add(id1);
                        SelectStruct.NoEnb = z;
                        z++;
                    }
                    if (S1apCauseKindOnline != "")
                    {
                        TreeNode id2 = new TreeNode("Item" + z + ":id-Cause:" + S1apCauseKindOnline + ":" + S1apCauseNameOnline, 1, 3);
                        scmessageType.Nodes.Add(id2);
                        SelectStruct.NoCause = z;
                        z++;
                    }
                    if (IdentityOnline != "")
                    {
                        if (IdentityOnline.Length > 8)//当用户标识的长度大于8个字节，说明是IMSI，反之为tmsi
                        {
                            TreeNode id3 = new TreeNode("Item" + z + ":id-IMSI:" + IdentityOnline, 1, 3);
                            scmessageType.Nodes.Add(id3);
                        }
                        else
                        {
                            TreeNode id3 = new TreeNode("Item" + z + ":id-TMSI:" + IdentityOnline, 1, 3);
                            scmessageType.Nodes.Add(id3);
                        }
                        SelectStruct.NoIdentity = z;
                        z++;
                    }

                    //解析所选择的s1ap信令中包含的nas信令的信令名称，cause，用户标识，UE ip

                    nas_class nas_class1 = new nas_class();
                    s1ap_nas2.nas_decode(s1ap2[k]);
                    nas_class1.identity = s1ap_nas2.identity;
                    SelectStruct.NasIdentity = s1ap_nas2.identity;
                    SelectStruct.LengthNasId = s1ap_nas2.LengthIdentity;
                    if (k == 0)
                        SelectStruct.IndexNasId = s1ap_nas2.IndexIdentity;
                    else
                        SelectStruct.IndexNasId = s1ap_nas2.IndexIdentity + s1ap2[0].Length;
                    nas_class1.cause_str = s1ap_nas2.cause_str;
                    SelectStruct.NasCause = s1ap_nas2.cause_str;
                    SelectStruct.LengthNasCause = s1ap_nas2.LengthCause;
                    if (k == 0)
                        SelectStruct.IndexNasCause = s1ap_nas2.IndexCause;
                    else
                        SelectStruct.IndexNasCause = s1ap_nas2.IndexCause + s1ap2[0].Length;
                    nas_class1.nas_name = s1ap_nas2.nas_name;
                    if ((s1ap_nas2.EmmMessage1 != " ") && (s1ap_nas2.EsmMessage1 != " "))
                    {
                        SelectStruct.EmmName = s1ap_nas2.nas_name;
                        if (k == 0)
                            SelectStruct.IndexEmm = s1ap_nas2.IndexEmm;
                        else
                            SelectStruct.IndexEmm = s1ap_nas2.IndexEmm + SctpToS1ap.s1ap[0].Length;
                        SelectStruct.LengthEmm = s1ap_nas2.LengthEmm;
                    }
                    else if ((s1ap_nas2.EmmMessage1 == " ") && (s1ap_nas2.EsmMessage1 != " "))
                    {
                        SelectStruct.EmmName = "  " + s1ap_nas2.EsmMessage1;
                        if (k == 0)
                            SelectStruct.IndexEmm = s1ap_nas2.IndexEsm;
                        else
                            SelectStruct.IndexEmm = s1ap_nas2.IndexEsm + SctpToS1ap.s1ap[0].Length;
                        SelectStruct.LengthEmm = s1ap_nas2.LengthEsm;
                    }
                    else
                    {
                        SelectStruct.EmmName = " ";
                        SelectStruct.IndexEmm = 0;
                        SelectStruct.LengthEmm = 0;
                    }
                    //在线情况下不能解析出UEIP，故关于IP的各字段设为0
                    SelectStruct.UeIP = "";
                    SelectStruct.IndexUeIp = 0;
                    SelectStruct.LengthUeIp = 0;

                    //if (nas_class1.nas_name != " " && (nas_class1.nas_name != "ciperd message  "))
                    //{
                    //    #region
                    //    TreeNode id3 = new TreeNode("Item3:id-Nas:" + nas_class1.nas_name, 1, 3);
                    //    scmessageType.Nodes.Add(id3);
                    //    if (nas_class1.identity != "")
                    //    {
                    //        TreeNode id4 = new TreeNode("Item4:id-Identity:" + nas_class1.identity, 1, 3);
                    //        scmessageType.Nodes.Add(id4);
                    //    }
                    //    if (nas_class1.cause_str != "")
                    //    {
                    //        TreeNode id5 = new TreeNode("Item5:id-Nas Cause:" + nas_class1.cause_str, 1, 3);
                    //        scmessageType.Nodes.Add(id5);
                    //    }
                    //    #endregion
                    //}
                    //else if (nas_class1.nas_name == "ciperd message  ")
                    //{
                    //    TreeNode id3 = new TreeNode("Item3:id-Nas:" + nas_class1.nas_name, 1, 3);
                    //    scmessageType.Nodes.Add(id3);
                    //}
                    if ((SelectStruct.EmmName != " ") && (SelectStruct.EmmName != "ciperd message  "))
                    {
                        #region
                        TreeNode id4 = new TreeNode("Item" + z + ":id-Nas:" + SelectStruct.EmmName, 1, 3);
                        scmessageType.Nodes.Add(id4);
                        SelectStruct.NoEmm = z;
                        z++;
                        if (SelectStruct.NasIdentity != "")
                        {
                            if (SelectStruct.NasIdentity.Length > 8)
                            {
                                TreeNode id5 = new TreeNode("Item" + z + ":id-IMSI:" + SelectStruct.NasIdentity, 1, 3);
                                scmessageType.Nodes.Add(id5);
                            }
                            else
                            {
                                TreeNode id5 = new TreeNode("Item" + z + ":id-TMSI:" + SelectStruct.NasIdentity, 1, 3);
                                scmessageType.Nodes.Add(id5);
                            }
                            SelectStruct.NoNasIdentity = z;
                            z++;
                        }

                        if (SelectStruct.NasCause != "")
                        {
                            TreeNode id6 = new TreeNode("Item" + z + ":id-Nas Cause:" + SelectStruct.NasCause, 1, 3);
                            scmessageType.Nodes.Add(id6);
                            SelectStruct.NoNasCause = z;
                            z++;
                        }
                        if (SelectStruct.UeIP != "")
                        {
                            TreeNode id7 = new TreeNode("Item" + z + ":PDN address:" + SelectStruct.UeIP, 1, 3);
                            scmessageType.Nodes.Add(id7);
                            SelectStruct.NoUeIp = z;
                            z++;
                        }
                        #endregion
                    }
                    else if (SelectStruct.EmmName == "ciperd message  ")
                    {
                        TreeNode id4 = new TreeNode("Item" + z + ":id-Nas:" + "ciperd message  ", 1, 3);
                        scmessageType.Nodes.Add(id4);
                    }
                    sctr.ExpandAll();
                    #endregion
                    OnlineStruct.Add(SelectStruct);
                }
                #endregion
            }
        }


        private void listView_Process_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void gTPv2呼叫跟踪ToolStripMenuItem_Click(object sender, EventArgs e)
        {


        }

        private void diameter呼叫跟踪ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void s1apToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void 信令流程ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void listView_Data_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void 暂停ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button_Save_Click(object sender, EventArgs e)
        {

        }


        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton_Start_Click(object sender, EventArgs e)
        {
            if (bStatus == false)//首次开始监测或停止监听之后的开始
            {

                Clear();
                startTime = DateTime.Now;
                toolStripButton_Start.Enabled = false;
                toolStripButton_Pause.Enabled = true;
                toolStripButton_Stop.Enabled = true;
                Catch.Start(bStatues_CBTC, bStatues_Signal, Bind.socket);
                ServiceStatus.Text = "开始监听显示";
                //UpdateStatus();

            }
            else//暂停之后的开始监听
            {
                toolStripButton_Start.Enabled = false;
                toolStripButton_Pause.Enabled = true;
                toolStripButton_Stop.Enabled = true;
                toolStripButton_Filter.Enabled = false;
                Catch.Start(bStatues_CBTC, bStatues_Signal, Bind.socket);
                ServiceStatus.Text = "开始监听显示";
                //UpdateStatus();
            }


            toolStripButton_Filter.Enabled = false;
        }

        private void toolStripButton_Stop_Click(object sender, EventArgs e)
        {
            toolStripButton_Stop.Enabled = false;
            toolStripButton_Start.Enabled = true;
            toolStripButton_Filter.Enabled = true;
            this.toolStripButton_Pause.Enabled = false;
            Catch.Stop();
            bStatus = false;//状态切换
            ServiceStatus.Text = "停止显示";
            //UpdateStatus();
            toolStripButton_Filter.Enabled = true;
        }

        private void toolStripButton_Pause_Click(object sender, EventArgs e)
        {
            toolStripButton_Stop.Enabled = false;
            toolStripButton_Pause.Enabled = false;
            toolStripButton_Start.Enabled = true;
            Catch.Stop();
            bStatus = true;//状态切换
            ServiceStatus.Text = "暂停显示";
            //UpdateStatus();
            toolStripButton_Filter.Enabled = false;
        }

        private void toolStripButton_Filter_Click(object sender, EventArgs e)
        {
            FilterOptionForm.accept += new EventHandler(f2_accept); //绑定过滤窗体
            if (FilterOptionForm.ShowDialog() == DialogResult.OK)
            {
                FilterOptionForm.Show();
            }
        }
        static TreeNode selectedNode1 = null;
        string parentOnNode = "";
        private void treeView_mess_MouseClick(object sender, MouseEventArgs e)
        {
            if ((sender as TreeView) != null)
            {
                treeView_mess.SelectedNode = treeView_mess.GetNodeAt(e.X, e.Y);
                selectedNode1 = treeView_mess.SelectedNode;
            }
            if (selectedNode1.Parent != null)//获取所选节点的父节点
                parentOnNode = selectedNode1.Parent.Text;
            System.Timers.Timer t1 = new System.Timers.Timer(10);
            t1.Elapsed += new System.Timers.ElapsedEventHandler(SelectedOnLine);
            t1.AutoReset = false;
            t1.Enabled = true;
            #region
            //#region 点击diameter节点时对应凸显其对应的数据流
            //if (protocolSelected_Online == "DIAMETER-SCTP" || protocolSelected_Online == "DIAMETER-TCP")
            //{
            //    string nodeText = selectedNode.Text;//点击行的文本
            //    switch (nodeText)
            //    {
            //        case "SCTP Protocol":
            //            TextBox_Hex.SelectionStart = 0 * 3;
            //            TextBox_Hex.SelectionLength = diameterLocationOnline * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //            break;
            //        case "TCP Protocol":
            //            TextBox_Hex.SelectionStart = 0 * 3;
            //            TextBox_Hex.SelectionLength = diameterLocationOnline * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //            break;
            //        case "diameter":
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline) * 3;
            //            TextBox_Hex.SelectionLength = diameterLengthOnline * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //            break;
            //        case "Version:1":
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline) * 3;
            //            TextBox_Hex.SelectionLength = 1 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //            break;
            //        case "User-Name AVP":
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_UserNameOnline) * 3;
            //            TextBox_Hex.SelectionLength = avpLength_UserNameOnline * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //            break;
            //        case "Result-CODE AVP":
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_ResultOnline) * 3;
            //            TextBox_Hex.SelectionLength = avpLength_ResultOnline * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //            break;
            //        case "Authentication-Info AVP":
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_AuthenticationOnline) * 3;
            //            TextBox_Hex.SelectionLength = avpLength_AuthenticationOnline * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //            break;
            //        case "E-UTRAN-Vector1 AVP":
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[0]) * 3;
            //            TextBox_Hex.SelectionLength = avpLength_EUTRANVectorOnline[0] * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //            break;
            //        case "E-UTRAN-Vector2 AVP":
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[1]) * 3;
            //            TextBox_Hex.SelectionLength = avpLength_EUTRANVectorOnline[1] * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //            break;
            //        case "E-UTRAN-Vector3 AVP":
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[2]) * 3;
            //            TextBox_Hex.SelectionLength = avpLength_EUTRANVectorOnline[2] * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //            break;
            //        case "E-UTRAN-Vector4 AVP":
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[3]) * 3;
            //            TextBox_Hex.SelectionLength = avpLength_EUTRANVectorOnline[3] * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //            break;
            //        case "E-UTRAN-Vector5 AVP":
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[4]) * 3;
            //            TextBox_Hex.SelectionLength = avpLength_EUTRANVectorOnline[4] * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //            break;
            //        case "KASME1 AVP":
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_KASMEOnline[0]) * 3;
            //            TextBox_Hex.SelectionLength = avpLength_KASMEOnline[0] * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //            break;
            //        case "KASME2 AVP":
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_KASMEOnline[1]) * 3;
            //            TextBox_Hex.SelectionLength = avpLength_KASMEOnline[1] * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //            break;
            //        case "KASME3 AVP":
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_KASMEOnline[2]) * 3;
            //            TextBox_Hex.SelectionLength = avpLength_KASMEOnline[2] * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //            break;
            //        case "KASME4 AVP":
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_KASMEOnline[3]) * 3;
            //            TextBox_Hex.SelectionLength = avpLength_KASMEOnline[3] * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //            break;
            //        case "KASME5 AVP":
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_KASMEOnline[4]) * 3;
            //            TextBox_Hex.SelectionLength = avpLength_KASMEOnline[4] * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //            break;
            //        case "Session-ID AVP":
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_SessionIDOnline) * 3;
            //            TextBox_Hex.SelectionLength = avpLength_SessionIDOnline * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //            break;
            //    }
            //    if (nodeText.Length >= 7)
            //    {
            //        if (nodeText.Substring(0, 7) == "Length:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + 1) * 3;
            //            TextBox_Hex.SelectionLength = 3 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }

            //    if (nodeText.Length >= 5)
            //    {
            //        if (nodeText.Substring(0, 5) == "Flag:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + 4) * 3;
            //            TextBox_Hex.SelectionLength = 1 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 12)
            //    {
            //        if (nodeText.Substring(0, 12) == "CommandCODE:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + 5) * 3;
            //            TextBox_Hex.SelectionLength = 3 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //        if (nodeText.Substring(0, 12) == "3GPP S6a/S6d" || nodeText.Substring(0, 12) == "Diameter Com" || nodeText.Substring(0, 12) == "Diameter Bas")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + 8) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 3)
            //    {
            //        if (nodeText.Substring(0, 3) == "Hop")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + 12) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //        if (nodeText.Substring(0, 3) == "End")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + 16) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 19)
            //    {
            //        if (nodeText.Substring(0, 19) == "User-Name AVP Code:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_UserNameOnline) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //        if (nodeText.Substring(0, 19) == "User-Name AVP Flag:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_UserNameOnline + 4) * 3;
            //            TextBox_Hex.SelectionLength = 1 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 21)
            //    {
            //        if (nodeText.Substring(0, 21) == "User-Name AVP Length:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_UserNameOnline + 5) * 3;
            //            TextBox_Hex.SelectionLength = 3 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 3)
            //    {
            //        if (nodeText.Substring(0, 3) == "460")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + location_UserNameStrOnline) * 3;
            //            TextBox_Hex.SelectionLength = length_UserNameStrOnline * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 21)
            //    {
            //        if (nodeText.Substring(0, 21) == "Result-CODE AVP Code:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_ResultOnline) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //        if (nodeText.Substring(0, 21) == "Result-CODE AVP Flag:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_ResultOnline + 4) * 3;
            //            TextBox_Hex.SelectionLength = 1 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 23)
            //    {
            //        if (nodeText.Substring(0, 23) == "Result-CODE AVP Length:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_ResultOnline + 5) * 3;
            //            TextBox_Hex.SelectionLength = 3 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 11)
            //    {
            //        if (nodeText.Substring(0, 11) == "ResultCode:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + location_ResultCodeStrOnline) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 29)
            //    {
            //        if (nodeText.Substring(0, 29) == "Authentication-Info AVP Code:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_AuthenticationOnline) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 28)
            //    {
            //        if (nodeText.Substring(0, 28) == "Authentication-Info AVPFlag:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_AuthenticationOnline + 4) * 3;
            //            TextBox_Hex.SelectionLength = 1 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 30)
            //    {
            //        if (nodeText.Substring(0, 30) == "Authentication-Info AVPLength:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_AuthenticationOnline + 5) * 3;
            //            TextBox_Hex.SelectionLength = 3 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 32)
            //    {
            //        if (nodeText.Substring(0, 32) == "Authentication-Info AVPVendorID:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_AuthenticationOnline + 8) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 21)
            //    {
            //        if (nodeText.Substring(0, 21) == "E-UTRAN-Vector1 Code:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[0]) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //        if (nodeText.Substring(0, 21) == "E-UTRAN-Vector1 Flag:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[0] + 4) * 3;
            //            TextBox_Hex.SelectionLength = 1 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 23)
            //    {
            //        if (nodeText.Substring(0, 23) == "E-UTRAN-Vector1 Length:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[0] + 5) * 3;
            //            TextBox_Hex.SelectionLength = 3 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 25)
            //    {
            //        if (nodeText.Substring(0, 25) == "E-UTRAN-Vector1 VendorID:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[0] + 8) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 21)
            //    {
            //        if (nodeText.Substring(0, 21) == "E-UTRAN-Vector2 Code:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[1]) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //        if (nodeText.Substring(0, 21) == "E-UTRAN-Vector2 Flag:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[1] + 4) * 3;
            //            TextBox_Hex.SelectionLength = 1 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 23)
            //    {
            //        if (nodeText.Substring(0, 23) == "E-UTRAN-Vector2 Length:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[1] + 5) * 3;
            //            TextBox_Hex.SelectionLength = 3 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 25)
            //    {
            //        if (nodeText.Substring(0, 25) == "E-UTRAN-Vector2 VendorID:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[1] + 8) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 21)
            //    {
            //        if (nodeText.Substring(0, 21) == "E-UTRAN-Vector3 Code:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[2]) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //        if (nodeText.Substring(0, 21) == "E-UTRAN-Vector3 Flag:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[2] + 4) * 3;
            //            TextBox_Hex.SelectionLength = 1 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 23)
            //    {
            //        if (nodeText.Substring(0, 23) == "E-UTRAN-Vector3 Length:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[2] + 5) * 3;
            //            TextBox_Hex.SelectionLength = 3 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 25)
            //    {
            //        if (nodeText.Substring(0, 25) == "E-UTRAN-Vector3 VendorID:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[2] + 8) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 21)
            //    {
            //        if (nodeText.Substring(0, 21) == "E-UTRAN-Vector4 Code:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[3]) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //        if (nodeText.Substring(0, 21) == "E-UTRAN-Vector4 Flag:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[3] + 4) * 3;
            //            TextBox_Hex.SelectionLength = 1 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 23)
            //    {
            //        if (nodeText.Substring(0, 23) == "E-UTRAN-Vector4 Length:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[3] + 5) * 3;
            //            TextBox_Hex.SelectionLength = 3 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 25)
            //    {
            //        if (nodeText.Substring(0, 25) == "E-UTRAN-Vector4 VendorID:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[3] + 8) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 21)
            //    {
            //        if (nodeText.Substring(0, 21) == "E-UTRAN-Vector5 Code:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[4]) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //        if (nodeText.Substring(0, 21) == "E-UTRAN-Vector5 Flag:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[4] + 4) * 3;
            //            TextBox_Hex.SelectionLength = 1 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 23)
            //    {
            //        if (nodeText.Substring(0, 23) == "E-UTRAN-Vector5 Length:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[4] + 5) * 3;
            //            TextBox_Hex.SelectionLength = 3 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 25)
            //    {
            //        if (nodeText.Substring(0, 25) == "E-UTRAN-Vector5 VendorID:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[4] + 8) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 12)
            //    {

            //        if (nodeText.Substring(0, 12) == "KASME1 Code:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_KASMEOnline[0]) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //        if (nodeText.Substring(0, 12) == "KASME1 Flag:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_KASMEOnline[0] + 4) * 3;
            //            TextBox_Hex.SelectionLength = 1 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 14)
            //    {
            //        if (nodeText.Substring(0, 14) == "KASME1 Length:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_KASMEOnline[0] + 5) * 3;
            //            TextBox_Hex.SelectionLength = 3 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 16)
            //    {
            //        if (nodeText.Substring(0, 16) == "KASME1 VendorID:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_KASMEOnline[0] + 8) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 7)
            //    {
            //        if (nodeText.Substring(0, 7) == "KASME1:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + location_KASMEStrOnline[0]) * 3;
            //            TextBox_Hex.SelectionLength = 32 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 12)
            //    {
            //        if (nodeText.Substring(0, 12) == "KASME2 Code:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_KASMEOnline[1]) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //        if (nodeText.Substring(0, 12) == "KASME2 Flag:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_KASMEOnline[1] + 4) * 3;
            //            TextBox_Hex.SelectionLength = 1 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 14)
            //    {
            //        if (nodeText.Substring(0, 14) == "KASME2 Length:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_KASMEOnline[1] + 5) * 3;
            //            TextBox_Hex.SelectionLength = 3 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 16)
            //    {
            //        if (nodeText.Substring(0, 16) == "KASME2 VendorID:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_KASMEOnline[1] + 8) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 7)
            //    {
            //        if (nodeText.Substring(0, 7) == "KASME2:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + location_KASMEStrOnline[1]) * 3;
            //            TextBox_Hex.SelectionLength = 32 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 12)
            //    {
            //        if (nodeText.Substring(0, 12) == "KASME3 Code:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_KASMEOnline[2]) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //        if (nodeText.Substring(0, 12) == "KASME3 Flag:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_KASMEOnline[2] + 4) * 3;
            //            TextBox_Hex.SelectionLength = 1 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 14)
            //    {
            //        if (nodeText.Substring(0, 14) == "KASME3 Length:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_KASMEOnline[2] + 5) * 3;
            //            TextBox_Hex.SelectionLength = 3 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 16)
            //    {
            //        if (nodeText.Substring(0, 16) == "KASME3 VendorID:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_KASMEOnline[2] + 8) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 7)
            //    {
            //        if (nodeText.Substring(0, 7) == "KASME3:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + location_KASMEStrOnline[2]) * 3;
            //            TextBox_Hex.SelectionLength = 32 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 12)
            //    {
            //        if (nodeText.Substring(0, 12) == "KASME4 Code:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_KASMEOnline[3]) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //        if (nodeText.Substring(0, 12) == "KASME4 Flag:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_KASMEOnline[3] + 4) * 3;
            //            TextBox_Hex.SelectionLength = 1 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 14)
            //    {
            //        if (nodeText.Substring(0, 14) == "KASME4 Length:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_KASMEOnline[3] + 5) * 3;
            //            TextBox_Hex.SelectionLength = 3 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 16)
            //    {
            //        if (nodeText.Substring(0, 16) == "KASME4 VendorID:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_KASMEOnline[3] + 8) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 7)
            //    {
            //        if (nodeText.Substring(0, 7) == "KASME4:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + location_KASMEStrOnline[3]) * 3;
            //            TextBox_Hex.SelectionLength = 32 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 12)
            //    {
            //        if (nodeText.Substring(0, 12) == "KASME5 Code:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_KASMEOnline[4]) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //        if (nodeText.Substring(0, 12) == "KASME5 Flag:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_KASMEOnline[4] + 4) * 3;
            //            TextBox_Hex.SelectionLength = 1 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 14)
            //    {
            //        if (nodeText.Substring(0, 14) == "KASME5 Length:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_KASMEOnline[4] + 5) * 3;
            //            TextBox_Hex.SelectionLength = 3 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 16)
            //    {
            //        if (nodeText.Substring(0, 16) == "KASME5 VendorID:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_KASMEOnline[4] + 8) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 7)
            //    {
            //        if (nodeText.Substring(0, 7) == "KASME5:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + location_KASMEStrOnline[4]) * 3;
            //            TextBox_Hex.SelectionLength = 32 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 20)
            //    {
            //        if (nodeText.Substring(0, 20) == "Session-ID AVP Code:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_SessionIDOnline) * 3;
            //            TextBox_Hex.SelectionLength = 4 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 19)
            //    {
            //        if (nodeText.Substring(0, 19) == "Session-ID AVPFlag:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_SessionIDOnline + 4) * 3;
            //            TextBox_Hex.SelectionLength = 1 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 21)
            //    {
            //        if (nodeText.Substring(0, 21) == "Session-ID AVPLength:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + avpLocation_SessionIDOnline + 5) * 3;
            //            TextBox_Hex.SelectionLength = 3 * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }
            //    if (nodeText.Length >= 24)
            //    {
            //        if (nodeText.Substring(0, 24) == "Session-ID AVPSessionid:")
            //        {
            //            TextBox_Hex.SelectionStart = (diameterLocationOnline + location_SessionID4Online) * 3;
            //            TextBox_Hex.SelectionLength = length_SessionID4Online * 3;
            //            TextBox_Hex.SelectionColor = Color.Red;
            //        }
            //    }

            //}
            //#endregion

            //#region                            //点击s1ap节点时凸显对应的编码
            //if (selectedNode.Text == OnlineStruct.Name)
            //{
            //    TextBox_Hex.SelectionStart = OnlineStruct.IndexName * 3;
            //    TextBox_Hex.SelectionLength = OnlineStruct.LengthName * 3;
            //    TextBox_Hex.SelectionColor = Color.Red;
            //}
            //else if (selectedNode.Text == "Item" + OnlineStruct.NoMme + ":id-MME-UE-S1AP-ID" + "(" + OnlineStruct.mme + ")")
            //{
            //    TextBox_Hex.SelectionStart = OnlineStruct.indexMme * 3;
            //    TextBox_Hex.SelectionLength = OnlineStruct.LengthMme * 3;
            //    TextBox_Hex.SelectionColor = Color.Red;
            //}
            //else if (selectedNode.Text == "Item" + OnlineStruct.NoEnb + ":id-eNB-UE-S1AP-ID" + "(" + OnlineStruct.enb + ")")
            //{
            //    TextBox_Hex.SelectionStart = OnlineStruct.IndexEnb * 3;
            //    TextBox_Hex.SelectionLength = OnlineStruct.LengthEnb * 3;
            //    TextBox_Hex.SelectionColor = Color.Red;
            //}
            //else if (selectedNode.Text == "Item" + OnlineStruct.NoCause + ":id-Cause" + OnlineStruct.CauseKind + ":" + OnlineStruct.cause)
            //{
            //    TextBox_Hex.SelectionStart = OnlineStruct.IndexCause * 3;
            //    TextBox_Hex.SelectionLength = OnlineStruct.LengthCause * 3;
            //    TextBox_Hex.SelectionColor = Color.Red;
            //}
            //else if (selectedNode.Text == "Item" + OnlineStruct.NoIdentity + ":id-IMSI:" + OnlineStruct.Identity)
            //{
            //    TextBox_Hex.SelectionStart = OnlineStruct.IndexIdentity * 3;
            //    TextBox_Hex.SelectionLength = OnlineStruct.LengthIdentity * 3;
            //    TextBox_Hex.SelectionColor = Color.Red;
            //}
            //else if (selectedNode.Text == "Item" + OnlineStruct.NoIdentity + ":id-TMSI:" + OnlineStruct.Identity)
            //{
            //    TextBox_Hex.SelectionStart = OnlineStruct.IndexIdentity * 3;
            //    TextBox_Hex.SelectionLength = OnlineStruct.LengthIdentity * 3;
            //    TextBox_Hex.SelectionColor = Color.Red;
            //}
            //else if (selectedNode.Text == "Item" + OnlineStruct.NoEmm + ":id-Nas:" + OnlineStruct.EmmName)
            //{
            //    TextBox_Hex.SelectionStart = OnlineStruct.IndexEmm * 3;
            //    TextBox_Hex.SelectionLength = OnlineStruct.LengthEmm * 3;
            //    TextBox_Hex.SelectionColor = Color.Red;
            //}
            //else if (selectedNode.Text == "Item" + OnlineStruct.NoNasIdentity + ":id-IMSI:" + OnlineStruct.NasIdentity)
            //{
            //    TextBox_Hex.SelectionStart = OnlineStruct.IndexNasId * 3;
            //    TextBox_Hex.SelectionLength = OnlineStruct.LengthNasId * 3;
            //    TextBox_Hex.SelectionColor = Color.Red;
            //}
            //else if (selectedNode.Text == "Item" + OnlineStruct.NoNasIdentity + ":id-TMSI:" + OnlineStruct.NasIdentity)
            //{
            //    TextBox_Hex.SelectionStart = OnlineStruct.IndexNasId * 3;
            //    TextBox_Hex.SelectionLength = OnlineStruct.LengthNasId * 3;
            //    TextBox_Hex.SelectionColor = Color.Red;
            //}
            //else if (selectedNode.Text == "Item" + OnlineStruct.NoNasCause + ":id-Nas Cause:" + OnlineStruct.NasCause)
            //{
            //    TextBox_Hex.SelectionStart = OnlineStruct.IndexNasCause * 3;
            //    TextBox_Hex.SelectionLength = OnlineStruct.LengthNasCause * 3;
            //    TextBox_Hex.SelectionColor = Color.Red;
            //}
            //else if (selectedNode.Text == "Item" + OnlineStruct.NoUeIp + ":PDN address:" + OnlineStruct.UeIP)
            //{
            //    TextBox_Hex.SelectionStart = OnlineStruct.IndexUeIp * 3;
            //    TextBox_Hex.SelectionLength = OnlineStruct.LengthUeIp * 3;
            //    TextBox_Hex.SelectionColor = Color.Red;
            //}
            //#endregion

            //#region 点击gtp节点时对应
            //if (protocolSelected_Offline == "GTPv2")
            //{
            //    string nodeText = treeView_mess.Nodes[0].Nodes[0].Text;//点击行的文本
            //    string SubnodeText = treeView_mess.SelectedNode.Text.Substring(0, 5);
            //    if (nodeText == "Echo Request" || nodeText == "Echo Response")
            //    {
            //        switch (SubnodeText)
            //        {
            //            case "010. ":
            //                textBox_OffLine.SelectionStart = 28 * 3;
            //                textBox_OffLine.SelectionLength = 3;
            //                textBox_OffLine.SelectionColor = Color.Red;
            //                break;
            //            case "...0 ":
            //                textBox_OffLine.SelectionStart = 28 * 3;
            //                textBox_OffLine.SelectionLength = 3;
            //                textBox_OffLine.SelectionColor = Color.Red;
            //                break;
            //            case ".... ":
            //                textBox_OffLine.SelectionStart = 28 * 3;
            //                textBox_OffLine.SelectionLength = 3;
            //                textBox_OffLine.SelectionColor = Color.Red;
            //                break;
            //            case "Messa":
            //                textBox_OffLine.SelectionStart = 30 * 3;
            //                textBox_OffLine.SelectionLength = 6;
            //                textBox_OffLine.SelectionColor = Color.Red;
            //                break;
            //            case "Seque":
            //                textBox_OffLine.SelectionStart = 32 * 3;
            //                textBox_OffLine.SelectionLength = 9;
            //                textBox_OffLine.SelectionColor = Color.Red;
            //                break;
            //            case "spare":
            //                textBox_OffLine.SelectionStart = 35 * 3;
            //                textBox_OffLine.SelectionLength = 3;
            //                textBox_OffLine.SelectionColor = Color.Red;
            //                break;
            //            default:
            //                break;
            //        }
            //        if (selectedNode == treeView_OffLine.Nodes[0].Nodes[1].Nodes[3])
            //        {
            //            textBox_OffLine.SelectionStart = 29 * 3;
            //            textBox_OffLine.SelectionLength = 3;
            //            textBox_OffLine.SelectionColor = Color.Red;
            //        }
            //        if (selectedNode == treeView_OffLine.Nodes[0].Nodes[0])
            //        {
            //            textBox_OffLine.SelectionStart = 28 * 3;
            //            textBox_OffLine.SelectionLength = textBox_OffLine.Text.Length;
            //            textBox_OffLine.SelectionColor = Color.Red;
            //        }
            //    }
            //    else
            //    {
            //        switch (SubnodeText)
            //        {
            //            case "010. ":
            //                textBox_OffLine.SelectionStart = 28 * 3;
            //                textBox_OffLine.SelectionLength = 3;
            //                textBox_OffLine.SelectionColor = Color.Red;
            //                break;
            //            case "...0 ":
            //                textBox_OffLine.SelectionStart = 28 * 3;
            //                textBox_OffLine.SelectionLength = 3;
            //                textBox_OffLine.SelectionColor = Color.Red;
            //                break;
            //            case ".... ":
            //                textBox_OffLine.SelectionStart = 28 * 3;
            //                textBox_OffLine.SelectionLength = 3;
            //                textBox_OffLine.SelectionColor = Color.Red;
            //                break;
            //            case "Messa":
            //                textBox_OffLine.SelectionStart = 30 * 3;
            //                textBox_OffLine.SelectionLength = 6;
            //                textBox_OffLine.SelectionColor = Color.Red;
            //                break;
            //            case "Tunne":
            //                textBox_OffLine.SelectionStart = 32 * 3;
            //                textBox_OffLine.SelectionLength = 12;
            //                textBox_OffLine.SelectionColor = Color.Red;
            //                break;
            //            case "Seque":
            //                textBox_OffLine.SelectionStart = 36 * 3;
            //                textBox_OffLine.SelectionLength = 9;
            //                textBox_OffLine.SelectionColor = Color.Red;
            //                break;
            //            case "spare":
            //                textBox_OffLine.SelectionStart = 39 * 3;
            //                textBox_OffLine.SelectionLength = 3;
            //                textBox_OffLine.SelectionColor = Color.Red;
            //                break;
            //            default:
            //                break;
            //        }
            //        if (selectedNode == treeView_OffLine.Nodes[0].Nodes[1].Nodes[3])
            //        {
            //            textBox_OffLine.SelectionStart = 29 * 3;
            //            textBox_OffLine.SelectionLength = 3;
            //            textBox_OffLine.SelectionColor = Color.Red;
            //        }
            //        if (selectedNode == treeView_OffLine.Nodes[0].Nodes[0])
            //        {
            //            textBox_OffLine.SelectionStart = 28 * 3;
            //            textBox_OffLine.SelectionLength = textBox_OffLine.Text.Length;
            //            textBox_OffLine.SelectionColor = Color.Red;
            //        }
            //    }

            //}
            //#endregion
            #endregion
        }

        private void lTEMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool ControlSignal = false;
            for (int i = 0; i < tabControl1.TabCount; i++)
            {
                if (tabControl1.TabPages[i].Text == "LTE-M实时信令")
                {
                    ControlSignal = true;
                }
            }
            if (!ControlSignal)
            {
                this.tabControl1.TabPages.Add(tabPage1);
                this.tabControl1.SelectedTab = this.tabPage1;
            }
            if (tabControl1.TabPages.Count >= 0)
            {
                bool isExist = false;
                for (int i = 0; i < tabControl1.TabPages.Count; i++)
                {
                    if (tabControl1.TabPages[i] == tabPage1)
                    {
                        tabControl1.SelectedTab = tabPage1;
                        isExist = true;
                        break;
                    }
                }
                if (!isExist)
                {
                    tabControl1.TabPages.Add(tabPage1);
                    tabControl1.SelectedTab = tabPage1;
                }
            }
        }

        private void cBTCToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            bool ControlCBTC = false;
            for (int i = 0; i < tabControl1.TabCount; i++)
            {
                if (tabControl1.TabPages[i].Text == "CBTC实时业务")
                {
                    ControlCBTC = true;
                }
            }
            if (!ControlCBTC)
            {
                this.tabControl1.TabPages.Add(tabPage7);
                this.tabControl1.SelectedTab = this.tabPage7;
            }
            if (tabControl1.TabPages.Count >= 0)
            {
                bool isExist = false;
                for (int i = 0; i < tabControl1.TabPages.Count; i++)
                {
                    if (tabControl1.TabPages[i] == tabPage7)
                    {
                        tabControl1.SelectedTab = tabPage7;
                        isExist = true;
                        break;
                    }
                }
                if (!isExist)
                {
                    tabControl1.TabPages.Add(tabPage7);
                    tabControl1.SelectedTab = tabPage7;
                }
            }
        }

        private void gTPv2呼叫跟踪ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            tabpage_GTPv2 = true;
            bool GTPv2 = false;
            for (int i = 0; i < tabControl1.TabCount; i++)
            {
                if (tabControl1.TabPages[i].Text == "GTPv2呼叫跟踪")
                {
                    GTPv2 = true;
                }
            }
            if (!GTPv2 & OffLine)
            {
                this.tabControl1.TabPages.Add(tabPage3);
                this.tabControl1.SelectedTab = this.tabPage3;
            }
            if (tabControl1.TabPages.Count >= 0)
            {
                bool isExist = false;
                for (int i = 0; i < tabControl1.TabPages.Count; i++)
                {
                    if (tabControl1.TabPages[i] == tabPage3)
                    {
                        tabControl1.SelectedTab = tabPage3;
                        isExist = true;
                        break;
                    }
                }
                if (!isExist)
                {
                    tabControl1.TabPages.Add(tabPage3);
                    tabControl1.SelectedTab = tabPage3;
                }
            }
        }

        private void diameter窗口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabpage_DIAMETER = true;

            bool Diameter = false;
            for (int i = 0; i < tabControl1.TabCount; i++)
            {
                if (tabControl1.TabPages[i].Text == "DIAMETER呼叫跟踪")
                {
                    Diameter = true;
                }
            }
            if (!Diameter & OffLine)
            {
                this.tabControl1.TabPages.Add(tabPage4);
                this.tabControl1.SelectedTab = this.tabPage4;
            }
            if (tabControl1.TabPages.Count >= 0)
            {
                bool isExist = false;
                for (int i = 0; i < tabControl1.TabPages.Count; i++)
                {
                    if (tabControl1.TabPages[i] == tabPage4)
                    {
                        tabControl1.SelectedTab = tabPage4;
                        isExist = true;
                        break;
                    }
                }
                if (!isExist)
                {
                    tabControl1.TabPages.Add(tabPage4);
                    tabControl1.SelectedTab = tabPage4;
                }
            }
        }

        private void s1AP窗口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabpage_S1AP = true;
            bool S1ap = false;
            for (int i = 0; i < tabControl1.TabCount; i++)
            {
                if (tabControl1.TabPages[i].Text == "S1AP呼叫跟踪")
                {
                    S1ap = true;
                }
            }
            if (!S1ap & OffLine)
            {
                this.tabControl1.TabPages.Add(tabPage5);
                this.tabControl1.SelectedTab = this.tabPage5;
            }
            if (tabControl1.TabPages.Count >= 0)
            {
                bool isExist = false;
                for (int i = 0; i < tabControl1.TabPages.Count; i++)
                {
                    if (tabControl1.TabPages[i] == tabPage5)
                    {
                        tabControl1.SelectedTab = tabPage5;
                        isExist = true;
                        break;
                    }
                }
                if (!isExist)
                {
                    tabControl1.TabPages.Add(tabPage5);
                    tabControl1.SelectedTab = tabPage5;
                }
            }
        }

        private void cDR信令流程ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabpage_CDR = true;

            bool Xinling = false;
            for (int i = 0; i < tabControl1.TabCount; i++)
            {
                if (tabControl1.TabPages[i].Text == "信令合成流程")
                {
                    Xinling = true;
                }
            }
            if (!Xinling & OffLine)
            {
                this.tabControl1.TabPages.Add(tabPage6);
                this.tabControl1.SelectedTab = this.tabPage6;
            }
            if (tabControl1.TabPages.Count >= 0)
            {
                bool isExist = false;
                for (int i = 0; i < tabControl1.TabPages.Count; i++)
                {
                    if (tabControl1.TabPages[i] == tabPage6)
                    {
                        tabControl1.SelectedTab = tabPage6;
                        isExist = true;
                        break;
                    }
                }
                if (!isExist)
                {
                    tabControl1.TabPages.Add(tabPage6);
                    tabControl1.SelectedTab = tabPage6;
                }
            }
        }

        private void 离线消息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool Lixian = false;
            for (int i = 0; i < tabControl1.TabCount; i++)
            {
                if (tabControl1.TabPages[i].Text == "LTE-M离线消息")
                {
                    Lixian = true;
                }
            }
            if (!Lixian & OffLine)
            {
                this.tabControl1.TabPages.Add(tabPage8);
                this.tabControl1.SelectedTab = this.tabPage8;
            }
            if (tabControl1.TabPages.Count >= 0)
            {
                bool isExist = false;
                for (int i = 0; i < tabControl1.TabPages.Count; i++)
                {
                    if (tabControl1.TabPages[i] == tabPage8)
                    {
                        tabControl1.SelectedTab = tabPage8;
                        isExist = true;
                        break;
                    }
                }
                if (!isExist)
                {
                    tabControl1.TabPages.Add(tabPage8);
                    tabControl1.SelectedTab = tabPage8;
                }
            }
        }



        private void label1_LTE_Click(object sender, EventArgs e)
        {

        }

        private void label2_LTE_Click(object sender, EventArgs e)
        {

        }

        private void label1_GTPv2_Click(object sender, EventArgs e)
        {

        }

        private void label1_DIAMETER_Click(object sender, EventArgs e)
        {

        }

        private void label2_DIAMETER_Click(object sender, EventArgs e)
        {

        }

        private void label1_S1AP_Click(object sender, EventArgs e)
        {

        }

        private void label2_S1AP_Click(object sender, EventArgs e)
        {

        }

        private void label1_CDR_Click(object sender, EventArgs e)
        {

        }

        private void label2_CDR_Click(object sender, EventArgs e)
        {

        }

        private void label1_OffLine_Click(object sender, EventArgs e)
        {

        }

        private void label1_GTPv2_DoubleClick(object sender, EventArgs e)
        {
            Point Mx = label1_GTPv2.PointToClient(Control.MousePosition);
            int x = Mx.X;
            int y = Mx.Y;
            //计算关闭区域  
            Point p = label1_GTPv2.Location; // 获取或设置该控件的左上角相对于其容器的左上角的坐标。   
            int x1 = label1_GTPv2.Size.Width;
            int y1 = label1_GTPv2.Size.Height;
            //如果鼠标在区域内就关闭选项卡   
            bool isClose = x > p.X && x < p.X + x1 && y > p.Y && y < p.Y + y1;
            if (isClose == true)
            {
                if (splitContainer17.Panel2Collapsed) //&& splitContainer18.Panel2Collapsed)
                    splitContainer6.Panel2Collapsed = true;
                else
                {
                    splitContainer17.Panel1Collapsed = true;
                    //splitContainer18.Panel1Collapsed = true;
                }
            }
        }

        private void label1_DIAMETER_DoubleClick(object sender, EventArgs e)
        {

            Point Mx = label1_DIAMETER.PointToClient(Control.MousePosition);
            int x = Mx.X;
            int y = Mx.Y;
            //计算关闭区域  
            Point p = label1_DIAMETER.Location; // 获取或设置该控件的左上角相对于其容器的左上角的坐标。   
            int x1 = label1_DIAMETER.Size.Width;
            int y1 = label1_DIAMETER.Size.Height;
            //如果鼠标在区域内就关闭选项卡   
            bool isClose = x > p.X && x < p.X + x1 && y > p.Y && y < p.Y + y1;
            if (isClose == true)
            {
                if (splitContainer15.Panel2Collapsed)//&& splitContainer16.Panel2Collapsed)
                    splitContainer5.Panel2Collapsed = true;
                else
                {
                    splitContainer15.Panel1Collapsed = true;
                    //splitContainer16.Panel1Collapsed = true;
                }
            }
        }

        private void label2_DIAMETER_DoubleClick(object sender, EventArgs e)
        {
            Point Mx = label2_DIAMETER.PointToClient(Control.MousePosition);
            int x = Mx.X;
            int y = Mx.Y;
            //计算关闭区域  
            Point p = label2_DIAMETER.Location; // 获取或设置该控件的左上角相对于其容器的左上角的坐标。   
            int x1 = label2_DIAMETER.Size.Width;
            int y1 = label2_DIAMETER.Size.Height;
            //如果鼠标在区域内就关闭选项卡   
            bool isClose = x > p.X && x < p.X + x1 && y > p.Y && y < p.Y + y1;
            if (isClose == true)
            {
                if (splitContainer15.Panel1Collapsed)//&& splitContainer16.Panel1Collapsed)
                    splitContainer5.Panel2Collapsed = true;
                else
                {
                    splitContainer15.Panel2Collapsed = true;
                    //splitContainer16.Panel2Collapsed = true;
                }
            }
        }

        private void label1_S1AP_DoubleClick(object sender, EventArgs e)
        {
            Point Mx = label1_S1AP.PointToClient(Control.MousePosition);
            int x = Mx.X;
            int y = Mx.Y;
            //计算关闭区域  
            Point p = label1_S1AP.Location; // 获取或设置该控件的左上角相对于其容器的左上角的坐标。   
            int x1 = label1_S1AP.Size.Width;
            int y1 = label1_S1AP.Size.Height;
            //如果鼠标在区域内就关闭选项卡   
            bool isClose = x > p.X && x < p.X + x1 && y > p.Y && y < p.Y + y1;
            if (isClose == true)
            {
                if (splitContainer13.Panel2Collapsed) //&& splitContainer14.Panel2Collapsed)
                    splitContainer7.Panel2Collapsed = true;
                else
                {
                    splitContainer13.Panel1Collapsed = true;
                    //splitContainer14.Panel1Collapsed = true;
                }
            }
        }

        private void label2_S1AP_DoubleClick(object sender, EventArgs e)
        {
            Point Mx = label2_S1AP.PointToClient(Control.MousePosition);
            int x = Mx.X;
            int y = Mx.Y;
            //计算关闭区域  
            Point p = label2_S1AP.Location; // 获取或设置该控件的左上角相对于其容器的左上角的坐标。   
            int x1 = label2_S1AP.Size.Width;
            int y1 = label2_S1AP.Size.Height;
            //如果鼠标在区域内就关闭选项卡   
            bool isClose = x > p.X && x < p.X + x1 && y > p.Y && y < p.Y + y1;
            if (isClose == true)
            {
                if (splitContainer13.Panel1Collapsed)//&& splitContainer14.Panel1Collapsed)
                    splitContainer7.Panel2Collapsed = true;
                else
                {
                    splitContainer13.Panel2Collapsed = true;
                    //splitContainer14.Panel2Collapsed = true;
                }
            }
        }

        private void label1_OffLine_DoubleClick(object sender, EventArgs e)
        {


            Point Mx = label1_OffLine.PointToClient(Control.MousePosition);
            int x = Mx.X;
            int y = Mx.Y;
            //计算关闭区域  
            Point p = label1_OffLine.Location; // 获取或设置该控件的左上角相对于其容器的左上角的坐标。   
            int x1 = label1_OffLine.Size.Width;
            int y1 = label1_OffLine.Size.Height;
            //如果鼠标在区域内就关闭选项卡   
            bool isClose = x > p.X && x < p.X + x1 && y > p.Y && y < p.Y + y1;
            if (isClose == true)
            {
                if (splitContainer9.Panel2Collapsed) //&& splitContainer14.Panel2Collapsed)
                    splitContainer8.Panel2Collapsed = true;
                else
                {
                    splitContainer9.Panel1Collapsed = true;
                    //splitContainer14.Panel1Collapsed = true;
                }
            }
        }

        private void label2_OffLine_DoubleClick(object sender, EventArgs e)
        {
            Point Mx = label2_OffLine.PointToClient(Control.MousePosition);
            int x = Mx.X;
            int y = Mx.Y;
            //计算关闭区域  
            Point p = label2_OffLine.Location; // 获取或设置该控件的左上角相对于其容器的左上角的坐标。   
            int x1 = label2_OffLine.Size.Width;
            int y1 = label2_OffLine.Size.Height;

            //如果鼠标在区域内就关闭选项卡   
            bool isClose = x > p.X && x < p.X + x1 && y > p.Y && y < p.Y + y1;
            if (isClose == true)
            {
                if (splitContainer9.Panel1Collapsed)//&& //splitContainer10.Panel1Collapsed)
                    splitContainer8.Panel2Collapsed = true;
                else
                {
                    splitContainer9.Panel2Collapsed = true;
                    //splitContainer10.Panel2Collapsed = true;
                }

            }
        }

        private void label1_LTE_DoubleClick(object sender, EventArgs e)
        {
            // Point Mx = splitContainer4.Panel1.PointToClient(Control.MousePosition);//计算鼠标相对于窗体/控件的位置
            Point Mx = label1_LTE.PointToClient(Control.MousePosition);
            int x = Mx.X;
            int y = Mx.Y;
            //计算关闭区域  
            Point p = label1_LTE.Location; // 获取或设置该控件的左上角相对于其容器的左上角的坐标。   
            int x1 = label1_LTE.Size.Width;
            int y1 = label1_LTE.Size.Height;
            //如果鼠标在区域内就关闭选项卡   
            bool isClose = x > p.X && x < p.X + x1 && y > p.Y && y < p.Y + y1;
            if (isClose == true)
            {
                //if (splitContainer3.Panel2Collapsed && splitContainer4.Panel2Collapsed)
                if (splitContainer4.Panel2Collapsed)
                    splitContainer2.Panel2Collapsed = true;
                else
                {
                    //splitContainer3.Panel1Collapsed = true;
                    splitContainer4.Panel1Collapsed = true;
                }
            }
        }



        private void label2_LTE_DoubleClick(object sender, EventArgs e)
        {
            //Point Mx = splitContainer4.Panel2.PointToClient(Control.MousePosition);//计算鼠标相对于窗体/控件的位置
            Point Mx = label2_LTE.PointToClient(Control.MousePosition);
            int x = Mx.X;
            int y = Mx.Y;
            //计算关闭区域  
            Point p = label2_LTE.Location; // 获取或设置该控件的左上角相对于其容器的左上角的坐标。   
            int x1 = label2_LTE.Size.Width;
            int y1 = label2_LTE.Size.Height;
            //如果鼠标在区域内就关闭选项卡   
            bool isClose = x > p.X && x < p.X + x1 && y > p.Y && y < p.Y + y1;
            if (isClose == true)
            {
                //if (splitContainer3.Panel1Collapsed && splitContainer4.Panel1Collapsed)
                if (splitContainer4.Panel1Collapsed)
                    splitContainer2.Panel2Collapsed = true;
                else
                {
                    //splitContainer3.Panel2Collapsed = true;
                    splitContainer4.Panel2Collapsed = true;
                }

            }
        }

        private void tabControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && tabControl1.SelectedTab == tabPage1)
            {
                Buzhuoxiaoxi.Show(MousePosition);
            }
            if (e.Button == MouseButtons.Right && tabControl1.SelectedTab == tabPage3)
            {
                GTPv2.Show(MousePosition);
            }
            if (e.Button == MouseButtons.Right && tabControl1.SelectedTab == tabPage4)
            {
                Diameter.Show(MousePosition);
            }
            if (e.Button == MouseButtons.Right && tabControl1.SelectedTab == tabPage5)
            {
                S1AP.Show(MousePosition);
            }
            if (e.Button == MouseButtons.Right && tabControl1.SelectedTab == tabPage6)
            {
                XinlingFlow.Show(MousePosition);
            }
            if (e.Button == MouseButtons.Right && tabControl1.SelectedTab == tabPage7)
            {
                contextM_CBTC.Show(MousePosition);
            }
            if (e.Button == MouseButtons.Right && tabControl1.SelectedTab == tabPage8)
            {
                contextM_OffLine.Show(MousePosition);
            }
        }
        private System.Diagnostics.Process p;
        private void 说明文档ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            p = System.Diagnostics.Process.Start(Application.StartupPath.ToString() + "\\HELP\\help.chm");

        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == MessageBox.Show("你确定要关闭应用程序吗？", "关闭提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1))
            {
                this.FormClosing -= new FormClosingEventHandler(this.Form1_FormClosing);//为保证Application.Exit();时不再弹出提示，所以将FormClosing事件取消
                Application.Exit();//退出整个应用程序
                if (p != null && !p.HasExited)
                {
                    p.Kill();
                }
            }
            //else
            //{
            //    e.Cancel = true;  //取消关闭事件
            //}
        }

        private void tabControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < tabControl1.TabPages.Count; i++)
                {
                    TabPage tp = tabControl1.TabPages[i];
                    if (tabControl1.GetTabRect(i).Contains(new Point(e.X, e.Y)))
                    {
                        tabControl1.SelectedTab = tp;
                        break;
                    }
                }
            }
        }



        private void toolStripButton_CBTC_Click(object sender, EventArgs e)
        {
            bool ControlCBTC = false;
            for (int i = 0; i < tabControl1.TabCount; i++)
            {
                if (tabControl1.TabPages[i].Text == "CBTC实时业务")
                {
                    ControlCBTC = true;
                }
            }
            if (!ControlCBTC)
            {
                this.tabControl1.TabPages.Add(tabPage7);
                this.tabControl1.SelectedTab = this.tabPage7;
            }
            if (tabControl1.TabPages.Count >= 0)
            {
                bool isExist = false;
                for (int i = 0; i < tabControl1.TabPages.Count; i++)
                {
                    if (tabControl1.TabPages[i] == tabPage7)
                    {
                        tabControl1.SelectedTab = tabPage7;
                        isExist = true;
                        break;
                    }
                }
                if (!isExist)
                {
                    tabControl1.TabPages.Add(tabPage7);
                    tabControl1.SelectedTab = tabPage7;
                }
            }
        }

        private void toolStripButton_LTE_Click(object sender, EventArgs e)
        {
            bool ControlSignal = false;
            for (int i = 0; i < tabControl1.TabCount; i++)
            {
                if (tabControl1.TabPages[i].Text == "LTE-M实时信令")
                {
                    ControlSignal = true;
                }
            }
            if (!ControlSignal)
            {
                this.tabControl1.TabPages.Add(tabPage1);
                this.tabControl1.SelectedTab = this.tabPage1;
            }
            if (tabControl1.TabPages.Count >= 0)
            {
                bool isExist = false;
                for (int i = 0; i < tabControl1.TabPages.Count; i++)
                {
                    if (tabControl1.TabPages[i] == tabPage1)
                    {
                        tabControl1.SelectedTab = tabPage1;
                        isExist = true;
                        break;
                    }
                }
                if (!isExist)
                {
                    tabControl1.TabPages.Add(tabPage1);
                    tabControl1.SelectedTab = tabPage1;
                }
            }
        }

        private void 关于软件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutSoftFrom = new AboutSoftware();
            AboutSoftFrom.Show();
        }







        private void button3_Click(object sender, EventArgs e)
        {
            //界面显示处理
            if (ActivePanel.Name == panel6.Name)
                return;
            ActivePanel = panel6;

            button3.SendToBack();
            button3.Dock = DockStyle.Top;

            button5.SendToBack();
            button5.Dock = DockStyle.Bottom;
            button2.SendToBack();
            button2.Dock = DockStyle.Bottom;
            button1.SendToBack();
            button1.Dock = DockStyle.Bottom;



            panel3.SendToBack();
            panel4.SendToBack();
            panel7.SendToBack();    //放到下面


            panel6.Visible = true;
            panel3.Visible = false;
            panel4.Visible = false;
            panel7.Visible = false;

            panel6.BringToFront();   //放到前面
            panel6.Dock = DockStyle.Fill;

            PanelReSize(this, e);

        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (ActivePanel.Name == panel7.Name)
                return;
            ActivePanel = panel7;


            button2.SendToBack();
            button2.Dock = DockStyle.Top;

            button5.SendToBack();
            button5.Dock = DockStyle.Top;
            button3.SendToBack();
            button3.Dock = DockStyle.Top;


            button1.SendToBack();
            button1.Dock = DockStyle.Bottom;


            panel7.SendToBack();
            panel6.SendToBack();
            panel3.SendToBack();
            panel4.SendToBack();


            panel7.Visible = true;
            panel6.Visible = false;
            // panel4.Visible = false;
            // panel3.Visible = false;

            panel7.BringToFront();
            panel7.Dock = DockStyle.Fill;

            PanelReSize(this, e);
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            //界面显示处理
            if (ActivePanel.Name == panel4.Name)
                return;
            ActivePanel = panel4;

            button5.SendToBack();
            button5.Dock = DockStyle.Top;


            button3.SendToBack();
            button3.Dock = DockStyle.Top;


            button2.SendToBack();
            button2.Dock = DockStyle.Bottom;
            button1.SendToBack();
            button1.Dock = DockStyle.Bottom;


            panel6.SendToBack();
            panel7.SendToBack();
            panel3.SendToBack();

            panel4.Visible = true;
            panel7.Visible = false;


            panel4.BringToFront();
            panel4.Dock = DockStyle.Fill;

            PanelReSize(this, e);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //界面显示处理
            if (ActivePanel.Name == panel3.Name)
                return;
            ActivePanel = panel3;

            button1.SendToBack();
            button1.Dock = DockStyle.Top;
            button2.SendToBack();
            button2.Dock = DockStyle.Top;
            button5.SendToBack();
            button5.Dock = DockStyle.Top;

            button3.SendToBack();
            button3.Dock = DockStyle.Top;

            panel6.SendToBack();
            panel7.SendToBack();
            panel4.SendToBack();

            panel3.Visible = true;
            //panel6.Visible = false;
            //panel7.Visible = false;
            panel4.Visible = false;


            panel3.BringToFront();
            panel3.Dock = DockStyle.Fill;

            PanelReSize(this, e);
        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button_GTPv2_Click(object sender, EventArgs e)
        {
            if (changePcap_gtpv2)//黄罡
            {
                if (!tabpage_GTPv2)
                {
                    this.tabControl1.TabPages.Add(tabPage3);
                    this.tabControl1.SelectedTab = this.tabPage3;
                }

                listView_GTPv2.Items.Clear();//清除上次报文在listview_GTPv2上的显示

                listView_GTPv2.BackColor = Color.SkyBlue;
                #region  GTPCDR合成
                #region 集合清空
                GTPCDR.Clear();
                GTPsubCDR.Clear();
                subCDR_Copy.Clear();
                subCDR.Clear();
                sortedTEID.Clear();
                nullTEID.Clear();
                TEIDExistClone.Clear();
                TEIDExist.Clear();
                sortedIP.Clear();
                gtpChunkCopy1.Clear();
                gtpChunkCopy2.Clear();
                #endregion
                List<gtpStruct> TeidZore = new List<gtpStruct>();
                subCDR_Copy.Clear();
                foreach (gtpStruct a in gtpChunk1)
                {
                    gtpChunkCopy1.Add(a);
                }
                foreach (gtpStruct a in gtpChunk1)
                {
                    gtpChunkCopy2.Add(a);
                }

                Sort_Ip.FindIpPair(gtpChunkCopy2, gtpChunkCopy1, sortedIP);
                for (int i = 0; i < sortedIP.Count; i++)
                {
                    TEIDExist.Clear();
                    nullTEID.Clear();
                    TEIDExist.Clear();
                    TEIDExistClone.Clear();
                    sortedTEID.Clear();
                    Sort_TEID.Exist_sort(sortedIP[i], TEIDExist, nullTEID);
                    foreach (gtpStruct l in TEIDExist)
                    {
                        TEIDExistClone.Add(l);
                    }
                    Sort_TEID.Sort(TEIDExist, TEIDExistClone, sortedTEID);
                    foreach (List<gtpStruct> a in sortedTEID)
                    {
                        int TEID;
                        TEID = a[0].gtpFrame[32] + a[0].gtpFrame[33] + a[0].gtpFrame[34] + a[0].gtpFrame[35];    //第一位是时间，从33开始
                        if (TEID == 0)
                        {
                            sortedTEID.Remove(a);
                            foreach (gtpStruct b in a)
                            {
                                TeidZore.Add(b);
                            }
                            break;
                        }
                    }
                    List<List<gtpStruct>> sortedTEID_Copy = new List<List<gtpStruct>>();
                    foreach (List<gtpStruct> a in sortedTEID)
                    {
                        sortedTEID_Copy.Add(a);
                    }
                    subCDR = SubCDR.CDRFunction(sortedTEID_Copy);
                    for (int k = 0; k < TeidZore.Count; k++)
                    {
                        String sn1 = Convert.ToString(TeidZore[k].gtpFrame[36]) + Convert.ToString(TeidZore[k].gtpFrame[37]) + Convert.ToString(TeidZore[k].gtpFrame[38]);  //第一位是时间，从36开始
                        foreach (List<gtpStruct> a in subCDR)
                        {
                            int flag = 0;
                            foreach (gtpStruct c in a)
                            {
                                String sn2 = Convert.ToString(c.gtpFrame[36]) + Convert.ToString(c.gtpFrame[37]) + Convert.ToString(c.gtpFrame[38]);  //第一位是时间，从36开始
                                if (sn1 == sn2)
                                {
                                    a.Add(TeidZore[k]);
                                    flag = 1;
                                    break;
                                }
                            }
                            if (flag == 1)
                            {
                                break;
                            }

                        }
                    }
                    foreach (List<gtpStruct> a in subCDR)
                    {
                        subCDR_Copy.Add(a);
                    }
                    List<gtpStruct> nullTEID_Copy = new List<gtpStruct>();
                    foreach (gtpStruct a in nullTEID)
                    {
                        nullTEID_Copy.Add(a);
                    }
                    if (nullTEID_Copy.Count > 0)
                    {
                        subCDR_Copy.Add(nullTEID_Copy);

                    }
                }
                //按照时间排序

                for (int i = 0; i < subCDR_Copy.Count; i++)
                {
                    //  Dictionary<double, gtpStruct> dic = new Dictionary<double, gtpStruct>();
                    Dictionary<int, gtpStruct> dic = new Dictionary<int, gtpStruct>();
                    //double[] timeList = new double[subCDR_Copy[i].Count];
                    int[] timeList = new int[subCDR_Copy[i].Count];
                    for (int k = 0; k < subCDR_Copy[i].Count; k++)
                    {
                        //dic.Add(subCDR_Copy[i][k].time, subCDR_Copy[i][k]);
                        // timeList[k] = subCDR_Copy[i][k].time;
                        dic.Add(subCDR_Copy[i][k].ID, subCDR_Copy[i][k]);
                        timeList[k] = subCDR_Copy[i][k].ID;

                    }
                    Array.Sort(timeList);  //从小到大
                    List<gtpStruct> temp = new List<gtpStruct>();
                    for (int n = 0; n < timeList.Length; n++)
                    {
                        temp.Add(dic[timeList[n]]);
                    }
                    GTPsubCDR.Add(temp);
                }
                for (int j = 0; j < GTPsubCDR.Count; j++)
                {
                    string SorIp, DesIp, IMSI = "";
                    gtpCDRstruct GTP;
                    GTP.CDRbuffer = GTPsubCDR[j];
                    GTP.CDRIMSI = "";
                    DesIp = Convert.ToString(GTPsubCDR[j][0].gtpFrame[16]) + "." + Convert.ToString(GTPsubCDR[j][0].gtpFrame[17]) + "." + Convert.ToString(GTPsubCDR[j][0].gtpFrame[18]) + "." + Convert.ToString(GTPsubCDR[j][0].gtpFrame[19]);
                    SorIp = Convert.ToString(GTPsubCDR[j][0].gtpFrame[12]) + "." + Convert.ToString(GTPsubCDR[j][0].gtpFrame[13]) + "." + Convert.ToString(GTPsubCDR[j][0].gtpFrame[14]) + "." + Convert.ToString(GTPsubCDR[j][0].gtpFrame[15]);  //第一位是时间，从13开始
                    gtpv2 liu = new gtpv2();
                    for (int k = 0; k < GTPsubCDR[j].Count; k++)            //第一位是时间，减29
                    {
                        byte[] GTPv2 = new byte[GTPsubCDR[j][k].gtpFrame.Length - 28];
                        for (int i = 0; i < GTPsubCDR[j][k].gtpFrame.Length - 28; i++)
                        {
                            GTPv2[i] = GTPsubCDR[j][k].gtpFrame[28 + i];
                        }
                        liu.decoder(GTPv2);
                        if (liu.IMSI != "")
                        {
                            IMSI = liu.IMSI;
                            GTP.CDRIMSI = IMSI;
                            //break;
                        }
                        liu.IMSI = "";                      //一个CDR里面解出了多个IMSI，CDR合成有问题啊！ 每个TEID下序列号都是0，怎么办？
                        //结构体是值类型，只能这样处理？
                        gtpStruct g = GTPsubCDR[j][k];
                        g.messageType = liu.mt;
                        bool isS10 = false;
                        for (int i = 0; i < S10Message.s10.Length; i++)
                        {
                            if (S10Message.s10[i] == g.messageType)
                            {
                                isS10 = true;
                                break;
                            }
                        }
                        if (isS10)
                        {
                            g.faceType = "S10";
                        }
                        else
                        {
                            if (liu.mesType > 4)
                            {
                                g.faceType = "S5";
                            }
                        }
                        GTPsubCDR[j][k] = g;
                    }
                    GTPCDR.Add(GTP);
                #endregion
                    listView_GTPv2.Items.Add(new ListViewItem(new string[] { (j + 1).ToString(), GTPsubCDR[j][0].timeAll, GTPsubCDR[j][GTPsubCDR[j].Count - 1].timeAll, SorIp, DesIp, IMSI }));
                }

                for (int i = 0; i < listView_GTPv2.Items.Count; i++)
                {
                    listView_GTPv2.Items[i].BackColor = System.Drawing.Color.LightGoldenrodYellow;

                }
            }
            changePcap_gtpv2 = false;//黄罡
            //button_GTPv2.Enabled = false;
            if (tabControl1.TabPages.Count >= 0)
            {
                bool isExist = false;
                for (int i = 0; i < tabControl1.TabPages.Count; i++)
                {
                    if (tabControl1.TabPages[i] == tabPage3)
                    {
                        tabControl1.SelectedTab = tabPage3;
                        isExist = true;
                        break;
                    }
                }
                if (!isExist)
                {
                    tabControl1.TabPages.Add(tabPage3);
                    tabControl1.SelectedTab = tabPage3;
                }
            }
        }

        private void button_S1AP_Click_1(object sender, EventArgs e)//CDR合成程序
        {
            if (changePcap_s1ap)
            {
                //添加一个tabPage，用于显示CDR
                if (!tabpage_S1AP)
                {
                    this.tabControl1.TabPages.Add(tabPage5);
                    this.tabControl1.SelectedTab = this.tabPage5;
                }

                //清空上一次的结果
                listView_S1AP.Items.Clear();
                List<CdrIdentity> CDR1 = new List<CdrIdentity>();//存储一个
                string ID1 = "0";
                string STime = string.Empty;// "";
                string Endtime = string.Empty;//"";
                string eNB = string.Empty;//"";
                string MME = string.Empty;//"";
                string IMSI = string.Empty;// "";
                string TMSI = string.Empty;// "";           
                listView_S1AP.BackColor = Color.SkyBlue;
                tmsi.Clear();

                //合成CDR
                Process.CdrCreate(S1apKey6All, S1apContentAll);
                CdrList = Process.CdrList;//CdrList的单元是一个CDR中得信令的所有信息（CdrList存放一个离线文件中的所有CDR）
                CdrIdpair = Process.CdrId;//CdrIdpair的单元是一个CDR的ID-pair （flow1存放一个离线文件中的所有CDR对应的ID-pair ） 
                //提取出每个CDR信令里第一个和最后一个用户标识（不管是tmsi，还是IMSI），作为CDR的特征值，用于后续的多段关联
                for (int j = 0; j < CdrList.Count; j++)
                {
                    #region
                    CdrIdentity CdrIdentitySingle = new CdrIdentity();//存储一个cdr中所有的用户标识
                    List<string> ImsiOrTmsi = new List<string>();//将cdr中的用户标识传给CDR（key4），且在一个cdr中最多有两个标识
                    IMSI = "";
                    TMSI = "";
                    tmsi.Clear();
                    STime = CdrList[j][0].time1;
                    Endtime = CdrList[j][CdrList[j].Count - 1].time1;
                    eNB = CdrIdpair[j].EnbUeS1apId.ToString();
                    MME = CdrIdpair[j].MmeUeS1apId.ToString();
                    CdrIdentitySingle.mmeUeS1apId = MME;
                    CdrIdentitySingle.enbUeS1apId = eNB;
                    ID1 = Convert.ToString(j);
                    for (int k = 0; k < CdrList[j].Count; k++)
                    {
                        //提取s1ap信令中的用户标识
                        if (CdrList[j][k].s1ap_identity != "")
                        {
                            if (CdrList[j][k].s1ap_identity.Length > 8)//用户标识长度大于8字节，则是IMSI；否则是tmsi
                                IMSI = CdrList[j][k].s1ap_identity;
                            else
                                tmsi.Add(CdrList[j][k].s1ap_identity);
                            ImsiOrTmsi.Add(CdrList[j][k].s1ap_identity);
                        }
                        //提取nas信令中的用户标识
                        if (CdrList[j][k].nas_identity != "")
                        {
                            if (CdrList[j][k].nas_identity.Length > 8)
                                IMSI = CdrList[j][k].nas_identity;
                            else
                                tmsi.Add(CdrList[j][k].nas_identity);
                            ImsiOrTmsi.Add(CdrList[j][k].nas_identity);
                        }
                    }
                    //CDR中有用户标识
                    if (ImsiOrTmsi.Count > 0)
                    {
                        CdrIdentitySingle.identity1 = ImsiOrTmsi[0];//CDR中第一个用户标识作为identity1
                        CdrIdentitySingle.identity2 = ImsiOrTmsi[ImsiOrTmsi.Count - 1];//CDR中最后一个用户标识作为identity2
                    }
                    else
                    {
                        CdrIdentitySingle.identity1 = "";
                        CdrIdentitySingle.identity2 = "";
                    }
                    CDR1.Add(CdrIdentitySingle);
                    if (tmsi.Count > 0)//若一个CDR中有多个tmsi,则最后一个是最新的tmsi
                        TMSI = tmsi[tmsi.Count - 1];
                    else
                        TMSI = "";
                    listView_S1AP.Items.Add(new ListViewItem(new string[] { ID1, STime, Endtime, eNB, MME, IMSI, TMSI }));

                    #endregion
                }
                for (int i = 0; i < CdrList.Count; i++)
                    listView_S1AP.Items[i].BackColor = System.Drawing.Color.LightGoldenrodYellow;
            }
            changePcap_s1ap = false;

            //button_S1AP.Enabled = false;
            if (tabControl1.TabPages.Count >= 0)
            {
                bool isExist = false;
                for (int i = 0; i < tabControl1.TabPages.Count; i++)
                {
                    if (tabControl1.TabPages[i] == tabPage5)
                    {
                        tabControl1.SelectedTab = tabPage5;
                        isExist = true;
                        break;
                    }
                }
                if (!isExist)
                {
                    tabControl1.TabPages.Add(tabPage5);
                    tabControl1.SelectedTab = tabPage5;
                }
            }

        }

        private void button_OffLine_Click_1(object sender, EventArgs e)
        {
            if (tabControl1.TabPages.Count >= 0)
            {
                bool isExist = false;
                for (int i = 0; i < tabControl1.TabPages.Count; i++)
                {
                    if (tabControl1.TabPages[i] == tabPage8)
                    {
                        tabControl1.SelectedTab = tabPage8;
                        isExist = true;
                        break;
                    }
                }
                if (!isExist)
                {
                    tabControl1.TabPages.Add(tabPage8);
                    tabControl1.SelectedTab = tabPage8;
                }
            }
        }

        private void lTEM离线消息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool LieKong = false;
            for (int i = 0; i < tabControl1.TabCount; i++)
            {
                if (tabControl1.TabPages[i].Text == "LTE-M离线消息")
                {
                    LieKong = true;
                }
            }
            if (!LieKong & OffLine)
            {
                this.tabControl1.TabPages.Add(tabPage8);
                this.tabControl1.SelectedTab = this.tabPage8;
            }
            if (tabControl1.TabPages.Count >= 0)
            {
                bool isExist = false;
                for (int i = 0; i < tabControl1.TabPages.Count; i++)
                {
                    if (tabControl1.TabPages[i] == tabPage8)
                    {
                        tabControl1.SelectedTab = tabPage8;
                        isExist = true;
                        break;
                    }
                }
                if (!isExist)
                {
                    tabControl1.TabPages.Add(tabPage8);
                    tabControl1.SelectedTab = tabPage8;
                }
            }
        }

        private void 列控信息分析ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        TreeNode selectedNode = null;//用于表示离线操作下选择的treeview的节点
        string parentOffNode = "";
        private void treeView_OffLine_MouseClick(object sender, MouseEventArgs e)
        {

            if ((sender as TreeView) != null)
            {
                treeView_OffLine.SelectedNode = treeView_OffLine.GetNodeAt(e.X, e.Y);
                selectedNode = treeView_OffLine.SelectedNode;
            }
            if (selectedNode.Parent != null)//获取所选节点的父节点
                parentOffNode = selectedNode.Parent.Text;
            System.Timers.Timer t = new System.Timers.Timer(10);
            t.Elapsed += new System.Timers.ElapsedEventHandler(SelectedOffLine);
            t.AutoReset = false;
            t.Enabled = true;
        }
        private void SelectedOffLine(object source, System.Timers.ElapsedEventArgs e)
        {
            int indexIE = 0;
            int LengthIE = 0;
            int No = -1;
            this.Invoke((EventHandler)(delegate
            {
                #region                            //点击s1ap节点时凸显对应的编码
                if (OfflineStruct.Count > 0)
                {
                    if (selectedNode.Text == OfflineStruct[0].Name)
                    {
                        indexIE = OfflineStruct[0].IndexName * 3;
                        LengthIE = OfflineStruct[0].LengthName * 3 - 1;
                        textBox_OffLine.Focus();
                        this.textBox_OffLine.Select(indexIE, LengthIE);
                    }
                    else if ((OfflineStruct.Count > 1) && (selectedNode.Text == OfflineStruct[1].Name))
                    {
                        indexIE = OfflineStruct[1].IndexName * 3;
                        LengthIE = OfflineStruct[1].LengthName * 3 - 1;
                        textBox_OffLine.Focus();
                        this.textBox_OffLine.Select(indexIE, LengthIE);
                    }
                    else
                    {
                        #region
                        if (parentOffNode == OfflineStruct[0].Name)
                            No = 0;
                        else
                            No = 1;
                        if (selectedNode.Text == "Item" + OfflineStruct[No].NoMme + ":id-MME-UE-S1AP-ID" + "(" + OfflineStruct[No].mme + ")")
                        {
                            indexIE = OfflineStruct[No].indexMme * 3;
                            LengthIE = OfflineStruct[No].LengthMme * 3 - 1;
                            textBox_OffLine.Focus();
                            this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                        else if (selectedNode.Text == "Item" + OfflineStruct[No].NoEnb + ":id-eNB-UE-S1AP-ID" + "(" + OfflineStruct[No].enb + ")")
                        {
                            indexIE = OfflineStruct[No].IndexEnb * 3;
                            LengthIE = OfflineStruct[No].LengthEnb * 3 - 1;
                            textBox_OffLine.Focus();
                            this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                        else if (selectedNode.Text == "Item" + OfflineStruct[No].NoCause + ":id-Cause:" + OfflineStruct[No].CauseKind + ":" + OfflineStruct[No].cause)
                        {
                            indexIE = OfflineStruct[No].IndexCause * 3;
                            LengthIE = OfflineStruct[No].LengthCause * 3 - 1;
                            textBox_OffLine.Focus();
                            this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                        else if (selectedNode.Text == "Item" + OfflineStruct[No].NoIdentity + ":id-IMSI:" + OfflineStruct[No].Identity)
                        {
                            indexIE = OfflineStruct[No].IndexIdentity * 3;
                            LengthIE = OfflineStruct[No].LengthIdentity * 3 - 1;
                            textBox_OffLine.Focus();
                            this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                        else if (selectedNode.Text == "Item" + OfflineStruct[No].NoIdentity + ":id-TMSI:" + OfflineStruct[No].Identity)
                        {
                            indexIE = OfflineStruct[No].IndexIdentity * 3;
                            LengthIE = OfflineStruct[No].LengthIdentity * 3 - 1;
                            textBox_OffLine.Focus();
                            this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                        else if (selectedNode.Text == "Item" + OfflineStruct[No].NoEmm + ":id-Nas:" + OfflineStruct[No].EmmName)
                        {
                            indexIE = OfflineStruct[No].IndexEmm * 3;
                            if (OfflineStruct[No].LengthEmm > 0)
                                LengthIE = OfflineStruct[No].LengthEmm * 3 - 1;
                            else
                                LengthIE = OfflineStruct[No].LengthEmm;
                            textBox_OffLine.Focus();
                            this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                        else if (selectedNode.Text == "Item" + OfflineStruct[No].NoNasIdentity + ":id-IMSI:" + OfflineStruct[No].NasIdentity)
                        {
                            indexIE = OfflineStruct[No].IndexNasId * 3;
                            LengthIE = OfflineStruct[No].LengthNasId * 3 - 1;
                            textBox_OffLine.Focus();
                            this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                        else if (selectedNode.Text == "Item" + OfflineStruct[No].NoNasIdentity + ":id-TMSI:" + OfflineStruct[No].NasIdentity)
                        {
                            indexIE = OfflineStruct[No].IndexNasId * 3;
                            LengthIE = OfflineStruct[No].LengthNasId * 3 - 1;
                            textBox_OffLine.Focus();
                            this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                        else if (selectedNode.Text == "Item" + OfflineStruct[No].NoNasCause + ":id-Nas Cause:" + OfflineStruct[No].NasCause)
                        {
                            indexIE = OfflineStruct[No].IndexNasCause * 3;
                            LengthIE = OfflineStruct[No].LengthNasCause * 3 - 1;
                            textBox_OffLine.Focus();
                            this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                        else if (selectedNode.Text == "Item" + OfflineStruct[No].NoUeIp + ":PDN address:" + OfflineStruct[No].UeIP)
                        {
                            indexIE = OfflineStruct[No].IndexUeIp * 3;
                            LengthIE = OfflineStruct[No].LengthUeIp * 3 - 1;
                            textBox_OffLine.Focus();
                            this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                        #endregion
                    }
                }
                #endregion

                #region 点击diameter节点时对应凸显其对应的数据流
                if (protocolSelected_Offline == "DIAMETER-SCTP" || protocolSelected_Offline == "DIAMETER-TCP")
                {
                    string nodeText = selectedNode.Text;//点击行的文本
                    switch (nodeText)
                    {
                        case "SCTP Protocol":
                            indexIE = 20 * 3;
                            LengthIE = diameterLocationOffline * 3 - 1;
                            textBox_OffLine.Focus();
                            this.textBox_OffLine.Select(indexIE, LengthIE);
                            break;
                        case "TCP Protocol":
                            indexIE = 20 * 3;
                            LengthIE = diameterLocationOffline * 3 - 1;
                            textBox_OffLine.Focus();
                            this.textBox_OffLine.Select(indexIE, LengthIE);
                            break;
                        case "diameter":
                            indexIE = (20 + diameterLocationOffline) * 3;
                            LengthIE = diameterLengthOffline * 3 - 1;
                            textBox_OffLine.Focus();
                            this.textBox_OffLine.Select(indexIE, LengthIE);
                            break;
                        case "Version:1":
                            indexIE = (20 + diameterLocationOffline) * 3;
                            LengthIE = 1 * 3 - 1;
                            textBox_OffLine.Focus();
                            this.textBox_OffLine.Select(indexIE, LengthIE);
                            break;
                        case "User-Name AVP":
                            indexIE = (20 + diameterLocationOffline + avpLocation_UserNameOffline) * 3;
                            LengthIE = avpLength_UserNameOffline * 3 - 1;
                            textBox_OffLine.Focus();
                            this.textBox_OffLine.Select(indexIE, LengthIE);
                            break;
                        case "Result-CODE AVP":
                            indexIE = (20 + diameterLocationOffline + avpLocation_ResultOffline) * 3;
                            LengthIE = avpLength_ResultOffline * 3 - 1;
                            textBox_OffLine.Focus();
                            this.textBox_OffLine.Select(indexIE, LengthIE);
                            break;
                        case "Authentication-Info AVP":

                            indexIE = (20 + diameterLocationOffline + avpLocation_AuthenticationOffline) * 3;
                            LengthIE = avpLength_AuthenticationOffline * 3 - 1;
                            textBox_OffLine.Focus();
                            this.textBox_OffLine.Select(indexIE, LengthIE);
                            break;
                        case "E-UTRAN-Vector1 AVP":
                            indexIE = (20 + diameterLocationOffline + avpLocation_EUTRANVectorOffline[0]) * 3;
                            LengthIE = avpLength_EUTRANVectorOffline[0] * 3 - 1;
                            textBox_OffLine.Focus();
                            this.textBox_OffLine.Select(indexIE, LengthIE);
                            break;
                        case "E-UTRAN-Vector2 AVP":
                            indexIE = (20 + diameterLocationOffline + avpLocation_EUTRANVectorOffline[1]) * 3;
                            LengthIE = avpLength_EUTRANVectorOffline[1] * 3 - 1;
                            textBox_OffLine.Focus();
                            this.textBox_OffLine.Select(indexIE, LengthIE);
                            break;
                        case "E-UTRAN-Vector3 AVP":
                            indexIE = (20 + diameterLocationOffline + avpLocation_EUTRANVectorOffline[2]) * 3;
                            LengthIE = avpLength_EUTRANVectorOffline[2] * 3 - 1;
                            textBox_OffLine.Focus();
                            this.textBox_OffLine.Select(indexIE, LengthIE);
                            break;
                        case "E-UTRAN-Vector4 AVP":
                            indexIE = (20 + diameterLocationOffline + avpLocation_EUTRANVectorOffline[3]) * 3;
                            LengthIE = avpLength_EUTRANVectorOffline[3] * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                            break;
                        case "E-UTRAN-Vector5 AVP":
                            indexIE = (20 + diameterLocationOffline + avpLocation_EUTRANVectorOffline[4]) * 3;
                            LengthIE = avpLength_EUTRANVectorOffline[4] * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                            break;
                        case "KASME1 AVP":
                            indexIE = (20 + diameterLocationOffline + avpLocation_KASMEOffline[0]) * 3;
                            LengthIE = avpLength_KASMEOffline[0] * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                            break;
                        case "KASME2 AVP":
                            indexIE = (20 + diameterLocationOffline + avpLocation_KASMEOffline[1]) * 3;
                            LengthIE = avpLength_KASMEOffline[1] * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                            break;
                        case "KASME3 AVP":
                            indexIE = (20 + diameterLocationOffline + avpLocation_KASMEOffline[2]) * 3;
                            LengthIE = avpLength_KASMEOffline[2] * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                            break;
                        case "KASME4 AVP":
                            indexIE = (20 + diameterLocationOffline + avpLocation_KASMEOffline[3]) * 3;
                            LengthIE = avpLength_KASMEOffline[3] * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                            break;
                        case "KASME5 AVP":
                            indexIE = (20 + diameterLocationOffline + avpLocation_KASMEOffline[4]) * 3;
                            LengthIE = avpLength_KASMEOffline[4] * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                            break;
                        case "Session-ID AVP":
                            indexIE = (20 + diameterLocationOffline + avpLocation_SessionIDOffline) * 3;
                            LengthIE = avpLength_SessionIDOffline * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                            break;
                    }
                    if (nodeText.Length >= 7)
                    {
                        if (nodeText.Substring(0, 7) == "Length:")
                        {
                            indexIE = (20 + diameterLocationOffline + 1) * 3;
                            LengthIE = 3 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }

                    if (nodeText.Length >= 5)
                    {
                        if (nodeText.Substring(0, 5) == "Flag:")
                        {
                            indexIE = (20 + diameterLocationOffline + 4) * 3;
                            LengthIE = 1 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 12)
                    {
                        if (nodeText.Substring(0, 12) == "CommandCODE:")
                        {
                            indexIE = (20 + diameterLocationOffline + 5) * 3;
                            LengthIE = 3 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                        if (nodeText.Substring(0, 12) == "3GPP S6a/S6d" || nodeText.Substring(0, 12) == "Diameter Com" || nodeText.Substring(0, 12) == "Diameter Bas")
                        {
                            indexIE = (20 + diameterLocationOffline + 8) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 3)
                    {
                        if (nodeText.Substring(0, 3) == "Hop")
                        {
                            indexIE = (20 + diameterLocationOffline + 12) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                        if (nodeText.Substring(0, 3) == "End")
                        {
                            indexIE = (20 + diameterLocationOffline + 16) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 19)
                    {
                        if (nodeText.Substring(0, 19) == "User-Name AVP Code:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_UserNameOffline) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                        if (nodeText.Substring(0, 19) == "User-Name AVP Flag:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_UserNameOffline + 4) * 3;
                            LengthIE = 1 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 21)
                    {
                        if (nodeText.Substring(0, 21) == "User-Name AVP Length:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_UserNameOffline + 5) * 3;
                            LengthIE = 3 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 3)
                    {
                        if (nodeText.Substring(0, 3) == "460")
                        {
                            indexIE = (20 + diameterLocationOffline + location_UserNameStrOffline) * 3;
                            LengthIE = length_UserNameStrOffline * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 21)
                    {
                        if (nodeText.Substring(0, 21) == "Result-CODE AVP Code:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_ResultOffline) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                        if (nodeText.Substring(0, 21) == "Result-CODE AVP Flag:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_ResultOffline + 4) * 3;
                            LengthIE = 1 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 23)
                    {
                        if (nodeText.Substring(0, 23) == "Result-CODE AVP Length:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_ResultOffline + 5) * 3;
                            LengthIE = 3 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 11)
                    {
                        if (nodeText.Substring(0, 11) == "ResultCode:")
                        {
                            indexIE = (20 + diameterLocationOffline + location_ResultCodeStrOffline) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 29)
                    {
                        if (nodeText.Substring(0, 29) == "Authentication-Info AVP Code:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_AuthenticationOffline) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 28)
                    {
                        if (nodeText.Substring(0, 28) == "Authentication-Info AVPFlag:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_AuthenticationOffline + 4) * 3;
                            LengthIE = 1 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 30)
                    {
                        if (nodeText.Substring(0, 30) == "Authentication-Info AVPLength:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_AuthenticationOffline + 5) * 3;
                            LengthIE = 3 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 32)
                    {
                        if (nodeText.Substring(0, 32) == "Authentication-Info AVPVendorID:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_AuthenticationOffline + 8) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 21)
                    {
                        if (nodeText.Substring(0, 21) == "E-UTRAN-Vector1 Code:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_EUTRANVectorOffline[0]) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                        if (nodeText.Substring(0, 21) == "E-UTRAN-Vector1 Flag:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_EUTRANVectorOffline[0] + 4) * 3;
                            LengthIE = 1 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 23)
                    {
                        if (nodeText.Substring(0, 23) == "E-UTRAN-Vector1 Length:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_EUTRANVectorOffline[0] + 5) * 3;
                            LengthIE = 3 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 25)
                    {
                        if (nodeText.Substring(0, 25) == "E-UTRAN-Vector1 VendorID:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_EUTRANVectorOffline[0] + 8) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 21)
                    {
                        if (nodeText.Substring(0, 21) == "E-UTRAN-Vector2 Code:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_EUTRANVectorOffline[1]) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                        if (nodeText.Substring(0, 21) == "E-UTRAN-Vector2 Flag:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_EUTRANVectorOffline[1] + 4) * 3;
                            LengthIE = 1 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 23)
                    {
                        if (nodeText.Substring(0, 23) == "E-UTRAN-Vector2 Length:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_EUTRANVectorOffline[1] + 5) * 3;
                            LengthIE = 3 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 25)
                    {
                        if (nodeText.Substring(0, 25) == "E-UTRAN-Vector2 VendorID:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_EUTRANVectorOffline[1] + 8) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 21)
                    {
                        if (nodeText.Substring(0, 21) == "E-UTRAN-Vector3 Code:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_EUTRANVectorOffline[2]) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                        if (nodeText.Substring(0, 21) == "E-UTRAN-Vector3 Flag:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_EUTRANVectorOffline[2] + 4) * 3;
                            LengthIE = 1 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 23)
                    {
                        if (nodeText.Substring(0, 23) == "E-UTRAN-Vector3 Length:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_EUTRANVectorOffline[2] + 5) * 3;
                            LengthIE = 3 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 25)
                    {
                        if (nodeText.Substring(0, 25) == "E-UTRAN-Vector3 VendorID:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_EUTRANVectorOffline[2] + 8) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 21)
                    {
                        if (nodeText.Substring(0, 21) == "E-UTRAN-Vector4 Code:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_EUTRANVectorOffline[3]) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                        if (nodeText.Substring(0, 21) == "E-UTRAN-Vector4 Flag:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_EUTRANVectorOffline[3] + 4) * 3;
                            LengthIE = 1 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 23)
                    {
                        if (nodeText.Substring(0, 23) == "E-UTRAN-Vector4 Length:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_EUTRANVectorOffline[3] + 5) * 3;
                            LengthIE = 3 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 25)
                    {
                        if (nodeText.Substring(0, 25) == "E-UTRAN-Vector4 VendorID:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_EUTRANVectorOffline[3] + 8) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 21)
                    {
                        if (nodeText.Substring(0, 21) == "E-UTRAN-Vector5 Code:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_EUTRANVectorOffline[4]) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                        if (nodeText.Substring(0, 21) == "E-UTRAN-Vector5 Flag:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_EUTRANVectorOffline[4] + 4) * 3;
                            LengthIE = 1 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 23)
                    {
                        if (nodeText.Substring(0, 23) == "E-UTRAN-Vector5 Length:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_EUTRANVectorOffline[4] + 5) * 3;
                            LengthIE = 3 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 25)
                    {
                        if (nodeText.Substring(0, 25) == "E-UTRAN-Vector5 VendorID:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_EUTRANVectorOffline[4] + 8) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 12)
                    {

                        if (nodeText.Substring(0, 12) == "KASME1 Code:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_KASMEOffline[0]) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                        if (nodeText.Substring(0, 12) == "KASME1 Flag:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_KASMEOffline[0] + 4) * 3;
                            LengthIE = 1 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 14)
                    {
                        if (nodeText.Substring(0, 14) == "KASME1 Length:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_KASMEOffline[0] + 5) * 3;
                            LengthIE = 3 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 16)
                    {
                        if (nodeText.Substring(0, 16) == "KASME1 VendorID:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_KASMEOffline[0] + 8) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 7)
                    {
                        if (nodeText.Substring(0, 7) == "KASME1:")
                        {
                            indexIE = (20 + diameterLocationOffline + location_KASMEStrOffline[0]) * 3;
                            LengthIE = 32 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 12)
                    {
                        if (nodeText.Substring(0, 12) == "KASME2 Code:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_KASMEOffline[1]) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                        if (nodeText.Substring(0, 12) == "KASME2 Flag:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_KASMEOffline[1] + 4) * 3;
                            LengthIE = 1 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 14)
                    {
                        if (nodeText.Substring(0, 14) == "KASME2 Length:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_KASMEOffline[1] + 5) * 3;
                            LengthIE = 3 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 16)
                    {
                        if (nodeText.Substring(0, 16) == "KASME2 VendorID:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_KASMEOffline[1] + 8) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 7)
                    {
                        if (nodeText.Substring(0, 7) == "KASME2:")
                        {
                            indexIE = (20 + diameterLocationOffline + location_KASMEStrOffline[1]) * 3;
                            LengthIE = 32 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 12)
                    {
                        if (nodeText.Substring(0, 12) == "KASME3 Code:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_KASMEOffline[2]) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                        if (nodeText.Substring(0, 12) == "KASME3 Flag:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_KASMEOffline[2] + 4) * 3;
                            LengthIE = 1 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 14)
                    {
                        if (nodeText.Substring(0, 14) == "KASME3 Length:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_KASMEOffline[2] + 5) * 3;
                            LengthIE = 3 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 16)
                    {
                        if (nodeText.Substring(0, 16) == "KASME3 VendorID:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_KASMEOffline[2] + 8) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 7)
                    {
                        if (nodeText.Substring(0, 7) == "KASME3:")
                        {
                            indexIE = (20 + diameterLocationOffline + location_KASMEStrOffline[2]) * 3;
                            LengthIE = 32 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 12)
                    {
                        if (nodeText.Substring(0, 12) == "KASME4 Code:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_KASMEOffline[3]) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                        if (nodeText.Substring(0, 12) == "KASME4 Flag:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_KASMEOffline[3] + 4) * 3;
                            LengthIE = 1 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 14)
                    {
                        if (nodeText.Substring(0, 14) == "KASME4 Length:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_KASMEOffline[3] + 5) * 3;
                            LengthIE = 3 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 16)
                    {
                        if (nodeText.Substring(0, 16) == "KASME4 VendorID:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_KASMEOffline[3] + 8) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 7)
                    {
                        if (nodeText.Substring(0, 7) == "KASME4:")
                        {
                            indexIE = (20 + diameterLocationOffline + location_KASMEStrOffline[3]) * 3;
                            LengthIE = 32 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 12)
                    {
                        if (nodeText.Substring(0, 12) == "KASME5 Code:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_KASMEOffline[4]) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                        if (nodeText.Substring(0, 12) == "KASME5 Flag:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_KASMEOffline[4] + 4) * 3;
                            LengthIE = 1 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 14)
                    {
                        if (nodeText.Substring(0, 14) == "KASME5 Length:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_KASMEOffline[4] + 5) * 3;
                            LengthIE = 3 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 16)
                    {
                        if (nodeText.Substring(0, 16) == "KASME5 VendorID:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_KASMEOffline[4] + 8) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 7)
                    {
                        if (nodeText.Substring(0, 7) == "KASME5:")
                        {
                            indexIE = (20 + diameterLocationOffline + location_KASMEStrOffline[4]) * 3;
                            LengthIE = 32 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 20)
                    {
                        if (nodeText.Substring(0, 20) == "Session-ID AVP Code:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_SessionIDOffline) * 3;
                            LengthIE = 4 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 19)
                    {
                        if (nodeText.Substring(0, 19) == "Session-ID AVPFlag:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_SessionIDOffline + 4) * 3;
                            LengthIE = 1 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 21)
                    {
                        if (nodeText.Substring(0, 21) == "Session-ID AVPLength:")
                        {
                            indexIE = (20 + diameterLocationOffline + avpLocation_SessionIDOffline + 5) * 3;
                            LengthIE = 3 * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }
                    if (nodeText.Length >= 24)
                    {
                        if (nodeText.Substring(0, 24) == "Session-ID AVPSessionid:")
                        {
                            indexIE = (20 + diameterLocationOffline + location_SessionID4Offline) * 3;
                            LengthIE = length_SessionID4Offline * 3 - 1;
                            textBox_OffLine.Focus(); this.textBox_OffLine.Select(indexIE, LengthIE);
                        }
                    }


                }
                #endregion

                try
                {
                    #region 点击gtp节点时对应
                    if (protocolSelected_Offline == "GTPv2")
                    {
                        string nodeText = treeView_OffLine.Nodes[0].Nodes[0].Text;//点击行的文本
                        string SubnodeText = treeView_OffLine.SelectedNode.Text.Substring(0, 5);
                        if (nodeText == "Echo Request" || nodeText == "Echo Response")
                        {
                            switch (SubnodeText)
                            {
                                case "010. ":
                                    indexIE = 28 * 3;
                                    LengthIE = 3 - 1;
                                    textBox_OffLine.Focus();
                                    this.textBox_OffLine.Select(indexIE, LengthIE);
                                    break;
                                case "...0 ":
                                    indexIE = 28 * 3;
                                    LengthIE = 3 - 1;
                                    textBox_OffLine.Focus();
                                    this.textBox_OffLine.Select(indexIE, LengthIE);
                                    break;
                                case ".... ":
                                    indexIE = 28 * 3;
                                    LengthIE = 3 - 1;
                                    textBox_OffLine.Focus();
                                    this.textBox_OffLine.Select(indexIE, LengthIE);
                                    break;
                                case "Messa":
                                    indexIE = 30 * 3;
                                    LengthIE = 6 - 1;
                                    textBox_OffLine.Focus();
                                    this.textBox_OffLine.Select(indexIE, LengthIE);
                                    break;
                                case "Seque":
                                    indexIE = 32 * 3;
                                    LengthIE = 9 - 1;
                                    textBox_OffLine.Focus();
                                    this.textBox_OffLine.Select(indexIE, LengthIE);
                                    break;
                                case "spare":
                                    indexIE = 35 * 3;
                                    LengthIE = 3 - 1;
                                    textBox_OffLine.Focus();
                                    this.textBox_OffLine.Select(indexIE, LengthIE);
                                    break;
                                default:
                                    break;
                            }
                            if (selectedNode == treeView_OffLine.Nodes[0].Nodes[1].Nodes[3])
                            {
                                indexIE = 29 * 3;
                                LengthIE = 3 - 1;
                                textBox_OffLine.Focus();
                                this.textBox_OffLine.Select(indexIE, LengthIE);
                            }
                            if (selectedNode == treeView_OffLine.Nodes[0].Nodes[0])
                            {
                                indexIE = 28 * 3;
                                LengthIE = textBox_OffLine.Text.Length - 1;
                                textBox_OffLine.Focus();
                                this.textBox_OffLine.Select(indexIE, LengthIE);
                            }
                        }
                        else
                        {
                            switch (SubnodeText)
                            {
                                case "010. ":
                                    indexIE = 28 * 3;
                                    LengthIE = 3 - 1;
                                    textBox_OffLine.Focus();
                                    this.textBox_OffLine.Select(indexIE, LengthIE);
                                    break;
                                case "...0 ":
                                    indexIE = 28 * 3;
                                    LengthIE = 3 - 1;
                                    textBox_OffLine.Focus();
                                    this.textBox_OffLine.Select(indexIE, LengthIE);
                                    break;
                                case ".... ":
                                    indexIE = 28 * 3;
                                    LengthIE = 3 - 1;
                                    textBox_OffLine.Focus();
                                    this.textBox_OffLine.Select(indexIE, LengthIE);
                                    break;
                                case "Messa":
                                    indexIE = 30 * 3;
                                    LengthIE = 6 - 1;
                                    textBox_OffLine.Focus();
                                    this.textBox_OffLine.Select(indexIE, LengthIE);
                                    break;
                                case "Tunne":
                                    indexIE = 32 * 3;
                                    LengthIE = 12 - 1;
                                    textBox_OffLine.Focus();
                                    this.textBox_OffLine.Select(indexIE, LengthIE);
                                    break;
                                case "Seque":
                                    indexIE = 36 * 3;
                                    LengthIE = 9 - 1;
                                    textBox_OffLine.Focus();
                                    this.textBox_OffLine.Select(indexIE, LengthIE);
                                    break;
                                case "spare":
                                    indexIE = 39 * 3;
                                    LengthIE = 3 - 1;
                                    textBox_OffLine.Focus();
                                    this.textBox_OffLine.Select(indexIE, LengthIE);
                                    break;
                                default:
                                    break;
                            }
                            if (selectedNode == treeView_OffLine.Nodes[0].Nodes[1].Nodes[3])
                            {
                                indexIE = 29 * 3;
                                LengthIE = 3 - 1;
                                textBox_OffLine.Focus();
                                this.textBox_OffLine.Select(indexIE, LengthIE);
                            }
                            if (selectedNode == treeView_OffLine.Nodes[0].Nodes[0])
                            {
                                indexIE = 28 * 3;
                                LengthIE = textBox_OffLine.Text.Length - 1;
                                textBox_OffLine.Focus();
                                this.textBox_OffLine.Select(indexIE, LengthIE);
                            }
                        }

                    }
                    #endregion
                }
                catch
                { }
            }));
        }
        private void SelectedOnLine(object source, System.Timers.ElapsedEventArgs e)
        {
            int indexIE1 = 0;
            int LengthIE1 = 0;
            int No = -1;
            this.Invoke((EventHandler)(delegate
           {
               #region 点击diameter节点时对应凸显其对应的数据流
               if (protocolSelected_Online == "DIAMETER-SCTP" || protocolSelected_Online == "DIAMETER-TCP")
               {
                   string nodeText = selectedNode.Text;//点击行的文本
                   switch (nodeText)
                   {
                       case "SCTP Protocol":
                           indexIE1 = 0 * 3;
                           LengthIE1 = diameterLocationOnline * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                           break;
                       case "TCP Protocol":
                           indexIE1 = 0 * 3;
                           LengthIE1 = diameterLocationOnline * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                           break;
                       case "diameter":
                           indexIE1 = (diameterLocationOnline) * 3;
                           LengthIE1 = diameterLengthOnline * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                           break;
                       case "Version:1":
                           indexIE1 = (diameterLocationOnline) * 3;
                           LengthIE1 = 1 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                           break;
                       case "User-Name AVP":
                           indexIE1 = (diameterLocationOnline + avpLocation_UserNameOnline) * 3;
                           LengthIE1 = avpLength_UserNameOnline * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                           break;
                       case "Result-CODE AVP":
                           indexIE1 = (diameterLocationOnline + avpLocation_ResultOnline) * 3;
                           LengthIE1 = avpLength_ResultOnline * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                           break;
                       case "Authentication-Info AVP":
                           indexIE1 = (diameterLocationOnline + avpLocation_AuthenticationOnline) * 3;
                           LengthIE1 = avpLength_AuthenticationOnline * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                           break;
                       case "E-UTRAN-Vector1 AVP":
                           indexIE1 = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[0]) * 3;
                           LengthIE1 = avpLength_EUTRANVectorOnline[0] * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                           break;
                       case "E-UTRAN-Vector2 AVP":
                           indexIE1 = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[1]) * 3;
                           LengthIE1 = avpLength_EUTRANVectorOnline[1] * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                           break;
                       case "E-UTRAN-Vector3 AVP":
                           indexIE1 = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[2]) * 3;
                           LengthIE1 = avpLength_EUTRANVectorOnline[2] * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                           break;
                       case "E-UTRAN-Vector4 AVP":
                           indexIE1 = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[3]) * 3;
                           LengthIE1 = avpLength_EUTRANVectorOnline[3] * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                           break;
                       case "E-UTRAN-Vector5 AVP":
                           indexIE1 = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[4]) * 3;
                           LengthIE1 = avpLength_EUTRANVectorOnline[4] * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                           break;
                       case "KASME1 AVP":
                           indexIE1 = (diameterLocationOnline + avpLocation_KASMEOnline[0]) * 3;
                           LengthIE1 = avpLength_KASMEOnline[0] * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                           break;
                       case "KASME2 AVP":
                           indexIE1 = (diameterLocationOnline + avpLocation_KASMEOnline[1]) * 3;
                           LengthIE1 = avpLength_KASMEOnline[1] * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                           break;
                       case "KASME3 AVP":
                           indexIE1 = (diameterLocationOnline + avpLocation_KASMEOnline[2]) * 3;
                           LengthIE1 = avpLength_KASMEOnline[2] * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                           break;
                       case "KASME4 AVP":
                           indexIE1 = (diameterLocationOnline + avpLocation_KASMEOnline[3]) * 3;
                           LengthIE1 = avpLength_KASMEOnline[3] * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                           break;
                       case "KASME5 AVP":
                           indexIE1 = (diameterLocationOnline + avpLocation_KASMEOnline[4]) * 3;
                           LengthIE1 = avpLength_KASMEOnline[4] * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                           break;
                       case "Session-ID AVP":
                           indexIE1 = (diameterLocationOnline + avpLocation_SessionIDOnline) * 3;
                           LengthIE1 = avpLength_SessionIDOnline * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                           break;
                   }
                   if (nodeText.Length >= 7)
                   {
                       if (nodeText.Substring(0, 7) == "Length:")
                       {
                           indexIE1 = (diameterLocationOnline + 1) * 3;
                           LengthIE1 = 3 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }

                   if (nodeText.Length >= 5)
                   {
                       if (nodeText.Substring(0, 5) == "Flag:")
                       {
                           indexIE1 = (diameterLocationOnline + 4) * 3;
                           LengthIE1 = 1 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 12)
                   {
                       if (nodeText.Substring(0, 12) == "CommandCODE:")
                       {
                           indexIE1 = (diameterLocationOnline + 5) * 3;
                           LengthIE1 = 3 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                       if (nodeText.Substring(0, 12) == "3GPP S6a/S6d" || nodeText.Substring(0, 12) == "Diameter Com" || nodeText.Substring(0, 12) == "Diameter Bas")
                       {
                           indexIE1 = (diameterLocationOnline + 8) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 3)
                   {
                       if (nodeText.Substring(0, 3) == "Hop")
                       {
                           indexIE1 = (diameterLocationOnline + 12) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                       if (nodeText.Substring(0, 3) == "End")
                       {
                           indexIE1 = (diameterLocationOnline + 16) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 19)
                   {
                       if (nodeText.Substring(0, 19) == "User-Name AVP Code:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_UserNameOnline) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                       if (nodeText.Substring(0, 19) == "User-Name AVP Flag:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_UserNameOnline + 4) * 3;
                           LengthIE1 = 1 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 21)
                   {
                       if (nodeText.Substring(0, 21) == "User-Name AVP Length:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_UserNameOnline + 5) * 3;
                           LengthIE1 = 3 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 3)
                   {
                       if (nodeText.Substring(0, 3) == "460")
                       {
                           indexIE1 = (diameterLocationOnline + location_UserNameStrOnline) * 3;
                           LengthIE1 = length_UserNameStrOnline * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 21)
                   {
                       if (nodeText.Substring(0, 21) == "Result-CODE AVP Code:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_ResultOnline) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                       if (nodeText.Substring(0, 21) == "Result-CODE AVP Flag:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_ResultOnline + 4) * 3;
                           LengthIE1 = 1 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 23)
                   {
                       if (nodeText.Substring(0, 23) == "Result-CODE AVP Length:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_ResultOnline + 5) * 3;
                           LengthIE1 = 3 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 11)
                   {
                       if (nodeText.Substring(0, 11) == "ResultCode:")
                       {
                           indexIE1 = (diameterLocationOnline + location_ResultCodeStrOnline) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 29)
                   {
                       if (nodeText.Substring(0, 29) == "Authentication-Info AVP Code:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_AuthenticationOnline) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 28)
                   {
                       if (nodeText.Substring(0, 28) == "Authentication-Info AVPFlag:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_AuthenticationOnline + 4) * 3;
                           LengthIE1 = 1 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 30)
                   {
                       if (nodeText.Substring(0, 30) == "Authentication-Info AVPLength:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_AuthenticationOnline + 5) * 3;
                           LengthIE1 = 3 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 32)
                   {
                       if (nodeText.Substring(0, 32) == "Authentication-Info AVPVendorID:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_AuthenticationOnline + 8) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 21)
                   {
                       if (nodeText.Substring(0, 21) == "E-UTRAN-Vector1 Code:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[0]) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                       if (nodeText.Substring(0, 21) == "E-UTRAN-Vector1 Flag:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[0] + 4) * 3;
                           LengthIE1 = 1 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 23)
                   {
                       if (nodeText.Substring(0, 23) == "E-UTRAN-Vector1 Length:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[0] + 5) * 3;
                           LengthIE1 = 3 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 25)
                   {
                       if (nodeText.Substring(0, 25) == "E-UTRAN-Vector1 VendorID:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[0] + 8) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 21)
                   {
                       if (nodeText.Substring(0, 21) == "E-UTRAN-Vector2 Code:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[1]) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                       if (nodeText.Substring(0, 21) == "E-UTRAN-Vector2 Flag:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[1] + 4) * 3;
                           LengthIE1 = 1 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 23)
                   {
                       if (nodeText.Substring(0, 23) == "E-UTRAN-Vector2 Length:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[1] + 5) * 3;
                           LengthIE1 = 3 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 25)
                   {
                       if (nodeText.Substring(0, 25) == "E-UTRAN-Vector2 VendorID:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[1] + 8) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 21)
                   {
                       if (nodeText.Substring(0, 21) == "E-UTRAN-Vector3 Code:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[2]) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                       if (nodeText.Substring(0, 21) == "E-UTRAN-Vector3 Flag:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[2] + 4) * 3;
                           LengthIE1 = 1 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 23)
                   {
                       if (nodeText.Substring(0, 23) == "E-UTRAN-Vector3 Length:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[2] + 5) * 3;
                           LengthIE1 = 3 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 25)
                   {
                       if (nodeText.Substring(0, 25) == "E-UTRAN-Vector3 VendorID:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[2] + 8) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 21)
                   {
                       if (nodeText.Substring(0, 21) == "E-UTRAN-Vector4 Code:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[3]) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                       if (nodeText.Substring(0, 21) == "E-UTRAN-Vector4 Flag:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[3] + 4) * 3;
                           LengthIE1 = 1 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 23)
                   {
                       if (nodeText.Substring(0, 23) == "E-UTRAN-Vector4 Length:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[3] + 5) * 3;
                           LengthIE1 = 3 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 25)
                   {
                       if (nodeText.Substring(0, 25) == "E-UTRAN-Vector4 VendorID:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[3] + 8) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 21)
                   {
                       if (nodeText.Substring(0, 21) == "E-UTRAN-Vector5 Code:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[4]) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                       if (nodeText.Substring(0, 21) == "E-UTRAN-Vector5 Flag:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[4] + 4) * 3;
                           LengthIE1 = 1 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 23)
                   {
                       if (nodeText.Substring(0, 23) == "E-UTRAN-Vector5 Length:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[4] + 5) * 3;
                           LengthIE1 = 3 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 25)
                   {
                       if (nodeText.Substring(0, 25) == "E-UTRAN-Vector5 VendorID:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_EUTRANVectorOnline[4] + 8) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 12)
                   {

                       if (nodeText.Substring(0, 12) == "KASME1 Code:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_KASMEOnline[0]) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                       if (nodeText.Substring(0, 12) == "KASME1 Flag:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_KASMEOnline[0] + 4) * 3;
                           LengthIE1 = 1 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 14)
                   {
                       if (nodeText.Substring(0, 14) == "KASME1 Length:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_KASMEOnline[0] + 5) * 3;
                           LengthIE1 = 3 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 16)
                   {
                       if (nodeText.Substring(0, 16) == "KASME1 VendorID:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_KASMEOnline[0] + 8) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 7)
                   {
                       if (nodeText.Substring(0, 7) == "KASME1:")
                       {
                           indexIE1 = (diameterLocationOnline + location_KASMEStrOnline[0]) * 3;
                           LengthIE1 = 32 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 12)
                   {
                       if (nodeText.Substring(0, 12) == "KASME2 Code:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_KASMEOnline[1]) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                       if (nodeText.Substring(0, 12) == "KASME2 Flag:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_KASMEOnline[1] + 4) * 3;
                           LengthIE1 = 1 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 14)
                   {
                       if (nodeText.Substring(0, 14) == "KASME2 Length:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_KASMEOnline[1] + 5) * 3;
                           LengthIE1 = 3 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 16)
                   {
                       if (nodeText.Substring(0, 16) == "KASME2 VendorID:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_KASMEOnline[1] + 8) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 7)
                   {
                       if (nodeText.Substring(0, 7) == "KASME2:")
                       {
                           indexIE1 = (diameterLocationOnline + location_KASMEStrOnline[1]) * 3;
                           LengthIE1 = 32 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 12)
                   {
                       if (nodeText.Substring(0, 12) == "KASME3 Code:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_KASMEOnline[2]) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                       if (nodeText.Substring(0, 12) == "KASME3 Flag:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_KASMEOnline[2] + 4) * 3;
                           LengthIE1 = 1 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 14)
                   {
                       if (nodeText.Substring(0, 14) == "KASME3 Length:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_KASMEOnline[2] + 5) * 3;
                           LengthIE1 = 3 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 16)
                   {
                       if (nodeText.Substring(0, 16) == "KASME3 VendorID:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_KASMEOnline[2] + 8) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 7)
                   {
                       if (nodeText.Substring(0, 7) == "KASME3:")
                       {
                           indexIE1 = (diameterLocationOnline + location_KASMEStrOnline[2]) * 3;
                           LengthIE1 = 32 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 12)
                   {
                       if (nodeText.Substring(0, 12) == "KASME4 Code:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_KASMEOnline[3]) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                       if (nodeText.Substring(0, 12) == "KASME4 Flag:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_KASMEOnline[3] + 4) * 3;
                           LengthIE1 = 1 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 14)
                   {
                       if (nodeText.Substring(0, 14) == "KASME4 Length:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_KASMEOnline[3] + 5) * 3;
                           LengthIE1 = 3 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 16)
                   {
                       if (nodeText.Substring(0, 16) == "KASME4 VendorID:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_KASMEOnline[3] + 8) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 7)
                   {
                       if (nodeText.Substring(0, 7) == "KASME4:")
                       {
                           indexIE1 = (diameterLocationOnline + location_KASMEStrOnline[3]) * 3;
                           LengthIE1 = 32 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 12)
                   {
                       if (nodeText.Substring(0, 12) == "KASME5 Code:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_KASMEOnline[4]) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                       if (nodeText.Substring(0, 12) == "KASME5 Flag:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_KASMEOnline[4] + 4) * 3;
                           LengthIE1 = 1 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 14)
                   {
                       if (nodeText.Substring(0, 14) == "KASME5 Length:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_KASMEOnline[4] + 5) * 3;
                           LengthIE1 = 3 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 16)
                   {
                       if (nodeText.Substring(0, 16) == "KASME5 VendorID:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_KASMEOnline[4] + 8) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 7)
                   {
                       if (nodeText.Substring(0, 7) == "KASME5:")
                       {
                           indexIE1 = (diameterLocationOnline + location_KASMEStrOnline[4]) * 3;
                           LengthIE1 = 32 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 20)
                   {
                       if (nodeText.Substring(0, 20) == "Session-ID AVP Code:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_SessionIDOnline) * 3;
                           LengthIE1 = 4 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 19)
                   {
                       if (nodeText.Substring(0, 19) == "Session-ID AVPFlag:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_SessionIDOnline + 4) * 3;
                           LengthIE1 = 1 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 21)
                   {
                       if (nodeText.Substring(0, 21) == "Session-ID AVPLength:")
                       {
                           indexIE1 = (diameterLocationOnline + avpLocation_SessionIDOnline + 5) * 3;
                           LengthIE1 = 3 * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }
                   if (nodeText.Length >= 24)
                   {
                       if (nodeText.Substring(0, 24) == "Session-ID AVPSessionid:")
                       {
                           indexIE1 = (diameterLocationOnline + location_SessionID4Online) * 3;
                           LengthIE1 = length_SessionID4Online * 3 - 1;
                           TextBox_Hex.Focus(); this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                   }

               }
               #endregion

               #region                            //点击s1ap节点时凸显对应的编码
               if (OnlineStruct.Count > 0)
               {
                   if (selectedNode1.Text == "procedureCode:" + OnlineStruct[0].Name)
                   {
                       indexIE1 = OnlineStruct[0].IndexName * 3;
                       LengthIE1 = OnlineStruct[0].LengthName * 3 - 1;
                       TextBox_Hex.Focus();
                       this.TextBox_Hex.Select(indexIE1, LengthIE1);
                   }
                   else if ((OnlineStruct.Count > 1) && (selectedNode1.Text == OnlineStruct[1].Name))
                   {
                       indexIE1 = OnlineStruct[1].IndexName * 3;
                       LengthIE1 = OnlineStruct[1].LengthName * 3 - 1;
                       TextBox_Hex.Focus();
                       this.TextBox_Hex.Select(indexIE1, LengthIE1);
                   }
                   else
                   {
                       #region
                       if (parentOnNode == "procedureCode:" + OnlineStruct[0].Name)
                           No = 0;
                       else
                           No = 1;
                       if (selectedNode1.Text == "Item" + OnlineStruct[No].NoMme + ":id-MME-UE-S1AP-ID" + "(" + OnlineStruct[No].mme + ")")
                       {
                           indexIE1 = OnlineStruct[No].indexMme * 3;
                           LengthIE1 = OnlineStruct[No].LengthMme * 3 - 1;
                           TextBox_Hex.Focus();
                           this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                       else if (selectedNode1.Text == "Item" + OnlineStruct[No].NoEnb + ":id-eNB-UE-S1AP-ID" + "(" + OnlineStruct[No].enb + ")")
                       {
                           indexIE1 = OnlineStruct[No].IndexEnb * 3;
                           LengthIE1 = OnlineStruct[No].LengthEnb * 3 - 1;
                           TextBox_Hex.Focus();
                           this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                       else if (selectedNode1.Text == "Item" + OnlineStruct[No].NoCause + ":id-Cause" + OnlineStruct[No].CauseKind + ":" + OnlineStruct[No].cause)
                       {
                           indexIE1 = OnlineStruct[No].IndexCause * 3;
                           LengthIE1 = OnlineStruct[No].LengthCause * 3 - 1;
                           TextBox_Hex.Focus();
                           this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                       else if (selectedNode1.Text == "Item" + OnlineStruct[No].NoIdentity + ":id-IMSI:" + OnlineStruct[No].Identity)
                       {
                           indexIE1 = OnlineStruct[No].IndexIdentity * 3;
                           LengthIE1 = OnlineStruct[No].LengthIdentity * 3 - 1;
                           TextBox_Hex.Focus();
                           this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                       else if (selectedNode1.Text == "Item" + OnlineStruct[No].NoIdentity + ":id-TMSI:" + OnlineStruct[No].Identity)
                       {
                           indexIE1 = OnlineStruct[No].IndexIdentity * 3;
                           LengthIE1 = OnlineStruct[No].IndexIdentity * 3 - 1;
                           TextBox_Hex.Focus();
                           this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                       else if (selectedNode1.Text == "Item" + OnlineStruct[No].NoEmm + ":id-Nas:" + OnlineStruct[No].EmmName)
                       {
                           indexIE1 = OnlineStruct[No].IndexEmm * 3;
                           if (OnlineStruct[No].LengthEmm > 0)
                               LengthIE1 = OnlineStruct[No].LengthEmm * 3 - 1;
                           else
                               LengthIE1 = OnlineStruct[No].LengthEmm * 3;
                           TextBox_Hex.Focus();
                           this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                       else if (selectedNode1.Text == "Item" + OnlineStruct[No].NoNasIdentity + ":id-IMSI:" + OnlineStruct[No].NasIdentity)
                       {
                           indexIE1 = OnlineStruct[No].IndexNasId * 3;
                           LengthIE1 = OnlineStruct[No].LengthNasId * 3 - 1;
                           TextBox_Hex.Focus();
                           this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                       else if (selectedNode1.Text == "Item" + OnlineStruct[No].NoNasIdentity + ":id-TMSI:" + OnlineStruct[No].NasIdentity)
                       {
                           indexIE1 = OnlineStruct[No].IndexNasId * 3;
                           LengthIE1 = OnlineStruct[No].LengthNasId * 3 - 1;
                           TextBox_Hex.Focus();
                           this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                       else if (selectedNode1.Text == "Item" + OnlineStruct[No].NoNasCause + ":id-Nas Cause:" + OnlineStruct[No].NasCause)
                       {
                           indexIE1 = OnlineStruct[No].IndexNasCause * 3;
                           LengthIE1 = OnlineStruct[No].LengthNasCause * 3 - 1;
                           TextBox_Hex.Focus();
                           this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                       else if (selectedNode1.Text == "Item" + OnlineStruct[No].NoUeIp + ":PDN address:" + OnlineStruct[No].UeIP)
                       {
                           indexIE1 = OnlineStruct[No].IndexUeIp * 3;
                           LengthIE1 = OnlineStruct[No].LengthUeIp * 3 - 1;
                           TextBox_Hex.Focus();
                           this.TextBox_Hex.Select(indexIE1, LengthIE1);
                       }
                       #endregion
                   }
               }

               #endregion

               try
               {
                   #region 点击gtp节点时对应
                   if (protocolSelected_Offline == "GTPv2")
                   {
                       int indexIE = 0;
                       int LengthIE = 0;
                       string nodeText = treeView_mess.Nodes[0].Nodes[0].Text;//点击行的文本
                       string SubnodeText = treeView_mess.SelectedNode.Text.Substring(0, 5);
                       if (nodeText == "Echo Request" || nodeText == "Echo Response")
                       {
                           switch (SubnodeText)
                           {
                               case "010. ":
                                   indexIE = 28 * 3;
                                   LengthIE = 3 - 1;
                                   TextBox_Hex.Focus();
                                   this.TextBox_Hex.Select(indexIE, LengthIE);
                                   break;
                               case "...0 ":
                                   indexIE = 28 * 3;
                                   LengthIE = 3 - 1;
                                   TextBox_Hex.Focus();
                                   this.TextBox_Hex.Select(indexIE, LengthIE);
                                   break;
                               case ".... ":
                                   indexIE = 28 * 3;
                                   LengthIE = 3 - 1;
                                   TextBox_Hex.Focus();
                                   this.TextBox_Hex.Select(indexIE, LengthIE);
                                   break;
                               case "Messa":
                                   indexIE = 30 * 3;
                                   LengthIE = 6 - 1;
                                   TextBox_Hex.Focus();
                                   this.TextBox_Hex.Select(indexIE, LengthIE);
                                   break;
                               case "Seque":
                                   indexIE = 32 * 3;
                                   LengthIE = 9 - 1;
                                   TextBox_Hex.Focus();
                                   this.TextBox_Hex.Select(indexIE, LengthIE);
                                   break;
                               case "spare":
                                   indexIE = 35 * 3;
                                   LengthIE = 3 - 1;
                                   TextBox_Hex.Focus();
                                   this.TextBox_Hex.Select(indexIE, LengthIE);
                                   break;
                               default:
                                   break;
                           }
                           if (selectedNode == treeView_OffLine.Nodes[0].Nodes[1].Nodes[3])
                           {
                               indexIE = 29 * 3;
                               LengthIE = 3 - 1;
                               TextBox_Hex.Focus();
                               this.TextBox_Hex.Select(indexIE, LengthIE);
                           }
                           if (selectedNode == treeView_OffLine.Nodes[0].Nodes[0])
                           {
                               indexIE = 28 * 3;
                               LengthIE = textBox_OffLine.Text.Length - 1;
                               TextBox_Hex.Focus();
                               this.TextBox_Hex.Select(indexIE, LengthIE);
                           }
                       }
                       else
                       {
                           switch (SubnodeText)
                           {
                               case "010. ":
                                   indexIE = 28 * 3;
                                   LengthIE = 3 - 1;
                                   TextBox_Hex.Focus();
                                   this.TextBox_Hex.Select(indexIE, LengthIE);
                                   break;
                               case "...0 ":
                                   indexIE = 28 * 3;
                                   LengthIE = 3 - 1;
                                   TextBox_Hex.Focus();
                                   this.TextBox_Hex.Select(indexIE, LengthIE);
                                   break;
                               case ".... ":
                                   indexIE = 28 * 3;
                                   LengthIE = 3 - 1;
                                   TextBox_Hex.Focus();
                                   this.TextBox_Hex.Select(indexIE, LengthIE);
                                   break;
                               case "Messa":
                                   indexIE = 30 * 3;
                                   LengthIE = 6 - 1;
                                   TextBox_Hex.Focus();
                                   this.TextBox_Hex.Select(indexIE, LengthIE);
                                   break;
                               case "Tunne":
                                   indexIE = 32 * 3;
                                   LengthIE = 12 - 1;
                                   TextBox_Hex.Focus();
                                   this.TextBox_Hex.Select(indexIE, LengthIE);
                                   break;
                               case "Seque":
                                   indexIE = 36 * 3;
                                   LengthIE = 9 - 1;
                                   TextBox_Hex.Focus();
                                   this.TextBox_Hex.Select(indexIE, LengthIE);
                                   break;
                               case "spare":
                                   indexIE = 39 * 3;
                                   LengthIE = 3 - 1;
                                   TextBox_Hex.Focus();
                                   this.TextBox_Hex.Select(indexIE, LengthIE);
                                   break;
                               default:
                                   break;
                           }
                           if (selectedNode == treeView_OffLine.Nodes[0].Nodes[1].Nodes[3])
                           {
                               indexIE = 29 * 3;
                               LengthIE = 3 - 1;
                               TextBox_Hex.Focus();
                               this.TextBox_Hex.Select(indexIE, LengthIE);
                           }
                           if (selectedNode == treeView_OffLine.Nodes[0].Nodes[0])
                           {
                               indexIE = 28 * 3;
                               LengthIE = textBox_OffLine.Text.Length - 1;
                               TextBox_Hex.Focus();
                               this.TextBox_Hex.Select(indexIE, LengthIE);
                           }

                       }

                   }
                   #endregion
               }
               catch
               { }
           }));
        }

        private void label2_GTPv2_DoubleClick(object sender, EventArgs e)
        {
            Point Mx = label2_GTPv2.PointToClient(Control.MousePosition);
            int x = Mx.X;
            int y = Mx.Y;
            //计算关闭区域  
            Point p = label2_GTPv2.Location; // 获取或设置该控件的左上角相对于其容器的左上角的坐标。   
            int x1 = label2_GTPv2.Size.Width;
            int y1 = label2_GTPv2.Size.Height;
            //如果鼠标在区域内就关闭选项卡   
            bool isClose = x > p.X && x < p.X + x1 && y > p.Y && y < p.Y + y1;
            if (isClose == true)
            {
                if (splitContainer17.Panel1Collapsed)// && splitContainer18.Panel1Collapsed)
                    splitContainer6.Panel2Collapsed = true;
                else
                {
                    splitContainer17.Panel2Collapsed = true;
                    // splitContainer18.Panel2Collapsed = true;
                }
            }
        }
    }
}


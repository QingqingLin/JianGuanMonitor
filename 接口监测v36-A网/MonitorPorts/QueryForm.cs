using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace MonitorPorts
{
    public partial class QueryForm : Form
    {

        #region 全局变量
        private int ItemID;
        private double captureTime;
        private int FilterCount;
        DateTime startTime;
        delegate void listViewDelegate(string ID, string Capturetime, string Protocol, string SourceIP, string SourcePort, string DestIP, string DestPort, string AllLength, string MessageBodyTxt, string MessageBodyLen, string MessageBodyHex);
        Dictionary<string, string> dic;
        MainForm main;
        #endregion
        public QueryForm(MainForm main)
        {
            InitializeComponent();
            this.main = main;
            ItemID = 0;
            FilterCount = 0;
            // By Kuang
            panel2.BackColor = Color.FromArgb(55, 159, 251);
            }

       
        private void QueryForm_Load(object sender, EventArgs e)
        {

        }

        private void Button_Filter_Click(object sender, EventArgs e)
        {

            Clear();
            DateTime startData = startDate.Value.Date;                                                  // dateTimePickerStart.Value.Date;//开始日期

            DateTime endData = endDate.Value.Date;                                                      //终止日期

            DateTime _startTime = startData + new TimeSpan(startHour.Value.Hour, startHour.Value.Minute, startHour.Value.Second);//开始时间

            DateTime _endTime = endData + new TimeSpan(endHour.Value.Hour, endHour.Value.Minute, endHour.Value.Second); ;//终止时间

            int frameID = 1;                                                                        // 数据帧序号记录
            FilterPcap filter = new FilterPcap();                                                    //实例化一个根据文件时间筛选数据帧的类
            List<string> fileNameList = filter.getReqrFileList(_startTime, _endTime, Application.StartupPath + "\\CBTC");      //获得所有满足条件的文件的文件名

            if (fileNameList.Count == 0)      //不符合时间筛选条件的处理
            {
                MessageBox.Show("没有找到指定的数据信息，请重新配置筛选条件！");    //时间查找不符合
            }
            else  //符合时间筛选条件的处理
            {
                //选取筛选的IP

                string sourIP = textBox1.Text;
                string destIP = textBox2.Text;
                string srcPort = textBox3.Text;
                string dstPort = textBox4.Text;
                DateTime preTime = new DateTime(), nowTime = new DateTime();             //   统计相邻两个数据帧时间间隔

                // 符合文件的遍历
                foreach (string filepath in fileNameList)
                {
                    try
                    {
                        int flag = 1;                                                          //当前文件流到达末尾标志  
                        Boolean isNew = true;                                                 // 结束当前文件解析，读取下一个文件标志，每个PCAP文件只处理一次文件头
                        //FileStream fs = new FileStream(Application.StartupPath + "\\pcap\\" + filepath, FileMode.Open, FileAccess.Read, FileShare.Read);       //创建流文件读取
                        FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);       //创建流文件读取

                        while (flag != 0)      // 当前文件读取
                        {
                            //PcapDecoder pcd = new PcapDecoder(Application.StartupPath + "\\pcap\\" + filepath);       //实例化解析类 
                            PcapDecoder pcd = new PcapDecoder(filepath);       //实例化解析类 
                            if (isNew)         //判断是否是一个新的文件，新的pcap文件要先解析文件头
                            {
                                pcd.GetFileHead(fs);        //解析文件头
                                isNew = false;
                            }

                            pcd.GetDataHead(fs);           //解析当前文件数据帧头部

                            if (pcd.isFinish == 0)         //判断是否已经读取到文件末尾
                            {
                                flag = 0;                 //达到末尾标志置0
                            }
                            else
                            {
                                pcd.ReadPcapFile(fs);     //解析数据帧


                                #region 将解析内容添加到列表显示
                                string[] frame = new string[11];                    //列表行内容

                                frame[4] = pcd.PcapFile.IPHead.SourceIP;                        // 第五列  源IP地址
                                frame[6] = pcd.PcapFile.IPHead.DestIP;                          // 第七列  目的IP地址
                                frame[5] = pcd.PcapFile.UDPHead.SourcePort;                     // 第六列  源端口号
                                frame[7] = pcd.PcapFile.UDPHead.DestPort;                       // 第八列  目的端口号

                                //筛选Ip
                                if ((frame[4] == sourIP) && (frame[6] == destIP) && frame[5] == srcPort && frame[7] == dstPort)   //符合IP筛选条件的处理
                                {
                                    frame[0] = frameID.ToString();                      //第一列 帧序号
                                    frame[1] = pcd.PcapFile.DataHead.Time.ToString("yyyy-MM-dd HH:mm:ss fff");  // 第二列 帧时间
                                    //  时间间隔处理
                                    if (frameID == 1)
                                    {
                                        preTime = pcd.PcapFile.DataHead.Time;
                                        nowTime = pcd.PcapFile.DataHead.Time;
                                    }
                                    else
                                    {
                                        preTime = nowTime;
                                        nowTime = pcd.PcapFile.DataHead.Time;
                                    }

                                    if ((nowTime - preTime).TotalSeconds > 60)                  //两个不同文件的处理
                                    {
                                        frame[2] = Convert.ToString(6);
                                    }
                                    else
                                    {
                                        frame[2] = (nowTime - preTime).TotalSeconds.ToString();     //第三列  时间间隔

                                    }
                                    frame[3] = pcd.PcapFile.Protocol;                           // 第四列  协议类型
                                    frame[4] = pcd.PcapFile.IPHead.SourceIP;                        // 第五列  源IP地址
                                    //frame[5] = pcd.PcapFile.UDPHead.SourcePort;                     // 第六列  源端口号
                                    frame[6] = pcd.PcapFile.IPHead.DestIP;                          // 第七列  目的IP地址
                                    //frame[7] = pcd.PcapFile.UDPHead.DestPort;                       // 第八列  目的端口号
                                    frame[8] = (pcd.PcapFile.DataHead.GetDataLength - 14).ToString();  //  第九列 消息长度
                                    frame[9] = pcd.PcapFile.DataLength.ToString();
                                    foreach (int a in pcd.PcapFile.Data)                            //第十列 数据帧长度
                                    {
                                        string b = a.ToString("X2");                              //数据帧内容转换为两位16进制大写
                                        frame[10] = frame[10] + b + " ";                            //第十一列  数据帧内容
                                    }

                                    //listView_Data.Items.Add(new ListViewItem(frame));           //添加列表

                                    //listView_Data.Columns[10].Width = -1;   //设置列表每一列的宽度根据内容自适应
                                    //listView_Data.Columns[1].Width = -1;
                                #endregion

                                    frameID++;                               //数据帧序号加1
                                    Frame.Add(frame);

                                    //#region 统计画图
                                    //this.Chart_Offline.Series[0].Points.AddY(Convert.ToDouble(frame[2]));
                                    //this.Chart_Offline.Series[0].ChartType = SeriesChartType.Line;
                                    //Chart_Offline.Series[0].BorderWidth = 3;
                                    //Chart_Offline.Series[0].Color = Color.Blue;
                                    //Update();
                                    //#endregion

                                }
                            } // 当前数据帧解析结束   
                        }    //while循环结束 当前文件解析结束
                    }

                    catch
                    {
                        //空执行，随后处理-------lishuai
                    }
                }       //foreach 符合文件遍历结束
                if (Frame.Count == 0)  //不符合IP筛选的处理
                {
                    MessageBox.Show("没有找到指定的数据信息，请重新配置筛选条件！");
                    Frame.Clear();     //筛选出的数据集合清空，合理性待考虑
                }
                else
                {
                    listView_Data.Update();
                    foreach (string[] s in Frame)
                    {
                        listView_Data.Items.Add(new ListViewItem(s));
                    }
                    listView_Data.EndUpdate();
                    plot = new Thread(Plot);
                    plot.Start();
                    //show = new Thread(Show);               //////线程这一部分有问题，要再考虑
                    //show.Start();
                }
            }         //符合时间筛选结束


        }
        Thread plot;
        //Thread show;
        List<string[]> Frame = new List<string[]>();   //数据帧

        private void Plot()
        {
            #region 统计画图
            this.Invoke(new EventHandler(delegate
            {
                foreach (string[] s in Frame)
                {
                    this.Chart_Offline.Series[0].Points.AddY(Convert.ToDouble(s[2]));
                    this.Chart_Offline.Series[0].ChartType = SeriesChartType.Line;
                    Chart_Offline.Series[0].BorderWidth = 3;
                    Chart_Offline.Series[0].Color = Color.Blue;
                }
            }
            ));

            #endregion
        }
        private void Show()
        {
            #region 添加列表
            this.Invoke(new EventHandler(delegate
            {
                //listView_Data.Update();
                foreach (string[] s in Frame)
                {
                    listView_Data.Items.Add(new ListViewItem(s));
                    // t1.Interval(3000);
                }
                //listView_Data.EndUpdate();
            }
            ));

            #endregion
        }
        private void Button_OutputExcel_Click(object sender, EventArgs e)
        {
            ExportToExecl();
        }

        private void Button_Clear_Click(object sender, EventArgs e)
        {
            Clear();
            TextBox_Hex.Clear();
        }

        private void listView_Data_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            TextBox_Hex.Text = e.Item.SubItems[10].Text;
        }

        #region 导出到Excel
        private void ExportToExecl()
        {
            saveFileDialog1.Filter = "Excel文件|*.xls|所有文件|*.*";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!System.String.IsNullOrEmpty(saveFileDialog1.FileName))
                {
                    string ExcelFileName = saveFileDialog1.FileName;
                    DoExport(ExcelFileName);
                }
            }
        }

        private void DoExport(string excelFileName)
        {
            int rowNum = listView_Data.Items.Count;
            int columnNum = listView_Data.Columns.Count;
            int rowIndex = 1;
            int columnIndex = 0;
            if (rowNum == 0 || string.IsNullOrEmpty(excelFileName))
            {
                return;
            }
            if (rowNum > 0)
            {
                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
                if (xlApp == null)
                {
                    MessageBox.Show("无法创建excel对象，可能您的系统没有安装excel");
                    return;
                }
                xlApp.DefaultFilePath = "";
                xlApp.DisplayAlerts = true;
                xlApp.SheetsInNewWorkbook = 1;
                Microsoft.Office.Interop.Excel.Workbook xlBook = xlApp.Workbooks.Add(true);
                //将ListView的列名导入Excel表第一行
                foreach (ColumnHeader dc in listView_Data.Columns)
                {
                    columnIndex++;
                    xlApp.Cells[rowIndex, columnIndex] = dc.Text;
                }
                //将ListView中的数据导入Excel中
                for (int i = 0; i < rowNum; i++)
                {
                    rowIndex++;
                    columnIndex = 0;
                    for (int j = 0; j < columnNum; j++)
                    {
                        columnIndex++;
                        xlApp.Cells[rowIndex, columnIndex] = Convert.ToString(listView_Data.Items[i].SubItems[j].Text) + "\t";
                    }
                }
                xlBook.SaveAs(excelFileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                xlApp = null;
                xlBook = null;
                MessageBox.Show("OK");
            }
        }
        #endregion

        private void Clear()
        {
            Frame.Clear();   //清除数据帧
            listView_Data.Items.Clear();   //清除列表
            for (int i = 0; i < Chart_Offline.Series.Count; i++)   //清除画图
            {
                Chart_Offline.Series[i].Points.Clear();
            }
        }

        private void QueryForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            main.Show();
        }


        #region 加载IPlist并显示到两个listbox中
        private Dictionary<string, string> ReadIPlist()
        {
            Dictionary<string, string> tmpdic = new Dictionary<string, string>();
            List<string> list = new List<string>();
            string path = "IPlist.csv";
            StreamReader sr = new StreamReader(path, Encoding.Default);
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                list.Add(line);
            }
            for (int i = 1; i < list.Count; i++)
            {
                string[] strs = list[i].Split(new char[] { ',' });
                string IP = strs[1];
                tmpdic.Add(strs[0], IP);
            }
            return tmpdic;
        }

        

        #endregion

        private void listView_Data_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView_Data.SelectedIndices != null && listView_Data.SelectedIndices.Count > 0)
            {
                ListView.SelectedIndexCollection c = listView_Data.SelectedIndices;
                this.TextBox_Hex.Text = listView_Data.Items[c[0]].SubItems[10].Text;   
            }
        }

        #region 限制选择框只能选择一项
        #endregion
    
    
    }
}

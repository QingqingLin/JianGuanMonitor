namespace MonitorPorts
{
    partial class QueryForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QueryForm));
            this.Chart_Offline = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.TextBox_Hex = new System.Windows.Forms.TextBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listView_Data = new MonitorPorts.ListViewNF();
            this.ID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Time = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Interval = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Protocol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SourceIP = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SourcePort = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.DestIP = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.DestPort = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PackageLength = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MessageLength = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MessageBodyTxt = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.GroupBox_Source = new System.Windows.Forms.GroupBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label_dstPort = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label_SrcPort = new System.Windows.Forms.Label();
            this.label_SourceIP = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.endDate = new System.Windows.Forms.DateTimePicker();
            this.startHour = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.endHour = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.startDate = new System.Windows.Forms.DateTimePicker();
            this.Button_Filter = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.Button_Clear = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.Button_OutputExcel = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.Chart_Offline)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.GroupBox_Source.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // Chart_Offline
            // 
            chartArea1.Name = "ChartArea1";
            this.Chart_Offline.ChartAreas.Add(chartArea1);
            this.Chart_Offline.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            legend1.Name = "Legend1";
            this.Chart_Offline.Legends.Add(legend1);
            this.Chart_Offline.Location = new System.Drawing.Point(0, 0);
            this.Chart_Offline.Name = "Chart_Offline";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.Chart_Offline.Series.Add(series1);
            this.Chart_Offline.Size = new System.Drawing.Size(779, 266);
            this.Chart_Offline.TabIndex = 1;
            this.Chart_Offline.Text = "chart1";
            // 
            // TextBox_Hex
            // 
            this.TextBox_Hex.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBox_Hex.Location = new System.Drawing.Point(0, 0);
            this.TextBox_Hex.Multiline = true;
            this.TextBox_Hex.Name = "TextBox_Hex";
            this.TextBox_Hex.Size = new System.Drawing.Size(519, 266);
            this.TextBox_Hex.TabIndex = 1;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.Chart_Offline);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.TextBox_Hex);
            this.splitContainer2.Size = new System.Drawing.Size(1302, 266);
            this.splitContainer2.SplitterDistance = 779;
            this.splitContainer2.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 104);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listView_Data);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1302, 615);
            this.splitContainer1.SplitterDistance = 345;
            this.splitContainer1.TabIndex = 76;
            // 
            // listView_Data
            // 
            this.listView_Data.BackColor = System.Drawing.Color.LightSteelBlue;
            this.listView_Data.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ID,
            this.Time,
            this.Interval,
            this.Protocol,
            this.SourceIP,
            this.SourcePort,
            this.DestIP,
            this.DestPort,
            this.PackageLength,
            this.MessageLength,
            this.MessageBodyTxt});
            this.listView_Data.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_Data.FullRowSelect = true;
            this.listView_Data.GridLines = true;
            this.listView_Data.HideSelection = false;
            this.listView_Data.LabelWrap = false;
            this.listView_Data.Location = new System.Drawing.Point(0, 0);
            this.listView_Data.Margin = new System.Windows.Forms.Padding(0);
            this.listView_Data.MultiSelect = false;
            this.listView_Data.Name = "listView_Data";
            this.listView_Data.ShowGroups = false;
            this.listView_Data.Size = new System.Drawing.Size(1302, 345);
            this.listView_Data.TabIndex = 3;
            this.listView_Data.UseCompatibleStateImageBehavior = false;
            this.listView_Data.View = System.Windows.Forms.View.Details;
            this.listView_Data.SelectedIndexChanged += new System.EventHandler(this.listView_Data_SelectedIndexChanged);
            // 
            // ID
            // 
            this.ID.Text = "NO.";
            this.ID.Width = 39;
            // 
            // Time
            // 
            this.Time.Text = "时间";
            this.Time.Width = 115;
            // 
            // Interval
            // 
            this.Interval.Text = "接收间隔";
            // 
            // Protocol
            // 
            this.Protocol.Text = "协议";
            // 
            // SourceIP
            // 
            this.SourceIP.Text = "源IP地址";
            this.SourceIP.Width = 127;
            // 
            // SourcePort
            // 
            this.SourcePort.Text = "源端口号";
            this.SourcePort.Width = 79;
            // 
            // DestIP
            // 
            this.DestIP.Text = "目的IP地址";
            this.DestIP.Width = 150;
            // 
            // DestPort
            // 
            this.DestPort.Text = "目的端口号";
            this.DestPort.Width = 79;
            // 
            // PackageLength
            // 
            this.PackageLength.Text = "数据包长度";
            this.PackageLength.Width = 90;
            // 
            // MessageLength
            // 
            this.MessageLength.Text = "消息长度";
            this.MessageLength.Width = 90;
            // 
            // MessageBodyTxt
            // 
            this.MessageBodyTxt.Text = "消息";
            this.MessageBodyTxt.Width = 683;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "xls";
            this.saveFileDialog1.Title = "保存";
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.CornflowerBlue;
            this.statusStrip1.Location = new System.Drawing.Point(0, 719);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1302, 22);
            this.statusStrip1.TabIndex = 74;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // GroupBox_Source
            // 
            this.GroupBox_Source.Controls.Add(this.textBox4);
            this.GroupBox_Source.Controls.Add(this.textBox2);
            this.GroupBox_Source.Controls.Add(this.textBox3);
            this.GroupBox_Source.Controls.Add(this.textBox1);
            this.GroupBox_Source.Controls.Add(this.label_dstPort);
            this.GroupBox_Source.Controls.Add(this.label5);
            this.GroupBox_Source.Controls.Add(this.label_SrcPort);
            this.GroupBox_Source.Controls.Add(this.label_SourceIP);
            this.GroupBox_Source.ForeColor = System.Drawing.SystemColors.Control;
            this.GroupBox_Source.Location = new System.Drawing.Point(12, 3);
            this.GroupBox_Source.Name = "GroupBox_Source";
            this.GroupBox_Source.Size = new System.Drawing.Size(296, 98);
            this.GroupBox_Source.TabIndex = 78;
            this.GroupBox_Source.TabStop = false;
            this.GroupBox_Source.Text = "IP地址查找";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(128, 74);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(88, 21);
            this.textBox4.TabIndex = 1;
            this.textBox4.Text = "13245";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(12, 73);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(88, 21);
            this.textBox2.TabIndex = 1;
            this.textBox2.Text = "192.168.11.11";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(128, 33);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(88, 21);
            this.textBox3.TabIndex = 1;
            this.textBox3.Text = "13245";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(11, 33);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(88, 21);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "30.30.0.40";
            // 
            // label_dstPort
            // 
            this.label_dstPort.AutoSize = true;
            this.label_dstPort.ForeColor = System.Drawing.Color.Black;
            this.label_dstPort.Location = new System.Drawing.Point(126, 58);
            this.label_dstPort.Name = "label_dstPort";
            this.label_dstPort.Size = new System.Drawing.Size(65, 12);
            this.label_dstPort.TabIndex = 0;
            this.label_dstPort.Text = "目的端口：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(10, 58);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "目的IP：";
            // 
            // label_SrcPort
            // 
            this.label_SrcPort.AutoSize = true;
            this.label_SrcPort.ForeColor = System.Drawing.Color.Black;
            this.label_SrcPort.Location = new System.Drawing.Point(126, 17);
            this.label_SrcPort.Name = "label_SrcPort";
            this.label_SrcPort.Size = new System.Drawing.Size(53, 12);
            this.label_SrcPort.TabIndex = 0;
            this.label_SrcPort.Text = "源端口：";
            // 
            // label_SourceIP
            // 
            this.label_SourceIP.AutoSize = true;
            this.label_SourceIP.ForeColor = System.Drawing.Color.Black;
            this.label_SourceIP.Location = new System.Drawing.Point(10, 17);
            this.label_SourceIP.Name = "label_SourceIP";
            this.label_SourceIP.Size = new System.Drawing.Size(41, 12);
            this.label_SourceIP.TabIndex = 0;
            this.label_SourceIP.Text = "源IP：";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.endDate);
            this.groupBox1.Controls.Add(this.startHour);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.endHour);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.startDate);
            this.groupBox1.ForeColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Location = new System.Drawing.Point(323, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(276, 98);
            this.groupBox1.TabIndex = 79;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "时间查找";
            // 
            // endDate
            // 
            this.endDate.Location = new System.Drawing.Point(69, 63);
            this.endDate.Name = "endDate";
            this.endDate.Size = new System.Drawing.Size(107, 21);
            this.endDate.TabIndex = 59;
            this.endDate.Value = new System.DateTime(2017, 6, 6, 20, 55, 0, 0);
            // 
            // startHour
            // 
            this.startHour.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.startHour.Location = new System.Drawing.Point(191, 29);
            this.startHour.Name = "startHour";
            this.startHour.ShowUpDown = true;
            this.startHour.Size = new System.Drawing.Size(75, 21);
            this.startHour.TabIndex = 61;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(11, 69);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 63;
            this.label6.Text = "终止时间";
            // 
            // endHour
            // 
            this.endHour.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.endHour.Location = new System.Drawing.Point(191, 63);
            this.endHour.Name = "endHour";
            this.endHour.ShowUpDown = true;
            this.endHour.Size = new System.Drawing.Size(75, 21);
            this.endHour.TabIndex = 58;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(11, 35);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 63;
            this.label7.Text = "起始时间";
            // 
            // startDate
            // 
            this.startDate.Location = new System.Drawing.Point(70, 29);
            this.startDate.Name = "startDate";
            this.startDate.Size = new System.Drawing.Size(106, 21);
            this.startDate.TabIndex = 57;
            this.startDate.Value = new System.DateTime(2017, 4, 20, 20, 55, 0, 0);
            // 
            // Button_Filter
            // 
            this.Button_Filter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(251)))));
            this.Button_Filter.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Button_Filter.BackgroundImage")));
            this.Button_Filter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Button_Filter.FlatAppearance.BorderSize = 0;
            this.Button_Filter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_Filter.ForeColor = System.Drawing.SystemColors.Control;
            this.Button_Filter.Location = new System.Drawing.Point(628, 23);
            this.Button_Filter.Name = "Button_Filter";
            this.Button_Filter.Size = new System.Drawing.Size(113, 56);
            this.Button_Filter.TabIndex = 80;
            this.Button_Filter.UseVisualStyleBackColor = false;
            this.Button_Filter.Click += new System.EventHandler(this.Button_Filter_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(671, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 81;
            this.label3.Text = "筛选";
            // 
            // Button_Clear
            // 
            this.Button_Clear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(251)))));
            this.Button_Clear.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Button_Clear.BackgroundImage")));
            this.Button_Clear.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Button_Clear.FlatAppearance.BorderSize = 0;
            this.Button_Clear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_Clear.ForeColor = System.Drawing.SystemColors.Control;
            this.Button_Clear.Location = new System.Drawing.Point(747, 23);
            this.Button_Clear.Name = "Button_Clear";
            this.Button_Clear.Size = new System.Drawing.Size(113, 56);
            this.Button_Clear.TabIndex = 82;
            this.Button_Clear.UseVisualStyleBackColor = false;
            this.Button_Clear.Click += new System.EventHandler(this.Button_Clear_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(789, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 83;
            this.label4.Text = "清除";
            // 
            // Button_OutputExcel
            // 
            this.Button_OutputExcel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(251)))));
            this.Button_OutputExcel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Button_OutputExcel.BackgroundImage")));
            this.Button_OutputExcel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Button_OutputExcel.FlatAppearance.BorderSize = 0;
            this.Button_OutputExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_OutputExcel.ForeColor = System.Drawing.SystemColors.Control;
            this.Button_OutputExcel.Location = new System.Drawing.Point(876, 23);
            this.Button_OutputExcel.Name = "Button_OutputExcel";
            this.Button_OutputExcel.Size = new System.Drawing.Size(113, 56);
            this.Button_OutputExcel.TabIndex = 84;
            this.Button_OutputExcel.UseVisualStyleBackColor = false;
            this.Button_OutputExcel.Click += new System.EventHandler(this.Button_OutputExcel_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(904, 82);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 12);
            this.label8.TabIndex = 85;
            this.label8.Text = "导出Excel";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(159)))), ((int)(((byte)(251)))));
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.Button_OutputExcel);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.Button_Clear);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.Button_Filter);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Controls.Add(this.GroupBox_Source);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1302, 104);
            this.panel2.TabIndex = 75;
            // 
            // QueryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1302, 741);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.statusStrip1);
            this.Name = "QueryForm";
            this.Text = "离线查询";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.QueryForm_FormClosed);
            this.Load += new System.EventHandler(this.QueryForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Chart_Offline)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.GroupBox_Source.ResumeLayout(false);
            this.GroupBox_Source.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColumnHeader MessageBodyTxt;
        private System.Windows.Forms.ColumnHeader PackageLength;
        private System.Windows.Forms.ColumnHeader DestPort;
        private System.Windows.Forms.ColumnHeader DestIP;
        private System.Windows.Forms.ColumnHeader SourcePort;
        private System.Windows.Forms.ColumnHeader SourceIP;
        private System.Windows.Forms.ColumnHeader Protocol;
        private System.Windows.Forms.ColumnHeader Interval;
        private System.Windows.Forms.ColumnHeader Time;
        private System.Windows.Forms.ColumnHeader ID;
        private ListViewNF listView_Data;
        private System.Windows.Forms.ColumnHeader MessageLength;
        private System.Windows.Forms.DataVisualization.Charting.Chart Chart_Offline;
        private System.Windows.Forms.TextBox TextBox_Hex;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.GroupBox GroupBox_Source;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label_SourceIP;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DateTimePicker endDate;
        private System.Windows.Forms.DateTimePicker startHour;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker endHour;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DateTimePicker startDate;
        private System.Windows.Forms.Button Button_Filter;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button Button_Clear;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button Button_OutputExcel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label_dstPort;
        private System.Windows.Forms.Label label_SrcPort;
        private System.Windows.Forms.TextBox textBox4;
    }
}
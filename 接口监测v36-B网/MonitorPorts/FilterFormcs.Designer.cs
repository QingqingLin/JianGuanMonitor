namespace MonitorPorts
{
    partial class FilterForm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.SavePath_CBTC = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtATSToVOBC = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtVOBCToATS = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCIToVOBC = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtVOBCToCI = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.txtZCToVOBC = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtVOBCToZC = new System.Windows.Forms.TextBox();
            this.GroupBox_Source = new System.Windows.Forms.GroupBox();
            this.DestSelectAll = new System.Windows.Forms.CheckBox();
            this.SourceSelectAll = new System.Windows.Forms.CheckBox();
            this.DestListBox2 = new System.Windows.Forms.CheckedListBox();
            this.SourceListBox1 = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label_SourceIP = new System.Windows.Forms.Label();
            this.GroupBox_Protocol = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtPcapLength = new System.Windows.Forms.TextBox();
            this.checkBox__CBTC = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.SavePath_LTE = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label17 = new System.Windows.Forms.Label();
            this.Lte_Length = new System.Windows.Forms.TextBox();
            this.checkBox_Signal = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.button_Signal_Cancel = new System.Windows.Forms.Button();
            this.button_Signal_OK = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ProtocolSelectAll = new System.Windows.Forms.CheckBox();
            this.CheckBox_S1AP = new System.Windows.Forms.CheckBox();
            this.CheckBox_DIAMETER = new System.Windows.Forms.CheckBox();
            this.CheckBox_GTPv2 = new System.Windows.Forms.CheckBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.GroupBox_Source.SuspendLayout();
            this.GroupBox_Protocol.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(1);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel1.Controls.Add(this.groupBox3);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1.Controls.Add(this.GroupBox_Source);
            this.splitContainer1.Panel1.Controls.Add(this.GroupBox_Protocol);
            this.splitContainer1.Panel1.Controls.Add(this.checkBox__CBTC);
            this.splitContainer1.Panel1.Controls.Add(this.label7);
            this.splitContainer1.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer1_Panel1_Paint);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox4);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox5);
            this.splitContainer1.Panel2.Controls.Add(this.checkBox_Signal);
            this.splitContainer1.Panel2.Controls.Add(this.label8);
            this.splitContainer1.Panel2.Controls.Add(this.button_Signal_Cancel);
            this.splitContainer1.Panel2.Controls.Add(this.button_Signal_OK);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer1_Panel2_Paint);
            this.splitContainer1.Size = new System.Drawing.Size(520, 651);
            this.splitContainer1.SplitterDistance = 432;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.SavePath_CBTC);
            this.groupBox3.Location = new System.Drawing.Point(189, 49);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(319, 45);
            this.groupBox3.TabIndex = 19;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "保存文件位置";
            // 
            // SavePath_CBTC
            // 
            this.SavePath_CBTC.Location = new System.Drawing.Point(6, 17);
            this.SavePath_CBTC.Multiline = true;
            this.SavePath_CBTC.Name = "SavePath_CBTC";
            this.SavePath_CBTC.ReadOnly = true;
            this.SavePath_CBTC.Size = new System.Drawing.Size(307, 21);
            this.SavePath_CBTC.TabIndex = 0;
            this.SavePath_CBTC.Text = "10";
            this.SavePath_CBTC.TextChanged += new System.EventHandler(this.SavePath_CBTC_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.txtATSToVOBC);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.txtVOBCToATS);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtCIToVOBC);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txtVOBCToCI);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.txtZCToVOBC);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.txtVOBCToZC);
            this.groupBox1.Location = new System.Drawing.Point(15, 305);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(452, 122);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "告警门限";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(232, 84);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(83, 12);
            this.label10.TabIndex = 10;
            this.label10.Text = "ATS发送给VOBC";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(381, 84);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 12);
            this.label11.TabIndex = 11;
            this.label11.Text = "单位（ms）";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 84);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(83, 12);
            this.label12.TabIndex = 12;
            this.label12.Text = "VOBC发送给ATS";
            // 
            // txtATSToVOBC
            // 
            this.txtATSToVOBC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtATSToVOBC.Location = new System.Drawing.Point(317, 80);
            this.txtATSToVOBC.Name = "txtATSToVOBC";
            this.txtATSToVOBC.Size = new System.Drawing.Size(56, 21);
            this.txtATSToVOBC.TabIndex = 8;
            this.txtATSToVOBC.Text = "2400";
            this.txtATSToVOBC.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(161, 84);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(65, 12);
            this.label13.TabIndex = 13;
            this.label13.Text = "单位（ms）";
            // 
            // txtVOBCToATS
            // 
            this.txtVOBCToATS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtVOBCToATS.Location = new System.Drawing.Point(97, 80);
            this.txtVOBCToATS.Name = "txtVOBCToATS";
            this.txtVOBCToATS.Size = new System.Drawing.Size(56, 21);
            this.txtVOBCToATS.TabIndex = 9;
            this.txtVOBCToATS.Text = "2400";
            this.txtVOBCToATS.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(234, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "CI发送给VOBC";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(381, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "单位（ms）";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "VOBC发送给CI";
            // 
            // txtCIToVOBC
            // 
            this.txtCIToVOBC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCIToVOBC.Location = new System.Drawing.Point(317, 50);
            this.txtCIToVOBC.Name = "txtCIToVOBC";
            this.txtCIToVOBC.Size = new System.Drawing.Size(56, 21);
            this.txtCIToVOBC.TabIndex = 2;
            this.txtCIToVOBC.Text = "2400";
            this.txtCIToVOBC.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(160, 54);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 7;
            this.label9.Text = "单位（ms）";
            // 
            // txtVOBCToCI
            // 
            this.txtVOBCToCI.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtVOBCToCI.Location = new System.Drawing.Point(96, 50);
            this.txtVOBCToCI.Name = "txtVOBCToCI";
            this.txtVOBCToCI.Size = new System.Drawing.Size(56, 21);
            this.txtVOBCToCI.TabIndex = 3;
            this.txtVOBCToCI.Text = "2400";
            this.txtVOBCToCI.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(235, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "ZC发送给VOBC";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(382, 22);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(65, 12);
            this.label14.TabIndex = 1;
            this.label14.Text = "单位（ms）";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(14, 23);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(77, 12);
            this.label15.TabIndex = 1;
            this.label15.Text = "VOBC发送给ZC";
            // 
            // txtZCToVOBC
            // 
            this.txtZCToVOBC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtZCToVOBC.Location = new System.Drawing.Point(318, 18);
            this.txtZCToVOBC.Name = "txtZCToVOBC";
            this.txtZCToVOBC.Size = new System.Drawing.Size(56, 21);
            this.txtZCToVOBC.TabIndex = 0;
            this.txtZCToVOBC.Text = "2400";
            this.txtZCToVOBC.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(161, 23);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(65, 12);
            this.label16.TabIndex = 1;
            this.label16.Text = "单位（ms）";
            // 
            // txtVOBCToZC
            // 
            this.txtVOBCToZC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtVOBCToZC.Location = new System.Drawing.Point(97, 19);
            this.txtVOBCToZC.Name = "txtVOBCToZC";
            this.txtVOBCToZC.Size = new System.Drawing.Size(56, 21);
            this.txtVOBCToZC.TabIndex = 0;
            this.txtVOBCToZC.Text = "2400";
            this.txtVOBCToZC.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // GroupBox_Source
            // 
            this.GroupBox_Source.Controls.Add(this.DestSelectAll);
            this.GroupBox_Source.Controls.Add(this.SourceSelectAll);
            this.GroupBox_Source.Controls.Add(this.DestListBox2);
            this.GroupBox_Source.Controls.Add(this.SourceListBox1);
            this.GroupBox_Source.Controls.Add(this.label1);
            this.GroupBox_Source.Controls.Add(this.label_SourceIP);
            this.GroupBox_Source.Location = new System.Drawing.Point(15, 109);
            this.GroupBox_Source.Name = "GroupBox_Source";
            this.GroupBox_Source.Size = new System.Drawing.Size(279, 188);
            this.GroupBox_Source.TabIndex = 17;
            this.GroupBox_Source.TabStop = false;
            this.GroupBox_Source.Text = "IP地址筛选";
            // 
            // DestSelectAll
            // 
            this.DestSelectAll.AutoSize = true;
            this.DestSelectAll.Location = new System.Drawing.Point(150, 42);
            this.DestSelectAll.Name = "DestSelectAll";
            this.DestSelectAll.Size = new System.Drawing.Size(48, 16);
            this.DestSelectAll.TabIndex = 7;
            this.DestSelectAll.Text = "全选";
            this.DestSelectAll.UseVisualStyleBackColor = true;
            this.DestSelectAll.CheckedChanged += new System.EventHandler(this.DestSelectAll_CheckedChanged);
            // 
            // SourceSelectAll
            // 
            this.SourceSelectAll.AutoSize = true;
            this.SourceSelectAll.Location = new System.Drawing.Point(17, 42);
            this.SourceSelectAll.Name = "SourceSelectAll";
            this.SourceSelectAll.Size = new System.Drawing.Size(48, 16);
            this.SourceSelectAll.TabIndex = 6;
            this.SourceSelectAll.Text = "全选";
            this.SourceSelectAll.UseVisualStyleBackColor = true;
            this.SourceSelectAll.CheckedChanged += new System.EventHandler(this.SourceSelectAll_CheckedChanged);
            // 
            // DestListBox2
            // 
            this.DestListBox2.FormattingEnabled = true;
            this.DestListBox2.Location = new System.Drawing.Point(147, 61);
            this.DestListBox2.Name = "DestListBox2";
            this.DestListBox2.Size = new System.Drawing.Size(123, 116);
            this.DestListBox2.TabIndex = 5;
            this.DestListBox2.SelectedIndexChanged += new System.EventHandler(this.DestListBox2_SelectedIndexChanged);
            // 
            // SourceListBox1
            // 
            this.SourceListBox1.FormattingEnabled = true;
            this.SourceListBox1.Location = new System.Drawing.Point(14, 61);
            this.SourceListBox1.Name = "SourceListBox1";
            this.SourceListBox1.Size = new System.Drawing.Size(123, 116);
            this.SourceListBox1.TabIndex = 8;
            this.SourceListBox1.SelectedIndexChanged += new System.EventHandler(this.SourceListBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(150, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "目的IP：";
            // 
            // label_SourceIP
            // 
            this.label_SourceIP.AutoSize = true;
            this.label_SourceIP.Location = new System.Drawing.Point(14, 23);
            this.label_SourceIP.Name = "label_SourceIP";
            this.label_SourceIP.Size = new System.Drawing.Size(41, 12);
            this.label_SourceIP.TabIndex = 0;
            this.label_SourceIP.Text = "源IP：";
            // 
            // GroupBox_Protocol
            // 
            this.GroupBox_Protocol.Controls.Add(this.label6);
            this.GroupBox_Protocol.Controls.Add(this.txtPcapLength);
            this.GroupBox_Protocol.Location = new System.Drawing.Point(9, 49);
            this.GroupBox_Protocol.Name = "GroupBox_Protocol";
            this.GroupBox_Protocol.Size = new System.Drawing.Size(168, 45);
            this.GroupBox_Protocol.TabIndex = 16;
            this.GroupBox_Protocol.TabStop = false;
            this.GroupBox_Protocol.Text = "保存文件大小";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(119, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(28, 17);
            this.label6.TabIndex = 1;
            this.label6.Text = "MB";
            // 
            // txtPcapLength
            // 
            this.txtPcapLength.Location = new System.Drawing.Point(26, 17);
            this.txtPcapLength.Name = "txtPcapLength";
            this.txtPcapLength.Size = new System.Drawing.Size(75, 21);
            this.txtPcapLength.TabIndex = 0;
            this.txtPcapLength.Text = "10";
            this.txtPcapLength.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPcapLength_KeyPress);
            // 
            // checkBox__CBTC
            // 
            this.checkBox__CBTC.AutoSize = true;
            this.checkBox__CBTC.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.checkBox__CBTC.Location = new System.Drawing.Point(167, 19);
            this.checkBox__CBTC.Name = "checkBox__CBTC";
            this.checkBox__CBTC.Size = new System.Drawing.Size(15, 14);
            this.checkBox__CBTC.TabIndex = 14;
            this.checkBox__CBTC.UseVisualStyleBackColor = false;
            this.checkBox__CBTC.CheckedChanged += new System.EventHandler(this.checkBox__CBTC_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.SystemColors.Control;
            this.label7.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(10, 12);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(154, 24);
            this.label7.TabIndex = 13;
            this.label7.Text = "CBTC数据监测";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.SavePath_LTE);
            this.groupBox4.Location = new System.Drawing.Point(189, 50);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(319, 45);
            this.groupBox4.TabIndex = 21;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "保存文件位置";
            // 
            // SavePath_LTE
            // 
            this.SavePath_LTE.Location = new System.Drawing.Point(6, 16);
            this.SavePath_LTE.Name = "SavePath_LTE";
            this.SavePath_LTE.ReadOnly = true;
            this.SavePath_LTE.Size = new System.Drawing.Size(307, 21);
            this.SavePath_LTE.TabIndex = 0;
            this.SavePath_LTE.Text = "10";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label17);
            this.groupBox5.Controls.Add(this.Lte_Length);
            this.groupBox5.Location = new System.Drawing.Point(9, 49);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(168, 46);
            this.groupBox5.TabIndex = 20;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "保存文件大小";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(119, 24);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(28, 17);
            this.label17.TabIndex = 2;
            this.label17.Text = "MB";
            // 
            // Lte_Length
            // 
            this.Lte_Length.Location = new System.Drawing.Point(22, 20);
            this.Lte_Length.Name = "Lte_Length";
            this.Lte_Length.Size = new System.Drawing.Size(75, 21);
            this.Lte_Length.TabIndex = 0;
            this.Lte_Length.Text = "10";
            this.Lte_Length.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Lte_Length_KeyPress);
            // 
            // checkBox_Signal
            // 
            this.checkBox_Signal.AutoSize = true;
            this.checkBox_Signal.Location = new System.Drawing.Point(178, 20);
            this.checkBox_Signal.Name = "checkBox_Signal";
            this.checkBox_Signal.Size = new System.Drawing.Size(15, 14);
            this.checkBox_Signal.TabIndex = 15;
            this.checkBox_Signal.UseVisualStyleBackColor = true;
            this.checkBox_Signal.CheckedChanged += new System.EventHandler(this.checkBox_Signal_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.SystemColors.Control;
            this.label8.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(11, 15);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(166, 24);
            this.label8.TabIndex = 14;
            this.label8.Text = "LTE-M信令监测";
            // 
            // button_Signal_Cancel
            // 
            this.button_Signal_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Signal_Cancel.Location = new System.Drawing.Point(357, 177);
            this.button_Signal_Cancel.Name = "button_Signal_Cancel";
            this.button_Signal_Cancel.Size = new System.Drawing.Size(75, 23);
            this.button_Signal_Cancel.TabIndex = 12;
            this.button_Signal_Cancel.Text = "取消";
            this.button_Signal_Cancel.UseVisualStyleBackColor = true;
            this.button_Signal_Cancel.Click += new System.EventHandler(this.button_Signal_Cancel_Click);
            // 
            // button_Signal_OK
            // 
            this.button_Signal_OK.Location = new System.Drawing.Point(269, 177);
            this.button_Signal_OK.Name = "button_Signal_OK";
            this.button_Signal_OK.Size = new System.Drawing.Size(75, 23);
            this.button_Signal_OK.TabIndex = 11;
            this.button_Signal_OK.Text = "确定";
            this.button_Signal_OK.UseVisualStyleBackColor = true;
            this.button_Signal_OK.Click += new System.EventHandler(this.button_Signal_OK_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ProtocolSelectAll);
            this.groupBox2.Controls.Add(this.CheckBox_S1AP);
            this.groupBox2.Controls.Add(this.CheckBox_DIAMETER);
            this.groupBox2.Controls.Add(this.CheckBox_GTPv2);
            this.groupBox2.Location = new System.Drawing.Point(9, 109);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(400, 58);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "协议筛选";
            // 
            // ProtocolSelectAll
            // 
            this.ProtocolSelectAll.AutoSize = true;
            this.ProtocolSelectAll.Location = new System.Drawing.Point(70, 0);
            this.ProtocolSelectAll.Name = "ProtocolSelectAll";
            this.ProtocolSelectAll.Size = new System.Drawing.Size(48, 16);
            this.ProtocolSelectAll.TabIndex = 9;
            this.ProtocolSelectAll.Text = "全选";
            this.ProtocolSelectAll.UseVisualStyleBackColor = true;
            this.ProtocolSelectAll.CheckedChanged += new System.EventHandler(this.ProtocolSelectAll_CheckedChanged);
            // 
            // CheckBox_S1AP
            // 
            this.CheckBox_S1AP.AutoSize = true;
            this.CheckBox_S1AP.Location = new System.Drawing.Point(177, 30);
            this.CheckBox_S1AP.Name = "CheckBox_S1AP";
            this.CheckBox_S1AP.Size = new System.Drawing.Size(48, 16);
            this.CheckBox_S1AP.TabIndex = 2;
            this.CheckBox_S1AP.Text = "S1AP";
            this.CheckBox_S1AP.UseVisualStyleBackColor = true;
            this.CheckBox_S1AP.CheckedChanged += new System.EventHandler(this.CheckBox_S1AP_CheckedChanged);
            // 
            // CheckBox_DIAMETER
            // 
            this.CheckBox_DIAMETER.AutoSize = true;
            this.CheckBox_DIAMETER.Location = new System.Drawing.Point(90, 30);
            this.CheckBox_DIAMETER.Name = "CheckBox_DIAMETER";
            this.CheckBox_DIAMETER.Size = new System.Drawing.Size(72, 16);
            this.CheckBox_DIAMETER.TabIndex = 1;
            this.CheckBox_DIAMETER.Text = "DIAMETER";
            this.CheckBox_DIAMETER.UseVisualStyleBackColor = true;
            this.CheckBox_DIAMETER.CheckedChanged += new System.EventHandler(this.CheckBox_DIAMETER_CheckedChanged);
            // 
            // CheckBox_GTPv2
            // 
            this.CheckBox_GTPv2.AutoSize = true;
            this.CheckBox_GTPv2.Location = new System.Drawing.Point(21, 30);
            this.CheckBox_GTPv2.Name = "CheckBox_GTPv2";
            this.CheckBox_GTPv2.Size = new System.Drawing.Size(54, 16);
            this.CheckBox_GTPv2.TabIndex = 0;
            this.CheckBox_GTPv2.Text = "GTPV2";
            this.CheckBox_GTPv2.UseVisualStyleBackColor = true;
            this.CheckBox_GTPv2.CheckedChanged += new System.EventHandler(this.CheckBox_GTPv2_CheckedChanged);
            // 
            // FilterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 651);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FilterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "筛选设置";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FilterForm_FormClosing);
            this.Load += new System.EventHandler(this.FilterForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.GroupBox_Source.ResumeLayout(false);
            this.GroupBox_Source.PerformLayout();
            this.GroupBox_Protocol.ResumeLayout(false);
            this.GroupBox_Protocol.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox CheckBox_S1AP;
        private System.Windows.Forms.CheckBox CheckBox_DIAMETER;
        private System.Windows.Forms.CheckBox CheckBox_GTPv2;
        private System.Windows.Forms.Button button_Signal_Cancel;
        private System.Windows.Forms.Button button_Signal_OK;
        private System.Windows.Forms.Label label7;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.CheckBox checkBox__CBTC;
        private System.Windows.Forms.GroupBox GroupBox_Source;
        private System.Windows.Forms.CheckBox DestSelectAll;
        private System.Windows.Forms.CheckBox SourceSelectAll;
        private System.Windows.Forms.CheckedListBox DestListBox2;
        private System.Windows.Forms.CheckedListBox SourceListBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_SourceIP;
        private System.Windows.Forms.GroupBox GroupBox_Protocol;
        private System.Windows.Forms.TextBox txtPcapLength;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtATSToVOBC;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtVOBCToATS;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtCIToVOBC;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtVOBCToCI;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtZCToVOBC;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.CheckBox checkBox_Signal;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtVOBCToZC;
        private System.Windows.Forms.CheckBox ProtocolSelectAll;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox SavePath_CBTC;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox SavePath_LTE;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox Lte_Length;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label17;
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MonitorPorts
{
    class CatchSocket
    {
        public bool KeepCatching;
        public GeneratePcapFile genePcapFile;
        public GeneratePcapFile genePcapFile_Signaling;
        private static int receiveBufferLength;
        private byte[] receiveBufferBytes;
        private Socket socket;
        private string _fileSavePath;
        private string _fileName;
        private string _fileSavePath_Signaling;
        private string _fileName_Signaling;
        //private delegate void UpdateState();
        //UpdateState Update;
        //public HandlePacket Handle = new HandlePacket();
        bool bStatus_Signal = false;//是否进行信号监测的标志位，FALSE表示否
        bool bStatus_Signaling = false;//是否进行信令监测的标志位，FALSE表示否
        public static List<PacketProperties> CBTCTemporaryStorage;
        public static List<PacketProperties> LTETemporaryStorage;


        public string FileSavePath
        {
            get { return _fileSavePath; }
            set { _fileSavePath = value; }
        }
        public string FileSavePath_Signaling
        {
            get { return _fileSavePath_Signaling; }
            set { _fileSavePath_Signaling = value; }
        }
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }
        public string FileName_Signaling
        {
            get { return _fileName_Signaling; }
            set { _fileName_Signaling = value; }
        }
        public CatchSocket(MainForm Main)
        {
            CreatPcapFile(System.DateTime.Now);       //林庆庆
            CreatPcapFile_Signaling(System.DateTime.Now);      //林庆庆

            HandlePacket.PacketArrival += Main.ShowInForm;
            receiveBufferLength = 1500;
            receiveBufferBytes = new byte[receiveBufferLength];
        }

        private void LTEOutput(List<PacketProperties> outputToPcapList)        //方法增加林庆庆
        {
            FileInfo File = new FileInfo(FileSavePath_Signaling + "\\" + FileName_Signaling + ".pcap");
            lock (outputToPcapList)
            {
                try
                {
                    foreach (PacketProperties Properties in outputToPcapList)
                    {
                        if (File.Length < FilterForm.PcapLengthLte)
                        {
                            WriteLTEToPcap(Properties.Buf, Properties.BufLength, Properties.CaptureTime);
                        }
                        else
                        {
                            CreatPcapFile_Signaling(Properties.CaptureTime);
                            File = new FileInfo(FileSavePath_Signaling + "\\" + FileName_Signaling + ".pcap");
                            WriteLTEToPcap(Properties.Buf, Properties.BufLength, Properties.CaptureTime);
                        }
                    }
                }
                catch (Exception)
                {
                }
                outputToPcapList.Clear();
            }
        }

        private void CBTCOutput(List<PacketProperties> outputToPcapList)         //方法增加林庆庆
        {
            FileInfo File = new FileInfo(FileSavePath + "\\" + FileName + ".pcap");
            lock (outputToPcapList)
            {
                try
                {
                    foreach (PacketProperties Properties in outputToPcapList)
                    {
                        if (File.Length < FilterForm.PcapLengthNum)
                        {
                            WriteToPcap(Properties.Buf, Properties.BufLength, Properties.CaptureTime);
                        }
                        else
                        {
                            CreatPcapFile(Properties.CaptureTime);
                            File = new FileInfo(FileSavePath + "\\" + FileName + ".pcap");
                            WriteToPcap(Properties.Buf, Properties.BufLength, Properties.CaptureTime);
                        }
                    }
                }
                catch (Exception)
                {
                }
                outputToPcapList.Clear();
            }
        }

        private void WriteToPcap(byte[] packet, int packetlenth, DateTime CaptureTime)
        {
            genePcapFile.WritePacketData(CaptureTime, packet, packetlenth);
        }

        private void WriteLTEToPcap(byte[] packet, int packetlenth, DateTime CaptureTime)
        {
            genePcapFile_Signaling.WritePacketData(CaptureTime, packet, packetlenth);
        }

        public void CreatPcapFile(DateTime CreatTime)//创建信号pcap文件         //林庆庆
        {
            genePcapFile = new GeneratePcapFile();
            string fileName = "\\CBTC";//信号pcap文件夹的名称
            FileName = CreatTime.ToString("yyyy") + "." + CreatTime.ToString("MM") + "." + CreatTime.ToString("dd") + "  " + CreatTime.ToString("HH：mm");      //林庆庆
            FileSavePath = SetFileSavePath(fileName, CreatTime);
            if (File.Exists(FileSavePath + "\\" + FileName + ".pcap"))
            {
                File.Delete(FileSavePath + "\\" + FileName + ".pcap");
                genePcapFile.CreatPcap(FileSavePath, FileName);
            }
            else
            {
                genePcapFile.CreatPcap(FileSavePath, FileName);
            }
        }

        public void CreatPcapFile_Signaling(DateTime CreatTime)//创建信令pcap文件     //林庆庆
        {
            genePcapFile_Signaling = new GeneratePcapFile();
            string fileName_Signaling = "\\LTE-M";//信令pcap文件夹的名称
            FileName_Signaling = CreatTime.ToString("yyyy") + "." + CreatTime.ToString("MM") + "." + CreatTime.ToString("dd") + "  " + CreatTime.ToString("HH：mm");      //林庆庆
            FileSavePath_Signaling = SetFileSavePath(fileName_Signaling, CreatTime);
            if (File.Exists(FileSavePath_Signaling + "\\" + FileName_Signaling + ".pcap"))
            {
                File.Delete(FileSavePath_Signaling + "\\" + FileName_Signaling + ".pcap");
                genePcapFile_Signaling.CreatPcap(FileSavePath_Signaling, FileName_Signaling);
            }
            else
            {
                genePcapFile_Signaling.CreatPcap(FileSavePath_Signaling, FileName_Signaling);
            }
        }

        private string SetFileSavePath(string fileName, DateTime CreatTime)      //林庆庆
        {
            string startPath = Application.StartupPath + fileName;
            string FilePath = startPath + "\\" + CreatTime.Year + "\\" + CreatTime.Month.ToString().PadLeft(2,'0') + "月" + "\\" + CreatTime.Day.ToString().PadLeft(2,'0') + "日";
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            return FilePath;
        }

        private void SetShowStorge()
        {
            if (ShowThread.ShowStorate == null)
            {
                ShowThread.ShowStorate = ShowThread.GUIStorage1;
            }
        }

        private void SetCBTCTemporaryStorge()
        {
            if (CBTCTemporaryStorage == null)
            {
                CBTCTemporaryStorage = MainForm.CBTCTemporaryStorage1;
            }
        }

        private void SetLTETemporaryStorge()
        {
            if (LTETemporaryStorage == null)
            {
                LTETemporaryStorage = MainForm.LTETemporaryStorage1;
            }
        }

        public void Start(bool bStatues_CBTC, bool bStatues_Signal, Socket socket)
        {
            if (MainForm.IsFirstClick)
            {
                bStatus_Signal = bStatues_CBTC;//信号的状态
                bStatus_Signaling = bStatues_Signal;//信令的状态
                SetCBTCTemporaryStorge();
                SetLTETemporaryStorge();
                SetShowStorge();
                if (bStatues_Signal)
                {
                    MainForm.ShowSignalingTimer.Change(200, 200);
                    MainForm.OutputCBTCDataTimer.Change(1000, 5000);
                }
                if (bStatus_Signaling)
                {
                    MainForm.OutputLTEDataTimer.Change(1000, 5000);
                }
                this.socket = socket;
                KeepCatching = true;
                BeginReceive();
                MainForm.IsFirstClick = false;
            }
            else
            {
                MainForm.ShowSignalingTimer.Change(200, 200);
                HandlePacket.Pause = false;
            }

        }

        public void BeginReceive()
        {
            if (socket != null)
            {
                object state = null;
                state = socket;
                try                                            //lishuai
                {
                    IAsyncResult ar = socket.BeginReceive(receiveBufferBytes, 0, receiveBufferLength, SocketFlags.None, new AsyncCallback(CallReceive), state);
                }
                catch (Exception e)
                {
                }

            }
        }

        HandlePacket handle;
        private void CallReceive(IAsyncResult ar)
        {
            try
            {
                int receivedBytes = socket.EndReceive(ar);
                if (KeepCatching == true)
                {
                    handle = new HandlePacket();
                    handle.Unpack(receiveBufferBytes, receivedBytes, this, bStatus_Signal, bStatus_Signaling);
                    handle = null;
                    Array.Clear(receiveBufferBytes, 0, receiveBufferBytes.Length);
                }
                Array.Clear(receiveBufferBytes, 0, receiveBufferBytes.Length);
            }
            catch (Exception e)
            {
            }
            BeginReceive();
        }

        public void Stop()
        {
            KeepCatching = true;
            HandlePacket.Pause = true;
        }

        public void CBTCOutToPcap(object state)
        {
            if (CBTCTemporaryStorage == MainForm.CBTCTemporaryStorage1)
            {
                CBTCTemporaryStorage = MainForm.CBTCTemporaryStorage2;
                CBTCOutput(MainForm.CBTCTemporaryStorage1);
            }
            else if (CBTCTemporaryStorage == MainForm.CBTCTemporaryStorage2)
            {
                CBTCTemporaryStorage = MainForm.CBTCTemporaryStorage1;
                CBTCOutput(MainForm.CBTCTemporaryStorage2);
            }
        }

        public void LTEOutToPcap(object state)
        {
            if (LTETemporaryStorage == MainForm.LTETemporaryStorage1)
            {
                LTETemporaryStorage = MainForm.LTETemporaryStorage2;
                LTEOutput(MainForm.LTETemporaryStorage1);
            }
            else if (LTETemporaryStorage == MainForm.LTETemporaryStorage2)
            {
                LTETemporaryStorage = MainForm.LTETemporaryStorage1;
                LTEOutput(MainForm.LTETemporaryStorage2);
            }
        }
    }
}

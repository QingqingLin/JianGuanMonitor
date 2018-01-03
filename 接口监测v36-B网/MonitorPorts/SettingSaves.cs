using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MonitorPorts
{
    class SettingSaves
    {
        #region 变量
        public string _Protocol;
        public string _SourceIP;
        public string _SourcePort;
        public string _DestIP;
        public string _DestPort;
        public string _SourceIPandPort;
        public string _DestIPandPort;
        public int _maxIntervalVtoZ;
        public int _maxIntervalZtoV;


        List<string> _listSourIPAndPort = new List<string>();
        List<string> _listDestIPAndPort = new List<string>();

        List<string> _listSourIP = new List<string>();
        List<string> _listDestIP = new List<string>();

        Dictionary<string ,string> _dicIPPortProperty;
        Dictionary<string, string> _dicIPProperty;


        List<int> lstSplits = new List<int> { 500, 1000, 2400 };
        
        #endregion

        #region 属性
        public string Protocol
        {
            set { _Protocol = value; }//可写
            get { return _Protocol; }//可读
        }
        public List<string> listSourIPandPort
        {
            set { _listSourIPAndPort = value; }//可写
            get { return _listSourIPAndPort; }//可读
        }
        public List<string> listDestIPandPort
        {
            set { _listDestIPAndPort = value; }//可写
            get { return _listDestIPAndPort; }//可读
        }
        public List<string> listSourIP
        {
            set { _listSourIP = value; }//可写
            get { return _listSourIP; }//可读
        }
        public List<string> listDestIP
        {
            set { _listDestIP = value; }//可写
            get { return _listDestIP; }//可读
        }
        public int MaxIntervalVtoZ
        {
            set { _maxIntervalVtoZ = value; }//可写
            get { return _maxIntervalVtoZ; }//可读
        }
        public int MaxIntervalZtoV
        {
            set { _maxIntervalZtoV = value; }//可写
            get { return _maxIntervalZtoV; }//可读
        }
        public  Dictionary<string ,string> DicIPPortProperty
        {
            set { _dicIPPortProperty = value; }//可写
            get { return _dicIPPortProperty; }//可读
        }
        public Dictionary<string, string> DicIPProperty
        {
            set { _dicIPProperty = value; }//可写
            get { return _dicIPProperty; }//可读
        }
        #endregion

        //构造函数
        public SettingSaves()
        {
            StreamReader sr = new StreamReader("a.txt", Encoding.Default);
            String line;
            List<string> _tmplist = new List<string>();

            while ((line = sr.ReadLine()) != null)
            {
                _tmplist.Add(line);
            }
            sr.Close();

            _Protocol = _tmplist[0];
            _SourceIPandPort = _tmplist[1];

            _DestIPandPort = _tmplist[2];
            _maxIntervalVtoZ = Convert.ToInt16(_tmplist[3]) / 1000;
            _maxIntervalZtoV = Convert.ToInt16(_tmplist[4]) / 1000;

            string[] strs = _SourceIPandPort.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < strs.Length;i++ )
            {
                if (strs[i]!="")
                {
                    _listSourIPAndPort.Add(strs[i]);
                    string[] tmp1 = strs[i].Split(new char[] { '-' });
                    _listSourIP.Add(tmp1[0]);
                }
            }

            strs = _DestIPandPort.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < strs.Length; i++)
            {
                if (strs[i] != "")
                {
                    _listDestIPAndPort.Add(strs[i]);
                    string[] tmp1 = strs[i].Split(new char[] { '-' });
                    _listDestIP.Add(tmp1[0]);

                }

            }
            _dicIPPortProperty = GetDicIPPortProperty();
            _dicIPProperty = GetDicIPProperty();
        }


        private Dictionary<string, string> GetDicIPPortProperty()
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
                string IP_Port = strs[1] + "-" + strs[2];
                tmpdic.Add(IP_Port, strs[3]);
                //listIP.Add(strs[1]);
            }
            return tmpdic;
        }
        private Dictionary<string, string> GetDicIPProperty()
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
                tmpdic.Add(IP, strs[3]);
                //listIP.Add(strs[1]);
            }
            return tmpdic;
        }

    }
}

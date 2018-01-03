using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace MonitorPorts
{
    class FilterPcap
    {
        public Boolean IsInTime(DateTime _startTime, DateTime _endTime, DateTime timeFileName)           //判断文件名时间是否符合时间筛选条件
        {
            if (timeFileName >= _startTime && timeFileName <= _endTime)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public List<string> getReqrFileList(DateTime _startTime, DateTime _endTime, string path)
        {
            List<string> fileName = new List<string>();
            DirectoryInfo fileFolder = new DirectoryInfo(path);
            foreach (DirectoryInfo YearFolder in fileFolder.GetDirectories())          //在保存pcap文件的目录下查找符合条件的文件
            {
                if (_startTime.Year < Convert.ToInt16(YearFolder.Name) && _endTime.Year > Convert.ToInt16(YearFolder.Name))     // 判断文件夹的年份在筛选条件起始和终止年份之间，该文件夹下所有文件都满足条件
                {
                    foreach (DirectoryInfo MonthFolder in YearFolder.GetDirectories())
                    {
                        foreach (DirectoryInfo DayFolder in MonthFolder.GetDirectories())
                        {
                            foreach (FileInfo file in DayFolder.GetFiles("*.pcap"))     //遍历该文件目录下的所有pcap文件
                            {
                                string s = file.ToString();               //当前pcap文件名
                                string filePath = file.FullName;
                                string name = Path.GetFileNameWithoutExtension(s);   //去掉后缀的文件名
                                name = name.Replace("：", ":");           //格式修改
                                DateTime nameToTime = Convert.ToDateTime(name);    //将文件名转换为时间
                                if (IsInTime(_startTime, _endTime, nameToTime))    //时间筛选
                                {
                                    fileName.Add(filePath);
                                }
                            }
                        }
                    }
                }
                else if (_startTime.Year == Convert.ToInt16(YearFolder.Name) && _endTime.Year > Convert.ToInt16(YearFolder.Name))          // 年份文件夹的年份等于筛选的起始年份，小于终止年份
                {
                    foreach (DirectoryInfo MonthFolder in YearFolder.GetDirectories())
                    {
                        if (Convert.ToInt16(MonthFolder.Name.Substring(0, 2)) == _startTime.Month)                    //月份文件夹的月份等于筛选的起始月份
                        {
                            foreach (DirectoryInfo DayFolder in MonthFolder.GetDirectories())
                            {
                                if (Convert.ToInt16(DayFolder.Name.Substring(0, 2)) >= _startTime.Day)                //日期文件夹日期大于筛选的起始日期，满足条件
                                {
                                    foreach (FileInfo file in DayFolder.GetFiles("*.pcap"))
                                    {
                                        string s = file.ToString();
                                        string filePath = file.FullName;
                                        string name = Path.GetFileNameWithoutExtension(s);
                                        name = name.Replace("：", ":");
                                        DateTime nameToTime = Convert.ToDateTime(name);
                                        if (IsInTime(_startTime, _endTime, nameToTime))
                                        {
                                            fileName.Add(filePath);
                                        }
                                    }
                                }
                            }
                        }
                        if (Convert.ToInt16(MonthFolder.Name.Substring(0, 2)) > _startTime.Month)                   // 月份文件夹的月份大于筛选的起始月份，文件夹下所有文件满筛选条件
                        {
                            foreach (DirectoryInfo DayFolder in MonthFolder.GetDirectories())
                            {
                                foreach (FileInfo file in DayFolder.GetFiles("*.pcap"))
                                {
                                    string s = file.ToString();
                                    string filePath = file.FullName;
                                    string name = Path.GetFileNameWithoutExtension(s);
                                    name = name.Replace("：", ":");
                                    DateTime nameToTime = Convert.ToDateTime(name);
                                    if (IsInTime(_startTime, _endTime, nameToTime))
                                    {
                                        fileName.Add(filePath);
                                    }
                                }

                            }
                        }
                    }
                }
                else if (_startTime.Year < Convert.ToInt16(YearFolder.Name) && _endTime.Year == Convert.ToInt16(YearFolder.Name))           ///年份文件夹的年份大于筛选的起始年份，等于于终止年份
                {
                    foreach (DirectoryInfo MonthFolder in YearFolder.GetDirectories())
                    {
                        if (Convert.ToInt16(MonthFolder.Name.Substring(0, 2)) == _endTime.Month)                                //月份文件夹的月份等于筛选终止月份
                        {
                            foreach (DirectoryInfo DayFolder in MonthFolder.GetDirectories())
                            {
                                if (Convert.ToInt16(DayFolder.Name.Substring(0, 2)) <= _endTime.Day)                               //日期文件夹日期小于筛选的终止日期，满足条件
                                {
                                    foreach (FileInfo file in DayFolder.GetFiles("*.pcap"))
                                    {
                                        string s = file.ToString();
                                        string filePath = file.FullName;
                                        string name = Path.GetFileNameWithoutExtension(s);
                                        name = name.Replace("：", ":");
                                        DateTime nameToTime = Convert.ToDateTime(name);
                                        if (IsInTime(_startTime, _endTime, nameToTime))
                                        {
                                            fileName.Add(filePath);
                                        }
                                    }
                                }
                            }
                        }
                        if (Convert.ToInt16(MonthFolder.Name.Substring(0, 2)) < _endTime.Month)                         //月份文件夹的月份小于筛选终止月份，文件夹下所有文件满筛选条件
                        {
                            foreach (DirectoryInfo DayFolder in MonthFolder.GetDirectories())
                            {
                                foreach (FileInfo file in DayFolder.GetFiles("*.pcap"))
                                {
                                    string s = file.ToString();
                                    string filePath = file.FullName;
                                    string name = Path.GetFileNameWithoutExtension(s);
                                    name = name.Replace("：", ":");
                                    DateTime nameToTime = Convert.ToDateTime(name);
                                    if (IsInTime(_startTime, _endTime, nameToTime))
                                    {
                                        fileName.Add(filePath);
                                    }
                                }

                            }
                        }
                    }
                }
                else if (_startTime.Year == Convert.ToInt16(YearFolder.Name) && _endTime.Year == Convert.ToInt16(YearFolder.Name))       ///年份文件夹的年份等于筛选的起始年份，也等于终止年份
                {
                    foreach (DirectoryInfo MonthFolder in YearFolder.GetDirectories())
                    {
                        if (Convert.ToInt16(MonthFolder.Name.Substring(0, 2)) < _endTime.Month && Convert.ToInt16(MonthFolder.Name.Substring(0, 2)) > _startTime.Month)           //月份文件夹的月份大于筛选的起始月份，小于终止月份，该文件夹下所有文件满足
                        {
                            foreach (DirectoryInfo DayFolder in MonthFolder.GetDirectories())
                            {
                                foreach (FileInfo file in DayFolder.GetFiles("*.pcap"))
                                {
                                    string s = file.ToString();
                                    string filePath = file.FullName;
                                    string name = Path.GetFileNameWithoutExtension(s);
                                    name = name.Replace("：", ":");
                                    DateTime nameToTime = Convert.ToDateTime(name);
                                    if (IsInTime(_startTime, _endTime, nameToTime))
                                    {
                                        fileName.Add(filePath);
                                    }
                                }
                            }
                        }
                        if (Convert.ToInt16(MonthFolder.Name.Substring(0, 2)) == _endTime.Month && Convert.ToInt16(MonthFolder.Name.Substring(0, 2)) > _startTime.Month)              //月份文件夹的月份大于筛选的起始月份，等于终止月份
                        {
                            foreach (DirectoryInfo DayFolder in MonthFolder.GetDirectories())
                            {
                                if (Convert.ToInt16(DayFolder.Name.Substring(0, 2)) <= _endTime.Day)                                                 //   日期文件夹的日期小于筛选的终止日期，满足条件
                                {
                                    foreach (FileInfo file in DayFolder.GetFiles("*.pcap"))
                                    {
                                        string s = file.ToString();
                                        string filePath = file.FullName;
                                        string name = Path.GetFileNameWithoutExtension(s);
                                        name = name.Replace("：", ":");
                                        DateTime nameToTime = Convert.ToDateTime(name);
                                        if (IsInTime(_startTime, _endTime, nameToTime))
                                        {
                                            fileName.Add(filePath);
                                        }
                                    }
                                }
                            }
                        }
                        if (Convert.ToInt16(MonthFolder.Name.Substring(0, 2)) < _endTime.Month && Convert.ToInt16(MonthFolder.Name.Substring(0, 2)) == _startTime.Month)             //月份文件夹的月份等于筛选的起始月份，小于终止月份
                        {
                            foreach (DirectoryInfo DayFolder in MonthFolder.GetDirectories())
                            {
                                if (Convert.ToInt16(DayFolder.Name.Substring(0, 2)) >= _startTime.Day)                                       //日期文件夹的日期大于筛选的起始日期，满足条件
                                {
                                    foreach (FileInfo file in DayFolder.GetFiles("*.pcap"))
                                    {
                                        string s = file.ToString();
                                        string filePath = file.FullName;
                                        string name = Path.GetFileNameWithoutExtension(s);
                                        name = name.Replace("：", ":");
                                        DateTime nameToTime = Convert.ToDateTime(name);
                                        if (IsInTime(_startTime, _endTime, nameToTime))
                                        {
                                            fileName.Add(filePath);
                                        }
                                    }
                                }
                            }
                        }
                        if (Convert.ToInt16(MonthFolder.Name.Substring(0, 2)) == _endTime.Month && Convert.ToInt16(MonthFolder.Name.Substring(0, 2)) == _startTime.Month)      //筛选条件为同一月份
                        {
                            foreach (DirectoryInfo DayFolder in MonthFolder.GetDirectories())
                            {
                                if (Convert.ToInt16(DayFolder.Name.Substring(0, 2)) <= _endTime.Day && Convert.ToInt16(DayFolder.Name.Substring(0, 2)) >= _startTime.Day)      //日期文件夹的日期应满足大于筛选起始日期小于筛选终止日期
                                {
                                    foreach (FileInfo file in DayFolder.GetFiles("*.pcap"))
                                    {
                                        string s = file.ToString();
                                        string filePath = file.FullName;
                                        string name = Path.GetFileNameWithoutExtension(s);
                                        name = name.Replace("：", ":");
                                        DateTime nameToTime = Convert.ToDateTime(name);
                                        if (IsInTime(_startTime, _endTime, nameToTime))
                                        {
                                            fileName.Add(filePath);
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
                else
                { }
            }
            return fileName;
        }
    }

}

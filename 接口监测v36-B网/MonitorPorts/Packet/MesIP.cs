using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonitorPorts.Packet
{
    class MesIP  //将消息按照IP进行分组
    {
       public List<List<int[]>> listMessIP = new List<List<int[]>>();
       public List<List<int[]>> listMessIP_gtp = new List<List<int[]>>();
       public List<List<string[]>> listIP = new List<List<string[]>>();
       int k = 0;
       public void mesip(List<int[]> Chunk, List<int[]> Chunk1) //输入的参数是去掉空格之后的数据流list类型,仅仅是一条数据流而已
        {
            //将数据根据IP地址进行分类
             List<string[]> listIPpair = new List<string[]>(); //存储的IP组（剔除之后的）
            while (Chunk.Count()!=0)
            {
                int[] comp = Chunk[0];
                string sourIP0 = string.Empty;
                string destIP0 = string.Empty;
                string sourIP = string.Empty;
                string destIP = string.Empty;
                string[] doubleIP = new string[2];
                sourIP0 = Convert.ToString(Chunk[0][28]) + "." + Convert.ToString(Chunk[0][29]) + "." + Convert.ToString(Chunk[0][30]) + "." + Convert.ToString(Chunk[0][31]);
                destIP0 = Convert.ToString(Chunk[0][32]) + "." + Convert.ToString(Chunk[0][33]) + "." + Convert.ToString(Chunk[0][34]) + "." + Convert.ToString(Chunk[0][35]);
                doubleIP[0] = sourIP;
                doubleIP[1] = destIP;
                listIPpair.Add(doubleIP);
                List<int[]> listMessIPSame = new List<int[]>();
                List<int[]> listMessIPSame_gtp = new List<int[]>();
                for (int j = 0; j < Chunk.Count(); j++)
                {
                    //bool sourIP1 = (comp[28] == Chunk[j][28] && comp[29] == Chunk[j][29] && comp[30] == Chunk[j][30] && comp[31] == Chunk[j][31]);
                    //bool destIP1 = (comp[32] == Chunk[j][32] && comp[33] == Chunk[j][33] && comp[34] == Chunk[j][34] && comp[35] == Chunk[j][35]);
                    //bool sourIP2 = (comp[28] == Chunk[j][32] && comp[29] == Chunk[j][33] && comp[30] == Chunk[j][34] && comp[31] == Chunk[j][35]);
                    //bool destIP2 = (comp[32] == Chunk[j][28] && comp[33] == Chunk[j][29] && comp[34] == Chunk[j][30] && comp[35] == Chunk[j][31]);
                    sourIP = Convert.ToString(Chunk[j][28]) + "." + Convert.ToString(Chunk[j][29]) + "." + Convert.ToString(Chunk[j][30]) + "." + Convert.ToString(Chunk[j][31]);
                    destIP = Convert.ToString(Chunk[j][32]) + "." + Convert.ToString(Chunk[j][33]) + "." + Convert.ToString(Chunk[j][34]) + "." + Convert.ToString(Chunk[j][35]);
                    
                    //if (((sourIP1 == destIP1) && (sourIP1 == destIP1))|| ((sourIP2 == destIP2) && (sourIP2 == destIP2)))
                    if (((sourIP0 == sourIP) && (destIP0 == destIP)) || ((sourIP0 == destIP) && (destIP0 == sourIP)))
                    {
                        listMessIPSame.Add(Chunk[j]);
                        listMessIPSame_gtp.Add(Chunk1[j]);
                        Chunk.Remove(Chunk[j]);
                        Chunk1.Remove(Chunk1[j]);
                        j = -1;
                    }
                }
                listMessIP.Add(listMessIPSame); //源数据流，即去掉空格之后的数据流，类型list嵌套
                listMessIP_gtp.Add(listMessIPSame_gtp);
            }




            //确定IP组
            while (listIPpair.Count() != 0)
            {
                string[] ip = listIPpair[0];
                List<string[]> listIPSame = new List<string[]>();
                for (int i = 0; i < listIPpair.Count(); i++)
                {
                 //   String.Compare(ip[0], listIPpair[0][0]);//相等的化，返回结果为0

                    if(((String.Compare(ip[0], listIPpair[i][0])==0)&& String.Compare(ip[1], listIPpair[i][1]) == 0)|| ((String.Compare(ip[0], listIPpair[i][1]) == 0) && String.Compare(ip[1], listIPpair[i][0]) == 0))
                    {
                        listIPSame.Add(listIPpair[i]);
                        listIPpair.Remove(listIPpair[i]);
                    }
                }
                listIP.Add(listIPpair); //存放IP组，单纯的不重复的IP
            }
            
        }

    }
}

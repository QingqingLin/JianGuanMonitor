using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MonitorPorts.Packet
{
    class Hash
    {
        public int HashSize;
        public List<int> key=new List<int>();
        unsafe List<int> addr = new List<int>();//定义一个List，用于存放头指针

        public int k;
        public void HashSto(List<int[]> Ls)
        {
            Hashtable ht = new Hashtable();//创建一个Hashtable实例
            HashSize = Ls.Count;
            #region //
            for (int i=0; i < Ls.Count; i++)
            {
                //求hash表地址
                k= Hash_key(Convert.ToInt64(Ls[i][8] * System.Math.Pow(256, 2) + Ls[i][9] * System.Math.Pow(256, 1) + Ls[i][10]),HashSize);
                //求hash表地址内部所存储的头指针，关键怎样确定该出的头指针就是对应的某一个链表????
                ht.Add(k,Ls);
              


                #region
                /*   foreach (int c in key)
                 {
                     if (k != c)
                     {
                         ht.Add(key[i], Ls[i]);
                     }
                     else
                     {

                     }
                 } 
                 key.Add(k);*/
                #endregion

            }
            #endregion


        }
        #region 关键字的处理

        int Hash_key(long Key,int HSize) //对关键字的处理
        {
            return (int)(Key % HSize);
        }

        #endregion



        //88888888888888888
        
            public static void LinkExap()
            {
               Linkframe Lf= new Linkframe();
            
                List a = new List();
                for (int i = 1; i <= 10; i++)
                    a.insert(new IntPtr(i));
                Console.WriteLine(a.currentNode());
                while (!a.isEnd())
                    Console.WriteLine(a.nextNode());
                a.reset();
                while (!a.isEnd())
                {
                    a.remove();
                }
                a.remove();
                a.reset();
                if (a.isEmpty())
                    Console.WriteLine("There is no Node in List!");
                Console.WriteLine("You can press return to quit!");
                try
                {
                    // 确保用户看清程序运行结果
                    Console.Read();
                }
                catch (IOException e)
                {
                }
            }
        }

        //*********************************







        //建立一个单链表结构
        //定义基本数据节点
        unsafe struct _NODE
        {
            int data;
            public link* next;
        }
        //定义hash表

      // 定义一个结构体，包含数据区和指针区
        unsafe struct link
        {
            public int x; //数据区
            public link* next;//指针区，指向下一个节点的指针
        }


        //定义一个链表，即在hash表中，存放的元素为头指针
      static unsafe void SinglyLink(string[] arg)

        {
            int val;
            link* head = stackalloc link[sizeof(link)];//stacklloc命令只是分配内存而已，不会把内存初始化为任何默认值，其后紧跟
                                                       //要存储数据类型名，分配的字节数为变量个数*sizeof（数据类型）
            link* q = head;
            for (int i = 0; i <= 10; i++)
            {
                val = i + 100;
                link* temp = stackalloc link[sizeof(link)];
                q->x = val;
                q->next = temp;
                q = temp;
            }

            link* t = head;
            while (t->next != null)
            {

                //   Console.WriteLine(t->x);
                t = t->next;
            }
        }
    }




   
}

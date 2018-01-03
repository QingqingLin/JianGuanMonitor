/////
/////CBTC互联互通标准——报文格式
/////
/////
/////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonitorPorts
{
    class CBTCByte
    {
        //构造函数
        public CBTCByte()
        { }

        //变量

        private int Index;
        private string Name;
        private int Len;
        private string DefValue;



        //属性
        public int index
        {
            get { return Index; }
            set { Index = value; }
        }
        public string name
        {
            get { return Name; }
            set { Name = value; }
        }
        public int len
        {
            get { return Len; }
            set { Len = value; }
        }
        public string defValue
        {
            get { return DefValue; }
            set { DefValue = value; }
        }


        #region 注释

        ///// <summary>
        /////  所对应的课程类型
        ///// </summary>
        //private string bookType;

        //public string BookType 
        //{
        //    get { return bookType; }
        //    set { bookType = value; }
        //}


        ///// <summary>
        ///// 书所对应的ISBN号
        ///// </summary>
        //private string bookISBN;

        //public string BookISBN
        //{
        //    get { return bookISBN; }
        //    set { bookISBN = value; }
        //}

        ///// <summary>
        ///// 书名
        ///// </summary>
        //private string bookName;

        //public string BookName
        //{
        //    get { return bookName; }
        //    set { bookName = value; }
        //}

        ///// <summary>
        ///// 作者
        ///// </summary>
        //private string bookAuthor;

        //public string BookAuthor
        //{
        //    get { return bookAuthor; }
        //    set { bookAuthor = value; }
        //}

        ///// <summary>
        ///// 价格
        ///// </summary>
        //private double bookPrice;

        //public double BookPrice
        //{
        //    get { return bookPrice; }
        //    set { bookPrice = value; }
        //}
        #endregion

    }

}

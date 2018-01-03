using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonitorPorts.Packet
{
    class Linkframe
    {
        // 构成链表的结点定义 
        public class Node
        {
            public Object data;
            public Node next;
            public Node(Object d)
            {
                data = d;
                next = null;
            }
        }
        public class List
        {
            // 用变量来实现表头
            private Node Head = null;
            private Node Tail = null;
            private Node Pointer = null;
            private int Length = 0;
            //清空整个链表 
            public void deleteAll()
            {
                Head = null;
                Tail = null;
                Pointer = null;
                Length = 0;
            }
            //链表复位，使第一个结点 成为当前结点
            public void reset()
            {
                Pointer = null;
            }
            //判断链表是否为空
            public bool isEmpty()
            {
                return (Length == 0);
            }
            //判断当前结点是否 为最后一个结点
            public bool isEnd()
            {
                if (Length == 0)
                    throw new System.Exception();
                else if (Length == 1)
                    return true;
                else
                    return (cursor() == Tail);
            }
            //返回当前结点的下一个结点的值， 并使其成为当前结点
            public Object nextNode()
            {
                if (Length == 1)
                    throw new System.Exception();
                else if (Length == 0)
                    throw new System.Exception();
                else
                {
                    Node temp = cursor();
                    Pointer = temp;
                    if (temp != Tail)
                        return (temp.next.data);
                    else
                        throw new System.Exception();
                }
            }
            //返回当前结点的值
            public Object currentNode()
            {
                Node temp = cursor();
                return temp.data;
            }
            //在当前结点前插入一个结点， 并使其成为当前结点
            public void insert(Object d)
            {
                Node e = new Node(d);
                if (Length == 0)
                {
                    Tail = e;
                    Head = e;
                }
                else
                {
                    Node temp = cursor();
                    e.next = temp;
                    if (Pointer == null)
                        Head = e;
                    else
                        Pointer.next = e;
                }
                Length++;
            }
            //返回链表的大小
            public int size()
            {
                return Length;
            }
            //将当前结点移出链表，下一个结点成为当前结点
            //如果移出的结点是最后一个结点，则第一个结点成为当前结点
            public Object remove()
            {
                Object temp;
                if (Length == 0)
                    throw new System.Exception();
                else if (Length == 1)
                {
                    temp = Head.data;
                    deleteAll();
                }
                else
                {
                    Node cur = cursor();
                    temp = cur.data;
                    if (cur == Head)
                        Head = cur.next;
                    else if (cur == Tail)
                    {
                        Pointer.next = null;
                        Tail = Pointer;
                        reset();
                    }
                    else
                        Pointer.next = cur.next;
                    Length--;
                }
                return temp;
            }
            //返回当前结点的指针
            private Node cursor()
            {
                if (Head == null)
                    throw new System.Exception();
                else if (Pointer == null)
                    return Head;
                else
                    return Pointer.next;
            }
         
        }
    }
}

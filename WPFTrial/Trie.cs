using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePrototype
{
    class Node
    {
        public Node []children;
        public bool valid, ending;
        public char val;
        public Node(char c = '#', bool isEnding = false)
        {
            val = c;
            valid = true;
            ending = isEnding;
            children = new Node[26];
            return;
        }
        //展开一个节点使得它能够扩展并存一个值
        public void unfold(char c,bool isEnding)
        {
            val = c;
            if (isEnding)
            {
                ending = true;
            }
            valid = true;
            children = new Node[26];
        }
    }

    class Trie
    {
        Node root;
        public Trie()
        {
            root = new Node('#',false);
        }
        public void init(string FileName)
        {
            string []lines = System.IO.File.ReadAllLines(FileName);
            foreach(string line in lines)
            {
                string str = line.TrimEnd();
                this.insert(str);
            }
        }
        public void insert(String str)
        {
            int len = str.Length;
            Node crt = root;
            for (int i = 0; i < len; i++)
            {
                char c = str[i];
                int defCode = c - 'a';
                if (crt.children[defCode] == null)
                {
                    crt.children[defCode] = new Node(c,false);
                    crt = crt.children[defCode];
                }
                else
                {
                    crt = crt.children[defCode];
                }

            }
            crt.ending = true;
        }
        public int find(String str)
        {
            int len = str.Length;
            Node crt = root;
            for (int i = 0; i < len; i++)
            {
                char c = str[i];
                int defCode = c - 'a';
                if (crt.children[defCode] == null)
                {
                    return -1;//mismatch 
                }
                crt = crt.children[defCode];
            }
            return 1;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryChecksumCheck
{
    public class Node
    {
        private Node next { get; set; }
        public string path { get; set; }
        public bool hasNext { get; set; }

        public Node()
        {
            next = null;
            path = null;
            hasNext = false;
        }

        public Node(String fullname)
        {
            next = null;
            path = fullname;
            hasNext = false;
        }

        public Node(String fullname, Node n)
        {
            next = n;
            path = fullname;
            hasNext = true;
        }

        public Node getNext()
        {
            return next;
        }

        public void setNext(Node n)
        {
            next = n;
            hasNext = true;
        }
        
    }
}

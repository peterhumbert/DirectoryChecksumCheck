using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryChecksumCheck
{
    class Node
    {
        private Node next { get; set; }
        private Node prev { get; set; }
        private string path { get; set; }
        public bool hasNext { get; set; }

        public Node()
        {
            next = null;
            prev = null;
            path = null;
            hasNext = false;
        }

        public Node()
        {
            next = null;
            prev = null;
            path = null;
            hasNext = false;
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

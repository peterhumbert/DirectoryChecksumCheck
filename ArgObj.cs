using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryChecksumCheck
{
    class ArgObj
    {
        public DirectoryInfo di1 { get; set; }
        public DirectoryInfo di2 { get; set; }
        public string[] exts { get; set; }

        public ArgObj(DirectoryInfo di1, string[] exts)
        {
            this.di1 = di1;
            this.exts = exts;
        }

        public ArgObj(DirectoryInfo di1, DirectoryInfo di2, string[] exts) : this(di1, exts)
        {
            this.di2 = di2;
        }
    }
}

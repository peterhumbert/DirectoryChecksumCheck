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
        public DirectoryInfo di { get; set; }
        public string[] exts { get; set; }

        public ArgObj(DirectoryInfo di, string[] exts)
        {
            this.di = di;
            this.exts = exts;
        }
    }
}

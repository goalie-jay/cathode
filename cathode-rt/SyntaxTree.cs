using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cathode_rt
{
    public class SyntaxTreeNode
    {
        public SyntaxTreeNode[] Children;
    }

    public class SyntaxTree
    {
        public SyntaxTreeNode Root;

        public SyntaxTree()
        {
            Root = new SyntaxTreeNode();
        }
    }
}

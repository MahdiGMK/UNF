using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
[Node(typeof(TestNode),"Nodes/TestNode")]
class TestNode : Node
{
    [Input]public object TestPort;
}

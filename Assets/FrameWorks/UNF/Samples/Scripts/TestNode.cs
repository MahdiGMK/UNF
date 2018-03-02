using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
[Node(typeof(TestNode),"Nodes/TestNode")]
public class TestNode : Node
{
    [Input]public float TestPort;
}

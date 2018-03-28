using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
[Node(typeof(TestNode),"Nodes/TestNode")]
public class TestNode : Node
{
    [PortType(NodePort.connectionMethod.Single,showBackingValueMethod = NodePort.showBackingValueMethod.Always)]
    [Output] public bool TestOutput;
    public float item001;
    [Input]public float TestInput;

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
[Node(typeof(TestNode),"TestNodes/TestNode")]
public class TestNode : Node
{
    [PortType(NodePort.connectionMethod.Multiple,showBackingValueMethod = NodePort.showBackingValueMethod.Always)]
    [Output] public bool TestOutput;
    public float item001;
    [PortType(NodePort.connectionMethod.Multiple, showBackingValueMethod = NodePort.showBackingValueMethod.Always)]
    [Input]public float TestInput;

}

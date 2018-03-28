using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Node(typeof(DisplayNode),"Debuggers/display")]
public class DisplayNode : Node
{
    [Input, PortType(NodePort.connectionMethod.Single, showBackingValueMethod = NodePort.showBackingValueMethod.Never)] public string Input;
    public override Color TitleColor()
    {
        return new Color(0.8f,0.15f,0);
    }

}

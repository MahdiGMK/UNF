using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[NodeEditor(typeof(DisplayNode))]
public class DisplayNodeEditor : NodeEditor
{
    public override void Draw(Node node)
    {
        base.Draw(node);
        //object val = node.GetInputValue(node.GetPort("Input"));
        
    }
}

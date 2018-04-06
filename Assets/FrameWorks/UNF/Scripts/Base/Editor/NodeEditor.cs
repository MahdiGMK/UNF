using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class NodeEditor {
    public virtual void Draw(Node n)
    {
        NodeEditorGUIUtility.NormalNodeDraw(n);
    }
    public virtual Vector2 GetNodePortPosition(NodePort port)
    {
        float DistancFromBorder = 5;
        float DistancFromEachLine = 7.5f;
        float PortSize = 20;

        Rect parentRect = NodeEditorGUIUtility.GetNodeRect(port.parentNode);
        Vector2 position = parentRect.position + new Vector2(port.IOType == NodePort.portType.Input ? DistancFromBorder * port.parentNode.graph.ZoomAmm : parentRect.width - DistancFromBorder * port.parentNode.graph.ZoomAmm - PortSize * port.parentNode.graph.ZoomAmm, (port.drawingPos + 1) * port.parentNode.graph.ZoomAmm * 30 + DistancFromEachLine * port.parentNode.graph.ZoomAmm);
        return position;
    }
    public virtual float GetHeight(Node node)
    {
        //Will be filled
        return 30 * node.fields.Count + 30 + 5;
    }
    public virtual float GetWidth(Node node)
    {
        return 250;
    }
}
public class NodeEditorAttribute : Attribute
{
    public readonly Type nodeType;
    public NodeEditorAttribute(Type nodeType)
    {
        this.nodeType = nodeType;
    }
}

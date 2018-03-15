using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NodeEditor {
    public virtual void Draw(Node n)
    {
        NodeEditorGUIUtility.DrawNode(n);
    }
    public virtual float GetHeight(Node node)
    {
        //Will be filled
        return 30;
    }
    public virtual float GetWidth(Node node)
    {
        return 100;
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

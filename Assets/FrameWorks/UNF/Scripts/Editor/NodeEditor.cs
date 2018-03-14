using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NodeEditor {
    public virtual void Draw(Node n)
    {
        NodeEditorGUIUtility.DrawNode(n);
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

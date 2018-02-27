using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Node : ScriptableObject {
    public List<NodePort> ports;
}
public class NodeAttribute : Attribute
{
    public readonly Type nodeType;
    public readonly string creatingPath;
    public NodeAttribute(Type nodeType,string creatingPath)
    {
        this.nodeType = nodeType;
        this.creatingPath = creatingPath;
    }
}
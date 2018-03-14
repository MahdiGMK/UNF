using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[Serializable]
public class NodePort
{
    public string fieldName;
    public Node parentNode;
    public Type Type;
    public portType IOType;
    public connectionMethod connectMethod;
    public int drawingPos;
    public List<Connection> connections
    {
        get
        {
            List<Connection> foundConnection = new List<Connection>();
            Thread mtJob = new Thread(() =>
            {
                GraphData graph = parentNode.graph;
                foreach (var connection in graph.connections)
                {
                    if (IOType == portType.Input)
                    {
                        if (connection.inputNode == parentNode && connection.inputFieldName == fieldName)
                        {
                            foundConnection.Add(connection);
                        }
                    }
                    else if (IOType == portType.Output)
                    {
                        if (connection.outputNode == parentNode && connection.outputFieldName == fieldName)
                        {
                            foundConnection.Add(connection);
                        }
                    }
                }
            });
            mtJob.Start();
            return foundConnection;
        }
    }
    public enum portType
    {
        Input,
        Output
    }
    public enum connectionMethod
    {
        Single,
        Multiple
    }
    public NodePort(string name,int pos, Node parent, portType portType, connectionMethod connectionMethod)
    {
        fieldName = name;
        parentNode = parent;
        connectMethod = connectionMethod;
        IOType = portType;
        drawingPos = pos;
    }


    public void CreateConnection(NodePort other)
    {
        if(IOType == portType.Input && other.IOType == portType.Output)
        {
            parentNode.graph.TryCreateConnection(this, other);
        }
        else if (IOType == portType.Output && other.IOType == portType.Input)
        {
            parentNode.graph.TryCreateConnection(this, other);
        }
    }
}
public class PortTypeAttribute : Attribute
{
    internal readonly NodePort.connectionMethod connectionMethod;
}
public class InputAttribute : Attribute
{

}
public class OutputAttribute : Attribute
{

}
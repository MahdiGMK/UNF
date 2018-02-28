using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class NodePort
{
    public string fieldName;
    public Node parentNode;
    public Type Type;
    public portType IOType;
    public connectionMethod connectMethod;
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
                        if (connection.toNode == parentNode && connection.toFieldName == fieldName)
                        {
                            foundConnection.Add(connection);
                        }
                    }
                    else if (IOType == portType.Output)
                    {
                        if (connection.fromeNode == parentNode && connection.fromeFieldName == fieldName)
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
    public NodePort(string name, Node parent, portType portType, connectionMethod connectionMethod)
    {
        fieldName = name;
        parentNode = parent;
        connectMethod = connectionMethod;
        IOType = portType;
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
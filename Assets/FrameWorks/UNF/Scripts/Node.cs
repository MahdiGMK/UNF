using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Node : ScriptableObject
{
    public string Name;
    public Vector2 position;
    public List<NodePort> ports;
    public List<string> fields;
    public GraphData graph;

    public override string ToString()
    {
        return name;
    }
    public void Init()
    {
        RedirectPorts();
    }
    protected virtual void RedirectPorts()
    {
        ports = new List<NodePort>();
        fields = new List<string>();
        var foundFields = GetType().GetFields();
        var baseFields = GetType().BaseType.GetFields();
        var baseFieldsList = new List<System.Reflection.FieldInfo>(baseFields);
        for (int i = 0; i < foundFields.Length; i++)
        {
            var field = foundFields[i];
            if (baseFieldsList.Find(obj => { return obj.Name == field.Name; }) != null || !field.IsPublic)
                continue;

            fields.Add(field.Name);
            PortTypeAttribute[] pa = (PortTypeAttribute[])(field.GetCustomAttributes(typeof(PortTypeAttribute), false));
            InputAttribute[] ia = (InputAttribute[])(field.GetCustomAttributes(typeof(InputAttribute), false));
            OutputAttribute[] oa = (OutputAttribute[])(field.GetCustomAttributes(typeof(OutputAttribute), false));
            if (ia.Length > 0)
            {
                if (pa.Length > 0)
                    ports.Add(new NodePort(field.Name, i, field.FieldType, this, NodePort.portType.Input, pa[0].connectionMethod));
                else
                    ports.Add(new NodePort(field.Name, i, field.FieldType, this, NodePort.portType.Input, NodePort.connectionMethod.Single));
            }
            else if (oa.Length > 0)
            {
                if (pa.Length > 0)
                    ports.Add(new NodePort(field.Name, i, field.FieldType, this, NodePort.portType.Output, pa[0].connectionMethod));
                else
                    ports.Add(new NodePort(field.Name, i, field.FieldType, this, NodePort.portType.Output, NodePort.connectionMethod.Multiple));
            }
        }
    }
    public void ClearConnections()
    {
        foreach (var connection in graph.connections)
        {
            if (connection.inputNode == this || connection.outputNode == this)
            {
                graph.connections.Remove(connection);
            }
        }
    }
    public object GetInputValue(NodePort port)
    {
        return port.connections[0].MovedData;
    }
    public List<object> GetInputValues(NodePort port)
    {
        List<object> val = new List<object>();
        foreach (var connection in port.connections)
        {
            val.Add(connection.MovedData);
        }
        return val;
    }
    public virtual object GetOutPutValue(string fieldName) { return null; }
    public NodePort GetPort(string fieldName)
    {
        return ports.Find(obj => { return obj.fieldName == fieldName; });
    }

}
public class NodeAttribute : Attribute
{
    public readonly Type nodeType;
    public readonly string creatingPath;
    public NodeAttribute(Type nodeType, string creatingPath)
    {
        this.nodeType = nodeType;
        this.creatingPath = creatingPath;
    }
}
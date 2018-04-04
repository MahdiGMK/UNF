using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GraphData : ScriptableObject
{
    public List<Node> nodes = new List<Node>();
    public List<Connection> connections = new List<Connection>();
#if UNITY_EDITOR
    public Vector2 cameraPosition;
    public float ZoomAmm = 1;
    [NonSerialized, HideInInspector]
    public List<Node> selectedNodes = new List<Node>();
    [NonSerialized,HideInInspector]
    public NodePort selectedNodePort;
#endif
    public void Init()
    {
        if (connections == null)
            connections = new List<Connection>();
        foreach (var node in nodes)
        {
            node.Init();
        }
    }

    public Vector2 CameraPosition(Vector2 ScreenSize)
    {
        return cameraPosition + ScreenSize / 2;
    }
    public Node CreateNode(Type t, Vector2 pos)
    {
        Node newNode = (Node)CreateInstance(t);
        newNode.position = pos;
        newNode.graph = this;
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.AddObjectToAsset(newNode, this);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
        newNode.Name = "New " + t.ToString();
        nodes.Add(newNode);
        newNode.Init();
        return newNode;
    }
    public Node CopyNode(Node original, Vector2 pos)
    {
        return CreateNode(original.GetType(), pos);
    }
    public void DestroyNode(Node node)
    {
        node.ClearConnections();
        nodes.Remove(node);
#if UNITY_EDITOR
        DestroyImmediate(node, true);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
        Init();
    }
    public void ResetNode(Node node)
    {
        node.Name = "New " + node.GetType().ToString();
        node.ClearConnections();
        Init();
    }
    public void TryCreateConnection(NodePort input, NodePort output)
    {
        //input.Type == output.Type
        bool connectCondition = input.parentNode != output.parentNode && connections.Find(obj => { return obj.inputNode == input.parentNode && obj.inputFieldName == input.fieldName && obj.outputNode == output.parentNode && obj.outputFieldName == output.fieldName; }) == null;
        if (connectCondition)
        {
            if (input.connectMethod == NodePort.connectionMethod.Single && input.connections.Count > 0)
                connections.Remove(input.connections[0]);
            if (output.connectMethod == NodePort.connectionMethod.Single && output.connections.Count > 0)
                connections.Remove(output.connections[0]);
            connections.Add(new Connection(input, output));
        }
        Init();
    }
    public void DestroyConnection(Connection connection)
    {
        connections.Remove(connection);
        Init();
    }
}

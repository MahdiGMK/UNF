using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GraphData : ScriptableObject {
    public List<Node> nodes = new List<Node>();
    public List<Connection> connections = new List<Connection>();
    public Vector2 CameraPosition;
    public Node CreateNode(Type t)
    {
        Node newNode = (Node)CreateInstance(t);
        newNode.Init();
        newNode.graph = this;
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.AddObjectToAsset(newNode, this);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
        nodes.Add(newNode);
        return newNode;
    }
    public Node CopyNode(Node original)
    {
        Node newNode = original;
        newNode.Init();
        newNode.graph = this;
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.AddObjectToAsset(newNode, this);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
        nodes.Add(newNode);
        return newNode;
    }
    public void TryCreateConnection(NodePort input,NodePort output)
    {
        if (Convert.ChangeType(output.Type.GetConstructors()[0].Invoke(null), input.Type) != null)
        {
            connections.Add(new Connection(input, output));
        }
    }
}

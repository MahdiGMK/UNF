using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class NodeEditorGUIUtility
{
    public static Dictionary<Type, NodeEditor> nodeTypeDic;
    public static Dictionary<Type, Color> portColor;
    public static void Init()
    {
        nodeTypeDic = new Dictionary<Type, NodeEditor>();
    }
    #region DrawNodes
    public static void DrawNode(Node node)
    {
        NodeEditor ne;
        if (nodeTypeDic.TryGetValue(node.GetType(), out ne))
        {
            if (ne != null)
                ne.Draw(node);
            else
                NormalNodeDraw(node);
        }
        else
        {
            Type[] types = typeof(NodeEditor).Assembly.GetTypes();
            foreach (var type in types)
            {
                var attributes = (NodeEditorAttribute[])type.GetCustomAttributes(typeof(NodeEditorAttribute), false);
                if (attributes[0].nodeType != node.GetType())
                    continue;

                if (attributes.Length > 0)
                {
                    ne = (NodeEditor)type.GetConstructors()[0].Invoke(null);
                    nodeTypeDic.Add(node.GetType(), ne);
                    ne.Draw(node);
                }
                else
                {
                    nodeTypeDic.Add(node.GetType(), null);
                    NormalNodeDraw(node);
                }
            }
        }
    }
    public static float GetHeight(Node node)
    {
        return 30;
    }
    static void NormalNodeDraw(Node node)
    {
        GUI.Box(new Rect(node.position, new Vector2(100, GetHeight(node))), node.name);
    }
    #endregion
    #region DrawNodePorts
    public static Color NodePortColor(NodePort port)
    {
        Color c;
        if (portColor.TryGetValue(port.Type, out c))
        {
            return c;
        }
        else
        {
            c = UnityEngine.Random.ColorHSV(0, 1, 0, 1, 0, 1, 1, 1);
            portColor.Add(port.Type, c);
            return c;
        }

    }
    #endregion
    #region DrawConnections
    public static void DrawConnection(Connection connection)
    {
        Vector2 startP = connection.inputNode.position;
        Vector2 endP = connection.outputNode.position;
        Vector2 startT = Mathf.Clamp(startP.x - endP.x, 5, 10) * Vector2.right + startP;
        Vector2 endT = Mathf.Clamp(startP.x - endP.x, 5, 10) * Vector2.left + endP;

        Handles.DrawBezier(startP, endP, startT, endT, NodePortColor(connection.inputNode.GetPort(connection.inputFieldName)),null,5);
    }
    #endregion
    #region NodeEditorWindow
    public static void DrawGraphData(GraphData data)
    {
        foreach (var node in data.nodes)
        {
            DrawNode(node);
        }

        foreach (var connection in data.connections)
        {
            DrawConnection(connection);
        }
    }
    #endregion
}

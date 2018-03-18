using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class NodeEditorGUIUtility
{
    public static Dictionary<Type, NodeEditor> nodeTypeDic;
    public static Dictionary<Type, Color> portColor;

    public static void Init()
    {
        nodeTypeDic = new Dictionary<Type, NodeEditor>();
        portColor = new Dictionary<Type, Color>();
        //LoadData();
    }
    public static void Finalize()
    {
        //SaveData();
    }
    /*
    public static void LoadData()
    {
        if (File.Exists("Assets/FrameWorks/UNF/guiData.gdata"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream("Assets/FrameWorks/UNF/guiData.gdata", FileMode.OpenOrCreate);
            Dictionary<Type, Color> data = bf.Deserialize(stream) as Dictionary<Type, Color>;

            portColor = data;
            stream.Flush();
            stream.Close();
        }
        else
        {
            portColor = new Dictionary<Type, Color>();
            SaveData();
        }
    }
    public static void SaveData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream("Assets/FrameWorks/UNF/guiData.gdata", FileMode.Create);
        Dictionary<Type, Color> streamingData = portColor;
        bf.Serialize(stream, streamingData);
        stream.Flush();
        stream.Close();
    }
    */

    #region DrawNodes
    public static Rect GetNodeRect(Node node)
    {
        Rect result = new Rect(Vector2.zero, new Vector2(GetNodeWidth(node), GetNodeHeight(node)) * node.graph.ZoomAmm);
        result.center = (node.position + node.graph.cameraPosition) * node.graph.ZoomAmm + new Vector2(Screen.width, Screen.height - 24) / 2;
        return result;
    }
    public static void DrawNode(Node node)
    {
        SetNodeName(node);

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
                if (attributes.Length > 0 && attributes[0].nodeType != node.GetType())
                    continue;

                if (attributes.Length > 0)
                {
                    ne = (NodeEditor)type.GetConstructors()[0].Invoke(null);
                    if (!nodeTypeDic.ContainsKey(node.GetType()))
                        nodeTypeDic.Add(node.GetType(), ne);
                    ne.Draw(node);
                }
                else
                {
                    if (!nodeTypeDic.ContainsKey(node.GetType()))
                        nodeTypeDic.Add(node.GetType(), null);
                    NormalNodeDraw(node);
                }
            }
        }
    }
    static void SetNodeName(Node node)
    {
        if (node.name.Replace(" ", "") != node.Name.Replace(" ", ""))
        {
            node.name = ObjectNames.NicifyVariableName(node.Name);
            AssetDatabase.SaveAssets();
        }
    }
    public static float GetNodeHeight(Node node)
    {
        NodeEditor ne;
        if (nodeTypeDic.TryGetValue(node.GetType(), out ne))
        {
            if (ne == null)
            {
                //Will be filled
                return 30 * node.fields.Count + 30 + 5;
            }
            else
            {
                return ne.GetHeight(node);
            }
        }
        else
        {
            //Will be filled
            return 30 * node.fields.Count + 30 + 5;
        }
    }
    public static float GetNodeWidth(Node node)
    {
        NodeEditor ne;
        if (nodeTypeDic.TryGetValue(node.GetType(), out ne))
        {
            if (ne == null)
            {
                //Will be filled
                return 100;
            }
            else
            {
                return ne.GetWidth(node);
            }
        }
        else
        {
            //Will be filled
            return 100;
        }
    }
    static void NormalNodeDraw(Node node)
    {
        GUI.Box(GetNodeRect(node), node.name);
        foreach (var nodePort in node.ports)
        {
            DrawNodePort(nodePort);
        }
    }
    #endregion
    #region DrawNodePorts
    public static Rect GetNodePortRect(NodePort port)
    {
        float DistancFromBorder = 5;
        float DistancFromEachLine = 7.5f;
        float PortSize = 15;

        Rect parentRect = GetNodeRect(port.parentNode);

        Rect result = new Rect(parentRect.position + new Vector2(port.IOType == NodePort.portType.Input ? DistancFromBorder * port.parentNode.graph.ZoomAmm : parentRect.width - DistancFromBorder * port.parentNode.graph.ZoomAmm - PortSize * port.parentNode.graph.ZoomAmm, (port.drawingPos + 1) * port.parentNode.graph.ZoomAmm * 30 + DistancFromEachLine * port.parentNode.graph.ZoomAmm), new Vector2(PortSize, PortSize) * port.parentNode.graph.ZoomAmm);
        return result;
    }
    public static Color NodePortColor(NodePort port)
    {
        Color c;
        if (portColor.TryGetValue(port.Type, out c))
        {
            return c;
        }
        else
        {
            c = UnityEngine.Random.ColorHSV(0, 1, .5f, 1, .5f, 1, 1, 1);
            portColor.Add(port.Type, c);
            return c;
        }

    }
    public static void DrawNodePort(NodePort port)
    {
        Rect position = GetNodePortRect(port);
        GUI.color = NodePortColor(port);
        GUI.DrawTexture(position, Texture2D.whiteTexture);
        GUI.color = Color.white;
    }
    #endregion
    #region DrawConnections
    public static void DrawConnection(Connection connection)
    {
        Vector2 startP = connection.inputNode.position;
        Vector2 endP = connection.outputNode.position;
        Vector2 startT = Mathf.Clamp(startP.x - endP.x, 5, 10) * Vector2.right + startP;
        Vector2 endT = Mathf.Clamp(startP.x - endP.x, 5, 10) * Vector2.left + endP;

        Handles.DrawBezier(startP, endP, startT, endT, NodePortColor(connection.inputNode.GetPort(connection.inputFieldName)), null, 5);
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
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
    public static GUIData guiData;

    public static GUIStyle NodeBodyStyle, NodeTitleStyle, NodeOutLineStyle;
    public static Texture NodePortTexture;

    public static Dictionary<Type, NodeEditor> nodeTypeDic;
    public static Dictionary<Type, Color> portColor;

    public static void Init()
    {
        guiData = AssetDatabase.LoadAssetAtPath<GUIData>("Assets/FrameWorks/UNF/Scripts/Editor/Recources/GUI Data.asset");

        NodeBodyStyle = guiData.Style("NodeBody");
        NodeTitleStyle = guiData.Style("NodeTitle");
        NodeOutLineStyle = guiData.Style("OutLine");

        NodePortTexture = guiData.Texture("NodePort");

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
        Rect r = GetNodeRect(node);
        r.size += new Vector2(10, 10);
        r.position -= new Vector2(5, 5);
        if (node.graph.selectedNodes.Contains(node))
            GUI.Box(r, "",NodeOutLineStyle);
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
        if (nodeTypeDic.TryGetValue(node.GetType(), out ne) && ne != null)
        {
            return ne.GetHeight(node);
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

        if (nodeTypeDic.TryGetValue(node.GetType(), out ne) && ne != null)
        {
            return ne.GetWidth(node);
        }
        else
        {
            //Will be filled
            return 100;
        }
    }
    public static void NormalNodeDraw(Node node)
    {
        Rect r = GetNodeRect(node);
        GUI.Box(r, "", NodeBodyStyle);
        GUI.Box(new Rect(r.x, r.y, r.width, 30 * node.graph.ZoomAmm),node.name,NodeTitleStyle);
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
        float PortSize = 20;

        Rect parentRect = GetNodeRect(port.parentNode);

        NodeEditor ne;
        Vector2 position;
        if (nodeTypeDic.TryGetValue(port.parentNode.GetType(), out ne) && ne != null)
            position = ne.GetNodePortPosition(port);
        else
            position = parentRect.position + new Vector2(port.IOType == NodePort.portType.Input ? DistancFromBorder * port.parentNode.graph.ZoomAmm : parentRect.width - DistancFromBorder * port.parentNode.graph.ZoomAmm - PortSize * port.parentNode.graph.ZoomAmm, (port.drawingPos + 1) * port.parentNode.graph.ZoomAmm * 30 + DistancFromEachLine * port.parentNode.graph.ZoomAmm);

        Rect result = new Rect(position, new Vector2(PortSize, PortSize) * port.parentNode.graph.ZoomAmm);
        return result;
    }
    public static Color NodePortColor(NodePort port)
    {
        Color c = Color.black;
        if (!portColor.TryGetValue(port.Type, out c))
        {
            c = UnityEngine.Random.ColorHSV(0, 1, .5f, 1, .5f, 1, 1, 1);
            portColor.Add(port.Type, c);
        }
        return c;

    }
    public static void DrawNodePort(NodePort port)
    {
        Rect position = GetNodePortRect(port);
        GUI.color = NodePortColor(port);
        GUI.DrawTexture(position, NodePortTexture);
        GUI.color = Color.white;
    }
    #endregion
    #region DrawConnections
    public static void DrawConnection(Connection connection)
    {
        Vector2 startP = GetNodePortRect(connection.outputNode.GetPort(connection.outputFieldName)).center;
        Vector2 endP = GetNodePortRect(connection.inputNode.GetPort(connection.inputFieldName)).center;
        DrawSpline(startP, endP, NodePortColor(connection.inputNode.GetPort(connection.inputFieldName)));
    }
    public static void DrawSpline(Vector2 startP, Vector2 endP, Color color)
    {
        Vector2 startT = Mathf.Clamp(Vector2.Distance(startP, endP), 5, 1000) * Vector2.right / 2 + startP;
        Vector2 endT = Mathf.Clamp(Vector2.Distance(startP, endP), 5, 1000) * Vector2.left / 2 + endP;

        Handles.DrawBezier(startP, endP, startT, endT, color, null, 5);
    }
    #endregion
    #region NodeEditorWindow
    public static void DrawGraphData(GraphData data)
    {
        DrawGrid(data, 100, 20,new Color(0.7f, 0.7f, 0.7f) );

        NodeBodyStyle = guiData.Style("NodeBody");
        NodeTitleStyle = guiData.Style("NodeTitle");
        NodeOutLineStyle = guiData.Style("OutLine");

        foreach (var node in data.nodes)
        {
            DrawNode(node);
        }

        foreach (var connection in data.connections)
        {
            DrawConnection(connection);
        }
    }
    public static void DrawGrid(GraphData data, float bigCellSize, float smallCellSize,Color backGroundColor)
    {
        GUI.color = backGroundColor;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height),Texture2D.whiteTexture);
        GUI.color = Color.white;
    }
    #endregion
}
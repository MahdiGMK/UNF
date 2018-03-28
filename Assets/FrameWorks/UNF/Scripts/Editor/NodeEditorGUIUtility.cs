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
    public static Color haloColor = new Color(1, 0.7f, 0);
    public static GUIData guiData;

    public static GUIStyle NodeBodyStyle, NodeTitleStyle, NodeOutLineStyle;
    public static Texture NodePortTexture;

    public static Dictionary<Type, NodeEditor> nodeTypeDic;
    public static Dictionary<Type, Color> portColor;

    public static Dictionary<Node, Rect> nodeRects;
    public static Dictionary<NodePort, Rect> nodePortRects;

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
    public static void OnFrameStart()
    {
        nodeRects = new Dictionary<Node, Rect>();
        nodePortRects = new Dictionary<NodePort, Rect>();
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
        Rect result;
        if (!nodeRects.TryGetValue(node, out result))
        {
            result = new Rect(Vector2.zero, new Vector2(GetNodeWidth(node), GetNodeHeight(node)) * node.graph.ZoomAmm);
            result.center = (node.position + node.graph.cameraPosition) * node.graph.ZoomAmm + new Vector2(Screen.width, Screen.height - 24) / 2;
            nodeRects.Add(node, result);
        }
        return result;
    }
    public static void DrawNode(Node node)
    {
        SetNodeName(node);
        Rect r = GetNodeRect(node);
        r.size += new Vector2(5, 5);
        r.position -= new Vector2(2.5f, 2.5f);
        GUI.color = haloColor;
        if (node.graph.selectedNodes.Contains(node))
            GUI.Box(r, "", NodeOutLineStyle);
        GUI.color = Color.white;

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
        if (node.Name == "")
        {
            node.Name = "New " + node.GetType();
        }
        if (node.name != node.Name)
        {
            node.name = node.Name;
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
            return 250;
        }
    }
    public static void NormalNodeDraw(Node node)
    {
        Rect r = GetNodeRect(node);
        GUI.color = node.BodyColor();
        GUI.Box(r, "", NodeBodyStyle);
        GUI.color = node.TitleColor();
        GUI.Box(new Rect(r.x, r.y, r.width, 30 * node.graph.ZoomAmm), node.name + "(" + node.GetType() + ")", NodeTitleStyle);
        GUI.color = Color.white;
        for (int i = 0; i < node.ports.Count; i++)
        {
            DrawNodePort(node.ports[i]);
        }
        for (int i = 0; i < node.fields.Count; i++)
        {
            DrawPorperty(node.fields[i], i, node);
        }
    }
    #endregion
    #region DrawNodePorts
    public static float PropertyDistancFromBorder = 2.5f;
    public static float PropertyDistancFromEachLine = 5f;
    public static float PortSize = 20;
    public static Rect GetNodePortRect(NodePort port)
    {
        Rect result;
        if (!nodePortRects.TryGetValue(port, out result))
        {
            Rect parentRect = GetNodeRect(port.parentNode);

            NodeEditor ne;
            Vector2 position;
            if (nodeTypeDic.TryGetValue(port.parentNode.GetType(), out ne) && ne != null)
                position = ne.GetNodePortPosition(port);
            else
                position = parentRect.position + new Vector2(port.IOType == NodePort.portType.Input ? PropertyDistancFromBorder * port.parentNode.graph.ZoomAmm : parentRect.width - PropertyDistancFromBorder * port.parentNode.graph.ZoomAmm - PortSize * port.parentNode.graph.ZoomAmm, (port.drawingPos + 1) * port.parentNode.graph.ZoomAmm * 30 + PropertyDistancFromEachLine * port.parentNode.graph.ZoomAmm);

            result = new Rect(position, new Vector2(PortSize, PortSize) * port.parentNode.graph.ZoomAmm);
            nodePortRects.Add(port, result);
        }
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
        Rect nodePos = GetNodeRect(port.parentNode);
        GUI.color = NodePortColor(port);
        GUI.DrawTexture(position, NodePortTexture);
        GUI.color = Color.white;
    }
    public static void DrawPorperty(int portIndex, NodePort port)
    {
        Rect nodeRect = GetNodeRect(port.parentNode);
        SerializedObject so = new SerializedObject(port.parentNode);
        SerializedProperty sp = so.FindProperty(port.fieldName);
        Rect rect = new Rect(Vector2.zero, new Vector2(nodeRect.width - PropertyDistancFromBorder * 2 - PortSize * port.parentNode.graph.ZoomAmm, EditorGUI.GetPropertyHeight(sp)));
        rect.position = new Vector2(nodeRect.x + (PropertyDistancFromBorder + PortSize) * port.parentNode.graph.ZoomAmm, 0);
        rect.center = new Vector2(rect.center.x, nodeRect.position.y + ((portIndex + 1) * 30 + PropertyDistancFromEachLine + PortSize / 2) * port.parentNode.graph.ZoomAmm);
        Action act = () =>
        {
            EditorGUI.PropertyField(rect, sp);
            so.ApplyModifiedProperties();
        };
        if (port.IOType == NodePort.portType.Input)
        {
            if (port.showBackValueMethod == NodePort.showBackingValueMethod.Always)
                act();
            else if (port.showBackValueMethod == NodePort.showBackingValueMethod.Unconnected && port.connections.Count == 0)
                act();
            else
                GUI.Label(rect, ObjectNames.NicifyVariableName(port.fieldName));
        }
        else
        {
            rect.x -= PortSize * port.parentNode.graph.ZoomAmm;
            GUI.Label(rect, ObjectNames.NicifyVariableName(port.fieldName));
        }
    }
    public static void DrawPorperty(string fieldName, int fieldIndex, Node node)
    {
        NodePort port = node.ports.Find(obj => { return obj.fieldName == fieldName; });
        if (port == null)
        {
            Rect nodeRect = GetNodeRect(node);
            SerializedObject so = new SerializedObject(node);
            SerializedProperty sp = so.FindProperty(fieldName);
            Rect rect = new Rect(Vector2.zero, new Vector2(nodeRect.width - PropertyDistancFromBorder * 2, EditorGUI.GetPropertyHeight(sp)));
            rect.center = new Vector2(nodeRect.center.x, nodeRect.position.y + ((fieldIndex + 1) * 30 + PropertyDistancFromEachLine + PortSize / 2) * node.graph.ZoomAmm);

            //GUI.Box(rect, "");
            EditorGUI.PropertyField(rect, sp);

            so.ApplyModifiedProperties();
        }
        else
        {
            DrawPorperty(fieldIndex, port);
        }
    }
    #endregion
    #region DrawConnections
    public static void DrawConnection(Connection connection)
    {
        Vector2 startP = GetNodePortRect(connection.outputNode.GetPort(connection.outputFieldName)).center;
        Vector2 endP = GetNodePortRect(connection.inputNode.GetPort(connection.inputFieldName)).center;
        DrawSpline(startP, endP, NodePortColor(connection.inputNode.GetPort(connection.inputFieldName)), 7.5f * connection.inputNode.graph.ZoomAmm);
    }
    public static void DrawSpline(Vector2 startP, Vector2 endP, Color color, float size)
    {
        Vector2 startT = Mathf.Clamp(Vector2.Distance(startP, endP), 5, 1000) * Vector2.right / 2 + startP;
        Vector2 endT = Mathf.Clamp(Vector2.Distance(startP, endP), 5, 1000) * Vector2.left / 2 + endP;

        Handles.DrawBezier(startP, endP, startT, endT, color, null, size);
    }
    #endregion
    #region NodeEditorWindow
    public static void DrawGraphData(GraphData data)
    {
        OnFrameStart();

        DrawGrid(data, 100, 20, new Color(0.7f, 0.7f, 0.7f));

        NodeBodyStyle = guiData.Style("NodeBody");
        NodeTitleStyle = guiData.Style("NodeTitle");
        NodeOutLineStyle = guiData.Style("OutLine");

        foreach (var connection in data.connections)
        {
            DrawConnection(connection);
        }
        foreach (var node in data.nodes)
        {
            DrawNode(node);
        }
        Rect r = new Rect();
        r.width = 150;
        r.height = 100;
        r.x = Screen.width - 10 - r.width;
        r.y = 10;

        DrawNodeMap(data, r);
        DrawMouseDragHalo(data);
    }
    public static void DrawGrid(GraphData data, float bigCellSize, float smallCellSize, Color backGroundColor)
    {
        GUI.color = backGroundColor;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;
    }
    public static void DrawNodeMap(GraphData data, Rect rect)
    {
        #region Draw Background
        {
            GUI.color = new Color(1, 1, 1, 0.8f);
            GUI.DrawTexture(new Rect(rect.x - 5, rect.y - 5, rect.width + 10, rect.height + 10), Texture2D.whiteTexture);
            GUI.color = new Color(0, 0, 0, 0.3f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
        }
        #endregion
        float minX, maxX, minY, maxY;
        minX = minY = float.MaxValue;
        maxX = maxY = float.MinValue;

        List<Rect> nodeRects = new List<Rect>();
        List<string> nodeNames = new List<string>();
        #region Get Node Rects
        foreach (var node in data.nodes)
        {
            Rect r = GetNodeRect(node);
            nodeRects.Add(r);
            nodeNames.Add(node.name);
            if (r.xMin < minX)
                minX = r.xMin;
            if (r.xMax > maxX)
                maxX = r.xMax;
            if (r.yMin < minY)
                minY = r.yMin;
            if (r.yMax > maxY)
                maxY = r.yMax;
        }
        #endregion
        Rect holderRect = Rect.zero;
        holderRect.xMin = minX - 100; holderRect.xMax = maxX + 100;
        holderRect.yMin = minY - 100; holderRect.yMax = maxY + 100;
        #region Draw Nodes
        for (int i = 0; i < nodeRects.Count; i++)
        {
            Rect nodeRect = nodeRects[i];
            string nodeName = nodeNames[i];
            Rect nodeR = new Rect((nodeRect.x - holderRect.x) / holderRect.width * rect.width + rect.x, (nodeRect.y - holderRect.y) / holderRect.height * rect.height + rect.y, nodeRect.width / holderRect.width * rect.width, nodeRect.height / holderRect.height * rect.height);
            GUI.color = new Color(0, 0, 0, 0.5f);
            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.fontSize = (int)(nodeR.size.magnitude / rect.size.magnitude * 30);
            GUI.Box(nodeR, nodeName, style);
            GUI.color = Color.white;
        }
        #endregion
        #region Draw Camera
        {
            Rect cameraR = new Rect();

            cameraR = holderRect;

            if (cameraR.xMin < 0)
                cameraR.xMin = 0;
            if (cameraR.xMax > Screen.width)
                cameraR.xMax = Screen.width;
            if (cameraR.yMin < 0)
                cameraR.yMin = 0;
            if (cameraR.yMax > Screen.height - 24)
                cameraR.yMax = Screen.height - 24;


            cameraR.x = (cameraR.x - holderRect.x) / holderRect.width * rect.width + rect.x;
            cameraR.y = (cameraR.y - holderRect.y) / holderRect.height * rect.height + rect.y;

            cameraR.width = cameraR.width / holderRect.width * rect.width;
            cameraR.height = cameraR.height / holderRect.height * rect.height;

            if (cameraR.width < 0)
                cameraR.width = 0;
            if (cameraR.height < 0)
                cameraR.height = 0;
            if (cameraR.x > rect.xMax)
                cameraR.x = rect.xMax;
            if (cameraR.y > rect.yMax)
                cameraR.y = rect.yMax;
            GUI.color = new Color(0, 0, 0, 0.2f);
            GUI.Box(cameraR, "**View**");
            GUI.color = Color.white;
        }
        #endregion

    }
    public static void DrawMouseDragHalo(GraphData data)
    {
        if (NodeEditorHandles.mouseDragRect.x > 0)
        {
            if (Event.current.shift)
                GUI.color = new Color(0, 0.9f, 0, 0.3f);
            else if (Event.current.control)
                GUI.color = new Color(0.9f, 0, 0, 0.3f);
            else
                GUI.color = new Color(0, 0.4f, 0.5f, 0.3f);
            GUI.Box(NodeEditorHandles.mouseDragRect, "");
            foreach (var node in data.nodes)
            {
                Rect nodeRect = GetNodeRect(node);
                if (nodeRect.Overlaps(NodeEditorHandles.mouseDragRect))
                {
                    if (Event.current.shift && !data.selectedNodes.Contains(node))
                        GUI.Box(new Rect(nodeRect.position - new Vector2(1, 1), nodeRect.size + new Vector2(2, 2)), "");
                    else if (Event.current.control && data.selectedNodes.Contains(node))
                        GUI.Box(new Rect(nodeRect.position - new Vector2(1, 1), nodeRect.size + new Vector2(2, 2)), "");
                    else if (!Event.current.shift && !Event.current.control)
                        GUI.Box(new Rect(nodeRect.position - new Vector2(1, 1), nodeRect.size + new Vector2(2, 2)), "");
                }
            }
            GUI.color = Color.white;
        }
    }
    #endregion
}
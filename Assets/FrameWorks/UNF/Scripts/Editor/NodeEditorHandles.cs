using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public static class NodeEditorHandles
{
    public static Node GetHoveredNode(GraphData data, Vector2 p)
    {
        foreach (var node in data.nodes)
        {
            if (NodeEditorGUIUtility.GetNodeRect(node).Contains(p))
            {
                return node;
            }
        }
        return null;
    }
    public static NodePort GetHoveredNodePort(Node node, Vector2 p)
    {
        foreach (var port in node.ports)
        {
            if (NodeEditorGUIUtility.GetNodePortRect(port).Contains(p))
            {
                return port;
            }
        }
        return null;
    }

    public static void HandleGraphData(GraphData data)
    {
        if (Event.current.control && Event.current.shift)
        {
            UnityEngine.Object script = null;
            if (Event.current.keyCode == KeyCode.H)
            {
                script = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/FrameWorks/UNF/Scripts/Editor/NodeEditorHandles.cs");
                AssetDatabase.OpenAsset(script);
            }
            if (Event.current.keyCode == KeyCode.G)
            {
                script = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/FrameWorks/UNF/Scripts/Editor/NodeEditorGUIUtility.cs");
                AssetDatabase.OpenAsset(script);
            }
        }
        DoMouseHandles(data);
    }
    public static bool mouseDragging;
    public static void DoMouseHandles(GraphData data)
    {
        Node hoveredNode = GetHoveredNode(data, Event.current.mousePosition);
        NodePort hoveredNodePort = null;
        if (hoveredNode)
            hoveredNodePort = GetHoveredNodePort(hoveredNode, Event.current.mousePosition);

        if (data.selectedNodePort != null && data.selectedNodePort.parentNode == null)
            data.selectedNodePort = null;
        switch (Event.current.type)
        {
            case EventType.MouseDown:
                switch (Event.current.button)
                {
                    //L
                    case 0:
                        #region Selection
                        if (hoveredNode)
                        {
                            Debug.Log("0");
                            if (hoveredNodePort == null)
                            {
                                if (Event.current.shift)
                                {
                                    if (!data.selectedNodes.Contains(hoveredNode))
                                    {
                                        data.selectedNodes.Add(hoveredNode);
                                    }
                                }
                                else if (Event.current.control)
                                {
                                    if (data.selectedNodes.Contains(hoveredNode))
                                    {
                                        data.selectedNodes.Remove(hoveredNode);
                                    }
                                }
                                else
                                {
                                    if (data.selectedNodes.Contains(hoveredNode))
                                    {

                                    }
                                    else
                                    {
                                        data.selectedNodes.Clear();
                                        data.selectedNodes.Add(hoveredNode);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!Event.current.shift && !Event.current.control)
                            {
                                data.selectedNodes.Clear();
                            }
                        }
                        #endregion
                        #region ConnectionCreation
                        if (hoveredNodePort != null)
                        {
                            data.selectedNodePort = hoveredNodePort;
                        }
                        #endregion
                        break;
                    //M
                    case 2:
                        break;
                    //R
                    case 1:
                        #region ConnectionDestroction
                        if (hoveredNodePort != null)
                        {
                            if (hoveredNodePort.connections.Count > 0)
                                data.DestroyConnection(hoveredNodePort.connections[0]);
                        }
                        #endregion
                        break;
                }
                if (data.selectedNodes.Count > 0)
                    Selection.objects = data.selectedNodes.ToArray();
                else
                    Selection.objects = new UnityEngine.Object[] { data };
                break;
            case EventType.MouseDrag:
                switch (Event.current.button)
                {
                    //L
                    case 0:
                        if (data.selectedNodes.Count > 0 && data.selectedNodePort == null)
                        {
                            foreach (var node in data.selectedNodes)
                            {
                                node.position += Event.current.delta / data.ZoomAmm;
                            }
                        }
                        break;
                    //M
                    case 2:
                        HandleCameraPan(data, Event.current.delta);
                        break;
                    //R
                    case 1:
                        break;
                }
                break;
            case EventType.MouseUp:
                switch (Event.current.button)
                {
                    //L
                    case 0:
                        #region ConnectionCreation
                        if (data.selectedNodePort != null && hoveredNodePort != null)
                            if (hoveredNodePort != data.selectedNodePort)
                            {
                                if (hoveredNodePort.IOType == NodePort.portType.Input && data.selectedNodePort.IOType == NodePort.portType.Output)
                                    data.TryCreateConnection(hoveredNodePort, data.selectedNodePort);
                                else if (hoveredNodePort.IOType == NodePort.portType.Output && data.selectedNodePort.IOType == NodePort.portType.Input)
                                    data.TryCreateConnection(data.selectedNodePort, hoveredNodePort);
                            }
                        data.selectedNodePort = null;
                        #endregion
                        break;
                    //M
                    case 2:
                        break;
                    //R
                    case 1:
                        break;
                }
                break;

            case EventType.ScrollWheel:
                HandleCameraZoomToPoint(data, Event.current.delta.y, 2.5f, Event.current.mousePosition);
                break;
        }
        if (data.selectedNodePort != null)
        {
            Color splineColor = Color.black;
            if (data.selectedNodePort.Type != null)
                splineColor = NodeEditorGUIUtility.NodePortColor(data.selectedNodePort);
            NodeEditorGUIUtility.DrawSpline(NodeEditorGUIUtility.GetNodePortRect(data.selectedNodePort).center, Event.current.mousePosition, splineColor, 7.5f * data.ZoomAmm);
        }
    }
    public static void HandleCameraPan(GraphData data, Vector2 delta)
    {
        //Pan
        data.cameraPosition += delta / data.ZoomAmm;
    }
    public static void HandleCameraZoomToCenter(GraphData data, float delta, float sensitivity)
    {
        //Zoom
        float d = Mathf.Clamp(delta, -0.1f, 0.1f) * -sensitivity;
        data.ZoomAmm = Mathf.Clamp(data.ZoomAmm + d, 0.5f, 1.5f);
    }
    public static void HandleCameraZoomToPoint(GraphData data, float delta, float sensitivity, Vector2 point)
    {
        //Zoom
        float df = Mathf.Clamp(delta, -0.1f, 0.1f) * -sensitivity;
        float prevZA = data.ZoomAmm;
        data.ZoomAmm = Mathf.Clamp(data.ZoomAmm + df, 0.5f, 1.5f);
        //GoToPoint
        if (data.ZoomAmm != prevZA)
        {
            Vector2 pointFC = point - new Vector2(Screen.width, Screen.height - 24) / 2;
            data.cameraPosition = Vector2.Lerp(data.cameraPosition, df > 0 ? data.cameraPosition - pointFC : data.cameraPosition + pointFC, Mathf.Abs(df));
        }
    }

}

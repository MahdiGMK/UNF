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
        DoMouseHandles(data);
    }
    public static void DoMouseHandles(GraphData data)
    {
        switch (Event.current.type)
        {
            case EventType.MouseDown:
                switch (Event.current.button)
                {
                    //L
                    case 0:
                        Node hoveredNode = GetHoveredNode(data, Event.current.mousePosition);
                        if (hoveredNode)
                        {
                            NodePort hoveredNodePort = GetHoveredNodePort(hoveredNode, Event.current.mousePosition);
                            if (hoveredNodePort != null)
                            {

                            }
                            else
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
                            if(!Event.current.shift && !Event.current.control)
                            {
                                data.selectedNodes.Clear();
                            }
                        }
                        break;
                    //M
                    case 2:
                        break;
                    //R
                    case 1:
                        break;
                }
                break;
            case EventType.MouseDrag:
                switch (Event.current.button)
                {
                    //L
                    case 0:
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

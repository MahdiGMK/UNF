using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public static class NodeEditorHandles
{
    public static Node GetHoveredNodes(GraphData data, Vector2 p)
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

    public static void HandleGraphData(GraphData data)
    {
        DoMouseHandles(data);
    }
    public static void DoMouseHandles(GraphData data)
    {
        switch (Event.current.button)
        {
            //L
            case 0:
                break;
            //M
            case 2:
                switch (Event.current.type)
                {
                    case EventType.MouseDrag:
                        HandleCameraPan(data, Event.current.delta);
                        break;
                }
                break;
            //R
            case 1:
                break;
        }
    }
    public static void HandleCameraPan(GraphData data, Vector2 delta)
    {
        //Pan
        data.CameraPosition += delta;
    }
}

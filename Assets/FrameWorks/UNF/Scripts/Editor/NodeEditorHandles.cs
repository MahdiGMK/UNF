using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public static class NodeEditorHandles
{
    public static Node GetHoveredNodes(GraphData data,Vector2 p)
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
        Debug.Log( GetHoveredNodes(data, Event.current.mousePosition));
    }
}

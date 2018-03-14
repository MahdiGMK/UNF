using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[InitializeOnLoad]
public class NodeEditorWindow : EditorWindow
{
    public GraphData data;
    public static NodeEditorWindow GetWindow(GraphData data)
    {
        NodeEditorWindow window = CreateInstance<NodeEditorWindow>();
        window.data = data;
        window.Show();
        return window;
    }
    public void OnGUI()
    {
        if(data)
        {
            NodeEditorGUIUtility.DrawGraphData(data);
        }
    }
}

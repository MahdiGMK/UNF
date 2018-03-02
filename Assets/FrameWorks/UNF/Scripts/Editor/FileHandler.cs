using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class FileHandler
{
    [OnOpenAsset(0)]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        Object obj = EditorUtility.InstanceIDToObject(instanceID);
        if (obj is GraphData)
        {
            GraphData data = (GraphData)obj;
            NodeEditorWindow.GetWindow(data);
            return true;
        }
        return false;
    }
}

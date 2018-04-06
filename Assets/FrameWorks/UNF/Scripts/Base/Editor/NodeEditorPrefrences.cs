using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class NodeEditorPreferences
{
    [Serializable]
    public class Setting : ISerializationCallbackReceiver
    {
        [SerializeField] public Color nodeSelectionColor = new Color(1, 0.4f, 0);

        [SerializeField] public Color backgroundColor = new Color(0.3f, 0.3f, 0.3f);
        //Grid
        [SerializeField] public Color gridLine1Color;

        [SerializeField] public Color gridLine2Color;
        //Mini Map
        [SerializeField] public Color miniMapNodeColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        [SerializeField] public Color miniMapBackgroundColor = new Color(0, 0, 0, 0.3f);
        [SerializeField] public Color miniMapBorderColor = new Color(1, 1, 1, 0.75f);
        [SerializeField] public Color miniMapViewColor = new Color(0, 0, 0, 0.2f);

        [NonSerialized] public Dictionary<Type, Color> typeColors = new Dictionary<Type, Color>();
        [SerializeField] string typeColorsData;
        public void OnAfterDeserialize()
        {
            typeColors = new Dictionary<Type, Color>();
            string[] allPairs = typeColorsData.Split(new char[] { ',' });
            foreach (var pair in allPairs)
            {
                string[] pairKeyVal = pair.Split(new char[] { ':' });
                Color color;
                if (ColorUtility.TryParseHtmlString(pairKeyVal[1], out color))
                    typeColors.Add(Type.GetType(pairKeyVal[0]), color);
            }
        }

        public void OnBeforeSerialize()
        {
            List<KeyValuePair<Type, Color>> allPairs = new List<KeyValuePair<Type, Color>>();
            foreach (var pair in typeColors)
            {
                allPairs.Add(pair);
            }
            typeColorsData = " ";
            for (int i = 0; i < allPairs.Count; i++)
            {
                typeColorsData += allPairs[i].Key + ":" + ColorUtility.ToHtmlStringRGB(allPairs[i].Value) + (i < allPairs.Count - 1 ? "," : "");
            }
        }
    }
    static bool prefsLoaded;
    static Setting setting;
    static Vector2 nodePortsScrollViewPos;
    [PreferenceItem("U-N-F")]
    public static void PreferencesGUI()
    {
        if (!prefsLoaded)
        {
            LoadSettings();
        }

        GUILayout.Label("Node");
        GUILayout.Space(10);
        setting.nodeSelectionColor = EditorGUILayout.ColorField("Sellection color", setting.nodeSelectionColor);
        GUILayout.Space(10);

        GUILayout.Label("Grid");
        GUILayout.Space(10);
        setting.backgroundColor = EditorGUILayout.ColorField("Background color", setting.backgroundColor);
        GUILayout.Space(5);
        setting.gridLine1Color = EditorGUILayout.ColorField("Line colors", setting.gridLine1Color);
        setting.gridLine2Color = EditorGUILayout.ColorField(" ", setting.gridLine2Color);

        GUILayout.Space(10);
        GUILayout.Label("Mini Map");
        GUILayout.Space(10);
        setting.miniMapBorderColor = EditorGUILayout.ColorField("Border Color", setting.miniMapBorderColor);
        setting.miniMapBackgroundColor = EditorGUILayout.ColorField("Background Color", setting.miniMapBackgroundColor);
        setting.miniMapNodeColor = EditorGUILayout.ColorField("Node Color", setting.miniMapNodeColor);
        setting.miniMapViewColor = EditorGUILayout.ColorField("View Color", setting.miniMapViewColor);
        GUILayout.Space(10);

        GUILayout.Label("Node Port Types");
        GUILayout.Space(10);
        List<KeyValuePair<Type, Color>> allPairs = new List<KeyValuePair<Type, Color>>();
        foreach (var pair in setting.typeColors)
        {
            allPairs.Add(pair);
        }

        foreach (var tc in allPairs)
        {
            setting.typeColors[tc.Key] = EditorGUILayout.ColorField(tc.Key.FullName, tc.Value);
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Use Defaults",GUILayout.Width(120)))
            setting = new Setting();
        if (GUI.changed)
        {
            SaveSettings();
        }
    }

    public static Setting currentSetting
    {
        get
        {
            if (setting == null)
                LoadSettings();
            return setting;
        }
    }

    static void SaveSettings()
    {
        EditorPrefs.SetString("UNF_Settings", JsonUtility.ToJson(setting));
    }
    static void LoadSettings()
    {
        if (EditorPrefs.HasKey("UNF_Settings"))
        {
            setting = JsonUtility.FromJson<Setting>(EditorPrefs.GetString("UNF_Settings"));
        }
        else
        {
            setting = new Setting();
            SaveSettings();
        }
        prefsLoaded = true;
    }
}

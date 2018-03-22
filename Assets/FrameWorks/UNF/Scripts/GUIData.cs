using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class GUIData : ScriptableObject
{
    public List<GUIStyle> styles;
    public List<GUITexture> textures;
    public GUIStyle Style(string name)
    {
        return styles.Find(obj =>
        {
            return obj.name == name;
        });
    }
    public Texture Texture(string name)
    {
        return textures.Find(obj =>
        {
            return obj.name == name;
        }).texture;
    }
}
[Serializable]
public class GUITexture
{
    public string name;
    public Texture texture;
}

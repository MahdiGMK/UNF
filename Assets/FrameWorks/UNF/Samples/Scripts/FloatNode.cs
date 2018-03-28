using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class FloatNode:Node
    {
    public float value;
    [Output]public float output;
    public override Color TitleColor()
    {
        return new Color(0,0.45f,0.6f);
    }
    public override object GetOutPutValue(string fieldName)
    {
        return value;
    }
}

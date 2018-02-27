using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Connection{
    public Node fromeNode;
    public string fromeFieldName;

    public Node toNode;
    public string toFieldName;
}

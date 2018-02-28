using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GraphData : ScriptableObject {
    public List<Node> nodes;
    public List<Connection> connections;
    public Vector2 CameraPosition;
}

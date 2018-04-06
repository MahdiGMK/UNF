using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Node(typeof(DebugLogNode),"Debug/Log")]
public class DebugLogNode : StateNode {
    [Input] public StateEvent Invoke;
    public string Massage;
    [Output] public StateEvent Next;

    public override void Act()
    {
        Debug.Log(Massage);
        ((StateEvent)GetPort("Next")).MoveState();
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Node(typeof(TestStateNode),"StateBased/test State Node",UsingID = "TSM" )]
public class TestStateNode : StateNode {
    [Input] public StateEvent StartEvent;
    [Output] public StateEvent EndEvent;
    public override void Act()
    {
    }
}

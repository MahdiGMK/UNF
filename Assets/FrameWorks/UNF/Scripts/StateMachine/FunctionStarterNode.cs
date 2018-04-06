using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Node(typeof(FunctionStarterNode),"StateMachineReq/FunctionStarterNode")]
public class FunctionStarterNode : StateNode {
    public string FunctionName = "new function";
    [Output] public StateEvent StateInvoker;
    public override void Act()
    {
        ((StateEvent)GetPort("StateInvoker")).MoveState();
    }

}

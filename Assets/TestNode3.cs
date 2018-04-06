using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Node(typeof(TestNode3),"MahdiGMK/test state",UsingID = "MahdiGMK")]
public class TestNode3 : StateNode {
    [Input] public StateEvent EventStarter;
    [Input,PortType(NodePort.connectionMethod.Multiple,showBackingValueMethod = NodePort.showBackingValueMethod.Never)] public bool I;
    [Output] public List<bool> O;
    public List<bool> s;
    [Output] public StateEvent EventFinisher;

    public override void Act()
    {
    }

    public override object GetOutPutValue(string fieldName)
    {
        if(fieldName == "O")
        {
            return GetInputValues(GetPort("I"));
        }
        return null;
    }
}

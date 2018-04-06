using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Node(typeof(TestNode2),"MahdiGMK/boolCollector",UsingID = "MahdiGMK")]
public class TestNode2 : Node {
    [Input,PortType(NodePort.connectionMethod.Multiple,showBackingValueMethod = NodePort.showBackingValueMethod.Never)] public bool I;
    [Output] public List<bool> O;

    public override object GetOutPutValue(string fieldName)
    {
        if(fieldName == "O")
        {
            return GetInputValues(GetPort("I"));
        }
        return null;
    }
}

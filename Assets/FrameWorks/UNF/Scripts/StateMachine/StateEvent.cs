using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateEvent : NodePort
{
    public StateEvent(string name, int pos, Type type, Node parent, portType portType, connectionMethod connectionMethod, showBackingValueMethod showBackingValueMethod) : base(name, pos, type, parent, portType, connectionMethod, showBackingValueMethod)
    {
        fieldName = name;
        parentNode = parent;
        Type = type;
        if (portType == portType.Input)
            connectMethod = connectionMethod.Multiple;
        else
            connectionMethod = connectionMethod.Single;
        showBackValueMethod = showBackingValueMethod.Never;
        IOType = portType;
        drawingPos = pos;
    }
    public void Act()
    {
        ((StateMachineGraphData)parentNode.graph).currentState = (StateNode)parentNode;
        ((StateNode)parentNode).Act();
    }
    public void MoveState()
    {
        if (connections.Count > 0)
        {
            if (IOType == portType.Input)
                ((StateEvent)connections[0].outputNode.GetPort(connections[0].outputFieldName)).Act();
            else
                ((StateEvent)connections[0].inputNode.GetPort(connections[0].inputFieldName)).Act();
        }
    }
}

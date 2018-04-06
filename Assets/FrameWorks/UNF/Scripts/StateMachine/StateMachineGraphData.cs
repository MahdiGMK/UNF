using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachineGraphData : GraphData
{
    public StateNode currentState;
    Dictionary<string, FunctionStarterNode> starterNodes;
    public override void Init()
    {
        base.Init();
        if (starterNodes == null)
        {
            starterNodes = new Dictionary<string, FunctionStarterNode>();
            foreach (var node in nodes)
            {
                if (node.GetType() == typeof(FunctionStarterNode))
                {
                    starterNodes.Add(((FunctionStarterNode)node).FunctionName, (FunctionStarterNode)node);
                }
            }
        }
    }
    public void Act(string TargetFuncion)
    {
        if (starterNodes == null)
        {
            Init();
        }

        if (starterNodes.ContainsKey(TargetFuncion))
        {
            currentState = starterNodes[TargetFuncion];
            currentState.Act();
        }
    }
}

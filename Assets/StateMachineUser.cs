using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineUser : MonoBehaviour {
    public StateMachineGraphData targetStateMachine;
	// Use this for initialization
	void Start () {
        StateMachineGraphData targetStateMachineAsset = targetStateMachine;
        //targetStateMachine = (StateMachineGraphData)GraphData.CopyGraphData(targetStateMachineAsset);
        targetStateMachine.Act("Start");
    }
	
	// Update is called once per frame
	void Update () {
        targetStateMachine.Act("Update");
    }
}

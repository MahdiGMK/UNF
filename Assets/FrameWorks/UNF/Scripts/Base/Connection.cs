using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Connection{
    public Node outputNode;
    public string outputFieldName;

    public virtual object MovedData
    {
        get
        {
            return outputNode.GetOutPutValue(outputFieldName);
        }
    }

    public Node inputNode;
    public string inputFieldName;
    public Connection(NodePort input,NodePort output)
    {
        outputNode = output.parentNode;
        outputFieldName = output.fieldName;
        //         Out
        //         \\//
        //          \/
        //          In
        inputNode = input.parentNode;
        inputFieldName = input.fieldName;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using FlowCanvas.Nodes;
using UnityEngine;

[Category("Actions/Utility")]
public class SampleNode : CallableActionNode<object>
{
    public override void Invoke(object obj)
    {
        Debug.Log(obj);
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameCore;
using Sirenix.OdinInspector;

public class MechaComponentDesignHelper : MonoBehaviour
{
    [LabelText("各品质配置列表")]
    public List<MechaComponentConfig> MechaComponentConfigs = new List<MechaComponentConfig>();

    void Awake()
    {
    }
}
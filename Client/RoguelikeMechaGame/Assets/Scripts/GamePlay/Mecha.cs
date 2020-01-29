using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mecha : PoolObject
{
    public MechaInfo MechaInfo;

    private List<MechaComponentBase> MechaComponents = new List<MechaComponentBase>();

    [SerializeField] private Transform MechaComponentBaseContainer;

    public void Initialize(MechaInfo mechaInfo)
    {
        MechaInfo = mechaInfo;
        foreach (MechaComponentInfo mci in mechaInfo.MechaComponentInfos)
        {
            MechaComponentBase mcb = MechaComponentBase.BaseInitialize(mci, MechaComponentBaseContainer);
            MechaComponents.Add(mcb);
        }
    }
}
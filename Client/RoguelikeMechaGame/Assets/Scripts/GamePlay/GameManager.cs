using System;
using UnityEngine;
using System.Collections;

public class GameManager : MonoSingleton<GameManager>
{
    public const int GridSize = 1;

    public Transform MechaComponentContainer;

    private void Start()
    {
        MechaComponentInfo mci = new MechaComponentInfo(MechaComponentType.Core, new GridPos(2, 2, 2, GridPos.Rotation.Clockwise_270));
        MechaComponentBase mcb = MechaComponentBase.BaseInitialize(mci, MechaComponentContainer);
        MechaComponentInfo mci2 = new MechaComponentInfo(MechaComponentType.Block, new GridPos(1, 1, 0, GridPos.Rotation.Clockwise_180));
        MechaComponentBase mcb2 = MechaComponentBase.BaseInitialize(mci2, MechaComponentContainer);
        MechaComponentInfo mci3 = new MechaComponentInfo(MechaComponentType.Gun, new GridPos(1, 0, 0, GridPos.Rotation.Clockwise_90));
        MechaComponentBase mcb3 = MechaComponentBase.BaseInitialize(mci3, MechaComponentContainer);
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class Mecha : PoolObject
{
    public MechaInfo MechaInfo;

    private List<MechaComponentBase> mechaComponents = new List<MechaComponentBase>();

    private MechaComponentBase[,] mechaComponentMatrix = new MechaComponentBase[ConfigManager.EDIT_AREA_SIZE * 2 + 1, ConfigManager.EDIT_AREA_SIZE * 2 + 1]; //[z,x]

    public void Initialize(MechaInfo mechaInfo)
    {
        MechaInfo = mechaInfo;
        RefreshMechaMatrix();
        foreach (MechaComponentInfo mci in mechaInfo.MechaComponentInfos)
        {
            AddMechaComponent(mci);
        }

        Initialize_Building(mechaInfo);
        Initialize_Fighting(mechaInfo);
    }

    public void ExertComponentBuffs()
    {
        foreach (MechaComponentBase mcb in mechaComponents)
        {
            mcb.ExertEffectOnOtherComponents();
        }
    }

    public void RemoveAllComponentBuffs()
    {
        foreach (MechaComponentBase mcb in mechaComponents)
        {
            mcb.UnlinkAllBuffs();
        }
    }

    void Update()
    {
        if (MechaInfo.MechaType == MechaType.Self)
        {
            Update_Building();
            Update_Fighting();
        }
    }

    void LateUpdate()
    {
        if (MechaInfo.MechaType == MechaType.Self)
        {
            if (GameManager.Instance.GetState() == GameState.Fighting)
            {
                LateUpdate_Fighting();
            }

            if (GameManager.Instance.GetState() == GameState.Building)
            {
                LateUpdate_Building();
            }
        }
    }

    public MechaComponentBase GetMechaComponent<T>() where T : IBuff
    {
        foreach (MechaComponentBase mcb in mechaComponents)
        {
            if (mcb is T)
            {
                return mcb;
            }
        }

        return null;
    }
}
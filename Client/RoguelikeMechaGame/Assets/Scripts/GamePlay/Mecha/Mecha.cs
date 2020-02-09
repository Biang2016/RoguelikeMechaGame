using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class Mecha : PoolObject
{
    public MechaInfo MechaInfo;

    private List<MechaComponentBase> mechaComponents = new List<MechaComponentBase>();

    private MechaComponentBase[,] mechaComponentMatrix = new MechaComponentBase[ConfigManager.EDIT_AREA_SIZE * 2 + 1, ConfigManager.EDIT_AREA_SIZE * 2 + 1]; //[z,x]

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        foreach (MechaComponentBase mcb in mechaComponents)
        {
            mcb.PoolRecycle();
        }

        mechaComponents.Clear();
    }

    public void Initialize(MechaInfo mechaInfo)
    {
        mechaComponentMatrix = new MechaComponentBase[ConfigManager.EDIT_AREA_SIZE * 2 + 1, ConfigManager.EDIT_AREA_SIZE * 2 + 1];
        MechaInfo = mechaInfo;
        RefreshMechaMatrix();
        foreach (MechaComponentInfo mci in mechaInfo.MechaComponentInfos)
        {
            AddMechaComponent(mci);
        }

        Initialize_Building(mechaInfo);
        Initialize_Fighting(mechaInfo);
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

    private void Die()
    {
        if (MechaInfo.MechaType == MechaType.Enemy)
        {
            BattleManager.Instance.EnemyMechas.Remove(this);
            PoolRecycle(0.5f);
        }
        else
        {
            //TODO endgame
        }
    }
}
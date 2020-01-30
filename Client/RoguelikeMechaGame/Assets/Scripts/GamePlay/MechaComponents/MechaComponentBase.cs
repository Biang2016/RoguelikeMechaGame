using System;
using UnityEngine;
using System.Collections;

public abstract class MechaComponentBase : PoolObject, IGridPos
{
    public static MechaComponentBase BaseInitialize(MechaComponentInfo mechaComponentInfo, Transform parent)
    {
        GameObjectPoolManager.PrefabNames prefabName = (GameObjectPoolManager.PrefabNames) Enum.Parse(typeof(GameObjectPoolManager.PrefabNames), "MechaComponent_" + mechaComponentInfo.M_MechaComponentType);
        MechaComponentBase mcb = GameObjectPoolManager.Instance.PoolDict[prefabName].AllocateGameObject<MechaComponentBase>(parent);
        mcb.Initialize(mechaComponentInfo);
        return mcb;
    }

    internal MechaComponentInfo MechaComponentInfo;

    public virtual void Initialize(MechaComponentInfo mechaComponentInfo)
    {
        MechaComponentInfo = mechaComponentInfo;
        GridPos.ApplyGridPosToLocalTrans(mechaComponentInfo.M_GridPos, transform);
    }

    public MechaComponentEditOperator MechaComponentEditOperator;
    public MechaComponentGrids MechaComponentGrids;
    public HitBoxRoot HitBoxRoot;

    #region Life

    private int m_LeftLife;

    public int M_LeftLife
    {
        get { return m_LeftLife; }
    }

    private int m_TotalLife;

    public int M_TotalLife
    {
        get { return m_TotalLife; }
    }

    public bool CheckAlive()
    {
        return M_LeftLife > 0;
    }

    public void AddLife(int addLifeValue)
    {
        m_TotalLife += addLifeValue;
        m_LeftLife += addLifeValue;
    }

    public void Heal(int healValue)
    {
    }

    public void Damage(int damage)
    {
        m_LeftLife -= damage;
    }

    public void HealAll()
    {
        m_LeftLife = M_TotalLife;
    }

    public void Change(int change)
    {
        m_LeftLife += change;
    }

    public void ChangeMaxLife(int change)
    {
        m_TotalLife += change;
    }

    #endregion

    public GridPos GetGridPos()
    {
        return GridPos.GetGridPosByLocalTrans(transform);
    }
}
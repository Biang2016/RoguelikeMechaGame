using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class MechaComponentBase : PoolObject, IGridPos, IDraggable, IBagItem
{
    internal Mecha ParentMecha = null;
    private GridSnapper GridSnapper;

    void Awake()
    {
        GridSnapper = GetComponent<GridSnapper>();
    }

    public static MechaComponentBase BaseInitialize(MechaComponentInfo mechaComponentInfo, Transform parent, Mecha parentMecha)
    {
        GameObjectPoolManager.PrefabNames prefabName = (GameObjectPoolManager.PrefabNames) Enum.Parse(typeof(GameObjectPoolManager.PrefabNames), "MechaComponent_" + mechaComponentInfo.M_MechaComponentType);
        MechaComponentBase mcb = GameObjectPoolManager.Instance.PoolDict[prefabName].AllocateGameObject<MechaComponentBase>(parent);
        mcb.Initialize(mechaComponentInfo, parentMecha);
        return mcb;
    }

    public MechaComponentInfo MechaComponentInfo;

    public virtual void Initialize(MechaComponentInfo mechaComponentInfo, Mecha parentMecha)
    {
        MechaComponentInfo = mechaComponentInfo;
        MechaComponentInfo.OccupiedGridPositions = CloneVariantUtils.List(MechaComponentGrids.MechaComponentGridPositions);
        GridPos.ApplyGridPosToLocalTrans(mechaComponentInfo.M_GridPos, transform, GameManager.GridSize);
        ParentMecha = parentMecha;
    }

    public MechaComponentGrids MechaComponentGrids;
    public HitBoxRoot HitBoxRoot;

    public GridPos GetGridPos()
    {
        return GridPos.GetGridPosByLocalTrans(transform, GameManager.GridSize);
    }

    public void Rotate()
    {
        transform.Rotate(0, 90, 0);
    }

    public void RefreshComponentOffset(Transform transform)
    {
        GridSnapper.Offset = transform;
    }

    #region Life

    private int _leftLife;

    public int M_LeftLife
    {
        get { return _leftLife; }
    }

    private int _totalLife;

    public int M_TotalLife
    {
        get { return _totalLife; }
    }

    public bool CheckAlive()
    {
        return M_LeftLife > 0;
    }

    public void AddLife(int addLifeValue)
    {
        _totalLife += addLifeValue;
        _leftLife += addLifeValue;
    }

    public void Heal(int healValue)
    {
    }

    public void Damage(int damage)
    {
        _leftLife -= damage;
    }

    public void HealAll()
    {
        _leftLife = M_TotalLife;
    }

    public void Change(int change)
    {
        _leftLife += change;
    }

    public void ChangeMaxLife(int change)
    {
        _totalLife += change;
    }

    #endregion

    #region  IDraggable

    public void DragComponent_OnMouseDown()
    {
    }

    public void DragComponent_OnMousePressed(DragAreaTypes dragAreaTypes)
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            Rotate();
        }

        switch (dragAreaTypes)
        {
            case DragAreaTypes.Bag:
            {
                break;
            }
        }
    }

    public void DragComponent_OnMouseUp(DragAreaTypes dragAreaTypes)
    {
        switch (dragAreaTypes)
        {
            case DragAreaTypes.Bag:
            {
                BagManager.Instance.AddMechaComponentToBag(MechaComponentInfo);
                PoolRecycle();
                break;
            }
        }
    }

    public void DragComponent_SetStates(ref bool canDrag, ref DragAreaTypes dragFrom)
    {
        canDrag = true;
        dragFrom = DragAreaTypes.Bag;
    }

    public float DragComponent_DragMinDistance()
    {
        return 0f;
    }

    public float DragComponent_DragMaxDistance()
    {
        return 9999f;
    }

    public void DragComponent_DragOutEffects()
    {
    }

    #endregion

    #region  IBagItem

    

    #endregion
}
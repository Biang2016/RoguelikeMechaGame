using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class MechaComponentBase : PoolObject, IDraggable
{
    internal Mecha ParentMecha = null;
    internal Draggable Draggable;
    private GridSnapper gridSnapper;

    void Awake()
    {
        gridSnapper = GetComponent<GridSnapper>();
        Draggable = GetComponent<Draggable>();
    }

    public static MechaComponentBase BaseInitialize(MechaComponentInfo mechaComponentInfo, Transform parent, Mecha parentMecha)
    {
        GameObjectPoolManager.PrefabNames prefabName = (GameObjectPoolManager.PrefabNames) Enum.Parse(typeof(GameObjectPoolManager.PrefabNames), "MechaComponent_" + mechaComponentInfo.MechaComponentType);
        MechaComponentBase mcb = GameObjectPoolManager.Instance.PoolDict[prefabName].AllocateGameObject<MechaComponentBase>(parent);
        mcb.Initialize(mechaComponentInfo, parentMecha);
        return mcb;
    }

    public MechaComponentInfo MechaComponentInfo;

    public virtual void Initialize(MechaComponentInfo mechaComponentInfo, Mecha parentMecha)
    {
        MechaComponentInfo = mechaComponentInfo;
        MechaComponentInfo.OccupiedGridPositions = CloneVariantUtils.List(MechaComponentGrids.MechaComponentGridPositions);
        GridPos.ApplyGridPosToLocalTrans(mechaComponentInfo.GridPos, transform, GameManager.GridSize);
        ParentMecha = parentMecha;
    }

    public MechaComponentGrids MechaComponentGrids;

    public void Rotate()
    {
        transform.Rotate(0, 90, 0);
    }

    #region Life

    private int _leftLife;

    public int M_LeftLife
    {
        get { return _leftLife; }
    }

    private int _totalLife;
    private float _dragComponentDragMinDistance;
    private float _dragComponentDragMaxDistance;

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

    #region IDraggable

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
                bool suc = BagManager.Instance.AddMechaComponentToBag(MechaComponentInfo);
                if (suc)
                {
                    PoolRecycle();
                }
                else
                {
                    DragManager.Instance.CurrentDrag.ReturnOriginalPositionRotation();
                }

                break;
            }
        }
    }

    public void DragComponent_SetStates(ref bool canDrag, ref DragAreaTypes dragFrom)
    {
        canDrag = true;
        dragFrom = DragAreaTypes.MechaEditorArea;
    }

    float IDraggable.DragComponent_DragMinDistance => 0f;

    float IDraggable.DragComponent_DragMaxDistance => 9999f;

    public void DragComponent_DragOutEffects()
    {
    }

    #endregion
}
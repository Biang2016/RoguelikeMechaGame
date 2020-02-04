using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class MechaComponentBase : PoolObject, IDraggable
{
    internal Mecha ParentMecha = null;
    internal Draggable Draggable;

    void Awake()
    {
        Draggable = GetComponent<Draggable>();
    }

    public static MechaComponentBase BaseInitialize(MechaComponentInfo mechaComponentInfo, Transform parent, Mecha parentMecha)
    {
        GameObjectPoolManager.PrefabNames prefabName = (GameObjectPoolManager.PrefabNames) Enum.Parse(typeof(GameObjectPoolManager.PrefabNames), "MechaComponent_" + mechaComponentInfo.MechaComponentType);
        MechaComponentBase mcb = GameObjectPoolManager.Instance.PoolDict[prefabName].AllocateGameObject<MechaComponentBase>(parent);
        mcb.Initialize(mechaComponentInfo, parentMecha);
        mcb.transform.rotation = Quaternion.Euler(0, 0, 0);
        return mcb;
    }

    public MechaComponentInfo MechaComponentInfo;

    public virtual void Initialize(MechaComponentInfo mechaComponentInfo, Mecha parentMecha)
    {
        IsDead = false;
        MechaComponentInfo = mechaComponentInfo;
        MechaComponentInfo.OccupiedGridPositions = CloneVariantUtils.List(MechaComponentGrids.MechaComponentGridPositions);
        GridPos.ApplyGridPosToLocalTrans(mechaComponentInfo.GridPos, transform, GameManager.GridSize);
        ParentMecha = parentMecha;
        AddLife(50);
    }

    public MechaComponentGrids MechaComponentGrids;

    public void Rotate()
    {
        transform.Rotate(0, 90, 0);
    }

    #region Life

    internal bool IsDead = false;


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
        FXManager.Instance.PlayFX(FX_Type.FX_BlockDamaged, transform.position);
        if (!IsDead && !CheckAlive())
        {
            FXManager.Instance.PlayFX(FX_Type.FX_BlockExplode, transform.position);
            ParentMecha.RemoveMechaComponent(this);
            PoolRecycle(0.2f);
        }
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
                ReturnToBag(true, true);
                break;
            }
        }
    }

    private bool ReturnToBag(bool cancelDrag, bool dragTheItem)
    {
        bool suc = BagManager.Instance.AddMechaComponentToBag(MechaComponentInfo, out BagItem bagItem);
        if (suc)
        {
            if (cancelDrag) DragManager.Instance.CancelCurrentDrag();
            if (dragTheItem)
            {
                DragManager.Instance.CurrentDrag = bagItem.gameObject.GetComponent<Draggable>();
                DragManager.Instance.CurrentDrag.IsOnDrag = true;
            }

            PoolRecycle();
        }

        return suc;
    }

    public void DragComponent_OnMouseUp(DragAreaTypes dragAreaTypes)
    {
        switch (dragAreaTypes)
        {
            case DragAreaTypes.Bag:
            {
                bool suc = ReturnToBag(false, false);
                if (!suc)
                {
                    DragManager.Instance.CurrentDrag.ReturnOriginalPositionRotation();
                }

                break;
            }
            case DragAreaTypes.MechaEditorArea:
            {
                //todo if not contact
                break;
            }
            case DragAreaTypes.DiscardedArea:
            {
                break;
            }
            case DragAreaTypes.None:
            {
                bool suc = ReturnToBag(false, false);
                if (!suc)
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
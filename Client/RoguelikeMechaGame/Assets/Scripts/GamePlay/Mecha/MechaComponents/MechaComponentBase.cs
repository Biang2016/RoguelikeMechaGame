using UnityEngine;
using System.Collections.Generic;

public abstract class MechaComponentBase : PoolObject, IDraggable
{
    internal Mecha ParentMecha = null;
    internal Draggable Draggable;

    internal MechaType MechaType => ParentMecha ? ParentMecha.MechaInfo.MechaType : MechaType.None;

    void Awake()
    {
        Draggable = GetComponent<Draggable>();
    }

    protected virtual void Update()
    {
    }

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        ParentMecha = null;

        foreach (FX lighteningFX in lighteningFXs)
        {
            lighteningFX.PoolRecycle();
        }

        lighteningFXs.Clear();
        isReturningToBag = false;
    }

    public static MechaComponentBase BaseInitialize(MechaComponentInfo mechaComponentInfo, Mecha parentMecha)
    {
        MechaComponentBase mcb = GameObjectPoolManager.Instance.MechaComponentPoolDict[mechaComponentInfo.MechaComponentType].AllocateGameObject<MechaComponentBase>(parentMecha ? parentMecha.transform : null);
        mcb.Initialize(mechaComponentInfo, parentMecha);
        return mcb;
    }

    public MechaComponentInfo MechaComponentInfo;

    private void Initialize(MechaComponentInfo mechaComponentInfo, Mecha parentMecha)
    {
        IsDead = false;
        UnlinkAllBuffs();
        MechaComponentInfo = mechaComponentInfo;
        GridPos.ApplyGridPosToLocalTrans(mechaComponentInfo.GridPos, transform, GameManager.GridSize);
        RefreshOccupiedGridPositions();
        ParentMecha = parentMecha;
        _totalLife = 50;
        _leftLife = 50;
    }

    public void SetGridPosition(GridPos gridPos)
    {
        if (!gridPos.Equals(MechaComponentInfo.GridPos))
        {
            MechaComponentInfo.GridPos = gridPos;
            GridPos.ApplyGridPosToLocalTrans(gridPos, transform, GameManager.GridSize);
            RefreshOccupiedGridPositions();
            ParentMecha?.RefreshMechaMatrix();
        }
    }

    public void RefreshOccupiedGridPositions()
    {
        if (BagManager.Instance.MechaComponentOccupiedGridPosDict.ContainsKey(MechaComponentInfo.MechaComponentType))
        {
            MechaComponentInfo.OccupiedGridPositions = GridPos.TransformOccupiedPositions(MechaComponentInfo.GridPos, BagManager.Instance.MechaComponentOccupiedGridPosDict[MechaComponentInfo.MechaComponentType]);
        }
    }

    public MechaComponentGrids MechaComponentGrids;
    public HitBoxRoot MechaHitBoxRoot;

    private void Rotate()
    {
        GridPos newGP = new GridPos(MechaComponentInfo.GridPos.x, MechaComponentInfo.GridPos.z, GridPos.RotateOrientationClockwise90(MechaComponentInfo.GridPos.orientation));
        SetGridPosition(newGP);
    }

    #region Buffs

    internal List<MechaComponentBuff> AttachedBuffs = new List<MechaComponentBuff>();
    internal List<MechaComponentBuff> GiveOutBuffs = new List<MechaComponentBuff>();

    public virtual void ExertEffectOnOtherComponents()
    {
    }

    public void UnlinkAllBuffs()
    {
        foreach (MechaComponentBuff buff in AttachedBuffs)
        {
            buff.RemoveBuff();
        }

        foreach (MechaComponentBuff buff in GiveOutBuffs)
        {
            buff.RemoveBuff();
        }

        AttachedBuffs.Clear();
        GiveOutBuffs.Clear();
    }

    #endregion

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

    private List<FX> lighteningFXs = new List<FX>();

    public void Damage(int damage)
    {
        if (_leftLife > M_TotalLife * 0.5f && _leftLife - damage <= M_TotalLife * 0.5f)
        {
            foreach (HitBox hb in MechaHitBoxRoot.HitBoxes)
            {
                FX lighteningFX = FXManager.Instance.PlayFX(FX_Type.FX_BlockDamagedLightening, hb.transform.position);
                lighteningFXs.Add(lighteningFX);
            }
        }

        _leftLife -= damage;
        FXManager.Instance.PlayFX(FX_Type.FX_BlockDamageHit, transform.position + Vector3.up * 0.5f);

        if (!IsDead && !CheckAlive())
        {
            IsDead = true;
            OnDied();
            PoolRecycle(0.2f);
        }
    }

    private void OnDied()
    {
        UnlinkAllBuffs();
        FXManager.Instance.PlayFX(FX_Type.FX_BlockExplode, transform.position);
        ParentMecha.RemoveMechaComponent(this);
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

        if (ParentMecha)
        {
            GridPos gridPos = GridPos.GetGridPosByMousePos(ParentMecha.transform, Vector3.up, GameManager.GridSize);
            gridPos.orientation = MechaComponentInfo.GridPos.orientation;
            SetGridPosition(gridPos);
        }
    }

    private bool isReturningToBag = false;

    public bool ReturnToBag(bool cancelDrag, bool dragTheItem)
    {
        bool suc = BagManager.Instance.AddMechaComponentToBag(MechaComponentInfo, out BagItem bagItem);
        if (suc)
        {
            if (cancelDrag)
            {
                isReturningToBag = true;
                DragManager.Instance.CancelCurrentDrag();
            }

            if (dragTheItem)
            {
                DragManager.Instance.CurrentDrag = bagItem.gameObject.GetComponent<Draggable>();
                DragManager.Instance.CurrentDrag.IsOnDrag = true;
            }

            ParentMecha?.RemoveMechaComponent(this);
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
                if (!isReturningToBag)
                {
                    bool suc = ReturnToBag(false, false);
                    if (!suc)
                    {
                        DragManager.Instance.CurrentDrag.ReturnOriginalPositionRotation();
                    }
                }

                break;
            }
            case DragAreaTypes.MechaEditorArea:
            {
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
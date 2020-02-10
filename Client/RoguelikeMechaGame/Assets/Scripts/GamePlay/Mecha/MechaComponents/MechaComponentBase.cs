using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public abstract class MechaComponentBase : PoolObject, IDraggable
{
    internal Mecha ParentMecha = null;
    [SerializeField] private GameObject Models;
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
        ParentMecha = null;
        MechaHitBoxRoot.SetInBattle(false);
        base.PoolRecycle();
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
        M_TotalLife = mechaComponentInfo.TotalLife;
        M_LeftLife = mechaComponentInfo.TotalLife;
        MechaHitBoxRoot.SetInBattle(true);
        Child_Initialize();
    }

    protected virtual void Child_Initialize()
    {
    }

    public void SetGridPosition(GridPos gridPos)
    {
        if (!gridPos.Equals(MechaComponentInfo.GridPos))
        {
            foreach (GridPos gp in MechaComponentInfo.OccupiedGridPositions)
            {
                GridPos gp_rot = GridPos.RotateGridPos(gp - MechaComponentInfo.GridPos, (GridPos.Orientation) ((gridPos.orientation - MechaComponentInfo.GridPos.orientation + 4) % 4));
                GridPos newGP = gp_rot + gridPos;
                if (newGP.x > ConfigManager.EDIT_AREA_SIZE)
                {
                    SetGridPosition(new GridPos(gridPos.x - 1, gridPos.z, gridPos.orientation));
                    return;
                }

                if (newGP.x < -ConfigManager.EDIT_AREA_SIZE)
                {
                    SetGridPosition(new GridPos(gridPos.x + 1, gridPos.z, gridPos.orientation));
                    return;
                }

                if (newGP.z > ConfigManager.EDIT_AREA_SIZE)
                {
                    SetGridPosition(new GridPos(gridPos.x, gridPos.z - 1, gridPos.orientation));
                    return;
                }

                if (newGP.z < -ConfigManager.EDIT_AREA_SIZE)
                {
                    SetGridPosition(new GridPos(gridPos.x, gridPos.z + 1, gridPos.orientation));
                    return;
                }
            }

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

    public void SetShown(bool shown)
    {
        Models.SetActive(shown);
    }

    #region Buffs

    internal List<MechaComponentBuff> AttachedBuffs = new List<MechaComponentBuff>();
    internal List<MechaComponentBuff> GiveOutBuffs = new List<MechaComponentBuff>();

    public virtual void ExertEffectOnOtherComponents()
    {
    }

    public void UnlinkAllBuffs()
    {
        //foreach (MechaComponentBuff buff in AttachedBuffs)
        //{
        //    buff.RemoveBuff();
        //}

        //foreach (MechaComponentBuff buff in GiveOutBuffs)
        //{
        //    buff.RemoveBuff();
        //}

        //AttachedBuffs.Clear();
        //GiveOutBuffs.Clear();
    }

    #endregion

    #region Life

    internal bool IsDead = false;

    internal UnityAction<int, int> OnLifeChange;

    private int _leftLife;

    public int M_LeftLife
    {
        get { return _leftLife; }
        set
        {
            if (value < 0)
            {
                value = 0;
            }

            if (_leftLife != value)
            {
                _leftLife = value;
                OnLifeChange?.Invoke(_leftLife, M_TotalLife);
            }
        }
    }

    private int _totalLife;

    public int M_TotalLife
    {
        get { return _totalLife; }
        set
        {
            if (_totalLife != value)
            {
                _totalLife = value;
                OnLifeChange?.Invoke(M_LeftLife, _totalLife);
            }
        }
    }

    public bool CheckAlive()
    {
        return M_LeftLife > 0;
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

        M_LeftLife -= damage;
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
        ParentMecha?.RemoveMechaComponent(this);
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

        if (ParentMecha && ParentMecha.MechaInfo.MechaType == MechaType.Self)
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
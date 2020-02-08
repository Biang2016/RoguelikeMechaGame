using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class Mecha
{
    private List<MechaComponentBase> forbidComponentIndicators = new List<MechaComponentBase>();
    private List<MechaComponentBase> isolatedComponentIndicators = new List<MechaComponentBase>();

    [SerializeField] private Transform MechaComponentContainer;
    [SerializeField] private Transform ForbidComponentIndicatorContainer;
    [SerializeField] private Transform IsolatedComponentIndicatorContainer;
    public MechaEditArea MechaEditArea;

    private void Initialize_Building(MechaInfo mechaInfo)
    {
        MechaEditArea.gameObject.SetActive(mechaInfo.MechaType == MechaType.Self);
        MechaEditArea.Hide();
        GridShown = false;
        SlotLightsShown = false;
    }

    public void AddMechaComponent(MechaComponentInfo mci)
    {
        MechaComponentBase mcb = MechaComponentBase.BaseInitialize(mci, this);
        AddMechaComponent(mcb);
    }

    public void AddMechaComponent(MechaComponentBase mcb)
    {
        mechaComponents.Add(mcb);
        mcb.transform.SetParent(MechaComponentContainer);
        mcb.MechaComponentGrids.SetGridShown(GridShown);
        mcb.MechaComponentGrids.SetSlotLightsShown(SlotLightsShown);
        RefreshMechaMatrix();
    }

    public void RemoveMechaComponent(MechaComponentBase mcb)
    {
        if (mcb.MechaComponentInfo.MechaComponentType == MechaComponentType.Forbid)
        {
            if (forbidComponentIndicators.Contains(mcb))
            {
                forbidComponentIndicators.Remove(mcb);
            }
        }
        else if (mcb.MechaComponentInfo.MechaComponentType == MechaComponentType.Isolated)
        {
            if (isolatedComponentIndicators.Contains(mcb))
            {
                isolatedComponentIndicators.Remove(mcb);
            }
        }
        else
        {
            if (mechaComponents.Contains(mcb))
            {
                mechaComponents.Remove(mcb);
                RefreshMechaMatrix();
            }
        }
    }

    private void ClearForbidComponentIndicators()
    {
        foreach (MechaComponentBase mcb in forbidComponentIndicators)
        {
            mcb.PoolRecycle();
        }

        forbidComponentIndicators.Clear();
    }

    private void ClearIsolatedComponentIndicators()
    {
        foreach (MechaComponentBase mcb in isolatedComponentIndicators)
        {
            mcb.PoolRecycle();
        }

        isolatedComponentIndicators.Clear();
    }

    private void AddForbidComponentIndicator(GridPos gp)
    {
        MechaComponentBase mcb = MechaComponentBase.BaseInitialize(new MechaComponentInfo(MechaComponentType.Forbid, gp), this);
        mcb.transform.SetParent(ForbidComponentIndicatorContainer);
        forbidComponentIndicators.Add(mcb);
    }

    private void AddIsolatedComponentIndicator(GridPos gp)
    {
        MechaComponentBase mcb = MechaComponentBase.BaseInitialize(new MechaComponentInfo(MechaComponentType.Isolated, gp), this);
        mcb.transform.SetParent(IsolatedComponentIndicatorContainer);
        forbidComponentIndicators.Add(mcb);
    }

    void Update_Building()
    {
        if (Input.GetKeyUp(KeyCode.G))
        {
            SlotLightsShown = !SlotLightsShown;
            GridShown = !GridShown;
        }
    }

    void LateUpdate_Building()
    {
    }

    public void MoveCenter(GridPos delta_local_GP)
    {
        foreach (MechaComponentBase mcb in mechaComponents)
        {
            mcb.transform.parent = MechaComponentContainer;
            GridPos newGP = new GridPos(mcb.MechaComponentInfo.GridPos.x + delta_local_GP.x, mcb.MechaComponentInfo.GridPos.z + delta_local_GP.z);
            mcb.SetGridPosition(newGP);
            mcb.RefreshOccupiedGridPositions();
        }

        RefreshMechaMatrix();
    }

    private bool _slotLightsShown = true;

    public bool SlotLightsShown
    {
        get { return _slotLightsShown; }
        set
        {
            if (_slotLightsShown != value)
            {
                foreach (MechaComponentBase mcb in mechaComponents)
                {
                    mcb.MechaComponentGrids.SetSlotLightsShown(value);
                }
            }

            _slotLightsShown = value;
        }
    }

    private bool _gridShown = true;

    public bool GridShown
    {
        get { return _gridShown; }
        set
        {
            if (_gridShown != value)
            {
                foreach (MechaComponentBase mcb in mechaComponents)
                {
                    mcb.MechaComponentGrids.SetGridShown(value);
                }
            }

            _gridShown = value;
        }
    }
}
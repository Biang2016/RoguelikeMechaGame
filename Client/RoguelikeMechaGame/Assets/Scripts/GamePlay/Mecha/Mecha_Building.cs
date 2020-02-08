using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class Mecha
{
    private List<MechaComponentBase> forbidComponentIndicators = new List<MechaComponentBase>();

    [SerializeField] private Transform MechaComponentContainer;
    [SerializeField] private Transform ForbotComponentIndicatorContainer;
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
        else
        {
            if (mechaComponents.Contains(mcb))
            {
                mechaComponents.Remove(mcb);
                RefreshMechaMatrix();
            }
        }
    }

    private void ClearForbidComponents()
    {
        foreach (MechaComponentBase mcb in forbidComponentIndicators)
        {
            mcb.PoolRecycle();
        }

        forbidComponentIndicators.Clear();
    }

    private void AddForbidComponentIndicator(GridPos gp)
    {
        MechaComponentBase mcb = MechaComponentBase.BaseInitialize(new MechaComponentInfo(MechaComponentType.Forbid, gp), this);
        mcb.transform.SetParent(ForbotComponentIndicatorContainer);
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

    public void RefreshCenter()
    {
        GridPos totalGP = new GridPos();
        if (mechaComponents.Count > 0)
        {
            foreach (MechaComponentBase mcb in mechaComponents)
            {
                mcb.transform.parent = null;
                totalGP.x += mcb.MechaComponentInfo.GridPos.x;
                totalGP.z += mcb.MechaComponentInfo.GridPos.z;
            }

            GridPos centerGP = new GridPos(Mathf.RoundToInt((float) totalGP.x / mechaComponents.Count), Mathf.RoundToInt((float) totalGP.z / mechaComponents.Count));
            transform.localPosition -= new Vector3(centerGP.x * GameManager.GridSize, 0, centerGP.z * GameManager.GridSize);
            foreach (MechaComponentBase mcb in mechaComponents)
            {
                mcb.transform.parent = MechaComponentContainer;
                GridPos newGP = new GridPos(mcb.MechaComponentInfo.GridPos.x - centerGP.x, mcb.MechaComponentInfo.GridPos.z - centerGP.z);
                mcb.SetGridPosition(newGP);
                mcb.RefreshOccupiedGridPositions();
            }

            RefreshMechaMatrix();
        }
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
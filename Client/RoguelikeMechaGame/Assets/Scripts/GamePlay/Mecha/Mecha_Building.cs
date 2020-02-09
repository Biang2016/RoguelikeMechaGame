using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class Mecha
{
    [SerializeField] private Transform MechaComponentContainer;
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
        mcb.MechaComponentGrids.TurnOffAllForbidIndicator();
        mcb.MechaComponentGrids.SetIsolatedIndicatorShown(false);
        RefreshMechaMatrix();
    }

    public void RemoveMechaComponent(MechaComponentBase mcb)
    {
        if (mechaComponents.Contains(mcb))
        {
            mechaComponents.Remove(mcb);
            RefreshMechaMatrix(out List<MechaComponentBase> _, out List<MechaComponentBase> isolatedComponents);
            foreach (MechaComponentBase m in isolatedComponents)
            {
                mechaComponents.Remove(m);
                mcb.MechaComponentGrids.SetIsolatedIndicatorShown(true);
                m.PoolRecycle(1f);
            }

            RefreshMechaMatrix();
        }
    }

    public void RefreshMechaMatrix()
    {
        RefreshMechaMatrix(out List<MechaComponentBase> _, out List<MechaComponentBase> _);
    }

    public void RefreshMechaMatrix(out List<MechaComponentBase> conflictComponents, out List<MechaComponentBase> isolatedComponents)
    {
        foreach (MechaComponentBase mcb in mechaComponents)
        {
            mcb.MechaComponentGrids.SetIsolatedIndicatorShown(false);
            mcb.MechaComponentGrids.TurnOffAllForbidIndicator();
        }

        List<GridPos> coreGPs = new List<GridPos>();
        List<MechaComponentBase> notConflictComponents = new List<MechaComponentBase>();

        // Find conflict components
        List<GridPos> conflictGridPositions = new List<GridPos>();
        conflictComponents = new List<MechaComponentBase>();

        for (int z = 0; z < mechaComponentMatrix.GetLength(0); z++)
        {
            for (int x = 0; x < mechaComponentMatrix.GetLength(1); x++)
            {
                mechaComponentMatrix[z, x] = null;
            }
        }

        foreach (MechaComponentBase mcb in mechaComponents)
        {
            bool isCore = mcb.MechaComponentInfo.MechaComponentType == MechaComponentType.Core;
            bool hasConflict = false;
            foreach (GridPos gp in mcb.MechaComponentInfo.OccupiedGridPositions)
            {
                GridPos gp_matrix = gp.ConvertGridPosToMatrixIndex();

                if (gp_matrix.x < 0 || gp_matrix.x >= mechaComponentMatrix.GetLength(1)
                                    || gp_matrix.z < 0 || gp_matrix.z >= mechaComponentMatrix.GetLength(0))
                {
                    hasConflict = true;
                    conflictGridPositions.Add(gp_matrix);
                }
                else
                {
                    if (mechaComponentMatrix[gp_matrix.z, gp_matrix.x] != null)
                    {
                        hasConflict = true;
                        conflictGridPositions.Add(gp_matrix);
                    }
                    else
                    {
                        mechaComponentMatrix[gp_matrix.z, gp_matrix.x] = mcb;
                        if (isCore) coreGPs.Add(gp_matrix);
                    }
                }
            }

            if (hasConflict)
            {
                conflictComponents.Add(mcb);
            }
            else
            {
                notConflictComponents.Add(mcb);
            }
        }

        foreach (GridPos gp in conflictGridPositions)
        {
            AddForbidComponentIndicator(gp);
        }

        // Find isolated components
        List<GridPos> isolatedGridPositions = new List<GridPos>();
        isolatedComponents = new List<MechaComponentBase>();

        int[,] connectedMatrix = new int[ConfigManager.EDIT_AREA_SIZE * 2 + 1, ConfigManager.EDIT_AREA_SIZE * 2 + 1];

        foreach (MechaComponentBase mcb in notConflictComponents)
        {
            foreach (GridPos gp in mcb.MechaComponentInfo.OccupiedGridPositions)
            {
                GridPos gp_matrix = gp.ConvertGridPosToMatrixIndex();
                connectedMatrix[gp_matrix.z, gp_matrix.x] = 1;
            }
        }

        Queue<GridPos> connectedQueue = new Queue<GridPos>();

        foreach (GridPos coreGP in coreGPs)
        {
            connectedMatrix[coreGP.z, coreGP.x] = 2;
            connectedQueue.Enqueue(coreGP);
        }

        void connectPos(int z, int x)
        {
            if (x < 0 || x >= mechaComponentMatrix.GetLength(1) || z < 0 || z >= mechaComponentMatrix.GetLength(0))
            {
                return;
            }
            else
            {
                int a = connectedMatrix[z, x];
                if (connectedMatrix[z, x] == 1)
                {
                    connectedQueue.Enqueue(new GridPos(x, z));
                    connectedMatrix[z, x] = 2;
                }
            }
        }

        while (connectedQueue.Count > 0)
        {
            GridPos gp = connectedQueue.Dequeue();
            connectPos(gp.z + 1, gp.x);
            connectPos(gp.z - 1, gp.x);
            connectPos(gp.z, gp.x - 1);
            connectPos(gp.z, gp.x + 1);
        }

        for (int z = 0; z < connectedMatrix.GetLength(0); z++)
        {
            for (int x = 0; x < connectedMatrix.GetLength(1); x++)
            {
                if (connectedMatrix[z, x] == 1)
                {
                    isolatedGridPositions.Add((new GridPos(x, z)));
                    MechaComponentBase isolatedComponent = mechaComponentMatrix[z, x];
                    if (!isolatedComponents.Contains(isolatedComponent))
                    {
                        isolatedComponents.Add(isolatedComponent);
                    }
                }
            }
        }

        foreach (GridPos gp in isolatedGridPositions)
        {
            AddIsolatedComponentIndicator(gp);
        }
    }

    private void AddForbidComponentIndicator(GridPos gp_matrix)
    {
        MechaComponentBase mcb = mechaComponentMatrix[gp_matrix.z, gp_matrix.x];
        if (mcb)
        {
            GridPos gp = gp_matrix.ConvertMatrixIndexToGridPos();
            GridPos gp_local_noRotate = gp - mcb.MechaComponentInfo.GridPos;
            GridPos gp_local_rotate = GridPos.RotateGridPos(gp_local_noRotate, (GridPos.Orientation) ((4 - (int) mcb.MechaComponentInfo.GridPos.orientation) % 4));
            mcb.MechaComponentGrids.SetForbidIndicatorShown(true, gp_local_rotate);
        }
    }

    private void AddIsolatedComponentIndicator(GridPos gp_matrix)
    {
        MechaComponentBase mcb = mechaComponentMatrix[gp_matrix.z, gp_matrix.x];
        if (mcb)
        {
            mcb.MechaComponentGrids.SetIsolatedIndicatorShown(true);
        }
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
            foreach (GridPos gp in mcb.MechaComponentInfo.OccupiedGridPositions)
            {
                GridPos newGP = gp + delta_local_GP;
                if (newGP.x > ConfigManager.EDIT_AREA_SIZE || newGP.x < -ConfigManager.EDIT_AREA_SIZE)
                {
                    MoveCenter(new GridPos(0, delta_local_GP.z));
                    return;
                }

                if (newGP.z > ConfigManager.EDIT_AREA_SIZE || newGP.z < -ConfigManager.EDIT_AREA_SIZE)
                {
                    MoveCenter(new GridPos(delta_local_GP.x, 0));
                    return;
                }
            }
        }

        foreach (MechaComponentBase mcb in mechaComponents)
        {
            mcb.transform.parent = MechaComponentContainer;
            GridPos newGP = mcb.MechaComponentInfo.GridPos + delta_local_GP;
            newGP.orientation = mcb.MechaComponentInfo.GridPos.orientation;
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
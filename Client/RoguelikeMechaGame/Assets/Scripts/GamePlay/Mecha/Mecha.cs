using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class Mecha : PoolObject
{
    public MechaInfo MechaInfo;

    private List<MechaComponentBase> mechaComponents = new List<MechaComponentBase>();

    private MechaComponentBase[,] mechaComponentMatrix = new MechaComponentBase[ConfigManager.EDIT_AREA_SIZE * 2 + 1, ConfigManager.EDIT_AREA_SIZE * 2 + 1]; //[z,x]

    public void Initialize(MechaInfo mechaInfo)
    {
        MechaInfo = mechaInfo;
        RefreshMechaMatrix();
        foreach (MechaComponentInfo mci in mechaInfo.MechaComponentInfos)
        {
            AddMechaComponent(mci);
        }

        Initialize_Building(mechaInfo);
        Initialize_Fighting(mechaInfo);
    }

    public void RefreshMechaMatrix()
    {
        RefreshMechaMatrix(out List<MechaComponentBase> _, out List<MechaComponentBase> _);
    }

    public void RefreshMechaMatrix(out List<MechaComponentBase> conflictComponents, out List<MechaComponentBase> isolatedComponents)
    {
        ClearForbidComponents();
        ClearIsolatedComponents();

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
                    conflictGridPositions.Add(gp);
                }
                else
                {
                    if (mechaComponentMatrix[gp_matrix.z, gp_matrix.x] != null)
                    {
                        hasConflict = true;
                        conflictGridPositions.Add(gp);
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
                    isolatedGridPositions.Add((new GridPos(z, x)).ConvertMatrixIndexToGridPos());
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

    public void ExertComponentBuffs()
    {
        foreach (MechaComponentBase mcb in mechaComponents)
        {
            mcb.ExertEffectOnOtherComponents();
        }
    }

    public void RemoveAllComponentBuffs()
    {
        foreach (MechaComponentBase mcb in mechaComponents)
        {
            mcb.UnlinkAllBuffs();
        }
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
}
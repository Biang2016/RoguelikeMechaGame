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

    public List<MechaComponentBase> RefreshMechaMatrix()
    {
        ClearForbidComponents();
        List<GridPos> conflictGridPositions = new List<GridPos>();
        List<MechaComponentBase> conflictComponents = new List<MechaComponentBase>();

        for (int z = 0; z < mechaComponentMatrix.GetLength(0); z++)
        {
            for (int x = 0; x < mechaComponentMatrix.GetLength(1); x++)
            {
                mechaComponentMatrix[z, x] = null;
            }
        }

        foreach (MechaComponentBase mcb in mechaComponents)
        {
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
                    }
                }
            }

            if (hasConflict)
            {
                conflictComponents.Add(mcb);
            }
        }

        foreach (GridPos gp in conflictGridPositions)
        {
            AddForbidComponentIndicator(gp);
        }

        return conflictComponents;
    }

    public void ClearConflictComponents()
    {
        
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
}
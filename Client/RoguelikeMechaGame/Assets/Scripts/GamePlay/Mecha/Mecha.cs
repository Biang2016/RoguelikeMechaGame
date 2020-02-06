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

    private void RefreshMechaMatrix()
    {
        ClearForbidComponents();
        List<GridPos> conflictGridPositions = new List<GridPos>();

        for (int z = 0; z < mechaComponentMatrix.GetLength(0); z++)
        {
            for (int x = 0; x < mechaComponentMatrix.GetLength(1); x++)
            {
                mechaComponentMatrix[z, x] = null;
            }
        }

        foreach (MechaComponentBase mcb in mechaComponents)
        {
            foreach (GridPos gp in mcb.MechaComponentInfo.OccupiedGridPositions)
            {
                GridPos gp_matrix = gp.ConvertGridPosToMatrixIndex();

                if (mechaComponentMatrix[gp_matrix.x, gp_matrix.z] != null)
                {
                    conflictGridPositions.Add(gp);
                }
                else
                {
                    mechaComponentMatrix[gp_matrix.x, gp_matrix.z] = mcb;
                }
            }
        }

        foreach (GridPos gp in conflictGridPositions)
        {
            AddForbidComponentIndicator(gp);
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
}
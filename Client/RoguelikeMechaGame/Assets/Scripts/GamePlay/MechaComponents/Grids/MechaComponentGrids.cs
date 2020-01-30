using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class MechaComponentGrids : MonoBehaviour
{
    private List<MechaComponentGrid> M_MechaComponentGrids = new List<MechaComponentGrid>();

    private List<GridPos> MechaComponentGridPositions = new List<GridPos>();

    void Awake()
    {
        M_MechaComponentGrids = GetComponentsInChildren<MechaComponentGrid>().ToList();
        foreach (MechaComponentGrid mcg in M_MechaComponentGrids)
        {
            MechaComponentGridPositions.Add(mcg.GetGridPos());
        }
    }
}
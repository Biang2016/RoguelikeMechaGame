using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class MechaComponentGrids : MonoBehaviour
{
    private List<MechaComponentGrid> M_MechaComponentGrids = new List<MechaComponentGrid>();

    internal List<GridPos> MechaComponentGridPositions = new List<GridPos>();

    void Awake()
    {
        M_MechaComponentGrids = GetComponentsInChildren<MechaComponentGrid>().ToList();
        foreach (MechaComponentGrid mcg in M_MechaComponentGrids)
        {
            MechaComponentGridPositions.Add(mcg.GetGridPos());
        }
    }

    public void SetSlotLightsShown(bool shown)
    {
        foreach (MechaComponentGrid mcg in M_MechaComponentGrids)
        {
           mcg.SetSlotLightsShown(shown);
        }
    }

    public void SetGridShown(bool shown)
    {
        foreach (MechaComponentGrid mcg in M_MechaComponentGrids)
        {
            mcg.SetGridShown(shown);
        }
    }
}
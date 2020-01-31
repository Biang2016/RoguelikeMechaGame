using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class MechaComponentGrids : MonoBehaviour
{
    private List<MechaComponentGrid> mechaComponentGrids = new List<MechaComponentGrid>();

    internal List<GridPos> MechaComponentGridPositions = new List<GridPos>();

    void Awake()
    {
        mechaComponentGrids = GetComponentsInChildren<MechaComponentGrid>().ToList();
        foreach (MechaComponentGrid mcg in mechaComponentGrids)
        {
            MechaComponentGridPositions.Add(mcg.GetGridPos());
        }
    }

    public void SetSlotLightsShown(bool shown)
    {
        foreach (MechaComponentGrid mcg in mechaComponentGrids)
        {
            mcg.SetSlotLightsShown(shown);
        }
    }

    public void SetGridShown(bool shown)
    {
        foreach (MechaComponentGrid mcg in mechaComponentGrids)
        {
            mcg.SetGridShown(shown);
        }
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MechaComponentGrid : MonoBehaviour, IGridPos
{
    private MeshRenderer GridMeshRenderer;

    public SortedDictionary<GridPos.Orientation, MechaComponentSlot> Slots = new SortedDictionary<GridPos.Orientation, MechaComponentSlot>
    {
        {GridPos.Orientation.Up, null},
        {GridPos.Orientation.Right, null},
        {GridPos.Orientation.Down, null},
        {GridPos.Orientation.Left, null},
    };

    public void Reset()
    {
        foreach (GridPos.Orientation key in Slots.Keys.ToList())
        {
            Slots[key] = null;
        }
    }

    void Awake()
    {
        GridMeshRenderer = GetComponent<MeshRenderer>();
        MechaComponentSlot[] slots = GetComponentsInChildren<MechaComponentSlot>();
        foreach (MechaComponentSlot slot in slots)
        {
            Slots[slot.Orientation] = slot;
        }
    }

    public void SetSlotLightsShown(bool shown)
    {
        foreach (KeyValuePair<GridPos.Orientation, MechaComponentSlot> kv in Slots)
        {
            if (kv.Value)
            {
                kv.Value.SetShown(shown);
            }
        }
    }

    public void SetGridShown(bool shown)
    {
        GridMeshRenderer.enabled = shown;
    }

    public GridPos GetGridPos()
    {
        return GridPos.GetGridPosByLocalTrans(transform, GameManager.GridSize);
    }
}
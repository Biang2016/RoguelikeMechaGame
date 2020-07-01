using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameCore;

namespace Client
{
    public class MechaComponentGrid : MonoBehaviour
    {
        [SerializeField] private MeshRenderer BorderIndicator;
        [SerializeField] private MeshRenderer ForbidIndicator;
        [SerializeField] private MeshRenderer IsolatedIndicator;

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
            MechaComponentSlot[] slots = GetComponentsInChildren<MechaComponentSlot>();
            foreach (MechaComponentSlot slot in slots)
            {
                slot.Initialize();
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
            BorderIndicator.enabled = shown;
        }

        public void SetForbidIndicatorShown(bool shown)
        {
            ForbidIndicator.enabled = shown;
        }

        public void SetIsolatedIndicatorShown(bool shown)
        {
            IsolatedIndicator.enabled = shown;
        }

        public GridPos GetGridPos()
        {
            return GridPos.GetGridPosByLocalTrans(transform, GameManager.GridSize);
        }
    }
}
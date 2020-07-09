using System.Collections.Generic;
using System.Linq;
using BiangStudio.GameDataFormat.Grid;
using GameCore;
using UnityEngine;

namespace Client
{
    public class MechaComponentGrid : MonoBehaviour
    {
        [SerializeField] private MeshRenderer BorderIndicator;
        [SerializeField] private MeshRenderer ForbidIndicator;
        [SerializeField] private MeshRenderer IsolatedIndicator;
        [SerializeField] private MechaComponentHitBox MechaComponentHitBox;

        public bool IsConflicted = false;

        public SortedDictionary<GridPosR.Orientation, MechaComponentSlot> Slots = new SortedDictionary<GridPosR.Orientation, MechaComponentSlot>
        {
            {GridPosR.Orientation.Up, null},
            {GridPosR.Orientation.Right, null},
            {GridPosR.Orientation.Down, null},
            {GridPosR.Orientation.Left, null},
        };

        public void Reset()
        {
            foreach (GridPosR.Orientation key in Slots.Keys.ToList())
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
            foreach (KeyValuePair<GridPosR.Orientation, MechaComponentSlot> kv in Slots)
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
            ForbidIndicator.enabled = IsConflicted && shown;
        }

        public void SetIsolatedIndicatorShown(bool shown)
        {
            IsolatedIndicator.enabled = shown;
        }

        public GridPos GetGridPos()
        {
            return GridPos.GetGridPosByLocalTransXZ(transform, ConfigManager.GridSize);
        }
    }
}
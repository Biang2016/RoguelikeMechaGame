using System;
using System.Collections.Generic;
using BiangStudio.GameDataFormat.Grid;
using GameCore;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Client
{
    [ExecuteInEditMode]
    public class MechaComponentGrid : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer BorderIndicator;

        [SerializeField]
        private MeshRenderer ForbidIndicator;

        [SerializeField]
        private MeshRenderer IsolatedIndicator;

        [SerializeField]
        private MechaComponentHitBox MechaComponentHitBox;

        public bool IsConflicted = false;

        [SerializeField]
        private MechaComponentSlot Slot_Up;

        [SerializeField]
        private MechaComponentSlot Slot_Right;

        [SerializeField]
        private MechaComponentSlot Slot_Down;

        [SerializeField]
        private MechaComponentSlot Slot_Left;

        public Dictionary<GridPosR.Orientation, MechaComponentSlot> Slots = new Dictionary<GridPosR.Orientation, MechaComponentSlot>();

        void Awake()
        {
            Slots.Add(GridPosR.Orientation.Up, Slot_Up);
            Slots.Add(GridPosR.Orientation.Right, Slot_Right);
            Slots.Add(GridPosR.Orientation.Down, Slot_Down);
            Slots.Add(GridPosR.Orientation.Left, Slot_Left);
            foreach (KeyValuePair<GridPosR.Orientation, MechaComponentSlot> kv in Slots)
            {
                kv.Value.Initialize();
            }
        }

        public void SetSlotLightsShown(bool shown)
        {
            foreach (KeyValuePair<GridPosR.Orientation, MechaComponentSlot> kv in Slots)
            {
                kv.Value.SetShown(shown);
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

        [EnumToggleButtons]
        [HideInPlayMode]
        [OnValueChanged("OnSlotEnumFlag_EditorChanged")]
        [ShowInInspector]
        [SerializeField]
        private GridPosR.OrientationFlag SlotEnumFlag_Editor;

        private void OnSlotEnumFlag_EditorChanged()
        {
            foreach (GridPosR.Orientation orientation in Enum.GetValues(typeof(GridPosR.Orientation)))
            {
                Slots[orientation].IsCandidate = SlotEnumFlag_Editor.HasFlag(orientation.ToFlag());
            }
        }
    }
}
using System.Collections.Generic;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.ShapedInventory;
using GameCore;
using UnityEngine;

namespace Client
{
    public partial class Mecha
    {
        [SerializeField] private Transform MechaComponentContainer;
        public MechaEditArea MechaEditArea;

        private void Initialize_Building(MechaInfo mechaInfo)
        {
            MechaEditArea.gameObject.SetActive(mechaInfo.MechaType == MechaType.Player);
            MechaEditArea.Hide();
            GridShown = false;
            SlotLightsShown = false;
        }

        private MechaComponentBase AddMechaComponent(MechaComponentInfo mci)
        {
            MechaComponentBase mcb = MechaComponentBase.BaseInitialize(mci, this);
            mcb.OnRemoveMechaComponentBaseSuc = RemoveMechaComponent;
            MechaComponentDict.Add(mci.GUID, mcb);
         
            if (MechaInfo.MechaType == MechaType.Player && mcb.MechaComponentInfo.MechaComponentType == MechaComponentType.Core)
            {
                MechaInfo.RefreshHUDPanelCoreLifeSliderCount?.Invoke();
            }

            mcb.transform.SetParent(MechaComponentContainer);
            mcb.MechaComponentGrids.SetGridShown(GridShown);
            mcb.MechaComponentGrids.SetSlotLightsShown(SlotLightsShown);
            mcb.MechaComponentGrids.ResetAllGridConflict();
            mcb.MechaComponentGrids.SetIsolatedIndicatorShown(false);
            return mcb;
        }

        private void RemoveMechaComponent(MechaComponentBase mcb)
        {
            mcb.OnRemoveMechaComponentBaseSuc = null;

            if (MechaComponentDict.ContainsKey(mcb.MechaComponentInfo.GUID))
            {
                MechaComponentDict.Remove(mcb.MechaComponentInfo.GUID);
                if (MechaInfo.MechaType == MechaType.Player && mcb.MechaComponentInfo.MechaComponentType == MechaComponentType.Core)
                {
                    MechaInfo.RefreshHUDPanelCoreLifeSliderCount?.Invoke();
                }

              

                if (MechaComponentDict.Count == 0)
                {
                    Die();
                }
            }
        }

        void Update_Building()
        {
            if (ControlManager.Instance.Building_ToggleWireLines.Down)
            {
                SlotLightsShown = !SlotLightsShown;
                GridShown = !GridShown;
            }
        }

        void LateUpdate_Building()
        {
        }

        private bool _slotLightsShown = true;

        public bool SlotLightsShown
        {
            get { return _slotLightsShown; }
            set
            {
                if (_slotLightsShown != value)
                {
                    foreach (KeyValuePair<int, MechaComponentBase> kv in MechaComponentDict)
                    {
                        kv.Value.MechaComponentGrids.SetSlotLightsShown(value);
                    }
                }

                _slotLightsShown = value;
            }
        }

        private bool _gridShown = true;

        public bool GridShown
        {
            get { return _gridShown; }
            set
            {
                if (_gridShown != value)
                {
                    foreach (KeyValuePair<int, MechaComponentBase> kv in MechaComponentDict)
                    {
                        kv.Value.MechaComponentGrids.SetGridShown(value);
                    }
                }

                _gridShown = value;
            }
        }
    }
}
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
        public MechaEditorContainer MechaEditorContainer;
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
            MechaComponents.Add(mci.GUID, mcb);
            InventoryItem item = new InventoryItem(mcb.MechaComponentInfo);
            item.OnIsolatedHandler = mcb.MechaComponentGrids.SetIsolatedIndicatorShown;
            item.OnConflictedHandler = mcb.MechaComponentGrids.SetGridConflicted;
            item.OnResetConflictHandler = mcb.MechaComponentGrids.ResetAllGridConflict;
            item.AmIRootItemInIsolationCalculationHandler = () => mcb.MechaComponentInfo.MechaComponentType == MechaComponentType.Core;
            MechaEditorContainer.TryAddItem(item);

            if (MechaInfo.MechaType == MechaType.Player && mcb.MechaComponentInfo.MechaComponentType == MechaComponentType.Core)
            {
                MechaInfo.RefreshHUDPanelCoreLifeSliderCount?.Invoke();
            }

            mcb.transform.SetParent(MechaComponentContainer);
            mcb.MechaComponentGrids.SetGridShown(GridShown);
            mcb.MechaComponentGrids.SetSlotLightsShown(SlotLightsShown);
            mcb.MechaComponentGrids.ResetAllGridConflict();
            mcb.MechaComponentGrids.SetIsolatedIndicatorShown(false);
            RefreshMechaMatrix();
            return mcb;
        }

        private void RemoveMechaComponent(MechaComponentBase mcb)
        {
            mcb.OnRemoveMechaComponentBaseSuc = null;

            MechaEditorContainer.RemoveItem(mcb.InventoryItem);

            if (MechaComponents.ContainsKey(mcb.MechaComponentInfo.GUID))
            {
                MechaComponents.Remove(mcb.MechaComponentInfo.GUID);
                if (MechaInfo.MechaType == MechaType.Player && mcb.MechaComponentInfo.MechaComponentType == MechaComponentType.Core)
                {
                    MechaInfo.RefreshHUDPanelCoreLifeSliderCount?.Invoke();
                }

                MechaEditorContainer.RefreshConflictAndIsolation(out List<InventoryItem> _, out List<InventoryItem> isolatedItems);
                if (MechaInfo.MechaType == MechaType.Enemy)
                {
                    foreach (InventoryItem item in isolatedItems)
                    {
                        MechaComponentInfo mci = (MechaComponentInfo) item.ItemContentInfo;
                        MechaComponentBase m = null;
                        foreach (KeyValuePair<int, MechaComponentBase> kv in MechaComponents)
                        {
                            if (mcb.MechaComponentInfo == mci)
                            {
                                m = kv.Value;
                                break;
                            }
                        }

                        if (m != null)
                        {
                            int ran = Random.Range(0, 100);
                            bool drop = ran < mci.DropProbability;
                            if (drop)
                            {
                                MechaComponentDropSprite mcds = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.MechaComponentDropSprite]
                                    .AllocateGameObject<MechaComponentDropSprite>(ClientBattleManager.Instance.MechaComponentDropSpriteContainerRoot);
                                mcds.Initialize(mci, m.transform.position);
                            }

                            mcb.MechaComponentGrids.SetIsolatedIndicatorShown(true);
                            RemoveMechaComponent(m);
                            m.PoolRecycle(1f);
                        }
                    }
                }

                if (MechaComponents.Count == 0)
                {
                    Die();
                }
            }
        }

        public void RefreshMechaMatrix()
        {
            MechaEditorContainer.RefreshConflictAndIsolation(out List<InventoryItem> _, out List<InventoryItem> _);
        }

        public void RefreshMechaMatrix(out List<InventoryItem> conflictItems, out List<InventoryItem> isolatedItems)
        {
            MechaEditorContainer.RefreshConflictAndIsolation(out conflictItems, out isolatedItems);
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

        public void MoveCenter(GridPos delta_local_GP)
        {
            foreach (KeyValuePair<int, MechaComponentBase> kv in MechaComponents)
            {
                foreach (GridPos gp in kv.Value.MechaComponentInfo.OccupiedGridPositions)
                {
                    GridPos newGP = gp + delta_local_GP;
                    if (newGP.x > ConfigManager.EDIT_AREA_HALF_SIZE || newGP.x < -ConfigManager.EDIT_AREA_HALF_SIZE)
                    {
                        MoveCenter(new GridPos(0, delta_local_GP.z));
                        return;
                    }

                    if (newGP.z > ConfigManager.EDIT_AREA_HALF_SIZE || newGP.z < -ConfigManager.EDIT_AREA_HALF_SIZE)
                    {
                        MoveCenter(new GridPos(delta_local_GP.x, 0));
                        return;
                    }
                }
            }

            foreach (KeyValuePair<int, MechaComponentBase> kv in MechaComponents)
            {
                kv.Value.transform.parent = MechaComponentContainer;
                GridPosR newGP = kv.Value.MechaComponentInfo.GridPos + (GridPosR) delta_local_GP;
                newGP.orientation = kv.Value.MechaComponentInfo.GridPos.orientation;
                kv.Value.SetGridPosition(newGP);
            }

            RefreshMechaMatrix();
        }

        private bool _slotLightsShown = true;

        public bool SlotLightsShown
        {
            get { return _slotLightsShown; }
            set
            {
                if (_slotLightsShown != value)
                {
                    foreach (KeyValuePair<int, MechaComponentBase> kv in MechaComponents)
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
                    foreach (KeyValuePair<int, MechaComponentBase> kv in MechaComponents)
                    {
                        kv.Value.MechaComponentGrids.SetGridShown(value);
                    }
                }

                _gridShown = value;
            }
        }
    }
}
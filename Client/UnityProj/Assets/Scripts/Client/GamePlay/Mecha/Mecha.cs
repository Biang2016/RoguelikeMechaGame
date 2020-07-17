using System.Collections.Generic;
using BiangStudio.ObjectPool;
using GameCore;
using UnityEngine;
using UnityEngine.Events;

namespace Client
{
    public partial class Mecha : PoolObject
    {
        public MechaInfo MechaInfo;
        public UnityAction<Mecha> OnRemoveMechaSuc;

        [SerializeField]
        private Transform MechaComponentContainer;

        public MechaEditArea MechaEditArea;

        public SortedDictionary<uint, MechaComponentBase> MechaComponentDict = new SortedDictionary<uint, MechaComponentBase>();
        public bool IsPlayer => MechaInfo.IsPlayer;
        public bool IsBuilding => GameStateManager.Instance.GetState() == GameState.Building;
        public bool IsFighting => GameStateManager.Instance.GetState() == GameState.Fighting;

        public override void PoolRecycle()
        {
            Clean();
            base.PoolRecycle();
        }

        public void Initialize(MechaInfo mechaInfo)
        {
            Clean();

            MechaInfo = mechaInfo;
            MechaInfo.OnAddMechaComponentInfoSuc = (mci, gp_matrix) => AddMechaComponent(mci);
            MechaInfo.OnRemoveMechaInfoSuc += (mi) => OnRemoveMechaSuc?.Invoke(this);
            MechaInfo.OnDropMechaComponent = (mci) =>
            {
                MechaComponentDropSprite mcds = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.MechaComponentDropSprite]
                    .AllocateGameObject<MechaComponentDropSprite>(ClientBattleManager.Instance.MechaComponentDropSpriteContainerRoot);
                mcds.Initialize(mci, MechaComponentDict[mci.GUID].transform.position);
            };

            MechaInfo.MechaEditorInventory = new MechaEditorInventory(
                DragAreaDefines.MechaEditorArea.ToString(),
                DragAreaDefines.MechaEditorArea,
                ConfigManager.GridSize,
                ConfigManager.EDIT_AREA_FULL_SIZE,
                ConfigManager.EDIT_AREA_FULL_SIZE,
                false,
                0,
                () => ControlManager.Instance.Building_RotateItem.Down);
            MechaInfo.MechaEditorInventory.OnRemoveItemSucAction = (item) => ((MechaComponentInfo) item.ItemContentInfo).RemoveMechaComponentInfo();
            MechaInfo.MechaEditorInventory.RefreshInventoryGrids();
            MechaInfo.MechaEditorInventory.RefreshConflictAndIsolation();

            foreach (KeyValuePair<uint, MechaComponentInfo> kv in mechaInfo.MechaComponentInfos)
            {
                AddMechaComponent(kv.Value);
            }

            MechaEditArea.Init(mechaInfo);

            GridShown = false;
            SlotLightsShown = false;
            Initialize_Fighting(mechaInfo);
        }

        public void Clean()
        {
            MechaEditArea.Clear();
            foreach (KeyValuePair<uint, MechaComponentBase> kv in MechaComponentDict)
            {
                kv.Value.PoolRecycle();
            }

            MechaComponentDict.Clear();
            OnRemoveMechaSuc = null;
            MechaInfo = null;
        }

        void Update()
        {
            MechaInfo.UpdateLifeChange();

            if (IsBuilding && IsPlayer)
            {
                Update_Building();
            }

            if (IsFighting)
            {
                Update_Fighting();
            }
        }

        void FixedUpdate()
        {
            if (IsFighting)
            {
                FixedUpdate_Fighting();
            }
            else
            {
                FixedUpdate_Building();
            }
        }

        void LateUpdate()
        {
            if (IsPlayer)
            {
                if (GameStateManager.Instance.GetState() == GameState.Fighting)
                {
                    LateUpdate_Fighting();
                }

                if (GameStateManager.Instance.GetState() == GameState.Building)
                {
                }
            }
        }

        public void SetShown(bool shown)
        {
            foreach (KeyValuePair<uint, MechaComponentBase> kv in MechaComponentDict)
            {
                kv.Value.SetShown(shown);
            }
        }

        private void Die()
        {
            if (IsPlayer)
            {
                // TODO Endgame
            }
            else
            {
                OnRemoveMechaSuc(this);
                PoolRecycle();
            }
        }

        private MechaComponentBase AddMechaComponent(MechaComponentInfo mci)
        {
            MechaComponentBase mcb = MechaComponentBase.BaseInitialize(mci, this);
            mcb.OnRemoveMechaComponentBaseSuc = RemoveMechaComponent;
            MechaComponentDict.Add(mci.GUID, mcb);

            if (IsPlayer && mcb.MechaComponentInfo.MechaComponentType == MechaComponentType.Core)
            {
                MechaInfo.RefreshHUDPanelCoreLifeSliderCount?.Invoke();
            }

            mcb.transform.SetParent(MechaComponentContainer);
            mcb.MechaComponentGridRoot.SetGridShown(GridShown);
            mcb.MechaComponentGridRoot.SetSlotLightsShown(SlotLightsShown);
            mcb.MechaComponentGridRoot.ResetAllGridConflict();
            mcb.MechaComponentGridRoot.SetIsolatedIndicatorShown(false);
            return mcb;
        }

        private void RemoveMechaComponent(MechaComponentBase mcb)
        {
            mcb.OnRemoveMechaComponentBaseSuc = null;

            if (MechaComponentDict.ContainsKey(mcb.MechaComponentInfo.GUID))
            {
                MechaComponentDict.Remove(mcb.MechaComponentInfo.GUID);
                if (IsPlayer && mcb.MechaComponentInfo.MechaComponentType == MechaComponentType.Core)
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

        void FixedUpdate_Building()
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
                    foreach (KeyValuePair<uint, MechaComponentBase> kv in MechaComponentDict)
                    {
                        kv.Value.MechaComponentGridRoot.SetSlotLightsShown(value);
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
                    foreach (KeyValuePair<uint, MechaComponentBase> kv in MechaComponentDict)
                    {
                        kv.Value.MechaComponentGridRoot.SetGridShown(value);
                    }
                }

                _gridShown = value;
            }
        }
    }
}
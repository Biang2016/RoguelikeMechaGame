using System.Collections.Generic;
using BiangStudio.GamePlay;
using GameCore;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using UnityEngine;
using UnityEngine.Events;

namespace Client
{
    public partial class Mecha : MechaBase
    {
        public UnityAction<Mecha> OnRemoveMechaSuc;

        [SerializeField]
        private Transform MechaComponentContainer;

        public MechaEditArea MechaEditArea;

        [SerializeField]
        protected BehaviourTreeOwner BehaviourTreeOwner;

        public SortedDictionary<uint, MechaComponent> MechaComponentDict = new SortedDictionary<uint, MechaComponent>();
        public bool IsPlayer => MechaInfo.IsPlayer;
        public bool IsBuilding => GameStateManager.Instance.GetState() == GameState.Building;
        public bool IsFighting => GameStateManager.Instance.GetState() == GameState.Fighting;

        public override void PoolRecycle()
        {
            Clean();
            if (MechaEditArea != null)
            {
                DestroyImmediate(MechaEditArea);
                MechaEditArea = null;
            }

            base.PoolRecycle();
        }

        public void Initialize(MechaInfo mechaInfo)
        {
            Clean();

            MechaInfo = mechaInfo;
            if (!IsPlayer)
            {
                MechaBaseAIAgent = new MechaBaseAIAgent(this);
            }

            // AI
            if (mechaInfo.MechaConfig != null)
            {
                string aiConfigKey = mechaInfo.MechaConfig.MechaAIConfigKey;
                if (!string.IsNullOrEmpty(aiConfigKey))
                {
                    BehaviourTree bt = ConfigManager.Instance.GetEnemyAIConfig(aiConfigKey);
                    BehaviourTreeOwner.behaviour = bt;
                    BehaviourTreeOwner.updateMode = Graph.UpdateMode.NormalUpdate;
                    BehaviourTreeOwner.StartBehaviour();
                    AIManager.Instance.AddBehaviourTreeOwner(BehaviourTreeOwner);
                }
            }

            MechaInfo.OnAddMechaComponentInfoSuc = (mci, gp_matrix) => AddMechaComponent(mci);
            MechaInfo.OnRemoveMechaInfoSuc += (mi) =>
            {
                OnRemoveMechaSuc?.Invoke(this);
                PoolRecycle();
            };
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
                false,
                false,
                0,
                () => ControlManager.Instance.Building_RotateItem.Down);
            MechaInfo.MechaEditorInventory.OnRemoveItemSucAction = (item) => { ((MechaComponentInfo) item.ItemContentInfo).RemoveMechaComponentInfo(); };
            MechaInfo.MechaEditorInventory.RefreshInventoryGrids();
            MechaInfo.OnInstantiated?.Invoke(); // 将已经积攒的未实例化的组件实例化
            MechaInfo.OnInstantiated = null;
            MechaInfo.MechaEditorInventory.RefreshConflictAndIsolation();

            // MechaEditorArea
            if (mechaInfo.MechaCamp == MechaCamp.Player)
            {
                GameObject mea = PrefabManager.Instance.GetPrefab(nameof(MechaEditArea));
                GameObject meaGO = Instantiate(mea);
                meaGO.transform.parent = transform;
                meaGO.transform.localPosition = new Vector3(0, -0.5f, 0);
                MechaEditArea = meaGO.GetComponent<MechaEditArea>();
                MechaEditArea.Init(mechaInfo);
            }

            foreach (KeyValuePair<uint, MechaComponentInfo> kv in mechaInfo.MechaComponentInfoDict) // 将其它的组件实例化
            {
                if (MechaComponentDict.ContainsKey(kv.Key)) continue;
                AddMechaComponent(kv.Value);
            }

            GridShown = false;
            SlotLightsShown = false;
            Initialize_Fighting(mechaInfo);
        }

        public bool IsAlive()
        {
            return !IsRecycled && MechaInfo != null && !MechaInfo.IsDead;
        }

        public void Clean()
        {
            MechaEditArea?.Clear();
            foreach (KeyValuePair<uint, MechaComponent> kv in MechaComponentDict)
            {
                kv.Value.PoolRecycle();
            }

            MechaComponentDict.Clear();
            OnRemoveMechaSuc = null;
            MechaInfo = null;
            BehaviourTreeOwner.behaviour = null;
            AIManager.Instance.RemoveBehaviourTreeOwner(BehaviourTreeOwner);
            if (MechaBaseAIAgent != null)
            {
                MechaBaseAIAgent.MechaBase = null;
                MechaBaseAIAgent = null;
            }

        }

        protected void Update()
        {
            if (!IsRecycled)
            {
                if (MechaInfo != null)
                {
                    MechaInfo.Position = transform.position;
                    MechaInfo.Rotation = transform.rotation;
                }

                if (IsBuilding && IsPlayer)
                {
                    Update_Building();
                }

                if (IsFighting)
                {
                    Update_Fighting();
                    if (!IsPlayer)
                    {
                        MechaBaseAIAgent?.Update();
                    }
                }
            }
        }

        void FixedUpdate()
        {
            if (!IsRecycled)
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
        }

        void LateUpdate()
        {
            if (!IsRecycled)
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
        }

        public void SetShown(bool shown)
        {
            foreach (KeyValuePair<uint, MechaComponent> kv in MechaComponentDict)
            {
                kv.Value.SetShown(shown);
            }
        }

        private MechaComponent AddMechaComponent(MechaComponentInfo mci)
        {
            MechaComponent mc = MechaComponent.BaseInitialize(mci, this);
            mc.OnRemoveMechaComponentSuc = RemoveMechaComponent;
            MechaComponentDict.Add(mci.GUID, mc);

            if (IsPlayer && mc.MechaComponentInfo.MechaComponentType == MechaComponentType.Core)
            {
                MechaInfo.RefreshHUDPanelCoreLifeSliderCount?.Invoke();
            }

            mc.transform.SetParent(MechaComponentContainer);
            mc.MechaComponentGridRoot.SetGridShown(GridShown);
            mc.MechaComponentGridRoot.SetSlotLightsShown(SlotLightsShown);
            mc.MechaComponentGridRoot.ResetAllGridConflict();
            mc.MechaComponentGridRoot.SetIsolatedIndicatorShown(false);
            return mc;
        }

        private void RemoveMechaComponent(MechaComponent mc)
        {
            mc.OnRemoveMechaComponentSuc = null;

            if (MechaComponentDict.ContainsKey(mc.MechaComponentInfo.GUID))
            {
                MechaComponentDict.Remove(mc.MechaComponentInfo.GUID);
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
                    foreach (KeyValuePair<uint, MechaComponent> kv in MechaComponentDict)
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
                    foreach (KeyValuePair<uint, MechaComponent> kv in MechaComponentDict)
                    {
                        kv.Value.MechaComponentGridRoot.SetGridShown(value);
                    }
                }

                _gridShown = value;
            }
        }
    }
}
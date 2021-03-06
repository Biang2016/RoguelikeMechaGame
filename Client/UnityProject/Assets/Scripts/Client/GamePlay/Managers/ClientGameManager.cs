﻿using System;
using System.Collections.Generic;
using BiangStudio.DragHover;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.GamePlay;
using BiangStudio.GamePlay.UI;
using BiangStudio.GridBackpack;
using BiangStudio.Log;
using BiangStudio.Messenger;
using BiangStudio.ShapedInventory;
using BiangStudio.Singleton;
using GameCore;
using UnityEngine;
using UnityEngine.SceneManagement;
using DragAreaDefines = GameCore.DragAreaDefines;

namespace Client
{
    public class ClientGameManager : MonoSingleton<ClientGameManager>
    {
        #region Managers

        #region Mono

        private AudioManager AudioManager => AudioManager.Instance;
        private CameraManager CameraManager => CameraManager.Instance;
        private UIManager UIManager => UIManager.Instance;

        #endregion

        #region TSingletonBaseManager

        #region Resources

        private ConfigManager ConfigManager => ConfigManager.Instance;
        private LayerManager LayerManager => LayerManager.Instance;
        private PrefabManager PrefabManager => PrefabManager.Instance;
        private GameObjectPoolManager GameObjectPoolManager => GameObjectPoolManager.Instance;

        #endregion

        #region Framework

        private ControlManager ControlManager => ControlManager.Instance;
        private GameStateManager GameStateManager => GameStateManager.Instance;
        private RoutineManager RoutineManager => RoutineManager.Instance;

        #endregion

        #region GamePlay

        private BackpackManager BackpackManager => BackpackManager.Instance;
        private DragManager DragManager => DragManager.Instance;
        private MouseHoverManager MouseHoverManager => MouseHoverManager.Instance;
        private DragExecuteManager DragExecuteManager => DragExecuteManager.Instance;
        private AIManager AIManager => AIManager.Instance;

        #region Level

        private LevelManager LevelManager => LevelManager.Instance;
        private ClientLevelManager ClientLevelManager => ClientLevelManager.Instance;
        private BattleManager BattleManager => BattleManager.Instance;
        private ClientBattleManager ClientBattleManager => ClientBattleManager.Instance;

        public Messenger BattleMessenger => BattleManager.BattleMessenger;

        private FXManager FXManager => FXManager.Instance;
        private UIBattleTipManager UIBattleTipManager => UIBattleTipManager.Instance;
        private ProjectileManager ProjectileManager => ProjectileManager.Instance;
        private ClientProjectileManager ClientProjectileManager => ClientProjectileManager.Instance;

        #endregion

        #endregion

        #endregion

        #endregion

        public DebugConsole DebugConsole;

        private void Awake()
        {
            UIManager.Init(
                (prefabName) => Instantiate(PrefabManager.GetPrefab(prefabName)),
                Debug.LogError,
                () => ControlManager.Instance.Common_MouseLeft.Down,
                () => ControlManager.Instance.Common_MouseRight.Down,
                () => ControlManager.Instance.Common_Exit.Down,
                () => ControlManager.Instance.Common_Confirm.Down,
                () => ControlManager.Instance.Common_Tab.Down
            );

            ConfigManager.Awake();
            LayerManager.Awake();
            PrefabManager.Awake();
            GameObjectPoolManager.Init(new GameObject("GameObjectPoolRoot").transform);
            GameObjectPoolManager.Awake();

            RoutineManager.LogErrorHandler = Debug.LogError;
            RoutineManager.Awake();
            GameStateManager.Awake();
            DebugConsole.OnDebugConsoleKeyDownHandler = () => ControlManager.Instance.Common_Debug.Down;
            DebugConsole.OnDebugConsoleToggleHandler = (enable) =>
            {
                ControlManager.Instance.EnableBattleInputActions(!enable);
                ControlManager.Instance.EnableBuildingInputActions(!enable);
            };

            BackpackManager.LoadBackpackItemConfigs(ConfigManager.BackpackGridSize);
            BackpackManager.Awake();

            DragManager.Awake();
            MouseHoverManager.Awake();
            MouseHoverManager.Initialize(
                () => ControlManager.Instance.Common_MouseLeft.Down,
                () => ControlManager.Instance.Common_MouseLeft.Up,
                () => ControlManager.Instance.Common_MousePosition);
            InitMouseHoverManager();
            DragExecuteManager.Init();
            DragExecuteManager.Awake();
            AIManager.Init(0.1f);
            AIManager.Awake();

            ClientLevelManager.Init();
            LevelManager.Init();
            ClientLevelManager.Awake();
            BattleManager.Awake();
            ClientBattleManager.Awake();
            FXManager.Awake();
            UIBattleTipManager.Awake();

            ClientProjectileManager.Init(new GameObject("ProjectileRoot").transform);
            ClientProjectileManager.Awake();

            ControlManager.Awake();

            QualityManager.Initialize();
        }

        private void Start()
        {
            ConfigManager.Start();
            LayerManager.Start();
            PrefabManager.Start();
            GameObjectPoolManager.Start();

            RoutineManager.Start();
            GameStateManager.Start();

            InitBackpack();
            BackpackManager.Start();
            DragManager.Start();
            MouseHoverManager.Start();
            DragExecuteManager.Start();
            AIManager.Start();

            ClientLevelManager.Start();
            BattleManager.Start();
            ClientBattleManager.Start();
            FXManager.Start();
            UIBattleTipManager.Start();
            ProjectileManager.Init(ClientProjectileManager.EmitProjectile);
            ClientProjectileManager.Start();

            ControlManager.Start();

            UIManager.Instance.ShowUIForms<DebugPanel>();
#if !DEBUG
            UIManager.Instance.CloseUIForm<DebugPanel>();
#endif

            StartGame();
        }

        private void Update()
        {
            if (ControlManager.Common_RestartGame.Up)
            {
                SceneManager.LoadScene(0);
                return;
            }

            ConfigManager.Update(Time.deltaTime);
            LayerManager.Update(Time.deltaTime);
            PrefabManager.Update(Time.deltaTime);
            GameObjectPoolManager.Update(Time.deltaTime);

            RoutineManager.Update(Time.deltaTime, Time.frameCount);
            GameStateManager.Update(Time.deltaTime);

            BackpackManager.Update(Time.deltaTime);
            DragManager.Update(Time.deltaTime);
            MouseHoverManager.Update(Time.deltaTime);
            DragExecuteManager.Update(Time.deltaTime);
            AIManager.Update(Time.deltaTime);

            ClientLevelManager.Update(Time.deltaTime);
            BattleManager.Update(Time.deltaTime);
            ClientBattleManager.Update(Time.deltaTime);
            FXManager.Update(Time.deltaTime);
            UIBattleTipManager.Update(Time.deltaTime);
            ClientProjectileManager.Update(Time.deltaTime);

            ControlManager.Update(Time.deltaTime);
        }

        void LateUpdate()
        {
            ConfigManager.LateUpdate(Time.deltaTime);
            LayerManager.LateUpdate(Time.deltaTime);
            PrefabManager.LateUpdate(Time.deltaTime);
            GameObjectPoolManager.LateUpdate(Time.deltaTime);

            RoutineManager.LateUpdate(Time.deltaTime);
            GameStateManager.LateUpdate(Time.deltaTime);

            BackpackManager.LateUpdate(Time.deltaTime);
            DragManager.LateUpdate(Time.deltaTime);
            MouseHoverManager.LateUpdate(Time.deltaTime);
            DragExecuteManager.LateUpdate(Time.deltaTime);
            AIManager.LateUpdate(Time.deltaTime);

            ClientLevelManager.LateUpdate(Time.deltaTime);
            BattleManager.LateUpdate(Time.deltaTime);
            ClientBattleManager.LateUpdate(Time.deltaTime);
            FXManager.LateUpdate(Time.deltaTime);
            UIBattleTipManager.LateUpdate(Time.deltaTime);
            ClientProjectileManager.LateUpdate(Time.deltaTime);

            ControlManager.LateUpdate(Time.deltaTime);
        }

        void FixedUpdate()
        {
            ConfigManager.FixedUpdate(Time.fixedDeltaTime);
            LayerManager.FixedUpdate(Time.fixedDeltaTime);
            PrefabManager.FixedUpdate(Time.fixedDeltaTime);
            GameObjectPoolManager.FixedUpdate(Time.fixedDeltaTime);

            RoutineManager.FixedUpdate(Time.fixedDeltaTime);
            GameStateManager.FixedUpdate(Time.fixedDeltaTime);

            BackpackManager.FixedUpdate(Time.fixedDeltaTime);
            DragManager.FixedUpdate(Time.fixedDeltaTime);
            MouseHoverManager.FixedUpdate(Time.fixedDeltaTime);
            DragExecuteManager.FixedUpdate(Time.fixedDeltaTime);
            AIManager.FixedUpdate(Time.fixedDeltaTime);

            ClientLevelManager.FixedUpdate(Time.fixedDeltaTime);
            BattleManager.FixedUpdate(Time.fixedDeltaTime);
            ClientBattleManager.FixedUpdate(Time.fixedDeltaTime);
            FXManager.FixedUpdate(Time.fixedDeltaTime);
            UIBattleTipManager.FixedUpdate(Time.fixedDeltaTime);
            ClientProjectileManager.FixedUpdate(Time.fixedDeltaTime);

            ControlManager.FixedUpdate(Time.fixedDeltaTime);
        }

        private void InitBackpack()
        {
            Backpack myBackPack = new Backpack(
                DragAreaDefines.BattleInventory.DragAreaName,
                DragAreaDefines.BattleInventory,
                ConfigManager.BackpackGridSize,
                10,
                8,
                false,
                true,
                true,
                65,
                () => ControlManager.Instance.Common_Tab.Down,
                () => ControlManager.Instance.Building_RotateItem.Down,
                (parent) => GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BackpackGrid].AllocateGameObject<BackpackGrid>(parent),
                (parent) => GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BackpackItem].AllocateGameObject<BackpackItem>(parent),
                (parent) => GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BackpackItemGrid].AllocateGameObject<BackpackItemGrid>(parent),
                (parent) => GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BackpackVirtualOccupationQuad].AllocateGameObject<BackpackVirtualOccupationQuad>(parent)
            );

            myBackPack.ToggleDebugKeyDownHandler = () => ControlManager.Instance.Common_Debug.Down;
            myBackPack.ToggleBackpackCallback = ToggleBattleInventory;
            myBackPack.ToggleDebugCallback = null;
            myBackPack.DragItemOutBackpackCallback = (backpackItem) =>
            {
                switch (backpackItem.InventoryItem.ItemContentInfo)
                {
                    case MechaComponentInfo mechaComponentInfo:
                    {
                        Ray ray = CameraManager.Instance.MainCamera.ScreenPointToRay(ControlManager.Instance.Building_MousePosition);
                        GridPos gp = GridUtils.GetGridPosByMousePos(ClientBattleManager.Instance.PlayerMecha.transform, ray, Vector3.up, ConfigManager.GridSize);
                        GridPosR gp_matrix = ClientBattleManager.Instance.PlayerMecha.MechaInfo.MechaEditorInventory.CoordinateTransformationHandler_FromPosToMatrixIndex(gp);
                        MechaComponentInfo mci = mechaComponentInfo.Clone();
                        ClientBattleManager.Instance.PlayerMecha.MechaInfo.AddMechaComponentInfo(mci, gp_matrix);
                        MechaComponent mc = ClientBattleManager.Instance.PlayerMecha.MechaComponentDict[mci.GUID];
                        mci.InventoryItem.SetGridPosition(gp_matrix);
                        DragManager.Instance.CurrentDrag = mc.Draggable;
                        mc.Draggable.SetOnDrag(true, null, DragManager.Instance.GetDragProcessor<MechaComponent>());
                        return true;
                    }
                }

                return false;
            };

            BackpackPanel backpackPanel = Instantiate(PrefabManager.Instance.GetPrefab("BattleInventoryPanel"), UIManager.Instance.UINormalRoot).GetComponent<BackpackPanel>();
            backpackPanel.Init(myBackPack,
                delegate(BackpackItem bi) { UIManager.Instance.ShowUIForms<BackpackItemInfoPanel>().Initialize(bi.InventoryItem.ItemContentInfo, backpackPanel); },
                delegate(BackpackItem bi) { UIManager.Instance.CloseUIForm<BackpackItemInfoPanel>(); });
            backpackPanel.gameObject.SetActive(false);
            BackpackManager.AddBackPack(myBackPack);
        }

        private void StartGame()
        {
            Backpack myBackPack = BackpackManager.Instance.GetBackPack(DragAreaDefines.BattleInventory.DragAreaName);
            InventoryInfo inventoryInfo = new InventoryInfo();
            MechaComponentGroupConfig mcg_config = ConfigManager.Instance.GetMechaComponentGroupConfig("EntryPlayerBattleInventory");
            foreach (MechaComponentGroupConfig.Config config in mcg_config.MechaComponentList)
            {
                MechaComponentInfo mci = new MechaComponentInfo(ConfigManager.Instance.GetMechaComponentConfig(config.MechaComponentKey), config.Quality);
                inventoryInfo.InventoryItems.Add(new InventoryItem(mci, myBackPack, new GridPosR(0, 0, GridPosR.Orientation.Up)));
            }

            myBackPack.LoadInventoryInfo(inventoryInfo);

            MechaInfo playerMechaInfo = new MechaInfo("Solar 0", MechaCamp.Player, null);
            playerMechaInfo.AddMechaComponentInfo(new MechaComponentInfo(ConfigManager.Instance.GetMechaComponentConfig("MC_BasicCore"), Quality.Common), new GridPosR(9, 9));

            BattleInfo battleInfo = new BattleInfo(playerMechaInfo);
            ClientBattleManager.Instance.StartBattle(battleInfo);
            UIBattleTipManager.Init();

            ClientLevelManager.Instance.StartLevel();
        }

        private void ShutDownGame()
        {
            UIBattleTipManager.Instance.ShutDown();
        }

        // todo 做成AI原子
        private void ToggleBattleInventory(bool open)
        {
            MouseHoverManager.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.BattleInventory);
            BackpackManager.Instance.GetBackPack(DragAreaDefines.BattleInventory.DragAreaName).BackpackPanel.gameObject.SetActive(open);
            if (open)
            {
                ClientBattleManager.Instance.SetAllEnemyMechaShown(false);
                ClientBattleManager.Instance.PlayerMecha.MechaEditArea.SetShown(true);
                ClientBattleManager.Instance.PlayerMecha.SlotLightsShown = true;
                CameraManager.Instance.MainCameraFollow.FOV_Level = 1;
                ClientBattleManager.Instance.PlayerMecha.GridShown = true;
                GameStateManager.Instance.SetState(GameState.Building);
            }
            else
            {
                ClientBattleManager.Instance.SetAllEnemyMechaShown(true);
                ClientBattleManager.Instance.PlayerMecha.MechaEditArea.SetShown(false);
                ClientBattleManager.Instance.PlayerMecha.SlotLightsShown = false;
                ClientBattleManager.Instance.PlayerMecha.GridShown = false;
                ClientBattleManager.Instance.PlayerMecha.MechaInfo.MechaEditorInventory.RefreshConflictAndIsolation(out List<InventoryItem> conflictItem, out List<InventoryItem> isolatedItem);

                Backpack battleInventory = BackpackManager.Instance.GetBackPack(DragAreaDefines.BattleInventory.DragAreaName);
                foreach (InventoryItem mc in conflictItem)
                {
                    ((MechaComponentInfo) mc.ItemContentInfo).RemoveMechaComponentInfo();
                    battleInventory.TryAddItem(new InventoryItem(mc.ItemContentInfo, battleInventory, GridPosR.Zero));
                }

                foreach (InventoryItem mc in isolatedItem)
                {
                    ((MechaComponentInfo) mc.ItemContentInfo).RemoveMechaComponentInfo();
                    battleInventory.TryAddItem(new InventoryItem(mc.ItemContentInfo, battleInventory, GridPosR.Zero));
                }

                CameraManager.Instance.MainCameraFollow.SetTarget(ClientBattleManager.Instance.PlayerMecha.transform);
                CameraManager.Instance.MainCameraFollow.FOV_Level = 2;
                GameStateManager.Instance.SetState(GameState.Fighting);
            }

            ControlManager.Instance.EnableBuildingInputActions(open);
            ControlManager.Instance.EnableBattleInputActions(!open);
        }

        /// <summary>
        /// Load all 2D sprites of mecha components
        /// </summary>
        private Dictionary<string, Sprite> LoadAllBackpackItemPics()
        {
            Dictionary<string, Sprite> backpackItemSpriteDict = new Dictionary<string, Sprite>();
            foreach (string s in Enum.GetNames(typeof(MechaComponentType)))
            {
                MechaComponentType mcType = (MechaComponentType) Enum.Parse(typeof(MechaComponentType), s);
                Sprite sprite = Resources.Load<Sprite>("BackpackItemPics/" + s);
                backpackItemSpriteDict.Add(typeof(MechaComponentType).FullName + "." + mcType, sprite);
            }

            return backpackItemSpriteDict;
        }

        private void InitMouseHoverManager()
        {
            MouseHoverManager.Instance.AddHoverAction(new MouseHoverManager.Hover<MouseHoverUI>(LayerManager.LayerMask_UI, MouseHoverManager.StateMachine.States.UI, UIManager.Instance.UICamera));
            MouseHoverManager.Instance.AddHoverAction(new MouseHoverManager.Hover<BackpackItem>(LayerManager.LayerMask_BackpackItemGrid, MouseHoverManager.StateMachine.States.BattleInventory, UIManager.Instance.UICamera));
        }
    }
}
using System;
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
            ConfigManager.Update();
            LayerManager.Update();
            PrefabManager.Update();
            GameObjectPoolManager.Update();

            RoutineManager.Update(Time.deltaTime, Time.frameCount);
            GameStateManager.Update();

            BackpackManager.Update();
            DragManager.Update();
            MouseHoverManager.Update();
            DragExecuteManager.Update();

            ClientLevelManager.Update();
            BattleManager.Update();
            ClientBattleManager.Update();
            FXManager.Update();
            UIBattleTipManager.Update();
            ClientProjectileManager.Update();

            ControlManager.Update();
        }

        void LateUpdate()
        {
            ConfigManager.LateUpdate();
            LayerManager.LateUpdate();
            PrefabManager.LateUpdate();
            GameObjectPoolManager.LateUpdate();

            RoutineManager.LateUpdate();
            GameStateManager.LateUpdate();

            BackpackManager.LateUpdate();
            DragManager.LateUpdate();
            MouseHoverManager.LateUpdate();
            DragExecuteManager.LateUpdate();

            ClientLevelManager.LateUpdate();
            BattleManager.LateUpdate();
            ClientBattleManager.LateUpdate();
            FXManager.LateUpdate();
            UIBattleTipManager.LateUpdate();
            ClientProjectileManager.LateUpdate();

            ControlManager.LateUpdate();
        }

        void FixedUpdate()
        {
            ConfigManager.FixedUpdate();
            LayerManager.FixedUpdate();
            PrefabManager.FixedUpdate();
            GameObjectPoolManager.FixedUpdate();

            RoutineManager.FixedUpdate();
            GameStateManager.FixedUpdate();

            BackpackManager.FixedUpdate();
            DragManager.FixedUpdate();
            MouseHoverManager.FixedUpdate();
            DragExecuteManager.FixedUpdate();

            ClientLevelManager.FixedUpdate();
            BattleManager.FixedUpdate();
            ClientBattleManager.FixedUpdate();
            FXManager.FixedUpdate();
            UIBattleTipManager.FixedUpdate();
            ClientProjectileManager.FixedUpdate();

            ControlManager.FixedUpdate();
        }

        private void InitBackpack()
        {
            Backpack myBackPack = new Backpack(
                DragAreaDefines.BattleInventory.DragAreaName,
                DragAreaDefines.BattleInventory,
                ConfigManager.BackpackGridSize,
                10,
                10,
                true,
                75,
                () => ControlManager.Instance.Common_Tab.Down,
                () => ControlManager.Instance.Building_RotateItem.Down,
                (parent) => GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BackpackGrid].AllocateGameObject<BackpackGrid>(parent),
                (parent) => GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BackpackItem].AllocateGameObject<BackpackItem>(parent),
                (parent) => GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BackpackVirtualOccupationQuad].AllocateGameObject<BackpackVirtualOccupationQuad>(parent),
                (parent) => GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BackpackItemGridHitBox].AllocateGameObject<BackpackItemGridHitBox>(parent)
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
            backpackPanel.gameObject.SetActive(false);
            backpackPanel.Init(myBackPack,
                delegate(BackpackItem bi) { UIManager.Instance.ShowUIForms<BackpackItemInfoPanel>().Initialize(bi.InventoryItem.ItemContentInfo); },
                delegate(BackpackItem bi) { UIManager.Instance.CloseUIForm<BackpackItemInfoPanel>(); });
            BackpackManager.AddBackPack(myBackPack);
        }

        private void StartGame()
        {
            Backpack myBackPack = BackpackManager.Instance.GetBackPack(DragAreaDefines.BattleInventory.DragAreaName);
            InventoryInfo inventoryInfo = new InventoryInfo();
            MechaComponentGroupConfig mcg_config = ConfigManager.Instance.GetMechaComponentGroupConfig("EntryPlayerBattleInventory");
            foreach (MechaComponentGroupConfig.Config config in mcg_config.MechaComponentList)
            {
                inventoryInfo.InventoryItems.Add(new InventoryItem(new MechaComponentInfo(ConfigManager.Instance.GetMechaComponentConfig(config.MechaComponentKey), config.Quality), myBackPack, GridPosR.Zero));
            }

            myBackPack.LoadInventoryInfo(inventoryInfo);

            MechaInfo playerMechaInfo = new MechaInfo("Solar 0", MechaType.Player);

            BattleInfo battleInfo = new BattleInfo(playerMechaInfo);
            ClientBattleManager.Instance.StartBattle(battleInfo);
            UIBattleTipManager.Init();

            playerMechaInfo.AddMechaComponentInfo(new MechaComponentInfo(ConfigManager.Instance.GetMechaComponentConfig("MC_BasicCore"), Quality.Poor), new GridPosR(9, 9));

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
            ClientBattleManager.Instance.PlayerMecha.MechaLight.enabled = open;
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
            MouseHoverManager.Instance.AddHoverAction(new MouseHoverManager.Hover<BackpackItem>(LayerManager.LayerMask_BackpackItemHitBox, MouseHoverManager.StateMachine.States.BattleInventory, UIManager.Instance.UICamera, 0.3f));
        }
    }
}
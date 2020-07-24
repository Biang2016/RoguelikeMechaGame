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
using GameCore.AbilityDataDriven;
using ParadoxNotion;
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
        private DragExecuteManager DragExecuteManager => DragExecuteManager.Instance;

        #region Level

        private LevelManager LevelManager => LevelManager.Instance;
        private ClientLevelManager ClientLevelManager => ClientLevelManager.Instance;
        private ClientBattleManager ClientBattleManager => ClientBattleManager.Instance;

        public Messenger BattleMessenger => ClientBattleManager.BattleInfo.BattleMessenger;

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

            ControlManager.Awake();

            RoutineManager.LogErrorHandler = Debug.LogError;
            RoutineManager.Awake();
            GameStateManager.Awake();
            DebugConsole.OnDebugConsoleKeyDownHandler = () => ControlManager.Instance.Common_Debug.Down;
            DebugConsole.OnDebugConsoleToggleHandler = (enable) =>
            {
                ControlManager.Instance.EnableBattleInputActions(!enable);
                ControlManager.Instance.EnableBuildingInputActions(!enable);
            };

            BackpackManager.Awake();

            DragManager.Awake();
            DragExecuteManager.Init();
            DragExecuteManager.Awake();

            ClientLevelManager.Init();
            LevelManager.Init();
            ClientLevelManager.Awake();
            ClientBattleManager.Awake();
            FXManager.Awake();

            ClientProjectileManager.Init(new GameObject("ProjectileRoot").transform);
            ClientProjectileManager.Awake();
        }

        private void Start()
        {
            ConfigManager.Start();
            LayerManager.Start();
            PrefabManager.Start();
            GameObjectPoolManager.Start();

            ControlManager.Start();
            RoutineManager.Start();
            GameStateManager.Start();

            Backpack myBackPack = new Backpack(
                DragAreaDefines.BattleInventory.DragAreaName,
                DragAreaDefines.BattleInventory,
                60,
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
                        MechaComponentBase mcb = ClientBattleManager.Instance.PlayerMecha.MechaComponentDict[mci.GUID];
                        mci.InventoryItem.SetGridPosition(gp_matrix);
                        DragManager.Instance.CurrentDrag = mcb.Draggable;
                        mcb.Draggable.SetOnDrag(true, null, DragManager.Instance.GetDragProcessor<MechaComponentBase>());
                        return true;
                    }
                }

                return false;
            };

            BackpackPanel backpackPanel = Instantiate(PrefabManager.Instance.GetPrefab("BattleInventoryPanel"), UIManager.Instance.UINormalRoot).GetComponent<BackpackPanel>();
            backpackPanel.gameObject.SetActive(false);
            backpackPanel.Init(myBackPack);
            BackpackManager.AddBackPack(myBackPack);

            //foreach (string s in Enum.GetNames(typeof(MechaComponentType)))
            //{
            //    MechaComponentType mcType = (MechaComponentType) Enum.Parse(typeof(MechaComponentType), s);
            //    InventoryItem ii = new InventoryItem(new MechaComponentInfo(mcType, new AbilityGroup(), 100, 0), myBackPack, GridPosR.Zero);
            //    myBackPack.TryAddItem(ii);
            //}

            BackpackManager.Start();
            DragManager.Start();
            DragExecuteManager.Start();

            ClientLevelManager.Start();
            ClientBattleManager.Start();
            FXManager.Start();
            ProjectileManager.Init(ClientProjectileManager.EmitProjectile);
            ClientProjectileManager.Start();

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

            ControlManager.Update();
            RoutineManager.Update(Time.deltaTime, Time.frameCount);
            GameStateManager.Update();

            BackpackManager.Update();
            DragManager.Update();
            DragExecuteManager.Update();

            ClientLevelManager.Update();
            ClientBattleManager.Update();
            FXManager.Update();
            ClientProjectileManager.Update();
        }

        void LateUpdate()
        {
            ConfigManager.LateUpdate();
            LayerManager.LateUpdate();
            PrefabManager.LateUpdate();
            GameObjectPoolManager.LateUpdate();

            ControlManager.LateUpdate();
            RoutineManager.LateUpdate();
            GameStateManager.LateUpdate();

            BackpackManager.LateUpdate();
            DragManager.LateUpdate();
            DragExecuteManager.LateUpdate();

            ClientLevelManager.LateUpdate();
            ClientBattleManager.LateUpdate();
            FXManager.LateUpdate();
            ClientProjectileManager.LateUpdate();
        }

        void FixedUpdate()
        {
            ConfigManager.FixedUpdate();
            LayerManager.FixedUpdate();
            PrefabManager.FixedUpdate();
            GameObjectPoolManager.FixedUpdate();

            ControlManager.FixedUpdate();
            RoutineManager.FixedUpdate();
            GameStateManager.FixedUpdate();

            BackpackManager.FixedUpdate();
            DragManager.FixedUpdate();
            DragExecuteManager.FixedUpdate();

            ClientLevelManager.FixedUpdate();
            ClientBattleManager.FixedUpdate();
            FXManager.FixedUpdate();
            ClientProjectileManager.FixedUpdate();
        }

        private void StartGame()
        {
            Backpack myBackPack = BackpackManager.Instance.GetBackPack(DragAreaDefines.BattleInventory.DragAreaName);
            InventoryInfo inventoryInfo = new InventoryInfo();
            InventoryItem ii = new InventoryItem(new MechaComponentInfo(MechaComponentType.Gun, ConfigManager.Instance.GetAbilityGroup("BasicGun"), 100, 0), myBackPack, GridPosR.Zero);
            inventoryInfo.InventoryItems.Add(ii);
            ii = new InventoryItem(new MechaComponentInfo(MechaComponentType.Gun, ConfigManager.Instance.GetAbilityGroup("BasicGun1"), 100, 0), myBackPack, GridPosR.Zero);
            inventoryInfo.InventoryItems.Add(ii);
            ii = new InventoryItem(new MechaComponentInfo(MechaComponentType.Gun, ConfigManager.Instance.GetAbilityGroup("BasicGun2"), 100, 0), myBackPack, GridPosR.Zero);
            inventoryInfo.InventoryItems.Add(ii);
            myBackPack.LoadInventoryInfo(inventoryInfo);

            MechaInfo playerMechaInfo = new MechaInfo("Solar 0", MechaType.Player);
            MechaInfo enemyMechaInfo = new MechaInfo("Junk Mecha", MechaType.Enemy);

            BattleInfo battleInfo = new BattleInfo(playerMechaInfo);
            ClientBattleManager.Instance.StartBattle(battleInfo);
            UIBattleTipManager.Init();

            battleInfo.SetPlayerMecha(playerMechaInfo);
            playerMechaInfo.AddMechaComponentInfo(new MechaComponentInfo(MechaComponentType.Core, ConfigManager.Instance.GetAbilityGroup("BasicGun"), 300, 0), new GridPosR(9, 9));
            battleInfo.AddEnemyMechaInfo(enemyMechaInfo);
            for (int i = -2; i <= 2; i++)
            {
                for (int j = -4; j <= 4; j++)
                {
                    MechaComponentInfo mci;
                    mci = new MechaComponentInfo(MechaComponentType.Core, ConfigManager.Instance.GetAbilityGroup("BasicGun"), 500, 0);
                    enemyMechaInfo.AddMechaComponentInfo(mci, new GridPosR(i, j, GridPosR.Orientation.Up));
                }
            }

            ClientBattleManager.MechaDict[enemyMechaInfo.GUID].transform.position = new Vector3(10, 0, 10);

            ClientLevelManager.Instance.StartLevel();
        }

        private void ShutDownGame()
        {
            UIBattleTipManager.Instance.ShutDown();
        }

        // todo 做成AI原子
        private void ToggleBattleInventory(bool open)
        {
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

                foreach (InventoryItem mcb in conflictItem)
                {
                    ((MechaComponentInfo) mcb.ItemContentInfo).RemoveMechaComponentInfo();
                }

                foreach (InventoryItem mcb in isolatedItem)
                {
                    ((MechaComponentInfo) mcb.ItemContentInfo).RemoveMechaComponentInfo();
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
    }
}
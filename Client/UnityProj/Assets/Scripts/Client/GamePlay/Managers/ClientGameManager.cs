using System;
using System.Collections.Generic;
using BiangStudio.DragHover;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.GamePlay;
using BiangStudio.GamePlay.UI;
using BiangStudio.GridBackpack;
using BiangStudio.Log;
using BiangStudio.ShapedInventory;
using BiangStudio.Singleton;
using GameCore;
using UnityEngine;

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

        private ClientLevelManager ClientLevelManager => ClientLevelManager.Instance;
        private ClientBattleManager ClientBattleManager => ClientBattleManager.Instance;
        private FXManager FXManager => FXManager.Instance;
        private ProjectileManager ProjectileManager => ProjectileManager.Instance;

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

            ClientLevelManager.Init(6789);
            ClientLevelManager.Awake();
            ClientBattleManager.Init(new GameObject("MechaContainerRoot").transform, new GameObject("MechaComponentDropSpriteContainerRoot").transform);
            ClientBattleManager.Awake();
            FXManager.Awake();
            ProjectileManager.Awake();
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
                () => ControlManager.Instance.Building_ToggleBackpack.Down,
                () => ControlManager.Instance.Building_RotateItem.Down,
                (parent) => GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BackpackGrid].AllocateGameObject<BackpackGrid>(parent),
                (parent) => GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BackpackItem].AllocateGameObject<BackpackItem>(parent),
                (parent) => GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BackpackItemGridHitBox].AllocateGameObject<BackpackItemGridHitBox>(parent)
            );

            myBackPack.ToggleDebugKeyDownHandler = () => ControlManager.Instance.Building_ToggleDebug.Down;
            myBackPack.ToggleBackpackCallback = ToggleBattleInventory;
            myBackPack.ToggleDebugCallback = null;
            myBackPack.DragItemOutBackpackCallback = (backpackItem) =>
            {
                switch (backpackItem.Data.ItemContentInfo)
                {
                    case MechaComponentInfo mechaComponentInfo:
                    {
                        MechaComponentInfo mci = mechaComponentInfo.Clone();
                        ClientBattleManager.Instance.PlayerMecha.MechaInfo.AddMechaComponentInfo(mci);

                        MechaComponentBase mcb = ClientBattleManager.Instance.PlayerMecha.MechaComponents[mci.GUID];
                        Ray ray = CameraManager.Instance.MainCamera.ScreenPointToRay(ControlManager.Instance.Building_MousePosition);
                        GridPos gp = GridUtils.GetGridPosByMousePos(ClientBattleManager.Instance.PlayerMecha.transform, ray, Vector3.up, ConfigManager.GridSize);
                        mcb.SetGridPosition(gp);
                        DragManager.Instance.CurrentDrag = mcb.Draggable;
                        mcb.Draggable.SetOnDrag(true, null, DragManager.Instance.GetDragProcessor<MechaComponentBase>());
                        backpackItem.Backpack.RemoveItem(backpackItem.Data);
                        backpackItem.PoolRecycle();
                        break;
                    }
                }
            };

            BackpackPanel backpackPanel = Instantiate(PrefabManager.Instance.GetPrefab("BattleInventoryPanel"), UIManager.Instance.UINormalRoot).GetComponent<BackpackPanel>();
            backpackPanel.gameObject.SetActive(false);
            backpackPanel.Init(myBackPack);
            BackpackManager.AddBackPack(myBackPack);

            foreach (string s in Enum.GetNames(typeof(MechaComponentType)))
            {
                MechaComponentType mcType = (MechaComponentType) Enum.Parse(typeof(MechaComponentType), s);
                InventoryItem ii = new InventoryItem(new MechaComponentInfo(mcType, new GridPosR(0, 0, GridPosR.Orientation.Up), 100, 0));
                myBackPack.TryAddItem(ii);
            }

            BackpackManager.Start();
            DragManager.Start();
            DragExecuteManager.Start();

            ClientLevelManager.Start();
            ClientBattleManager.Start();
            FXManager.Start();
            ProjectileManager.Start();

            UIManager.Instance.ShowUIForms<DebugPanel>();
#if !DEBUG
            UIManager.Instance.CloseUIForm<DebugPanel>();
#endif

            StartGame();
        }

        void Update()
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
            ProjectileManager.Update();
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
            ProjectileManager.LateUpdate();
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
            ProjectileManager.FixedUpdate();
        }

        private void StartGame()
        {
            ClientLevelManager.Instance.StartLevel();

            MechaInfo playerMechaInfo = new MechaInfo("Solar 0", MechaType.Player);
            playerMechaInfo.AddMechaComponentInfo(new MechaComponentInfo(MechaComponentType.Core, new GridPosR(0, 0, GridPosR.Orientation.Up), 300, 0));

            MechaInfo enemyMechaInfo = new MechaInfo("Junk Mecha", MechaType.Enemy);

            List<MechaComponentInfo> enemyComponentInfos = new List<MechaComponentInfo>();
            for (int i = -4; i <= 4; i++)
            {
                for (int j = -6; j <= 6; j++)
                {
                    MechaComponentInfo mci;
                    if (i == 0 && j == 0)
                    {
                        mci = new MechaComponentInfo(MechaComponentType.Core, new GridPosR(i, j, GridPosR.Orientation.Up), 500, 0);
                    }
                    else
                    {
                        mci = new MechaComponentInfo((MechaComponentType) ClientLevelManager.SRandom.Range(1, Enum.GetNames(typeof(MechaComponentType)).Length), new GridPosR(i, j, GridPosR.Orientation.Up), 50, 5);
                    }

                    enemyComponentInfos.Add(mci);
                }
            }

            foreach (MechaComponentInfo mci in enemyComponentInfos)
            {
                enemyMechaInfo.AddMechaComponentInfo(mci);
            }

            BattleInfo battleInfo = new BattleInfo(playerMechaInfo);
            ClientBattleManager.Instance.StartBattle(battleInfo);
            battleInfo.AddEnemyMechaInfo(enemyMechaInfo);

            ClientBattleManager.EnemyMechaDict[enemyMechaInfo.GUID].transform.position = new Vector3(10, 0, 10);
        }

        // todo 做成AI原子
        private void ToggleBattleInventory(bool open)
        {
            BackpackManager.Instance.GetBackPack(DragAreaDefines.BattleInventory.DragAreaName).BackpackPanel.gameObject.SetActive(open);
            if (open)
            {
                ClientBattleManager.Instance.SetAllEnemyShown(false);
                ClientBattleManager.Instance.PlayerMecha.MechaEditArea.Show();
                ClientBattleManager.Instance.PlayerMecha.SlotLightsShown = true;
                CameraManager.Instance.MainCameraFollow.FOV_Level = 1;
                ClientBattleManager.Instance.PlayerMecha.GridShown = true;
                GameStateManager.Instance.SetState(GameState.Building);
            }
            else
            {
                ClientBattleManager.Instance.SetAllEnemyShown(true);
                ClientBattleManager.Instance.PlayerMecha.MechaEditArea.Hide();
                ClientBattleManager.Instance.PlayerMecha.SlotLightsShown = false;
                ClientBattleManager.Instance.PlayerMecha.GridShown = false;
                ClientBattleManager.Instance.PlayerMecha.RefreshMechaMatrix(out List<InventoryItem> conflictItem, out List<InventoryItem> isolatedItem);

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
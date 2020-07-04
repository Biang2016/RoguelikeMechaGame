using System;
using System.Collections.Generic;
using BiangStudio.DragHover;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.GamePlay;
using BiangStudio.GamePlay.UI;
using BiangStudio.GridBag;
using BiangStudio.Log;
using UnityEngine;
using BiangStudio.Singleton;
using GameCore;

namespace Client
{
    public class GameManager : MonoSingleton<GameManager>
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

        private BagManager BagManager => BagManager.Instance;
        private DragManager DragManager => DragManager.Instance;
        private DragExecuteManager DragExecuteManager => DragExecuteManager.Instance;

        #region Level

        private LevelManager LevelManager => LevelManager.Instance;
        private BattleManager BattleManager => BattleManager.Instance;
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
                mouseLeftButtonDownHandler: () => ControlManager.Instance.Common_MouseLeft.Down,
                mouseRightButtonDownHandler: () => ControlManager.Instance.Common_MouseRight.Down,
                closeUIFormKeyDownHandler: () => ControlManager.Instance.Common_Exit.Down,
                confirmKeyDownHandler: () => ControlManager.Instance.Common_Confirm.Down,
                inputNavigateKeyDownHandler: () => ControlManager.Instance.Common_Tab.Down
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

            BagManager.Init(
                LoadAllBagItemPics(),
                ToggleBag,
                toggleBagKeyDownHandler: () => ControlManager.Instance.Building_ToggleBag.Down,
                rotateItemKeyDownHandler: () => ControlManager.Instance.Building_RotateItem.Down,
                dragItemOutBagAction: (bagItem) =>
                {
                    switch (bagItem.Data.BagItemContentInfo)
                    {
                        case MechaComponentInfo mechaComponentInfo:
                        {
                            MechaComponentBase mcb = MechaComponentBase.BaseInitialize(mechaComponentInfo.Clone(), BattleManager.Instance.PlayerMecha);
                            Ray ray = CameraManager.Instance.MainCamera.ScreenPointToRay(ControlManager.Instance.Building_MousePosition);
                            GridPos gp = GridUtils.GetGridPosByMousePos(BattleManager.Instance.PlayerMecha.transform, ray, Vector3.up, ConfigManager.GridSize);
                            mcb.SetGridPosition(gp);
                            BattleManager.Instance.PlayerMecha.AddMechaComponent(mcb);
                            DragManager.Instance.CurrentDrag = mcb.Draggable;
                            mcb.Draggable.SetOnDrag(true, null, DragManager.Instance.GetDragProcessor<MechaComponentBase>());
                            BagManager.Instance.BagInfo.RemoveItem(bagItem.Data);
                            bagItem.PoolRecycle();
                            break;
                        }
                    }
                },
                instantiateBagGridHandler: (transform) => GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BagGrid].AllocateGameObject<BagGrid>(transform),
                instantiateBagItemHandler: (transform) => GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BagItem].AllocateGameObject<BagItem>(transform),
                instantiateBagItemGridHitBoxHandler: (transform) => GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BagItemGridHitBox].AllocateGameObject<BagItemGridHitBox>(transform)
            );
            BagManager.Awake();
            DragManager.Awake();
            DragExecuteManager.Init();
            DragExecuteManager.Awake();

            LevelManager.Init(6789);
            LevelManager.Awake();
            BattleManager.Init(new GameObject("MechaContainerRoot").transform, new GameObject("MechaComponentDropSpriteContainerRoot").transform);
            BattleManager.Awake();
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

            BagManager.LoadBagInfo(new BagInfo(75));
            BagManager.Start();
            DragManager.Start();
            DragExecuteManager.Start();

            LevelManager.Start();
            BattleManager.Start();
            FXManager.Start();
            ProjectileManager.Start();

            UIManager.Instance.ShowUIForms<DebugPanel>();
#if !DEBUG
            UIManager.Instance.CloseUIForm<DebugPanel>();
#endif

            StartGame();

            foreach (string s in Enum.GetNames(typeof(MechaComponentType)))
            {
                MechaComponentType mcType = (MechaComponentType) Enum.Parse(typeof(MechaComponentType), s);
                BagItemInfo bii = new BagItemInfo(new MechaComponentInfo(mcType, new GridPosR(0, 0, GridPosR.Orientation.Up), 100, 0));
                BagManager.BagInfo.TryAddItem(bii);
            }
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

            BagManager.Update();
            DragManager.Update();
            DragExecuteManager.Update();

            LevelManager.Update();
            BattleManager.Update();
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

            BagManager.LateUpdate();
            DragManager.LateUpdate();
            DragExecuteManager.LateUpdate();

            LevelManager.LateUpdate();
            BattleManager.LateUpdate();
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

            BagManager.FixedUpdate();
            DragManager.FixedUpdate();
            DragExecuteManager.FixedUpdate();

            LevelManager.FixedUpdate();
            BattleManager.FixedUpdate();
            FXManager.FixedUpdate();
            ProjectileManager.FixedUpdate();
        }

        private void StartGame()
        {
            LevelManager.Instance.StartLevel();
            BattleManager.Instance.StartGame();
        }

        // todo 做成AI原子
        private void ToggleBag(bool open)
        {
            if (open)
            {
                BattleManager.Instance.SetAllEnemyShown(false);
                BattleManager.Instance.PlayerMecha.MechaEditArea.Show();
                BattleManager.Instance.PlayerMecha.SlotLightsShown = true;
                CameraManager.Instance.MainCameraFollow.FOV_Level = 1;
                BattleManager.Instance.PlayerMecha.GridShown = true;
                GameStateManager.Instance.SetState(GameState.Building);
            }
            else
            {
                BattleManager.Instance.SetAllEnemyShown(true);
                BattleManager.Instance.PlayerMecha.MechaEditArea.Hide();
                BattleManager.Instance.PlayerMecha.SlotLightsShown = false;
                BattleManager.Instance.PlayerMecha.GridShown = false;
                BattleManager.Instance.PlayerMecha.RefreshMechaMatrix(out List<MechaComponentBase> conflictComponents, out List<MechaComponentBase> isolatedComponents);

                foreach (MechaComponentBase mcb in conflictComponents)
                {
                    BattleManager.Instance.PlayerMecha.RemoveMechaComponent(mcb);
                    mcb.PoolRecycle();
                }

                foreach (MechaComponentBase mcb in isolatedComponents)
                {
                    BattleManager.Instance.PlayerMecha.RemoveMechaComponent(mcb);
                    mcb.PoolRecycle();
                }

                CameraManager.Instance.MainCameraFollow.SetTarget(BattleManager.Instance.PlayerMecha.transform);
                CameraManager.Instance.MainCameraFollow.FOV_Level = 2;
                GameStateManager.Instance.SetState(GameState.Fighting);
            }
        }

        /// <summary>
        /// Load all 2D sprites of mecha components
        /// </summary>
        private Dictionary<string, Sprite> LoadAllBagItemPics()
        {
            Dictionary<string, Sprite> bagItemSpriteDict = new Dictionary<string, Sprite>();
            foreach (string s in Enum.GetNames(typeof(MechaComponentType)))
            {
                MechaComponentType mcType = (MechaComponentType) Enum.Parse(typeof(MechaComponentType), s);
                Sprite sprite = Resources.Load<Sprite>("BagItemPics/" + s);
                bagItemSpriteDict.Add(typeof(MechaComponentType).FullName + "." + mcType, sprite);
            }

            return bagItemSpriteDict;
        }
    }
}
using BiangStudio.GamePlay;
using BiangStudio.GamePlay.UI;
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

        #region Level

        private LevelManager LevelManager => LevelManager.Instance;
        private BattleManager BattleManager => BattleManager.Instance;
        private FXManager FXManager => FXManager.Instance;
        private ProjectileManager ProjectileManager => ProjectileManager.Instance;

        #endregion

        #endregion

        #endregion

        #endregion

        private void Awake()
        {
            UIManager.Init(Instantiate, Debug.LogError,
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

            BagManager.Awake();
            DragManager.Awake();

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

            BagManager.Start();
            DragManager.Start();

            LevelManager.Start();
            BattleManager.Start();
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

            BagManager.Update();
            DragManager.Update();

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

            LevelManager.FixedUpdate();
            BattleManager.FixedUpdate();
            FXManager.FixedUpdate();
            ProjectileManager.FixedUpdate();
        }

        private void StartGame()
        {
            BagManager.Instance.Initialize();
            LevelManager.Instance.StartLevel();
            BattleManager.Instance.StartGame();
        }
    }
}
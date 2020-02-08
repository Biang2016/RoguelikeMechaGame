using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GamePlay;

public class GameManager : MonoSingleton<GameManager>
{
    public const int GridSize = 1;

    [NonSerialized] public int LayerMask_ComponentHitBox;
    [NonSerialized] public int LayerMask_DragAreas;
    [SerializeField] private bool ShowDebugPanel;

    public Camera MainCamera;
    public CameraFollow MainCameraFollow;
    [SerializeField] private Transform MechaContainer;

    internal Mecha PlayerMecha;
    internal Mecha EnemyMecha;

    void Awake()
    {
        LayerMask_ComponentHitBox = LayerMask.GetMask("ComponentHitBox");
        LayerMask_DragAreas = LayerMask.GetMask("DragAreas");
    }

    private void Start()
    {
        UIManager.Instance.ShowUIForms<DebugPanel>();
#if !UNITY_EDITOR
        if(!ShowDebugPanel) UIManager.Instance.CloseUIForm<DebugPanel>();
#endif

        Invoke("StartGame", 0.2f);
    }

    public void StartGame()
    {
        PlayerMecha = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.Mecha].AllocateGameObject<Mecha>(MechaContainer);
        PlayerMecha.Initialize(new MechaInfo(MechaType.Self, new List<MechaComponentInfo>
        {
            new MechaComponentInfo(MechaComponentType.Core, new GridPos(0, 0, GridPos.Orientation.Up)),
        }));

        MainCameraFollow.SetTarget(PlayerMecha.transform);

        EnemyMecha = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.Mecha].AllocateGameObject<Mecha>(MechaContainer);

        List<MechaComponentInfo> enemyComponentInfos = new List<MechaComponentInfo>();
        for (int i = 5; i <= 8; i++)
        {
            for (int j = -9; j <= 9; j++)
            {
                MechaComponentInfo mci;
                if (i == 7 && j == 0)
                {
                    mci = new MechaComponentInfo(MechaComponentType.Core, new GridPos(i, j, GridPos.Orientation.Up));
                }
                else
                {
                    mci = new MechaComponentInfo(MechaComponentType.Block, new GridPos(i, j, GridPos.Orientation.Up));
                }

                enemyComponentInfos.Add(mci);
            }
        }

        EnemyMecha.Initialize(new MechaInfo(MechaType.Enemy, enemyComponentInfos));

        SetState(GameState.Fighting);
    }

    void Update()
    {
    }

    private GameState state = GameState.Default;

    public void SetState(GameState newState)
    {
        if (state != newState)
        {
            switch (state)
            {
                case GameState.Fighting:
                {
                    break;
                }
                case GameState.Building:
                {
                    break;
                }
                case GameState.ESC:
                {
                    break;
                }
            }

            state = newState;
            switch (state)
            {
                case GameState.Fighting:
                {
                    DragManager.Instance.ForbidDrag = true;
                    Resume();
                    break;
                }
                case GameState.Building:
                {
                    DragManager.Instance.ForbidDrag = false;
                    Pause();
                    break;
                }
                case GameState.ESC:
                {
                    DragManager.Instance.ForbidDrag = true;
                    Pause();
                    break;
                }
            }
        }
    }

    public GameState GetState()
    {
        return state;
    }

    private void Pause()
    {
    }

    private void Resume()
    {
    }
}

public enum GameState
{
    Default,
    Fighting,
    Building,
    ESC,
}
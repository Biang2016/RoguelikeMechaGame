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
        PlayerMecha = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.Mecha].AllocateGameObject<Mecha>(MechaContainer);
        PlayerMecha.Initialize(new MechaInfo(MechaType.Self, new List<MechaComponentInfo>
        {
            new MechaComponentInfo(MechaComponentType.Core, new GridPos(0, 0, GridPos.Orientation.Up)),
        }));

        MainCameraFollow.SetTarget(PlayerMecha.transform);

        EnemyMecha = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.Mecha].AllocateGameObject<Mecha>(MechaContainer);
        EnemyMecha.Initialize(new MechaInfo(MechaType.Enemy, new List<MechaComponentInfo>
        {
            new MechaComponentInfo(MechaComponentType.Block, new GridPos(5, 5, GridPos.Orientation.Up)),
            new MechaComponentInfo(MechaComponentType.Block, new GridPos(4, 5, GridPos.Orientation.Up)),
            new MechaComponentInfo(MechaComponentType.Block, new GridPos(3, 5, GridPos.Orientation.Up)),
            new MechaComponentInfo(MechaComponentType.Block, new GridPos(2, 5, GridPos.Orientation.Up)),
            new MechaComponentInfo(MechaComponentType.Block, new GridPos(1, 5, GridPos.Orientation.Up)),
            new MechaComponentInfo(MechaComponentType.Block, new GridPos(5, 4, GridPos.Orientation.Up)),
            new MechaComponentInfo(MechaComponentType.Block, new GridPos(4, 4, GridPos.Orientation.Up)),
            new MechaComponentInfo(MechaComponentType.Block, new GridPos(3, 4, GridPos.Orientation.Up)),
            new MechaComponentInfo(MechaComponentType.Block, new GridPos(2, 4, GridPos.Orientation.Up)),
            new MechaComponentInfo(MechaComponentType.Block, new GridPos(1, 4, GridPos.Orientation.Up)),
            new MechaComponentInfo(MechaComponentType.Block, new GridPos(5, 3, GridPos.Orientation.Up)),
            new MechaComponentInfo(MechaComponentType.Block, new GridPos(4, 3, GridPos.Orientation.Up)),
            new MechaComponentInfo(MechaComponentType.Block, new GridPos(3, 3, GridPos.Orientation.Up)),
            new MechaComponentInfo(MechaComponentType.Block, new GridPos(2, 3, GridPos.Orientation.Up)),
            new MechaComponentInfo(MechaComponentType.Block, new GridPos(1, 3, GridPos.Orientation.Up)),
        }));

        SetState(GameState.Fighting);

        UIManager.Instance.ShowUIForms<DebugPanel>();
#if !UNITY_EDITOR
        if(!ShowDebugPanel) UIManager.Instance.CloseUIForm<DebugPanel>();
#endif
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
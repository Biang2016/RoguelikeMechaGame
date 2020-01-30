using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoSingleton<GameManager>
{
    public const int GridSize = 1;

    [NonSerialized] public int LayerMask_ComponentHitBox;
    [NonSerialized] public int LayerMask_DragAreas;

    void Awake()
    {
        LayerMask_ComponentHitBox = LayerMask.GetMask("ComponentHitBox");
        LayerMask_DragAreas = LayerMask.GetMask("DragAreas");
    }

    [SerializeField] private Transform MechaContainer;

    private Mecha PlayerMecha;

    private void Start()
    {
        PlayerMecha = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.Mecha].AllocateGameObject<Mecha>(MechaContainer);
        PlayerMecha.Initialize(new MechaInfo(MechaType.Self, new List<MechaComponentInfo>
        {
            new MechaComponentInfo(MechaComponentType.Core, new GridPos(0, 1, 0, GridPos.Orientation.Up)),
            new MechaComponentInfo(MechaComponentType.Core, new GridPos(1, 1, 1, GridPos.Orientation.Up)),
            new MechaComponentInfo(MechaComponentType.Block, new GridPos(1, 1, 1, GridPos.Orientation.Up)),
            new MechaComponentInfo(MechaComponentType.Armor, new GridPos(1, 1, 1, GridPos.Orientation.Up)),
            new MechaComponentInfo(MechaComponentType.Engine, new GridPos(1, 1, 1, GridPos.Orientation.Up)),
            new MechaComponentInfo(MechaComponentType.Gun, new GridPos(1, 1, 1, GridPos.Orientation.Up)),
            new MechaComponentInfo(MechaComponentType.Sword, new GridPos(1, 1, 1, GridPos.Orientation.Up)),
        }));
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
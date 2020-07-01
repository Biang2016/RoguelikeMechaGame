using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Client
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public const int GridSize = 1;

        [NonSerialized] public int LayerMask_ComponentHitBox;
        [NonSerialized] public int LayerMask_DragAreas;
        [NonSerialized] public int LayerMask_ItemDropped;
        [SerializeField] private bool ShowDebugPanel;

        public Camera MainCamera;
        public CameraFollow MainCameraFollow;

        void Awake()
        {
            LayerMask_ComponentHitBox = LayerMask.GetMask("ComponentHitBox");
            LayerMask_DragAreas = LayerMask.GetMask("DragAreas");
            LayerMask_ItemDropped = LayerMask.GetMask("ItemDropped");
        }

        private void Start()
        {
            GameCore.ConfigManager.LoadMechaComponentOccupiedGridPosDict();

            UIManager.Instance.ShowUIForms<DebugPanel>();
#if !UNITY_EDITOR
        if(!ShowDebugPanel) UIManager.Instance.CloseUIForm<DebugPanel>();
#endif

            Invoke("StartGame", 0.1f);
        }

        private void StartGame()
        {
            BagManager.Instance.Initialize();
            BattleManager.Instance.StartGame();
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
}
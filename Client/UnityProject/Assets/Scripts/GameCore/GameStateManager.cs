using BiangStudio.DragHover;
using BiangStudio.Singleton;

namespace GameCore
{
    public class GameStateManager : TSingletonBaseManager<GameStateManager>
    {
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
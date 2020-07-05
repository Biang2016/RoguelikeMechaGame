using BiangStudio.Singleton;

namespace GameCore
{
    public class GameManager : TSingleton<GameManager>
    {
        public LevelManager LevelManager => LevelManager.Instance;
    }
}
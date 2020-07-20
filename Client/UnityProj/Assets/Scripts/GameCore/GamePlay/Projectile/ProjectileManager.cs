using BiangStudio.Singleton;
using UnityEngine.Events;

namespace GameCore
{
    public class ProjectileManager : TSingleton<ProjectileManager>
    {
        public UnityAction<ProjectileInfo> EmitProjectileHandler;

        public void Init(UnityAction<ProjectileInfo> emitProjectileDelegate)
        {
            EmitProjectileHandler = emitProjectileDelegate;
        }
    }
}
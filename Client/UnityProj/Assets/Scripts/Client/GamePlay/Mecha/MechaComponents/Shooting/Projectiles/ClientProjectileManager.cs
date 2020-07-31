using System.Collections.Generic;
using BiangStudio.Singleton;
using GameCore;
using UnityEngine;

namespace Client
{
    public class ClientProjectileManager : TSingletonBaseManager<ClientProjectileManager>
    {
        public SortedDictionary<uint, Projectile> ProjectileDict = new SortedDictionary<uint, Projectile>();
        public Transform Root;

        public void Init(Transform root)
        {
            Root = root;
            Clear();
        }

        public void Clear()
        {
            foreach (KeyValuePair<uint, Projectile> kv in ProjectileDict)
            {
                kv.Value.PoolRecycle();
            }

            ProjectileDict.Clear();
        }

        public void EmitProjectile(ProjectileInfo projectileInfo)
        {
            Transform dummyPos = ClientBattleManager.Instance.FindMechaComponent(projectileInfo.ParentExecuteInfo.MechaComponentInfo.GUID).DummyPosDict[projectileInfo.ProjectileConfig.DummyPos];
            ShootProjectile(projectileInfo, dummyPos.position, dummyPos.forward, dummyPos);
        }

        private Projectile ShootProjectile(ProjectileInfo projectileInfo, Vector3 from, Vector3 dir, Transform dummyPos)
        {
            Projectile projectile = GameObjectPoolManager.Instance.ProjectileDict[projectileInfo.ProjectileType].AllocateGameObject<Projectile>(Root);
            projectile.transform.position = from;
            projectile.transform.LookAt(from + dir);
            projectile.Initialize(projectileInfo);
            projectile.Launch(dummyPos);
            return projectile;
        }
    }
}
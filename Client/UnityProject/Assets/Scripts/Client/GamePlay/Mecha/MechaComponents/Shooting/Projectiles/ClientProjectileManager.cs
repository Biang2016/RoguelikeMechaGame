using System.Collections.Generic;
using BiangStudio.Singleton;
using GameCore;
using GameCore.AbilityDataDriven;
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
            MechaComponent mc = ClientBattleManager.Instance.FindMechaComponent(projectileInfo.ParentExecuteInfo.MechaComponentInfo.GUID);
            if (mc != null && !mc.MechaInfo.IsDead)
            {
                Transform dummyPos = mc.DummyPosDict[projectileInfo.ProjectileConfig.DummyPos];
                ShootProjectile(projectileInfo, dummyPos.position, dummyPos.forward, dummyPos);
            }
        }

        private Projectile ShootProjectile(ProjectileInfo projectileInfo, Vector3 from, Vector3 dir, Transform dummyPos)
        {
            Projectile projectile = GameObjectPoolManager.Instance.ProjectileDict[projectileInfo.ProjectileType].AllocateGameObject<Projectile>(Root);
            if (projectileInfo.ProjectileConfig.CollisionFilter == ENUM_MultipleTargetTeam.UNIT_TARGET_TEAM_BOTH)
            {
                projectile.gameObject.layer = LayerManager.Instance.Layer_Projectile_Both;
            }
            else if (projectileInfo.ProjectileConfig.CollisionFilter == ENUM_MultipleTargetTeam.UNIT_TARGET_TEAM_ENEMY)
            {
                if (projectileInfo.MechaCamp == MechaCamp.Player || projectileInfo.MechaCamp == MechaCamp.Friend)
                {
                    projectile.gameObject.layer = LayerManager.Instance.Layer_Projectile_EnemyOnly;
                }

                if (projectileInfo.MechaCamp == MechaCamp.Enemy)
                {
                    projectile.gameObject.layer = LayerManager.Instance.Layer_Projectile_PlayerOnly;
                }
            }

            projectile.transform.position = from;
            projectile.transform.LookAt(from + dir);
            projectile.Initialize(projectileInfo);
            projectile.Launch(dummyPos);
            return projectile;
        }
    }
}
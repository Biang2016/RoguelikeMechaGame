﻿using System.Collections;
using BiangStudio.Singleton;
using GameCore;
using UnityEngine;

namespace Client
{
    public class ProjectileManager : TSingletonBaseManager<ProjectileManager>
    {
        public Transform Root;

        public void Init(Transform root)
        {
            Root = root;
        }

        public Projectile ShootProjectile(ProjectileInfo projectileInfo, Vector3 from, Vector3 dir)
        {
            Projectile projectile = GameObjectPoolManager.Instance.ProjectileDict[projectileInfo.ProjectileType].AllocateGameObject<Projectile>(Root);
            projectile.transform.position = from;
            projectile.transform.LookAt(from + dir);
            projectile.Initialize(projectileInfo);
            ClientLevelManager.Instance.ClientLevel.StartCoroutine(Co_ShootProjectile(projectile));
            return projectile;
        }

        IEnumerator Co_ShootProjectile(Projectile projectile)
        {
            yield return null;
            projectile.Play();
        }
    }
}
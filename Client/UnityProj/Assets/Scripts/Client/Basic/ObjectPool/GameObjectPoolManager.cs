using System;
using System.Collections.Generic;
using BiangStudio.GamePlay;
using BiangStudio.ObjectPool;
using BiangStudio.Singleton;
using GameCore;
using UnityEngine;

namespace Client
{
    public class GameObjectPoolManager : TSingletonBaseManager<GameObjectPoolManager>
    {
        public enum PrefabNames
        {
            Mecha,
            BackpackItem,
            BackpackGrid,
            BackpackItemGridHitBox,
            MechaComponentDropSprite,
            HUDSlider,
        }

        public Dictionary<PrefabNames, int> PoolConfigs = new Dictionary<PrefabNames, int>
        {
            {PrefabNames.Mecha, 4},
            {PrefabNames.BackpackItem, 20},
            {PrefabNames.BackpackGrid, 50},
            {PrefabNames.BackpackItemGridHitBox, 50},
            {PrefabNames.MechaComponentDropSprite, 5},
            {PrefabNames.HUDSlider, 3},
        };

        public Dictionary<PrefabNames, int> PoolWarmUpDict = new Dictionary<PrefabNames, int>
        {
        };

        public Dictionary<PrefabNames, GameObjectPool> PoolDict = new Dictionary<PrefabNames, GameObjectPool>();
        public Dictionary<MechaComponentType, GameObjectPool> MechaComponentPoolDict = new Dictionary<MechaComponentType, GameObjectPool>();
        public Dictionary<ProjectileType, GameObjectPool> ProjectileDict = new Dictionary<ProjectileType, GameObjectPool>();
        public Dictionary<ProjectileType, GameObjectPool> ProjectileHitDict = new Dictionary<ProjectileType, GameObjectPool>();
        public Dictionary<ProjectileType, GameObjectPool> ProjectileFlashDict = new Dictionary<ProjectileType, GameObjectPool>();
        public Dictionary<FX_Type, GameObjectPool> FXDict = new Dictionary<FX_Type, GameObjectPool>();

        private Transform Root;

        public void Init(Transform root)
        {
            Root = root;
        }

        public override void Awake()
        {
            foreach (KeyValuePair<PrefabNames, int> kv in PoolConfigs)
            {
                string prefabName = kv.Key.ToString();
                GameObject go_Prefab = PrefabManager.Instance.GetPrefab(prefabName);
                if (go_Prefab)
                {
                    GameObject go = new GameObject("Pool_" + prefabName);
                    GameObjectPool pool = go.AddComponent<GameObjectPool>();
                    pool.transform.SetParent(Root);
                    PoolDict.Add(kv.Key, pool);
                    PoolObject po = go_Prefab.GetComponent<PoolObject>();
                    pool.Initiate(po, kv.Value);
                }
            }

            foreach (string s in Enum.GetNames(typeof(MechaComponentType)))
            {
                string prefabName = "MechaComponent_" + s;
                MechaComponentType mechaComponentType = (MechaComponentType) Enum.Parse(typeof(MechaComponentType), s);
                GameObject go_Prefab = PrefabManager.Instance.GetPrefab(prefabName);
                if (go_Prefab)
                {
                    GameObject go = new GameObject("Pool_" + prefabName);
                    GameObjectPool pool = go.AddComponent<GameObjectPool>();
                    pool.transform.SetParent(Root);
                    MechaComponentPoolDict.Add(mechaComponentType, pool);
                    PoolObject po = go_Prefab.GetComponent<PoolObject>();
                    pool.Initiate(po, 20);
                }
            }

            foreach (string s in Enum.GetNames(typeof(ProjectileType)))
            {
                string prefabName = s;
                ProjectileType projectileType = (ProjectileType) Enum.Parse(typeof(ProjectileType), s);
                GameObject go_Prefab = PrefabManager.Instance.GetPrefab(prefabName);
                if (go_Prefab)
                {
                    GameObject go = new GameObject("Pool_" + prefabName);
                    GameObjectPool pool = go.AddComponent<GameObjectPool>();
                    pool.transform.SetParent(Root);
                    ProjectileDict.Add(projectileType, pool);
                    PoolObject po = go_Prefab.GetComponent<PoolObject>();
                    pool.Initiate(po, 20);
                }
            }

            foreach (string s in Enum.GetNames(typeof(ProjectileType)))
            {
                string prefabName = s.Replace("Projectile_", "Hit_");
                ProjectileType projectileType = (ProjectileType) Enum.Parse(typeof(ProjectileType), s);

                GameObject go_Prefab = PrefabManager.Instance.GetPrefab(prefabName);
                if (go_Prefab)
                {
                    GameObject go = new GameObject("Pool_" + prefabName);
                    GameObjectPool pool = go.AddComponent<GameObjectPool>();
                    pool.transform.SetParent(Root);
                    ProjectileHitDict.Add(projectileType, pool);
                    PoolObject po = go_Prefab.GetComponent<PoolObject>();
                    pool.Initiate(po, 20);
                }
            }

            foreach (string s in Enum.GetNames(typeof(ProjectileType)))
            {
                string prefabName = s.Replace("Projectile_", "Flash_");
                ProjectileType projectileType = (ProjectileType) Enum.Parse(typeof(ProjectileType), s);

                GameObject go_Prefab = PrefabManager.Instance.GetPrefab(prefabName);
                if (go_Prefab)
                {
                    GameObject go = new GameObject("Pool_" + prefabName);
                    GameObjectPool pool = go.AddComponent<GameObjectPool>();
                    pool.transform.SetParent(Root);
                    ProjectileFlashDict.Add(projectileType, pool);
                    PoolObject po = go_Prefab.GetComponent<PoolObject>();
                    pool.Initiate(po, 20);
                }
            }

            foreach (string s in Enum.GetNames(typeof(FX_Type)))
            {
                FX_Type fx_Type = (FX_Type) Enum.Parse(typeof(FX_Type), s);
                GameObject go_Prefab = PrefabManager.Instance.GetPrefab(s);
                if (go_Prefab)
                {
                    GameObject go = new GameObject("Pool_" + s);
                    GameObjectPool pool = go.AddComponent<GameObjectPool>();
                    pool.transform.SetParent(Root);
                    FXDict.Add(fx_Type, pool);
                    PoolObject po = go_Prefab.GetComponent<PoolObject>();
                    pool.Initiate(po, 20);
                }
            }
        }

        public void OptimizeAllGameObjectPools()
        {
            foreach (KeyValuePair<PrefabNames, GameObjectPool> kv in PoolDict)
            {
                kv.Value.OptimizePool();
            }
        }
    }
}
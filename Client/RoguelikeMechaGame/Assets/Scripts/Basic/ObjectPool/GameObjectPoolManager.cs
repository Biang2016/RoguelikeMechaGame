using System;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPoolManager : MonoSingleton<GameObjectPoolManager>
{
    private GameObjectPoolManager()
    {
    }

    public enum PrefabNames
    {
        Mecha,
        MechaComponent_Core,
        MechaComponent_Block,
        MechaComponent_Armor,
        MechaComponent_Gun,
        MechaComponent_Sword,
        MechaComponent_Engine,
        MechaComponent_Missile,
        BagItem,
        BagGrid,
        BagItemGridHitBox,
    }

    public Dictionary<PrefabNames, int> PoolConfigs = new Dictionary<PrefabNames, int>
    {
        {PrefabNames.Mecha, 4},
        {PrefabNames.MechaComponent_Core, 4},
        {PrefabNames.MechaComponent_Block, 4},
        {PrefabNames.MechaComponent_Armor, 4},
        {PrefabNames.MechaComponent_Gun, 4},
        {PrefabNames.MechaComponent_Sword, 4},
        {PrefabNames.MechaComponent_Engine, 4},
        {PrefabNames.MechaComponent_Missile, 4},
        {PrefabNames.BagItem, 4},
        {PrefabNames.BagGrid, 50},
        {PrefabNames.BagItemGridHitBox, 50},
    };

    public Dictionary<PrefabNames, int> PoolWarmUpDict = new Dictionary<PrefabNames, int>
    {
    };

    public Dictionary<PrefabNames, GameObjectPool> PoolDict = new Dictionary<PrefabNames, GameObjectPool>();
    public Dictionary<ProjectileType, GameObjectPool> ProjectileDict = new Dictionary<ProjectileType, GameObjectPool>();
    public Dictionary<ProjectileType, GameObjectPool> ProjectileHitDict = new Dictionary<ProjectileType, GameObjectPool>();
    public Dictionary<ProjectileType, GameObjectPool> ProjectileFlashDict = new Dictionary<ProjectileType, GameObjectPool>();
    public Dictionary<FX_Type, GameObjectPool> FXDict = new Dictionary<FX_Type, GameObjectPool>();

    void Awake()
    {
        PrefabManager.Instance.LoadPrefabs_Editor();

        foreach (KeyValuePair<PrefabNames, int> kv in PoolConfigs)
        {
            string prefabName = kv.Key.ToString();
            GameObject go = new GameObject("Pool_" + prefabName);
            GameObjectPool pool = go.AddComponent<GameObjectPool>();
            PoolDict.Add(kv.Key, pool);
            GameObject go_Prefab = PrefabManager.Instance.GetPrefab(prefabName);
            PoolObject po = go_Prefab.GetComponent<PoolObject>();
            pool.Initiate(po, kv.Value);
            pool.transform.SetParent(transform);
        }

        foreach (string s in Enum.GetNames(typeof(ProjectileType)))
        {
            string prefabName = s;
            ProjectileType projectileType = (ProjectileType) Enum.Parse(typeof(ProjectileType), s);
            GameObject go = new GameObject("Pool_" + prefabName);
            GameObjectPool pool = go.AddComponent<GameObjectPool>();
            ProjectileDict.Add(projectileType, pool);
            GameObject go_Prefab = PrefabManager.Instance.GetPrefab(prefabName);
            PoolObject po = go_Prefab.GetComponent<PoolObject>();
            pool.Initiate(po, 20);
            pool.transform.SetParent(transform);
        }

        foreach (string s in Enum.GetNames(typeof(ProjectileType)))
        {
            string prefabName = s.Replace("Projectile_", "Hit_");
            ProjectileType projectileType = (ProjectileType) Enum.Parse(typeof(ProjectileType), s);
            GameObject go = new GameObject("Pool_" + prefabName);
            GameObjectPool pool = go.AddComponent<GameObjectPool>();
            GameObject go_Prefab = PrefabManager.Instance.GetPrefab(prefabName);
            if (go_Prefab)
            {
                ProjectileHitDict.Add(projectileType, pool);
                PoolObject po = go_Prefab.GetComponent<PoolObject>();
                pool.Initiate(po, 20);
                pool.transform.SetParent(transform);
            }
        }

        foreach (string s in Enum.GetNames(typeof(ProjectileType)))
        {
            string prefabName = s.Replace("Projectile_", "Flash_");
            ProjectileType projectileType = (ProjectileType) Enum.Parse(typeof(ProjectileType), s);
            GameObject go = new GameObject("Pool_" + prefabName);
            GameObjectPool pool = go.AddComponent<GameObjectPool>();
            GameObject go_Prefab = PrefabManager.Instance.GetPrefab(prefabName);
            if (go_Prefab)
            {
                ProjectileFlashDict.Add(projectileType, pool);
                PoolObject po = go_Prefab.GetComponent<PoolObject>();
                pool.Initiate(po, 20);
                pool.transform.SetParent(transform);
            }
        }
        foreach (string s in Enum.GetNames(typeof(FX_Type)))
        {
            FX_Type fx_Type = (FX_Type) Enum.Parse(typeof(FX_Type), s);
            GameObject go = new GameObject("Pool_" + s);
            GameObjectPool pool = go.AddComponent<GameObjectPool>();
            GameObject go_Prefab = PrefabManager.Instance.GetPrefab(s);
            if (go_Prefab)
            {
                FXDict.Add(fx_Type, pool);
                PoolObject po = go_Prefab.GetComponent<PoolObject>();
                pool.Initiate(po, 20);
                pool.transform.SetParent(transform);
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
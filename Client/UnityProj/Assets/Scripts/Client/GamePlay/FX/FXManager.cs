﻿using BiangStudio.Singleton;
using UnityEngine;

namespace Client
{
    public class FXManager : TSingletonBaseManager<FXManager>
    {
        private Transform Root;

        public void Init(Transform root)
        {
            Root = root;
        }

        public FX PlayFX(FX_Type fx_Type, Vector3 from)
        {
            FX fx = GameObjectPoolManager.Instance.FXDict[fx_Type].AllocateGameObject<FX>(Root);
            fx.transform.position = from;
            fx.Play();
            return fx;
        }
    }

    public enum FX_Type
    {
        FX_BlockDamageHit = 1,
        FX_BlockDamagedLightening = 2,
        FX_BlockExplode = 3,
    }
}
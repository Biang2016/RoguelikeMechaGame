using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoSingleton<FXManager>
{
    public FX PlayFX(FX_Type fx_Type, Vector3 from)
    {
        FX fx = GameObjectPoolManager.Instance.FXDict[fx_Type].AllocateGameObject<FX>(transform);
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
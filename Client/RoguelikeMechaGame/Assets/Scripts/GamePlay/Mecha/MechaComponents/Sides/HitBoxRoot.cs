using UnityEngine;

public class HitBoxRoot : MonoBehaviour
{
    internal MechaComponentBase MechaComponentBase;

    void Awake()
    {
        MechaComponentBase = GetComponentInParent<MechaComponentBase>();
    }

    public void OnHit(int damage)
    {
        MechaComponentBase.Damage(damage);
    }
}
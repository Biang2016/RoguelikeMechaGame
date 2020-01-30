using UnityEngine;

public class HitBoxRoot : MonoBehaviour
{
    [SerializeField] private MechaComponentBase MechaComponentBase;

    public void OnHit(int damage)
    {
        MechaComponentBase.Damage(damage);
    }
}
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    private GameObjectPool Pool { get; set; }

    internal bool IsRecycled = false;

    public void SetObjectPool(GameObjectPool pool)
    {
        Pool = pool;
    }

    public virtual void PoolRecycle()
    {
        Pool.RecycleGameObject(this);
        IsRecycled = true;
    }
}
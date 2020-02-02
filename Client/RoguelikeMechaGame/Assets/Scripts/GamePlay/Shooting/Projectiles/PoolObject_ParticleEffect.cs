using UnityEngine;
using System.Collections;

public class PoolObject_ParticleEffect : PoolObject
{
    private int usedTimes = 0;

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        usedTimes++;
    }

    public virtual void PoolRecycle(float delay)
    {
        StartCoroutine(Co_PoolRecycle(delay, usedTimes));
    }

    IEnumerator Co_PoolRecycle(float delay, int timeMark)
    {
        yield return new WaitForSeconds(delay);
        if (!IsRecycled && usedTimes == timeMark)
        {
            PoolRecycle();
        }
    }
}
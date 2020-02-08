using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class GridSnapper : MonoBehaviour
{
    void LateUpdate()
    {
        Vector3 localPosition = transform.localPosition;
        GridPos gp = GridPos.GetGridPosByLocalTrans(transform, GameManager.GridSize);
        transform.localPosition = new Vector3(gp.x, localPosition.y, gp.z);
    }
}
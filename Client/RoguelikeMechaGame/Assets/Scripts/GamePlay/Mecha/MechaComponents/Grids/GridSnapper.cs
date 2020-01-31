using UnityEngine;

[ExecuteInEditMode]
public class GridSnapper : MonoBehaviour
{
    void LateUpdate()
    {
        Vector3 localPosition = transform.localPosition;
        float x = Mathf.Floor((localPosition.x) / GameManager.GridSize) * GameManager.GridSize;
        float y = Mathf.Floor((localPosition.y) / GameManager.GridSize) * GameManager.GridSize;
        float z = Mathf.Floor((localPosition.z) / GameManager.GridSize) * GameManager.GridSize;
        localPosition = new Vector3(x, y, z);
        transform.localPosition = localPosition;
    }
}
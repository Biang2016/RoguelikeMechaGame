using UnityEngine;

[ExecuteInEditMode]
public class GridSnapper : MonoBehaviour
{
    void LateUpdate()
    {
        float x = Mathf.Floor(transform.localPosition.x / GameManager.GridSize) * GameManager.GridSize;
        float y = Mathf.Floor(transform.localPosition.y / GameManager.GridSize) * GameManager.GridSize;
        float z = Mathf.Floor(transform.localPosition.z / GameManager.GridSize) * GameManager.GridSize;
        transform.localPosition = new Vector3(x, y, z);
    }
}
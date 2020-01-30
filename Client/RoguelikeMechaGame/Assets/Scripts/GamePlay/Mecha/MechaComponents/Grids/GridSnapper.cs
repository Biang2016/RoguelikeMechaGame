using UnityEngine;

[ExecuteInEditMode]
public class GridSnapper : MonoBehaviour
{
    internal Transform Offset;

    void LateUpdate()
    {
        float x_delta = 0;
        float y_delta = 0;
        float z_delta = 0;
        if (Offset)
        {
            x_delta = Offset.localPosition.x % GameManager.GridSize;
            y_delta = Offset.localPosition.y % GameManager.GridSize;
            z_delta = Offset.localPosition.z % GameManager.GridSize;
        }

        float x = Mathf.Floor((transform.localPosition.x - x_delta) / GameManager.GridSize) * GameManager.GridSize + x_delta;
        float y = Mathf.Floor((transform.localPosition.y - y_delta) / GameManager.GridSize) * GameManager.GridSize + y_delta;
        float z = Mathf.Floor((transform.localPosition.z - z_delta) / GameManager.GridSize) * GameManager.GridSize + z_delta;
        transform.localPosition = new Vector3(x, y, z);
    }
}
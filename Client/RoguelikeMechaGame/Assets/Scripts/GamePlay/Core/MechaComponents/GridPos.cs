using UnityEngine;

public struct GridPos
{
    public int x;
    public int y;
    public int z;
    public Rotation rotation;

    public GridPos(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        rotation = Rotation.None;
    }

    public GridPos(int x, int y, int z, Rotation rotation)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.rotation = rotation;
    }

    public static GridPos GetGridPosByLocalTrans(Transform transform)
    {
        int x = Mathf.FloorToInt(transform.localPosition.x / GameManager.GridSize) * GameManager.GridSize;
        int y = Mathf.FloorToInt(transform.localPosition.y / GameManager.GridSize) * GameManager.GridSize;
        int z = Mathf.FloorToInt(transform.localPosition.z / GameManager.GridSize) * GameManager.GridSize;
        int rotY = Mathf.RoundToInt(transform.localRotation.eulerAngles.y / 90f) % 4;
        return new GridPos(x, y, z, (Rotation) rotY);
    }

    public static void ApplyGridPosToLocalTrans(GridPos gridPos, Transform transform)
    {
        float x = gridPos.x * GameManager.GridSize;
        float y = gridPos.y * GameManager.GridSize;
        float z = gridPos.z * GameManager.GridSize;
        float rotY = (int) gridPos.rotation * 90f;
        transform.localPosition = new Vector3(x, y, z);
        transform.Rotate(0, rotY, 0);
    }

    public enum Rotation
    {
        None = 0,
        Clockwise_90 = 1,
        Clockwise_180 = 2,
        Clockwise_270 = 3,
    }
}
using UnityEngine;

public struct GridPos
{
    public int x;
    public int y;
    public int z;
    public Orientation orientation;

    public GridPos(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        orientation = Orientation.Up;
    }

    public GridPos(int x, int y, int z, Orientation orientation)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.orientation = orientation;
    }

    public static GridPos GetGridPosByLocalTrans(Transform transform, int gridSize)
    {
        int x = Mathf.FloorToInt(transform.localPosition.x / gridSize) * gridSize;
        int y = Mathf.FloorToInt(transform.localPosition.y / gridSize) * gridSize;
        int z = Mathf.FloorToInt(transform.localPosition.z / gridSize) * gridSize;
        int rotY = Mathf.RoundToInt(transform.localRotation.eulerAngles.y / 90f) % 4;
        return new GridPos(x, y, z, (Orientation) rotY);
    }

    public static void ApplyGridPosToLocalTrans(GridPos gridPos, Transform transform, int gridSize)
    {
        float x = gridPos.x * gridSize;
        float y = gridPos.y * gridSize;
        float z = gridPos.z * gridSize;
        float rotY = (int) gridPos.orientation * 90f;
        transform.localPosition = new Vector3(x, y, z);
        transform.Rotate(0, rotY, 0);
    }

    public enum Orientation
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3,
    }
}
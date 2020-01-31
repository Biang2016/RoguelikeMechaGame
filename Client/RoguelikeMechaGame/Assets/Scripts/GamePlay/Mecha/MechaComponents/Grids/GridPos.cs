using UnityEngine;

public struct GridPos
{
    public int x;
    public int z;
    public Orientation orientation;

    public GridPos(int x, int z)
    {
        this.x = x;
        this.z = z;
        orientation = Orientation.Up;
    }

    public GridPos(int x, int z, Orientation orientation)
    {
        this.x = x;
        this.z = z;
        this.orientation = orientation;
    }

    public static GridPos GetGridPosByLocalTrans(Transform transform, int gridSize)
    {
        int x = Mathf.FloorToInt(transform.localPosition.x / gridSize) * gridSize;
        int z = Mathf.FloorToInt(transform.localPosition.z / gridSize) * gridSize;
        int rotY = Mathf.RoundToInt(transform.localRotation.eulerAngles.y / 90f) % 4;
        return new GridPos(x, z, (Orientation) rotY);
    }

    public static void ApplyGridPosToLocalTrans(GridPos gridPos, Transform transform, int gridSize)
    {
        float x = gridPos.x * gridSize;
        float z = gridPos.z * gridSize;
        float rotY = (int) gridPos.orientation * 90f;
        transform.localPosition = new Vector3(x, transform.localPosition.y, z);
        transform.rotation = Quaternion.Euler(0, rotY, 0);
    }

    public static GridPos GetGridPosByMousePos(Transform parentTransform, Vector3 planeNormal, int gridSize)
    {
        Ray ray = GameManager.Instance.MainCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 intersect = ClientUtil.GetIntersectWithLineAndPlane(ray.origin, ray.direction, planeNormal, parentTransform.position);
        Vector3 diff = intersect - parentTransform.position + Vector3.one * gridSize / 2f;
        int x = Mathf.FloorToInt(diff.x / gridSize) * gridSize;
        int z = Mathf.FloorToInt(diff.z / gridSize) * gridSize;
        return new GridPos(x, z, Orientation.Up);
    }

    public static GridPos RotateGridPos(GridPos oriGP, Orientation orientation)
    {
        switch (orientation)
        {
            case Orientation.Up:
            {
                return oriGP;
            }
            case Orientation.Right:
            {
                return new GridPos(oriGP.z, -oriGP.x, Orientation.Up);
            }
            case Orientation.Down:
            {
                return new GridPos(-oriGP.x, -oriGP.z, Orientation.Up);
            }
            case Orientation.Left:
            {
                return new GridPos(-oriGP.z, oriGP.x, Orientation.Up);
            }
        }

        return new GridPos(0, 00, Orientation.Up);
    }

    public enum Orientation
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3,
    }
}
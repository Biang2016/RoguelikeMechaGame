using System.Collections.Generic;
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

    public static GridPos GetGridPosByPoint(Vector3 position, int gridSize)
    {
        int x = Mathf.FloorToInt(position.x / gridSize) * gridSize;
        int z = Mathf.FloorToInt(position.z / gridSize) * gridSize;
        return new GridPos(x, z, Orientation.Up);
    }

    public static void ApplyGridPosToLocalTrans(GridPos gridPos, Transform transform, int gridSize)
    {
        float x = gridPos.x * gridSize;
        float z = gridPos.z * gridSize;
        float rotY = (int) gridPos.orientation * 90f;
        transform.localPosition = new Vector3(x, transform.localPosition.y, z);
        transform.localRotation = Quaternion.Euler(0, rotY, 0);
    }

    public static GridPos GetGridPosByMousePos(Transform parentTransform, Vector3 planeNormal, int gridSize)
    {
        Ray ray = GameManager.Instance.MainCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 intersect = ClientUtils.GetIntersectWithLineAndPlane(ray.origin, ray.direction, planeNormal, parentTransform.position);

        Vector3 rot_intersect = parentTransform.InverseTransformPoint(intersect);
        GridPos local_GP = GetGridPosByPoint(rot_intersect + Vector3.one * gridSize / 2f, 1);

        int x = Mathf.FloorToInt(local_GP.x / gridSize) * gridSize;
        int z = Mathf.FloorToInt(local_GP.z / gridSize) * gridSize;
        return new GridPos(x, z, Orientation.Up);
    }

    public static Orientation RotateOrientationClockwise90(Orientation orientation)
    {
        return (Orientation) (((int) orientation + 1) % 4);
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

        return new GridPos(0, 0, Orientation.Up);
    }

    public static List<GridPos> TransformOccupiedPositions(GridPos localGridPos, List<GridPos> ori_occupiedPositions)
    {
        List<GridPos> resGP = new List<GridPos>();

        foreach (GridPos oriGP in ori_occupiedPositions)
        {
            GridPos temp_rot = RotateGridPos(oriGP, localGridPos.orientation);
            GridPos final = temp_rot + localGridPos;
            final.orientation = oriGP.orientation;
            resGP.Add(final);
        }

        return resGP;
    }

    public bool Equals(GridPos gp)
    {
        return gp.x == x && gp.z == z && gp.orientation == orientation;
    }

    public static GridPos operator -(GridPos a, GridPos b)
    {
        return new GridPos(a.x - b.x, a.z - b.z);
    }

    public static GridPos operator +(GridPos a, GridPos b)
    {
        return new GridPos(a.x + b.x, a.z + b.z);
    }

    public override string ToString()
    {
        return $"({x},{z},{orientation})";
    }

    public enum Orientation
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3,
    }
}
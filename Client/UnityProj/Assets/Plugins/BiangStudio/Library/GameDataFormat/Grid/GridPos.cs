using System;
using System.Collections.Generic;
using UnityEngine;

namespace BiangStudio.GameDataFormat.Grid
{
    [Serializable]
    public struct GridPosR
    {
        public int x;
        public int z;
        public Orientation orientation;

        private static readonly GridPos zeroGPR = new GridPosR(0, 0, Orientation.Up);
        public static GridPosR Zero => zeroGPR;

        public GridPosR(int x, int z)
        {
            this.x = x;
            this.z = z;
            orientation = Orientation.Up;
        }

        public GridPosR(int x, int z, Orientation orientation)
        {
            this.x = x;
            this.z = z;
            this.orientation = orientation;
        }

        public static GridPosR GetGridPosByLocalTrans(Transform transform, int gridSize)
        {
            int x = Mathf.FloorToInt(transform.localPosition.x / gridSize) * gridSize;
            int z = Mathf.FloorToInt(transform.localPosition.z / gridSize) * gridSize;
            int rotY = Mathf.RoundToInt(transform.localRotation.eulerAngles.y / 90f) % 4;
            return new GridPosR(x, z, (Orientation) rotY);
        }

        public static GridPos GetGridPosByTrans(Transform transform, int gridSize)
        {
            int x = Mathf.FloorToInt(transform.position.x / gridSize) * gridSize;
            int z = Mathf.FloorToInt(transform.position.z / gridSize) * gridSize;
            int rotY = Mathf.RoundToInt(transform.rotation.eulerAngles.y / 90f) % 4;
            return new GridPosR(x, z, (Orientation) rotY);
        }

        public static GridPos GetGridPosByPoint(Vector3 position, int gridSize)
        {
            int x = Mathf.FloorToInt(position.x / gridSize) * gridSize;
            int z = Mathf.FloorToInt(position.z / gridSize) * gridSize;
            return new GridPosR(x, z, Orientation.Up);
        }

        public static void ApplyGridPosToLocalTrans(GridPosR gridPos, Transform transform, int gridSize)
        {
            float x = gridPos.x * gridSize;
            float z = gridPos.z * gridSize;
            float rotY = (int) gridPos.orientation * 90f;
            transform.localPosition = new Vector3(x, transform.localPosition.y, z);
            transform.localRotation = Quaternion.Euler(0, rotY, 0);
        }

        public static Orientation RotateOrientationClockwise90(Orientation orientation)
        {
            return (Orientation) (((int) orientation + 1) % 4);
        }

        public static List<GridPos> TransformOccupiedPositions(GridPosR localGridPos, List<GridPos> ori_OccupiedPositions)
        {
            List<GridPos> resGP = new List<GridPos>();

            foreach (GridPos oriGP in ori_OccupiedPositions)
            {
                GridPos temp_rot = GridPos.RotateGridPos(oriGP, localGridPos.orientation);
                GridPos final = temp_rot + (GridPos) localGridPos;
                resGP.Add(final);
            }

            return resGP;
        }

        public static GridPos TransformOccupiedPosition(GridPosR localGridPos, GridPos ori_OccupiedPosition)
        {
            GridPos temp_rot = GridPos.RotateGridPos(ori_OccupiedPosition, localGridPos.orientation);
            GridPos final = temp_rot + (GridPos) localGridPos;
            return final;
        }

        public bool Equals(GridPosR gp)
        {
            return gp.x == x && gp.z == z && gp.orientation == orientation;
        }

        public bool Equals(GridPos gp)
        {
            return gp.x == x && gp.z == z;
        }

        public static GridPos operator -(GridPosR a, GridPosR b)
        {
            return new GridPosR(a.x - b.x, a.z - b.z, a.orientation);
        }

        public static GridPosR operator +(GridPosR a, GridPosR b)
        {
            return new GridPosR(a.x + b.x, a.z + b.z, a.orientation);
        }

        public static GridPosR operator +(GridPosR a, GridPos b)
        {
            return new GridPosR(a.x + b.x, a.z + b.z, a.orientation);
        }

        public static GridPosR operator -(GridPosR a, GridPos b)
        {
            return new GridPosR(a.x - b.x, a.z - b.z, a.orientation);
        }

        public static GridPosR operator +(GridPos a, GridPosR b)
        {
            return new GridPosR(a.x + b.x, a.z + b.z, b.orientation);
        }

        public static GridPosR operator *(GridPosR a, int b)
        {
            return new GridPosR(a.x * b, a.z * b, a.orientation);
        }

        public static GridPosR operator *(int b, GridPosR a)
        {
            return new GridPosR(a.x * b, a.z * b, a.orientation);
        }

        public override string ToString()
        {
            return $"({x},{z},{orientation})";
        }

        public string ToShortString()
        {
            return $"({x},{z})";
        }

        public enum Orientation
        {
            Up = 0,
            Right = 1,
            Down = 2,
            Left = 3,
        }
    }

    [Serializable]
    public struct GridPos
    {
        public int x;
        public int z;

        private static readonly GridPos zeroGP = new GridPos(0, 0);
        public static GridPos Zero => zeroGP;

        public GridPos(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        public static GridPos GetGridPosByLocalTransXZ(Transform transform, int gridSize)
        {
            return GetGridPosByPointXZ(transform.localPosition, gridSize);
        }

        public static GridPos GetGridPosByTransXZ(Transform transform, int gridSize)
        {
            return GetGridPosByPointXZ(transform.position, gridSize);
        }

        public static GridPos GetGridPosByPointXZ(Vector3 position, int gridSize)
        {
            int x = Mathf.RoundToInt(position.x / gridSize);
            int z = Mathf.RoundToInt(position.z / gridSize);
            return new GridPos(x, z);
        }

        public static GridPos GetGridPosByPointXY(Vector3 position, int gridSize)
        {
            int x = Mathf.RoundToInt(position.x / gridSize);
            int y = Mathf.RoundToInt(position.y / gridSize);
            return new GridPos(x, y);
        }

        public static void ApplyGridPosToLocalTransXZ(GridPos gridPos, Transform transform, int gridSize)
        {
            float x = gridPos.x * gridSize;
            float z = gridPos.z * gridSize;
            transform.localPosition = new Vector3(x, transform.localPosition.y, z);
        }

        public static GridPos RotateGridPos(GridPos oriGP, GridPosR.Orientation orientation)
        {
            switch (orientation)
            {
                case GridPosR.Orientation.Up:
                {
                    return oriGP;
                }
                case GridPosR.Orientation.Right:
                {
                    return new GridPos(oriGP.z, -oriGP.x);
                }
                case GridPosR.Orientation.Down:
                {
                    return new GridPos(-oriGP.x, -oriGP.z);
                }
                case GridPosR.Orientation.Left:
                {
                    return new GridPos(-oriGP.z, oriGP.x);
                }
            }

            return new GridPos(0, 0);
        }

        public static GridPos GetLocalGridPosByCenter(GridPosR center, GridPos gp_global)
        {
            GridPos diff = gp_global - (GridPos) center;
            GridPos localGP = RotateGridPos(diff, (GridPosR.Orientation) (4 - (int) center.orientation));
            return localGP;
        }

        public static List<GridPos> TransformOccupiedPositions(GridPos localGridPos, List<GridPos> ori_OccupiedPositions)
        {
            for (int i = 0; i < ori_OccupiedPositions.Count; i++)
            {
                ori_OccupiedPositions[i] += localGridPos;
            }

            return ori_OccupiedPositions;
        }

        public bool Equals(GridPos gp)
        {
            return gp.x == x && gp.z == z;
        }

        public bool Equals(GridPosR r)
        {
            return r.x == x && r.z == z;
        }

        public static GridPos operator -(GridPos a)
        {
            return new GridPos(-a.x, -a.z);
        }

        public static GridPos operator -(GridPos a, GridPos b)
        {
            return new GridPos(a.x - b.x, a.z - b.z);
        }

        public static GridPos operator +(GridPos a, GridPos b)
        {
            return new GridPos(a.x + b.x, a.z + b.z);
        }

        public static GridPos operator *(GridPos a, int b)
        {
            return new GridPos(a.x * b, a.z * b);
        }

        public static GridPos operator *(int b, GridPos a)
        {
            return new GridPos(a.x * b, a.z * b);
        }

        public static implicit operator GridPos(GridPosR r)
        {
            return new GridPos(r.x, r.z);
        }

        public static implicit operator GridPosR(GridPos gp)
        {
            return new GridPosR(gp.x, gp.z, GridPosR.Orientation.Up);
        }

        public override string ToString()
        {
            return $"({x},{z})";
        }
    }
}
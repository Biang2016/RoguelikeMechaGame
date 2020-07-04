using System.Collections.Generic;
using UnityEngine;

namespace BiangStudio.GameDataFormat.Grid
{
    public static class GridUtils
    {
        public static GridPos GetGridPosByMousePos(Transform parentTransform, Ray ray, Vector3 planeNormal, int gridSize)
        {
            Vector3 intersect = CommonUtils.GetIntersectWithLineAndPlane(ray.origin, ray.direction, planeNormal, parentTransform.position);

            Vector3 rot_intersect = parentTransform.InverseTransformPoint(intersect);
            GridPos local_GP = GridPos.GetGridPosByPoint(rot_intersect + Vector3.one * gridSize / 2f, 1);

            int x = Mathf.FloorToInt(local_GP.x / gridSize) * gridSize;
            int z = Mathf.FloorToInt(local_GP.z / gridSize) * gridSize;
            return new GridPos(x, z);
        }

        public static Vector3 GetIntersectWithLineAndPlane(Vector3 point, Vector3 direct, Vector3 planeNormal, Vector3 planePoint)
        {
            float d = Vector3.Dot(planePoint - point, planeNormal) / Vector3.Dot(direct.normalized, planeNormal);
            return d * direct.normalized + point;
        }

        public static string GridPositionListToString(this List<GridPos> gridPositions)
        {
            string res = "";
            foreach (GridPos gp in gridPositions)
            {
                res += gp + ", ";
            }

            return res;
        }

        public static GridRect GetBoundingRectFromListGridPos(this List<GridPos> gridPositions)
        {
            int X_min = 999;
            int X_max = -999;
            int Z_min = 999;
            int Z_max = -999;
            foreach (GridPos gp in gridPositions)
            {
                if (gp.x < X_min)
                {
                    X_min = gp.x;
                }

                if (gp.x > X_max)
                {
                    X_max = gp.x;
                }

                if (gp.z < Z_min)
                {
                    Z_min = gp.z;
                }

                if (gp.z > Z_max)
                {
                    Z_max = gp.z;
                }
            }

            return new GridRect(X_min, Z_min, X_max - X_min + 1, Z_max - Z_min + 1);
        }
    }
}
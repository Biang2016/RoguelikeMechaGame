using BiangStudio.GameDataFormat.Grid;
using GameCore;
using UnityEngine;

namespace Client
{
    [ExecuteInEditMode]
    public class GridSnapper : MonoBehaviour
    {
        void LateUpdate()
        {
            Vector3 localPosition = transform.localPosition;
            GridPos gp = GridPos.GetGridPosByLocalTransXZ(transform, ConfigManager.GridSize);
            transform.localPosition = new Vector3(gp.x, localPosition.y, gp.z);
            Vector3 eulerAngles = transform.localRotation.eulerAngles;
            float y = Mathf.RoundToInt(eulerAngles.y / 90) * 90;
            transform.localRotation = Quaternion.Euler(0, y, 0);
        }
    }
}
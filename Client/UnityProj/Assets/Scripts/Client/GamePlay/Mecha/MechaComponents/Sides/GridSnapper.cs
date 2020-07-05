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
            GridPos gp = GridPos.GetGridPosByLocalTrans(transform, ConfigManager.GridSize);
            transform.localPosition = new Vector3(gp.x, localPosition.y, gp.z);
        }
    }
}
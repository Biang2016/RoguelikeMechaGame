using BiangStudio.GameDataFormat.Grid;
using GameCore;
using UnityEngine;

namespace Client
{
    [ExecuteInEditMode]
    public class BackpackGridSnapper : MonoBehaviour
    {
        private RectTransform RectTransform;

        void LateUpdate()
        {
            if (!RectTransform) RectTransform = (RectTransform) transform;
            Vector2 localPosition = RectTransform.anchoredPosition;
            GridPos gp = GridPos.GetGridPosByPointXY(localPosition, ConfigManager.BackpackGridSize);
            Vector2 snappedPosition = new Vector2(gp.x * ConfigManager.BackpackGridSize, gp.z * ConfigManager.BackpackGridSize);
            RectTransform.anchoredPosition = snappedPosition;
        }
    }
}
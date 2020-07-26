using System.Collections.Generic;
using UnityEngine;
using BiangStudio.GameDataFormat.Grid;
using UnityEngine.UI;

namespace BiangStudio.GridBackpack
{
    [ExecuteInEditMode]
    public class BackpackItemDesignerHelper : MonoBehaviour
    {
        public Image Image;
        public GameObject BoxesRoot;

        public void ReadBackpackItemInfo(int backpackGridSize, out List<GridPos> occupiedGridPositions, out Sprite sprite)
        {
            BackpackItemGridHitBox[] boxes = BoxesRoot.GetComponentsInChildren<BackpackItemGridHitBox>();
            occupiedGridPositions = new List<GridPos>();
            foreach (BackpackItemGridHitBox box in boxes)
            {
                Vector2 localPosition = ((RectTransform)box.transform).anchoredPosition;
                GridPos gp = GridPos.GetGridPosByPointXY(localPosition, backpackGridSize);
                occupiedGridPositions.Add(gp);
            }

            sprite = Image.sprite;
        }
    }
}

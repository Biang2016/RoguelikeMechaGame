using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagItem : PoolObject
{
    [SerializeField] private Image Image;
    [SerializeField] private BagGridSnapper BagGridSnapper;

    void Awake()
    {
    }

    public void Initialize(MechaComponentInfo mci, List<GridPos> occupiedGridPositions)
    {
        int X_min = 999;
        int X_max = -1;
        int Z_min = 999;
        int Z_max = -1;
        foreach (GridPos gp in occupiedGridPositions)
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

        int width = X_max - X_min + 1;
        int height = Z_max - Z_min + 1;

        Image.sprite = BagManager.Instance.MechaComponentSpriteDict[mci.M_MechaComponentType];
        Image.rectTransform.sizeDelta = new Vector2(width * BagManager.Instance.BagItemGridSize, height * BagManager.Instance.BagItemGridSize);
    }
}
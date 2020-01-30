using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BagPanel : BaseUIForm
{
    [SerializeField] private GridLayoutGroup ItemContainerGridLayout;
    [SerializeField] private Transform GridContainer;
    [SerializeField] private Transform ItemContainer;

    private List<BagGrid> BagGrids = new List<BagGrid>();
    private List<BagItem> BagItems = new List<BagItem>();

    void Awake()
    {
        UIType.InitUIType(
            isClearStack: false,
            isESCClose: true,
            isClickElsewhereClose: false,
            uiForms_Type: UIFormTypes.Normal,
            uiForms_ShowMode: UIFormShowModes.Normal,
            uiForm_LucencyType: UIFormLucencyTypes.Penetrable);
        BagManager.Instance.BagItemGridSize = Mathf.RoundToInt(ItemContainerGridLayout.cellSize.x);

        for (int i = 0; i < 50; i++)
        {
            BagGrid big = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BagGrid].AllocateGameObject<BagGrid>(GridContainer);
            BagGrids.Add(big);
        }
    }

    void Update()
    {
    }

    void Reset()
    {
        foreach (BagGrid bg in BagGrids)
        {
            bg.PoolRecycle();
        }

        BagGrids.Clear();
        foreach (BagItem bi in BagItems)
        {
            bi.PoolRecycle();
        }

        BagItems.Clear();
    }

    public void AddItem(MechaComponentInfo mci, List<GridPos> occupiedGridPositions)
    {
        BagItem bi = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BagItem].AllocateGameObject<BagItem>(ItemContainer);
        bi.Initialize(mci, occupiedGridPositions);
        BagItems.Add(bi);
    }

    public void OnMouseEnterBag()
    {
        BagManager.Instance.IsMouseInsideBag = true;
    }

    public void OnMouseLeaveBag()
    {
        BagManager.Instance.IsMouseInsideBag = false;
    }
}
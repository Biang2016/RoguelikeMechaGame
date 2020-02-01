using System;
using UnityEngine;
using System.Collections.Generic;

public class BagManager : MonoSingleton<BagManager>
{
    internal int BagItemGridSize;

    public Dictionary<MechaComponentType, Sprite> MechaComponentSpriteDict = new Dictionary<MechaComponentType, Sprite>();
    public Dictionary<MechaComponentType, List<GridPos>> MechaComponentOccupiedGridPosDict = new Dictionary<MechaComponentType, List<GridPos>>();

    private BagPanel bagPanel;
    private List<MechaComponentInfo> mechaComponentInfosInBag = new List<MechaComponentInfo>();

    void Awake()
    {
    }

    void Start()
    {
        LoadAllBlockItemPics();
        LoadBlockOccupiedGridInfo();
        bagPanel = UIManager.Instance.ShowUIForms<BagPanel>();
        bagPanel.CloseUIForm();
        Initialize();
    }

    /// <summary>
    /// Load all prefabs to see which grids does a mecha component take
    /// </summary>
    private void LoadBlockOccupiedGridInfo()
    {
        MechaComponentOccupiedGridPosDict.Clear();
        List<MechaComponentBase> mcbs = new List<MechaComponentBase>();

        foreach (string s in Enum.GetNames(typeof(MechaComponentType)))
        {
            MechaComponentType mcType = (MechaComponentType) Enum.Parse(typeof(MechaComponentType), s);
            MechaComponentBase mcb = MechaComponentBase.BaseInitialize(new MechaComponentInfo(mcType, new GridPos(0, 0, GridPos.Orientation.Up)), null, null);
            mcbs.Add(mcb);
            MechaComponentOccupiedGridPosDict.Add(mcType, CloneVariantUtils.List(mcb.MechaComponentInfo.OccupiedGridPositions));
        }

        foreach (MechaComponentBase mcb in mcbs)
        {
            DestroyImmediate(mcb.gameObject);
        }
    }

    /// <summary>
    /// Load all 2D sprites of mecha components
    /// </summary>
    private void LoadAllBlockItemPics()
    {
        foreach (string s in Enum.GetNames(typeof(MechaComponentType)))
        {
            MechaComponentType mcType = (MechaComponentType) Enum.Parse(typeof(MechaComponentType), s);
            Sprite sprite = Resources.Load<Sprite>("BlockItemPics/" + s);
            MechaComponentSpriteDict.Add(mcType, sprite);
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            if (bagPanel.gameObject.activeInHierarchy)
            {
                bagPanel.CloseUIForm();
                GameManager.Instance.SetState(GameState.Fighting);
            }
            else
            {
                UIManager.Instance.ShowUIForms<BagPanel>();
                GameManager.Instance.SetState(GameState.Building);
            }
        }
    }

    private void Reset()
    {
        mechaComponentInfosInBag.Clear();
    }

    private void Initialize()
    {
        UnlockBagGridTo(40);
        AddMechaComponentToBag(new MechaComponentInfo(MechaComponentType.Block, new GridPos(1, 0, GridPos.Orientation.Down)));
        AddMechaComponentToBag(new MechaComponentInfo(MechaComponentType.Gun, new GridPos(1, 0, GridPos.Orientation.Right)));
        AddMechaComponentToBag(new MechaComponentInfo(MechaComponentType.Armor, new GridPos(3, 3, GridPos.Orientation.Right)));
        AddMechaComponentToBag(new MechaComponentInfo(MechaComponentType.Engine, new GridPos(-2, 3, GridPos.Orientation.Right)));
        AddMechaComponentToBag(new MechaComponentInfo(MechaComponentType.Sword, new GridPos(-2, 3, GridPos.Orientation.Right)));
        AddMechaComponentToBag(new MechaComponentInfo(MechaComponentType.Core, new GridPos(3, 3, GridPos.Orientation.Right)));
    }

    public bool AddMechaComponentToBag(MechaComponentInfo mci)
    {
        bool suc = bagPanel.TryAddItem(mci, true);
        if (suc)
        {
            mechaComponentInfosInBag.Add(mci);
        }

        return suc;
    }

    public void RemoveMechaComponentFromBag(BagItem bagItem,bool temporary)
    {
        bagPanel.RemoveItem(bagItem,temporary);
        mechaComponentInfosInBag.Remove(bagItem.MechaComponentInfo);
    }

    public void UnlockBagGridTo(int gridNumber)
    {
        bagPanel.UnlockBagGridTo(gridNumber);
    }
}
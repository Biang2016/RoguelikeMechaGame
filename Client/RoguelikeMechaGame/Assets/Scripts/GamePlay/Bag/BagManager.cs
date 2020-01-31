using System;
using UnityEngine;
using System.Collections.Generic;

public class BagManager : MonoSingleton<BagManager>
{
    private BagPanel BagPanel;

    internal int BagItemGridSize;

    void Awake()
    {
    }

    void Start()
    {
        RefreshBlockOccupiedGridInfo();
        LoadAllBlockItemPics();
        BagPanel = UIManager.Instance.ShowUIForms<BagPanel>();
        BagPanel.CloseUIForm();
        Initialize();
    }

    private Dictionary<MechaComponentType, List<GridPos>> MechaComponentOccupiedGridPosDict = new Dictionary<MechaComponentType, List<GridPos>>();
    public Dictionary<MechaComponentType, Sprite> MechaComponentSpriteDict = new Dictionary<MechaComponentType, Sprite>();

    private void RefreshBlockOccupiedGridInfo()
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
            if (BagPanel.gameObject.activeInHierarchy)
            {
                BagPanel.CloseUIForm();
                GameManager.Instance.SetState(GameState.Fighting);
            }
            else
            {
                UIManager.Instance.ShowUIForms<BagPanel>();
                GameManager.Instance.SetState(GameState.Building);
            }
        }
    }

    internal bool IsMouseInsideBag = false;

    private void Reset()
    {
        MechaComponentInfos.Clear();
    }

    public Transform BagContainer;
    private List<MechaComponentInfo> MechaComponentInfos = new List<MechaComponentInfo>();

    private void Initialize()
    {
        BagPanel.UnlockBagGridTo(40);
        AddMechaComponentToBag(new MechaComponentInfo(MechaComponentType.Block, new GridPos(1, 0, GridPos.Orientation.Down)));
        AddMechaComponentToBag(new MechaComponentInfo(MechaComponentType.Gun, new GridPos(1, 0, GridPos.Orientation.Right)));
        AddMechaComponentToBag(new MechaComponentInfo(MechaComponentType.Armor, new GridPos(3, 3, GridPos.Orientation.Right)));
        AddMechaComponentToBag(new MechaComponentInfo(MechaComponentType.Engine, new GridPos(-2, 3, GridPos.Orientation.Right)));
        AddMechaComponentToBag(new MechaComponentInfo(MechaComponentType.Sword, new GridPos(-2, 3, GridPos.Orientation.Right)));
        AddMechaComponentToBag(new MechaComponentInfo(MechaComponentType.Core, new GridPos(3, 3, GridPos.Orientation.Right)));
    }

    public bool AddMechaComponentToBag(MechaComponentInfo mci)
    {
        bool suc = BagPanel.AddItem(mci, MechaComponentOccupiedGridPosDict[mci.M_MechaComponentType]);
        if (suc)
        {
            MechaComponentInfos.Add(mci);
        }

        return suc;
    }

    public void RemoveMechaCoponentFromBag(BagItem bagItem)
    {
        BagPanel.RemoveItem(bagItem);
    }

    public void UnlockBagGridTo(int gridNumber)
    {
    }
}
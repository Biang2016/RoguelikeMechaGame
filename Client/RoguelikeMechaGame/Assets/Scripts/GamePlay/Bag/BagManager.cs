using System;
using UnityEngine;
using System.Collections.Generic;

public class BagManager : MonoSingleton<BagManager>
{
    internal bool InfiniteComponents = false;

    internal int BagItemGridSize;

    public Dictionary<MechaComponentType, Sprite> MechaComponentSpriteDict = new Dictionary<MechaComponentType, Sprite>();
    public Dictionary<MechaComponentType, List<GridPos>> MechaComponentOccupiedGridPosDict = new Dictionary<MechaComponentType, List<GridPos>>();

    internal BagPanel BagPanel;
    private List<MechaComponentInfo> mechaComponentInfosInBag = new List<MechaComponentInfo>();

    void Awake()
    {
    }

    void Start()
    {
        LoadAllBlockItemPics();
        LoadBlockOccupiedGridInfo();
        BagPanel = UIManager.Instance.ShowUIForms<BagPanel>();
        BagPanel.CloseUIForm();
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
            MechaComponentBase mcb = MechaComponentBase.BaseInitialize(new MechaComponentInfo(mcType, new GridPos(0, 0, GridPos.Orientation.Up), 0), null);
            mcbs.Add(mcb);
            MechaComponentOccupiedGridPosDict.Add(mcType, CloneVariantUtils.List(mcb.MechaComponentGrids.MechaComponentGridPositions));
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
        if (Input.GetButtonDown("Bag"))
        {
            if (!BagPanel.gameObject.activeInHierarchy)
            {
                OpenBag();
            }
            else
            {
                CloseBag();
            }
        }
    }

    public void OpenBag()
    {
        if (!BagPanel.gameObject.activeInHierarchy)
        {
            BattleManager.Instance.SetAllEnemyShown(false);
            UIManager.Instance.ShowUIForms<BagPanel>();
            BattleManager.Instance.PlayerMecha.RemoveAllComponentBuffs();
            BattleManager.Instance.PlayerMecha.MechaEditArea.Show();
            BattleManager.Instance.PlayerMecha.SlotLightsShown = true;
            GameManager.Instance.MainCameraFollow.FOW_Level = 1;
            BattleManager.Instance.PlayerMecha.GridShown = true;
            GameManager.Instance.SetState(GameState.Building);
        }
    }

    public void CloseBag()
    {
        if (BagPanel.gameObject.activeInHierarchy)
        {
            BattleManager.Instance.SetAllEnemyShown(true);
            BagPanel.CloseUIForm();
            BattleManager.Instance.PlayerMecha.MechaEditArea.Hide();
            BattleManager.Instance.PlayerMecha.SlotLightsShown = false;
            BattleManager.Instance.PlayerMecha.GridShown = false;
            BattleManager.Instance.PlayerMecha.RefreshMechaMatrix(out List<MechaComponentBase> conflictComponents, out List<MechaComponentBase> isolatedComponents);

            foreach (MechaComponentBase mcb in conflictComponents)
            {
                BattleManager.Instance.PlayerMecha.RemoveMechaComponent(mcb);
                mcb.PoolRecycle();
            }

            foreach (MechaComponentBase mcb in isolatedComponents)
            {
                BattleManager.Instance.PlayerMecha.RemoveMechaComponent(mcb);
                mcb.PoolRecycle();
            }

            BattleManager.Instance.PlayerMecha.ExertComponentBuffs();
            GameManager.Instance.MainCameraFollow.SetTarget(BattleManager.Instance.PlayerMecha.transform);
            GameManager.Instance.MainCameraFollow.FOW_Level = 2;
            GameManager.Instance.SetState(GameState.Fighting);
        }
    }

    private void Reset()
    {
        mechaComponentInfosInBag.Clear();
    }

    private void Initialize()
    {
        CurrentBagGridNumber = 100;
        AddMechaComponentToBag(new MechaComponentInfo(MechaComponentType.Core, new GridPos(1, 0, GridPos.Orientation.Down)), out BagItem _);
        AddMechaComponentToBag(new MechaComponentInfo(MechaComponentType.Block, new GridPos(1, 0, GridPos.Orientation.Right)), out BagItem _);
        AddMechaComponentToBag(new MechaComponentInfo(MechaComponentType.Gun, new GridPos(1, 0, GridPos.Orientation.Right)), out BagItem _);
        AddMechaComponentToBag(new MechaComponentInfo(MechaComponentType.Engine, new GridPos(-2, 3, GridPos.Orientation.Right)), out BagItem _);
        AddMechaComponentToBag(new MechaComponentInfo(MechaComponentType.PowerUp, new GridPos(-2, 3, GridPos.Orientation.Right)), out BagItem _);
        AddMechaComponentToBag(new MechaComponentInfo(MechaComponentType.Missile, new GridPos(-2, 3, GridPos.Orientation.Right)), out BagItem _);
    }

    public bool AddMechaComponentToBag(MechaComponentInfo mci, out BagItem bagItem)
    {
        bool suc = BagPanel.TryAddItem(mci, out bagItem);
        if (suc)
        {
            mechaComponentInfosInBag.Add(mci);
        }

        return suc;
    }

    public bool AddMechaComponentToBag(MechaComponentInfo mci, GridPos.Orientation orientation, List<GridPos> realGridPoses, out BagItem bagItem)
    {
        bool suc = BagPanel.TryAddItem(mci, orientation, realGridPoses, out bagItem);
        if (suc)
        {
            mechaComponentInfosInBag.Add(mci);
        }

        return suc;
    }

    public void RemoveMechaComponentFromBag(BagItem bagItem, bool temporary)
    {
        BagPanel.RemoveItem(bagItem, temporary);
        if (!temporary) mechaComponentInfosInBag.Remove(bagItem.MechaComponentInfo);
    }

    private int _currentBagGridNumber = 0;

    public int CurrentBagGridNumber
    {
        get { return _currentBagGridNumber; }

        set
        {
            if (_currentBagGridNumber != value)
            {
                BagPanel.UnlockBagGridTo(value);
                _currentBagGridNumber = value;
            }
        }
    }
}
using BiangStudio.GamePlay.UI;
using BiangStudio.GridBackpack;
using BiangStudio.ShapedInventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class BackpackItemInfoPanel : BaseUIPanel
    {
        void Awake()
        {
            UIType.InitUIType(
                false,
                true,
                false,
                UIFormTypes.Normal,
                UIFormShowModes.Normal,
                UIFormLucencyTypes.ImPenetrable);
        }

        private IInventoryItemContentInfo IInventoryItemContentInfo;

        [SerializeField]
        private TextMeshProUGUI ItemCategoryText;

        [SerializeField]
        private TextMeshProUGUI ItemNameText;

        [SerializeField]
        private Image ItemImage;

        [SerializeField]
        private TextMeshProUGUI ItemQualityText;

        [SerializeField]
        private TextMeshProUGUI ItemDetailInfoText;

        public void Initialize(IInventoryItemContentInfo iInventoryItemContentInfo)
        {
            IInventoryItemContentInfo = iInventoryItemContentInfo;
            ItemCategoryText.text = IInventoryItemContentInfo.ItemCategoryName;
            ItemNameText.text = IInventoryItemContentInfo.ItemName;
            ItemImage.sprite = BackpackManager.Instance.GetBackpackItemSprite(iInventoryItemContentInfo.ItemSpriteKey);
            ItemQualityText.text = IInventoryItemContentInfo.ItemQuality;
            ItemQualityText.color = IInventoryItemContentInfo.ItemColor;
            ItemDetailInfoText.text = IInventoryItemContentInfo.ItemDetailInfo;
        }
    }
}
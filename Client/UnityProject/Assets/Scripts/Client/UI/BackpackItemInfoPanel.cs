using BiangStudio;
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
        private float ItemImageMaxHeight;

        void Awake()
        {
            ItemImageMaxHeight = ItemImageContainer.sizeDelta.y;
            UIType.InitUIType(
                false,
                false,
                false,
                UIFormTypes.Normal,
                UIFormShowModes.Normal,
                UIFormLucencyTypes.ImPenetrable);
        }

        private IInventoryItemContentInfo IInventoryItemContentInfo;

        [SerializeField]
        private Image ItemNameBG;

        [SerializeField]
        private TextMeshProUGUI ItemNameText;

        [SerializeField]
        private RectTransform ItemImageContainer;

        [SerializeField]
        private Image ItemImage;

        [SerializeField]
        private TextMeshProUGUI ItemCategoryText;

        [SerializeField]
        private TextMeshProUGUI ItemQualityText;

        [SerializeField]
        private TextMeshProUGUI ItemBasicInfoText;

        [SerializeField]
        private TextMeshProUGUI ItemDetailedInfoText;

        [SerializeField]
        private Image[] Decorators;

        public void Initialize(IInventoryItemContentInfo iInventoryItemContentInfo, BackpackPanel backpackPanel)
        {
            ((RectTransform) transform).anchoredPosition = new Vector2(-((RectTransform) backpackPanel.Container.transform).sizeDelta.x, ((RectTransform) transform).anchoredPosition.y);

            IInventoryItemContentInfo = iInventoryItemContentInfo;
            Color bgColor = IInventoryItemContentInfo.ItemColor;
            ItemNameBG.color = bgColor;
            ItemNameText.text = IInventoryItemContentInfo.ItemName;

            ItemImage.sprite = BackpackManager.Instance.GetBackpackItemSprite(iInventoryItemContentInfo.ItemSpriteKey);
            Rect rect = ItemImage.sprite.rect;
            float ratio = Mathf.Min(ItemImageContainer.sizeDelta.x / rect.width, ItemImageMaxHeight / rect.height);
            rect.height = rect.height * ratio;
            ItemImageContainer.sizeDelta = new Vector2(ItemImageContainer.sizeDelta.x, rect.height);

            ItemCategoryText.text = IInventoryItemContentInfo.ItemCategoryName;
            ItemCategoryText.color = IInventoryItemContentInfo.ItemColor;
            ItemQualityText.text = IInventoryItemContentInfo.ItemQuality;
            ItemQualityText.color = IInventoryItemContentInfo.ItemColor;
            ItemBasicInfoText.text = IInventoryItemContentInfo.ItemBasicInfo;
            ItemDetailedInfoText.text = IInventoryItemContentInfo.ItemDetailedInfo;

            foreach (Image image in Decorators)
            {
                image.color = bgColor;
            }

            StartCoroutine(CommonUtils.UpdateLayout((RectTransform) transform));
        }
    }
}
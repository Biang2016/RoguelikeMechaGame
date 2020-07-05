using BiangStudio.ObjectPool;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Client
{
    public class HUDSlider : PoolObject
    {
        [SerializeField] private Text ValueText;
        [SerializeField] private Slider Slider;
        [SerializeField] private Image FillImage;

        private const float SLIDER_HEIGHT = 30f;

        public override void PoolRecycle()
        {
            base.PoolRecycle();
            core_OnValueChange = null;
        }

        private UnityAction<int, int> core_OnValueChange;

        public void Initialize(float heightRatio, Color color, out UnityAction<int, int> onValueChange)
        {
            FillImage.color = color;
            ((RectTransform) transform).sizeDelta = new Vector2(0, heightRatio * SLIDER_HEIGHT);
            onValueChange = SetValue;
            core_OnValueChange = onValueChange;
        }

        public void SetValue(int left, int total)
        {
            ValueText.text = left + " / " + total;
            if (total == 0)
            {
                Slider.value = 0f;
            }
            else
            {
                Slider.value = (float) left / total;
            }
        }
    }
}
using UnityEngine;
using System.Collections.Generic;
using BiangStudio.ObjectPool;
using UnityEngine.UI;

namespace Client
{
    public class MechaInfoHUD : PoolObject
    {
        [SerializeField] private Text MechaNameText;
        [SerializeField] private Transform SliderContainer;
        [SerializeField] private Transform CoreLifeSliderContainer;
        [SerializeField] private Color LifeSliderColor;
        [SerializeField] private Color CoreLifeSliderColor;
        [SerializeField] private Color PowerSliderColor;

        private Mecha targetMecha;

        public override void PoolRecycle()
        {
            Initialize(null);
            base.PoolRecycle();
        }

        public void Clean()
        {
            if (targetMecha)
            {
                targetMecha.RefreshHUDPanelCoreLifeSliderCount = null;
                targetMecha.OnLifeChange = null;
                targetMecha.OnPowerChange = null;
                targetMecha = null;
            }

            foreach (HUDSlider coreHudSlider in Core_HUDSliders)
            {
                coreHudSlider.PoolRecycle();
            }

            Core_HUDSliders.Clear();
            LifeSlider?.PoolRecycle();
            PowerSlider?.PoolRecycle();
            MechaNameText.text = "";
            SliderContainer.gameObject.SetActive(false);
        }

        public void Initialize(Mecha mecha)
        {
            if (mecha == null)
            {
                Clean();
            }
            else
            {
                if (targetMecha != mecha)
                {
                    Clean();
                    SliderContainer.gameObject.SetActive(true);
                    targetMecha = mecha;
                    LifeSlider = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.HUDSlider].AllocateGameObject<HUDSlider>(SliderContainer);
                    LifeSlider.Initialize(2, LifeSliderColor, out targetMecha.OnLifeChange);
                    PowerSlider = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.HUDSlider].AllocateGameObject<HUDSlider>(SliderContainer);
                    PowerSlider.Initialize(2, PowerSliderColor, out targetMecha.OnPowerChange);

                    MechaNameText.text = mecha.MechaInfo.MechaName;

                    LifeSlider.SetValue(mecha.M_LeftLife, mecha.M_TotalLife);
                    PowerSlider.SetValue(mecha.M_LeftPower, mecha.M_TotalPower);

                    targetMecha.RefreshHUDPanelCoreLifeSliderCount = RefreshCoreLifeSliders;
                    targetMecha.RefreshHUDPanelCoreLifeSliderCount();
                }
            }
        }

        internal HUDSlider LifeSlider;
        internal HUDSlider PowerSlider;
        public List<HUDSlider> Core_HUDSliders = new List<HUDSlider>();

        public void RefreshCoreLifeSliders()
        {
            if (BattleManager.Instance.PlayerMecha)
            {
                List<MechaComponentBase> mcbs = targetMecha.GetCoreLifeChangeDelegates();

                foreach (HUDSlider coreHudSlider in Core_HUDSliders)
                {
                    coreHudSlider.PoolRecycle();
                }

                Core_HUDSliders.Clear();

                foreach (MechaComponentBase mcb in mcbs)
                {
                    AddCoreLifeSlider(mcb);
                }
            }
        }

        private void AddCoreLifeSlider(MechaComponentBase mcb)
        {
            HUDSlider hudSlider = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.HUDSlider].AllocateGameObject<HUDSlider>(CoreLifeSliderContainer);
            hudSlider.Initialize(1f, CoreLifeSliderColor, out mcb.OnLifeChange);
            hudSlider.SetValue(mcb.M_LeftLife, mcb.M_TotalLife);
            Core_HUDSliders.Add(hudSlider);
        }
    }
}
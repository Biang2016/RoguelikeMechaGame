using System.Collections.Generic;
using BiangStudio.ObjectPool;
using GameCore;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class MechaInfoHUD : PoolObject
    {
        [SerializeField]
        private Text MechaNameText;

        [SerializeField]
        private Transform SliderContainer;

        [SerializeField]
        private Transform CoreLifeSliderContainer;

        [SerializeField]
        private Color LifeSliderColor;

        [SerializeField]
        private Color CoreLifeSliderColor;

        [SerializeField]
        private Color PowerSliderColor;

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
                if (targetMecha.MechaInfo != null)
                {
                    targetMecha.MechaInfo.RefreshHUDPanelCoreLifeSliderCount = null;
                    targetMecha.MechaInfo.OnLifeChange = null;
                    //targetMecha.MechaInfo.OnInputPowerChange = null;
                }

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
                    LifeSlider.Initialize(2, LifeSliderColor, out mecha.MechaInfo.OnLifeChange);
                    //PowerSlider = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.HUDSlider].AllocateGameObject<HUDSlider>(SliderContainer);
                    //PowerSlider.Initialize(2, PowerSliderColor, out mecha.MechaInfo.OnInputPowerChange);

                    MechaNameText.text = mecha.MechaInfo.MechaName;

                    LifeSlider.SetValue(mecha.MechaInfo.M_LeftLife, mecha.MechaInfo.M_TotalLife);
                    //PowerSlider.SetValue(mecha.MechaInfo.M_LeftPower, mecha.MechaInfo.M_TotalPower);

                    targetMecha.MechaInfo.RefreshHUDPanelCoreLifeSliderCount = RefreshCoreLifeSliders;
                    targetMecha.MechaInfo.RefreshHUDPanelCoreLifeSliderCount();
                }
            }
        }

        internal HUDSlider LifeSlider;
        internal HUDSlider PowerSlider;
        public List<HUDSlider> Core_HUDSliders = new List<HUDSlider>();

        public void RefreshCoreLifeSliders()
        {
            if (ClientBattleManager.Instance.PlayerMecha)
            {
                foreach (HUDSlider coreHudSlider in Core_HUDSliders)
                {
                    coreHudSlider.PoolRecycle();
                }

                if (targetMecha?.MechaInfo != null)
                {
                    List<MechaComponentInfo> mcis = targetMecha.MechaInfo.GetCoreLifeChangeDelegates();

                    Core_HUDSliders.Clear();

                    foreach (MechaComponentInfo mci in mcis)
                    {
                        AddCoreLifeSlider(mci);
                    }
                }
            }
        }

        private void AddCoreLifeSlider(MechaComponentInfo mci)
        {
            HUDSlider hudSlider = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.HUDSlider].AllocateGameObject<HUDSlider>(CoreLifeSliderContainer);
            hudSlider.Initialize(1f, CoreLifeSliderColor, out mci.OnLifeChange);
            hudSlider.SetValue(mci.M_LeftLife, mci.M_TotalLife);
            Core_HUDSliders.Add(hudSlider);
        }
    }
}
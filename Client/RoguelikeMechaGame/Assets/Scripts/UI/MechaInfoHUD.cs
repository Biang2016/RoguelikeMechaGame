using UnityEngine;
using System.Collections.Generic;

public class MechaInfoHUD : PoolObject
{
    [SerializeField] private Transform SliderContainer;
    [SerializeField] private Transform CoreLifeSliderContainer;
    [SerializeField] private Color LifeSliderColor;
    [SerializeField] private Color CoreLifeSliderColor;
    [SerializeField] private Color PowerSliderColor;

    private Mecha targetMecha;

    public void Initialize(Mecha mecha)
    {
        targetMecha = mecha;
        targetMecha.RefreshHUDPanelCoreLifeSliderCount = RefreshCoreLifeSliders;
        LifeSlider = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.HUDSlider].AllocateGameObject<HUDSlider>(SliderContainer);
        LifeSlider.Initialize(2, LifeSliderColor, out targetMecha.OnLifeChange);
        PowerSlider = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.HUDSlider].AllocateGameObject<HUDSlider>(SliderContainer);
        PowerSlider.Initialize(2, PowerSliderColor, out targetMecha.OnPowerChange);
    }

    internal HUDSlider LifeSlider;
    internal HUDSlider PowerSlider;
    private List<HUDSlider> Core_HUDSliders = new List<HUDSlider>();

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
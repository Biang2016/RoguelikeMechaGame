using System.Collections;
using UnityEngine;

namespace Client
{
    public class MechaComponentModel : MonoBehaviour
    {
        private Renderer Renderer;

        private Material BasicMaterial;
        private Material HighLightMaterial;

        public MechaComponentModelTwinkleConfigSSO MechaComponentModelTwinkleConfigSSO;

        private Color BuildingBasicMaterialEmissionColor;

        private Color DefaultBasicMaterialEmissionColor;
        private Color DefaultHighLightMaterialEmissionColor;

        void Awake()
        {
            Renderer = GetComponentInChildren<MeshRenderer>();
            BasicMaterial = Renderer.materials[0];
            HighLightMaterial = Renderer.materials[1];
            DefaultBasicMaterialEmissionColor = BasicMaterial.GetColor("_EmissionColor");
            DefaultHighLightMaterialEmissionColor = HighLightMaterial.GetColor("_EmissionColor");
        }

        /// <summary>
        /// 主要用于更改机甲建造时的孤立、冲突颜色
        /// </summary>
        /// <param name="basicEmissionColor"></param>
        public void SetBuildingBasicEmissionColor(Color basicEmissionColor)
        {
            BuildingBasicMaterialEmissionColor = basicEmissionColor;
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            Renderer.GetPropertyBlock(mpb, 0);
            mpb.SetColor("_EmissionColor", BuildingBasicMaterialEmissionColor);
            Renderer.SetPropertyBlock(mpb, 0);
        }

        public void ResetBuildingBasicEmissionColor()
        {
            BuildingBasicMaterialEmissionColor = DefaultBasicMaterialEmissionColor;
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            Renderer.GetPropertyBlock(mpb, 0);
            mpb.SetColor("_EmissionColor", BuildingBasicMaterialEmissionColor);
            Renderer.SetPropertyBlock(mpb, 0);
        }

        /// <summary>
        /// 组件品质、能量发生变化后，其默认的荧光颜色、强度也会发生变化
        /// </summary>
        /// <param name="highLightEmissionColor"></param>
        public void SetDefaultHighLightEmissionColor(Color highLightEmissionColor)
        {
            // DefaultBasicMaterialEmissionColor = basicEmissionColor;
            DefaultHighLightMaterialEmissionColor = highLightEmissionColor;
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            Renderer.GetPropertyBlock(mpb, 1);
            mpb.SetColor("_EmissionColor", highLightEmissionColor);
            Renderer.SetPropertyBlock(mpb, 1);
        }

        public void SetShown(bool shown)
        {
            Renderer.enabled = shown;
        }

        private Coroutine onDamageCoroutine;

        public void OnDamage(float portion)
        {
            if (onDamageCoroutine != null)
            {
                StopCoroutine(onDamageCoroutine);
            }

            onDamageCoroutine = StartCoroutine(Co_OnDamage(portion));
        }

        public void OnPowerChange(float portion)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            Renderer.GetPropertyBlock(mpb, 1);
            mpb.SetColor("_EmissionColor", DefaultHighLightMaterialEmissionColor * MechaComponentModelTwinkleConfigSSO.PowerIntensityCurve.Evaluate(portion));
            Renderer.SetPropertyBlock(mpb, 1);
        }

        IEnumerator Co_OnDamage(float portion)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            Renderer.GetPropertyBlock(mpb, 0);
            mpb.SetColor("_EmissionColor", MechaComponentModelTwinkleConfigSSO.DamageGradient.Evaluate(portion) * MechaComponentModelTwinkleConfigSSO.DamageIntensityCurve.Evaluate(portion));
            Renderer.SetPropertyBlock(mpb, 0);

            yield return new WaitForSeconds(MechaComponentModelTwinkleConfigSSO.DamageTwinkleDuration);

            Renderer.GetPropertyBlock(mpb, 0);
            mpb.SetColor("_EmissionColor", DefaultBasicMaterialEmissionColor);
            Renderer.SetPropertyBlock(mpb, 0);
        }

        public void ResetColor()
        {
            StopAllCoroutines();
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            Renderer.GetPropertyBlock(mpb, 0);
            mpb.SetColor("_EmissionColor", DefaultBasicMaterialEmissionColor);
            Renderer.SetPropertyBlock(mpb, 0);
            Renderer.GetPropertyBlock(mpb, 1);
            mpb.SetColor("_EmissionColor", DefaultHighLightMaterialEmissionColor);
            Renderer.SetPropertyBlock(mpb, 1);
        }
    }
}
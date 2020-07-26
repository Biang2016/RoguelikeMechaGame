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
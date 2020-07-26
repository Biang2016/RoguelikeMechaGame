using System.Collections;
using UnityEngine;

namespace Client
{
    public class MechaComponentModel : MonoBehaviour
    {
        private Renderer Renderer;

        private Material BasicMaterial;
        private Material HighLightMaterial;

        public Gradient DamageGradient;
        public AnimationCurve DamageIntensityCurve;
        public Gradient PowerGradient;
        public AnimationCurve PowerIntensityCurve;

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

        private Coroutine onPowerChangeCoroutine;

        public void OnPowerChange(float portion)
        {
            if (onPowerChangeCoroutine != null)
            {
                StopCoroutine(onPowerChangeCoroutine);
            }

            StartCoroutine(Co_OnPowerChange(portion));
        }

        IEnumerator Co_OnDamage(float portion)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            Renderer.GetPropertyBlock(mpb, 0);
            mpb.SetColor("_EmissionColor", DamageGradient.Evaluate(portion) * DamageIntensityCurve.Evaluate(portion));
            Renderer.SetPropertyBlock(mpb, 0);

            yield return new WaitForSeconds(0.1f);

            Renderer.GetPropertyBlock(mpb, 0);
            mpb.SetColor("_EmissionColor", DefaultBasicMaterialEmissionColor * 1f);
            Renderer.SetPropertyBlock(mpb, 0);
        }

        IEnumerator Co_OnPowerChange(float portion)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            Renderer.GetPropertyBlock(mpb, 1);
            mpb.SetColor("_EmissionColor", PowerGradient.Evaluate(portion) * PowerIntensityCurve.Evaluate(portion));
            Renderer.SetPropertyBlock(mpb, 1);

            yield return new WaitForSeconds(0.1f);

            Renderer.GetPropertyBlock(mpb, 1);
            mpb.SetColor("_EmissionColor", DefaultHighLightMaterialEmissionColor * 1f);
            Renderer.SetPropertyBlock(mpb, 1);
        }

        public void ResetColor()
        {
            StopAllCoroutines();
            BasicMaterial.SetColor("_EmissionColor", DefaultBasicMaterialEmissionColor);
        }
    }
}
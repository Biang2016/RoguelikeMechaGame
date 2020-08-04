using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class MechaComponentModel : MonoBehaviour
    {
        private List<Model> ModelList = new List<Model>();

        private class Model
        {
            private Renderer Renderer;
            private MechaComponentModel Parent;

            private Material BasicMaterial;
            private Material HighLightMaterial;

            private Color BuildingBasicMaterialEmissionColor;

            private Color DefaultBasicMaterialEmissionColor;
            private Color DefaultHighLightMaterialEmissionColor;

            public Model(Renderer renderer, MechaComponentModel parent)
            {
                Renderer = renderer;
                Parent = parent;
                BasicMaterial = Renderer.materials[0];
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
                    Parent.StopCoroutine(onDamageCoroutine);
                }

                onDamageCoroutine = Parent.StartCoroutine(Co_OnDamage(portion));
            }

            public void OnPowerChange(float portion)
            {
                MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                Renderer.GetPropertyBlock(mpb, 1);
                mpb.SetColor("_EmissionColor", DefaultHighLightMaterialEmissionColor * Parent.MechaComponentModelTwinkleConfigSSO.PowerIntensityCurve.Evaluate(portion));
                Renderer.SetPropertyBlock(mpb, 1);
            }

            IEnumerator Co_OnDamage(float portion)
            {
                MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                Renderer.GetPropertyBlock(mpb, 0);
                mpb.SetColor("_EmissionColor", Parent.MechaComponentModelTwinkleConfigSSO.DamageGradient.Evaluate(portion) * Parent.MechaComponentModelTwinkleConfigSSO.DamageIntensityCurve.Evaluate(portion));
                Renderer.SetPropertyBlock(mpb, 0);

                yield return new WaitForSeconds(Parent.MechaComponentModelTwinkleConfigSSO.DamageTwinkleDuration);

                Renderer.GetPropertyBlock(mpb, 0);
                mpb.SetColor("_EmissionColor", DefaultBasicMaterialEmissionColor);
                Renderer.SetPropertyBlock(mpb, 0);
            }

            public void ResetColor()
            {
                MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                Renderer.GetPropertyBlock(mpb, 0);
                mpb.SetColor("_EmissionColor", DefaultBasicMaterialEmissionColor);
                Renderer.SetPropertyBlock(mpb, 0);
                Renderer.GetPropertyBlock(mpb, 1);
                mpb.SetColor("_EmissionColor", DefaultHighLightMaterialEmissionColor);
                Renderer.SetPropertyBlock(mpb, 1);
            }
        }

        public MechaComponentModelTwinkleConfigSSO MechaComponentModelTwinkleConfigSSO;

        void Awake()
        {
            Renderer[] renderers = GetComponentsInChildren<MeshRenderer>();
            foreach (Renderer r in renderers)
            {
                Model model = new Model(r, this);
                ModelList.Add(model);
            }
        }

        /// <summary>
        /// 主要用于更改机甲建造时的孤立、冲突颜色
        /// </summary>
        /// <param name="basicEmissionColor"></param>
        public void SetBuildingBasicEmissionColor(Color basicEmissionColor)
        {
            foreach (Model model in ModelList)
            {
                model.SetBuildingBasicEmissionColor(basicEmissionColor);
            }
        }

        public void ResetBuildingBasicEmissionColor()
        {
            foreach (Model model in ModelList)
            {
                model.ResetBuildingBasicEmissionColor();
            }
        }

        /// <summary>
        /// 组件品质、能量发生变化后，其默认的荧光颜色、强度也会发生变化
        /// </summary>
        /// <param name="highLightEmissionColor"></param>
        public void SetDefaultHighLightEmissionColor(Color highLightEmissionColor)
        {
            foreach (Model model in ModelList)
            {
                model.SetDefaultHighLightEmissionColor(highLightEmissionColor);
            }
        }

        public void SetShown(bool shown)
        {
            foreach (Model model in ModelList)
            {
                model.SetShown(shown);
            }
        }

        public void OnDamage(float portion)
        {
            foreach (Model model in ModelList)
            {
                model.OnDamage(portion);
            }
        }

        public void OnPowerChange(float portion)
        {
            foreach (Model model in ModelList)
            {
                model.OnPowerChange(portion);
            }
        }

        public void ResetColor()
        {
            StopAllCoroutines();
            foreach (Model model in ModelList)
            {
                model.ResetColor();
            }
        }
    }
}
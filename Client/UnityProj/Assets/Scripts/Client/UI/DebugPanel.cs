using BiangStudio.GamePlay.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class DebugPanel : BaseUIPanel
    {
        void Awake()
        {
            UIType.InitUIType(
                false,
                false,
                false,
                UIFormTypes.Fixed,
                UIFormShowModes.Normal,
                UIFormLucencyTypes.Penetrable);
        }

        public Text fpsText;
        private float deltaTime;

        void Update()
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;
            fpsText.text = Mathf.Ceil(fps).ToString();
        }
    }
}
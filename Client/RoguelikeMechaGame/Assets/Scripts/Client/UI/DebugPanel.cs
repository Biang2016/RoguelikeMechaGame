using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Client
{
    public class DebugPanel : BaseUIForm
    {
        void Awake()
        {
            UIType.InitUIType(
                isClearStack: false,
                isESCClose: false,
                isClickElsewhereClose: false,
                uiForms_Type: UIFormTypes.Fixed,
                uiForms_ShowMode: UIFormShowModes.Normal,
                uiForm_LucencyType: UIFormLucencyTypes.Penetrable);
        }

        public Text fpsText;
        private float deltaTime;

        void Update()
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;
            fpsText.text = Mathf.Ceil(fps).ToString();
        }

        public void AddEnemy()
        {
            BattleManager.Instance.AddEnemy();
        }
    }
}
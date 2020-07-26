using BiangStudio.GameDataFormat.Grid;
using BiangStudio.GamePlay.UI;
using GameCore;
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

        public void AddEnemy()
        {
            MechaInfo enemyMechaInfo = new MechaInfo("Junk Mecha", MechaType.Enemy);
            BattleManager.Instance.AddEnemyMecha(enemyMechaInfo);
            for (int i = -2; i <= 2; i++)
            {
                for (int j = -4; j <= 4; j++)
                {
                    MechaComponentInfo mci;
                    mci = new MechaComponentInfo(MechaComponentType.Core, ConfigManager.Instance.GetAbilityGroup("BasicGun"), 100, 0);
                    enemyMechaInfo.AddMechaComponentInfo(mci, new GridPosR(i, j, GridPosR.Orientation.Up));
                }
            }

            ClientBattleManager.Instance.MechaDict[enemyMechaInfo.GUID].transform.position = new Vector3(10, 0, 10);
        }
    }
}
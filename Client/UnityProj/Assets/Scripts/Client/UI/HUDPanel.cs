using BiangStudio.GamePlay.UI;
using UnityEngine;

namespace Client
{
    public class HUDPanel : BaseUIPanel
    {
        void Awake()
        {
            UIType.InitUIType(
                false,
                true,
                false,
                UIFormTypes.Normal,
                UIFormShowModes.Normal,
                UIFormLucencyTypes.Penetrable);
        }

        [SerializeField] private MechaInfoHUD PlayerMechaInfoHUD;
        [SerializeField] private MechaInfoHUD EnemyMechaInfoHUD;

        public void Initialize()
        {
            PlayerMechaInfoHUD.Initialize(ClientBattleManager.Instance.PlayerMecha);
        }

        public void LoadEnemyMech(Mecha enemyMecha)
        {
            EnemyMechaInfoHUD.Initialize(enemyMecha);
        }
    }
}
using UnityEngine;
using System.Collections.Generic;

public class BattleManager : MonoSingleton<BattleManager>
{
    [SerializeField] private Transform MechaContainer;
    public Transform MechaComponentDropSpriteContainer;
    private HUDPanel HUDPanel;

    internal Mecha PlayerMecha;
    internal List<Mecha> EnemyMechas = new List<Mecha>();

    void Start()
    {
        HUDPanel = UIManager.Instance.ShowUIForms<HUDPanel>();
    }

    public void StartGame()
    {
        PlayerMecha?.PoolRecycle();
        PlayerMecha = null;
        foreach (Mecha em in EnemyMechas)
        {
            em.PoolRecycle();
        }

        EnemyMechas.Clear();

        PlayerMecha = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.Mecha].AllocateGameObject<Mecha>(MechaContainer);
        PlayerMecha.Initialize(new MechaInfo(MechaType.Self, new List<MechaComponentInfo>
        {
            new MechaComponentInfo(MechaComponentType.Core, new GridPos(0, 0, GridPos.Orientation.Up), 0),
        }));

        GameManager.Instance.MainCameraFollow.SetTarget(PlayerMecha.transform);
        GameManager.Instance.SetState(GameState.Fighting);

        HUDPanel.Initialize();
        PlayerMecha.RefreshHUDPanelCoreLifeSliderCount();
    }

    public void AddEnemy()
    {
        Mecha EnemyMecha = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.Mecha].AllocateGameObject<Mecha>(MechaContainer);
        List<MechaComponentInfo> enemyComponentInfos = new List<MechaComponentInfo>();
        for (int i = 5; i <= 8; i++)
        {
            for (int j = -6; j <= 6; j++)
            {
                MechaComponentInfo mci;
                if (i == 7 && j == 0)
                {
                    mci = new MechaComponentInfo(MechaComponentType.Core, new GridPos(i, j, GridPos.Orientation.Up), 0);
                }
                else
                {
                    mci = new MechaComponentInfo((MechaComponentType) Random.Range(1, 6), new GridPos(i, j, GridPos.Orientation.Up), 10);
                }

                enemyComponentInfos.Add(mci);
            }
        }

        EnemyMecha.Initialize(new MechaInfo(MechaType.Enemy, enemyComponentInfos));
        EnemyMechas.Add(EnemyMecha);
    }

    public void SetAllEnemyShown(bool shown)
    {
        foreach (Mecha em in EnemyMechas)
        {
            em.SetShown(shown);
        }
    }
}
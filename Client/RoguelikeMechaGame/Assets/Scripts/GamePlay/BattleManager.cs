using UnityEngine;
using System.Collections.Generic;

public class BattleManager : MonoSingleton<BattleManager>
{
    [SerializeField] private Transform MechaContainer;
    public Transform MechaComponentDropSpriteContainer;

    internal Mecha PlayerMecha;
    internal List<Mecha> EnemyMechas = new List<Mecha>();

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
            new MechaComponentInfo(MechaComponentType.Core, new GridPos(0, 0, GridPos.Orientation.Up)),
        }));

        GameManager.Instance.MainCameraFollow.SetTarget(PlayerMecha.transform);
        GameManager.Instance.SetState(GameState.Fighting);
    }

    public void AddEnemy()
    {
        Mecha EnemyMecha = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.Mecha].AllocateGameObject<Mecha>(MechaContainer);
        List<MechaComponentInfo> enemyComponentInfos = new List<MechaComponentInfo>();
        for (int i = 5; i <= 8; i++)
        {
            for (int j = -9; j <= 9; j++)
            {
                MechaComponentInfo mci;
                if (i == 7 && j == 0)
                {
                    mci = new MechaComponentInfo(MechaComponentType.Core, new GridPos(i, j, GridPos.Orientation.Up));
                }
                else
                {
                    mci = new MechaComponentInfo(MechaComponentType.Block, new GridPos(i, j, GridPos.Orientation.Up));
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
            em.gameObject.SetActive(shown);
        }
    }
}
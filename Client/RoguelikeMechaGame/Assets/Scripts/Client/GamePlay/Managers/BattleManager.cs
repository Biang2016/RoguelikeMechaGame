using System;
using UnityEngine;
using System.Collections.Generic;
using GameCore;
using Random = UnityEngine.Random;

namespace Client
{
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
            PlayerMecha.Initialize(new MechaInfo("Solar 0", MechaType.Self, new List<MechaComponentInfo>
            {
                new MechaComponentInfo(MechaComponentType.Core, new GridPosR(0, 0, GridPosR.Orientation.Up), 300, 0),
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
            for (int i = -4; i <= 4; i++)
            {
                for (int j = -6; j <= 6; j++)
                {
                    MechaComponentInfo mci;
                    if (i == 0 && j == 0)
                    {
                        mci = new MechaComponentInfo(MechaComponentType.Core, new GridPosR(i, j, GridPosR.Orientation.Up), 500, 0);
                    }
                    else
                    {
                        mci = new MechaComponentInfo((MechaComponentType) Random.Range(1, Enum.GetNames(typeof(MechaComponentType)).Length), new GridPosR(i, j, GridPosR.Orientation.Up), 50, 5);
                    }

                    enemyComponentInfos.Add(mci);
                }
            }

            EnemyMecha.Initialize(new MechaInfo("Junk Mecha", MechaType.Enemy, enemyComponentInfos));
            EnemyMecha.transform.position = new Vector3(10, 0, 10);
            EnemyMechas.Add(EnemyMecha);
        }

        public void SetAllEnemyShown(bool shown)
        {
            foreach (Mecha em in EnemyMechas)
            {
                em.SetShown(shown);
            }
        }

        void Update()
        {
            Ray ray = GameManager.Instance.MainCamera.ScreenPointToRay(ControlManager.Instance.Common_MousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, GameManager.Instance.LayerMask_ComponentHitBox))
            {
                if (hit.collider)
                {
                    MechaComponentHitBox hitBox = hit.collider.GetComponent<MechaComponentHitBox>();
                    Mecha mecha = hitBox?.ParentHitBoxRoot?.MechaComponentBase?.ParentMecha;
                    if (mecha && mecha.MechaInfo.MechaType == MechaType.Enemy)
                    {
                        HUDPanel.LoadEnemyMech(mecha);
                    }
                }
                else
                {
                    HUDPanel.LoadEnemyMech(null);
                }
            }
            else
            {
                HUDPanel.LoadEnemyMech(null);
            }
        }
    }
}
using System.Collections.Generic;
using BiangStudio.GamePlay.UI;
using BiangStudio.Singleton;
using GameCore;
using UnityEngine;

namespace Client
{
    public class ClientBattleManager : TSingletonBaseManager<ClientBattleManager>
    {
        public BattleInfo BattleInfo;
        internal Mecha PlayerMecha;
        internal SortedDictionary<int, Mecha> EnemyMechaDict = new SortedDictionary<int, Mecha>();

        private HUDPanel HUDPanel;
        public Transform MechaContainerRoot;
        public Transform MechaComponentDropSpriteContainerRoot;

        public void Init(Transform mechaContainerRoot, Transform mechaComponentDropSpriteContainerRoot)
        {
            MechaContainerRoot = mechaContainerRoot;
            MechaComponentDropSpriteContainerRoot = mechaComponentDropSpriteContainerRoot;
        }

        public override void Awake()
        {
        }

        public override void Start()
        {
            HUDPanel = UIManager.Instance.ShowUIForms<HUDPanel>();
        }

        public override void LogicTick()
        {
            PlayerMecha.LogicTick();
            foreach (KeyValuePair<int, Mecha> kv in EnemyMechaDict)
            {
                kv.Value.LogicTick();
            }
        }

        public override void Update()
        {
            Ray ray = CameraManager.Instance.MainCamera.ScreenPointToRay(ControlManager.Instance.Battle_MousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerManager.Instance.LayerMask_ComponentHitBox))
            {
                if (hit.collider)
                {
                    MechaComponentHitBox hitBox = hit.collider.GetComponent<MechaComponentHitBox>();
                    if (hitBox.Mecha != null)
                    {
                        if (hitBox.Mecha.MechaInfo.MechaType == MechaType.Enemy)
                        {
                            HUDPanel.LoadEnemyMech(hitBox.Mecha);
                        }
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

        public void Clean()
        {
            PlayerMecha?.PoolRecycle();
            PlayerMecha = null;
            foreach (KeyValuePair<int, Mecha> kv in EnemyMechaDict)
            {
                kv.Value.PoolRecycle();
            }

            EnemyMechaDict.Clear();
            BattleInfo = null;
        }

        public void StartBattle(BattleInfo battleInfo)
        {
            Clean();
            BattleInfo = battleInfo;
            BattleInfo.OnAddEnemyMechaInfoSuc = AddMecha;

            PlayerMecha = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.Mecha].AllocateGameObject<Mecha>(MechaContainerRoot);
            PlayerMecha.Initialize(battleInfo.PlayerMechaInfo);

            foreach (KeyValuePair<int, MechaInfo> kv in battleInfo.EnemyMechaInfoDict)
            {
            }

            CameraManager.Instance.MainCameraFollow.SetTarget(PlayerMecha.transform);
            GameStateManager.Instance.SetState(GameState.Fighting);

            HUDPanel.Initialize();
            PlayerMecha.MechaInfo.RefreshHUDPanelCoreLifeSliderCount();
        }

        private void AddMecha(MechaInfo mechaInfo)
        {
            Mecha mecha = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.Mecha].AllocateGameObject<Mecha>(MechaContainerRoot);
            mecha.Initialize(mechaInfo);
            mecha.OnRemoveMechaSuc = RemoveMecha;
            EnemyMechaDict.Add(mecha.MechaInfo.GUID, mecha);
        }

        private void RemoveMecha(Mecha mecha)
        {
            EnemyMechaDict.Remove(mecha.MechaInfo.GUID);
            mecha.PoolRecycle();
        }

        public void SetAllEnemyShown(bool shown)
        {
            foreach (KeyValuePair<int, Mecha> kv in EnemyMechaDict)
            {
                kv.Value.SetShown(shown);
            }
        }
    }
}